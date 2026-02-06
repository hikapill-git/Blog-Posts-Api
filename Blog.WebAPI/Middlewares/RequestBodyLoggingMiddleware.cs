using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts; // Required namespaces

namespace Blog.WebAPI.Middlewares
{
    public class RequestBodyLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestBodyLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            // Filter: Only log text/json to avoid logging file uploads or binary data
            if (IsTextBasedContentType(context.Request.Headers["Content-Type"]))
            {
                // 1. Enable buffering so the stream can be read here AND by the Controller later
                context.Request.EnableBuffering();

                // 2. Read the stream
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
                {
                    var requestBody = await reader.ReadToEndAsync();

                    // 3. Reset position for the next middleware
                    context.Request.Body.Position = 0;

                    // 4. Log to Azure App Insights
                    var requestTelemetry = context.Features.Get<RequestTelemetry>();
                    if (requestTelemetry != null && !string.IsNullOrEmpty(requestBody))
                    {
                        // Truncate to prevent 8kb limit errors in App Insights
                        if (requestBody.Length > 8000) requestBody = requestBody.Substring(0, 8000) + "...[TRUNCATED]";

                        if (!requestTelemetry.Properties.ContainsKey("RequestBody"))
                        {
                            requestTelemetry.Properties.Add("RequestBody", requestBody);
                        }
                    }
                }
            }

            // 5. Move to next middleware
            await _next(context);




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
