using Blog.Application.Interfaces.Auth;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Application.Services;
using Blog.Infrastructure;
using Blog.WebAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<BlogContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    providerOptions => providerOptions.EnableRetryOnFailure())//very helpful if firsttime db connection
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors();
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Add Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 2; // Max requests
        limiterOptions.Window = TimeSpan.FromSeconds(20); // Time window
        //limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        //limiterOptions.QueueLimit = 2; // Requests queued before rejecting
    });
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        var result = new { error = "Rate limit exceeded. Please try again later." };
        await context.HttpContext.Response.WriteAsJsonAsync(result, cancellationToken);
    };
    // Custom response when limit is exceeded
    //options.OnRejected = async (context, cancellationToken) =>
    //{
    //    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
    //    await context.HttpContext.Response.WriteAsync(
    //        "Rate limit exceeded. Please try again later.", cancellationToken);
    //};
});

// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true; // Adds headers with supported versions
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost4200", policy =>
    {
        //policy.WithOrigins("http://localhost:4200") // exact origin
        policy.WithOrigins("https://blog-platform-fydbc3fmdaffbggk.centralindia-01.azurewebsites.net") // exact origin
                 .AllowAnyHeader()
                 .AllowAnyMethod()
                 .AllowCredentials(); // allow cookies/auth headers
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// The SDK automatically checks:
// 1. Environment Variable "APPLICATIONINSIGHTS_CONNECTION_STRING" (Azure Priority)
// 2. appsettings.json section "ApplicationInsights:ConnectionString" (Local Fallback)
//builder.Services.AddApplicationInsightsTelemetry();
var app = builder.Build();
// Use Rate Limiting Middleware


//app.UseCors("default");
// Use CORS Middleware (Must be before Auth/Controllers)
app.UseCors("AllowLocalhost4200");
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        //Log.Error(exception, "Unhandled exception occurred. {ExceptionDetails}", exception?.ToString());
        Console.WriteLine(exception?.ToString());
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
    });
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("My API");
        options.WithTheme(ScalarTheme.BluePlanet);
        options.WithSidebar(true);
    });

    app.UseSwaggerUi(options =>
    {
        //options
        options.DocumentPath = "openapi/v1.json";
    });
}

app.UseHttpsRedirection();
// 1. Add Response Middleware FIRST (Outer Layer)
// It starts first, creates the memory stream buffer, calls next, then waits for the return trip to log.
app.UseMiddleware<ResponseBodyLoggingMiddleware>();

// 2. Add Request Middleware SECOND (Inner Layer)
// It reads the input, logs it, and passes it to the controller.
app.UseMiddleware<RequestBodyLoggingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.Run();