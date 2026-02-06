using Microsoft.ApplicationInsights.DataContracts;
using System.Text;

namespace Blog.WebAPI.Middlewares
{
    public class ResponseBodyLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public ResponseBodyLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            // 1. Hold reference to the original stream
            var originalBodyStream = context.Response.Body;

            // 2. Create a new memory stream to hold the response temporarily
            using var responseBodyMemoryStream = new MemoryStream();
            context.Response.Body = responseBodyMemoryStream;

            // 3. Pass control down the pipeline (to RequestMiddleware -> Controller)
            await _next(context);

            // ---------------------------------------------------------
            // The Controller has now executed and written to our MemoryStream
            // ---------------------------------------------------------

            // 4. Reset position to read the stream
            context.Response.Body.Position = 0;

            // Filter: Only log text/json
            if (IsTextBasedContentType(context.Response.ContentType))
            {
                using (var reader = new StreamReader(context.Response.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
                {
                    var responseBody = await reader.ReadToEndAsync();

                    // 5. Log to Azure App Insights
                    var requestTelemetry = context.Features.Get<RequestTelemetry>();
                    if (requestTelemetry != null && !string.IsNullOrEmpty(responseBody))
                    {
                        // Truncate
                        if (responseBody.Length > 8000) responseBody = responseBody.Substring(0, 8000) + "...[TRUNCATED]";

                        if (!requestTelemetry.Properties.ContainsKey("ResponseBody"))
                        {
                            requestTelemetry.Properties.Add("ResponseBody", responseBody);
                        }

                        // Optional: Log status code explicitly if needed, though App Insights captures this automatically
                        if (!requestTelemetry.Properties.ContainsKey("ResponseStatusCode"))
                        {
                            requestTelemetry.Properties.Add("ResponseStatusCode", context.Response.StatusCode.ToString());
                        }
                    }
                }
            }

            // 6. IMPORTANT: Copy the contents back to the original stream so the client gets the response
            context.Response.Body.Position = 0;
            await responseBodyMemoryStream.CopyToAsync(originalBodyStream);

            // Restore the original stream reference
            context.Response.Body = originalBodyStream;
        }
        private bool IsTextBasedContentType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType)) return false;
            return contentType.Contains("application/json") ||
                   contentType.Contains("text/plain") ||
                   contentType.Contains("application/xml");
        }
    }
}
