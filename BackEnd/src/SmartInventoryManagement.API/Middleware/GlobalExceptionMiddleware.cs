using Microsoft.AspNetCore.Mvc;
using SmartInventoryManagement.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace SmartInventoryManagement.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                
                var correlationId = context.TraceIdentifier;
                _logger.
                    LogError(ex, "An unhandled exception occurred. CorrelationId: {CorrelationId}",
                    correlationId);

                await HandleExceptionAsync(context, ex, correlationId);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId)
        {
            context.Response.ContentType = "application/problem+json";

            // بناء ProblemDetails افتراضي
            var problemDetails = new ProblemDetails
            {
                Instance = context.Request.Path,
                Extensions = { ["correlationId"] = correlationId }
            };

            switch (exception)
            {
                case NotFoundException notFoundEx:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    problemDetails.Title = "Not Found";
                    problemDetails.Detail = notFoundEx.Message;
                    break;

                case ValidationException validationEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    problemDetails.Title = "Validation Error";
                    problemDetails.Detail = validationEx.Message;

                
                   
                    break;

                case ConflictException conflictEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    problemDetails.Title = "Conflict";
                    problemDetails.Detail = conflictEx.Message;
                    break;

                case UnauthorizedException unauthorizedEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    problemDetails.Title = "Unauthorized";
                    problemDetails.Detail = unauthorizedEx.Message;
                    break;

            
                case ArgumentException argEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    problemDetails.Title = "Invalid Argument";
                    problemDetails.Detail = argEx.Message;
                    break;

                case UnauthorizedAccessException _:
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    problemDetails.Title = "Forbidden";
                    problemDetails.Detail = "Access denied.";
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    problemDetails.Title = "Internal Server Error";
                    
                    problemDetails.Detail = _env.IsDevelopment()
                        ? exception.Message : "An unexpected error occurred.";
                    break;
            }

         
            if (context.Response.StatusCode != (int)HttpStatusCode.InternalServerError)
            {
                _logger.LogWarning(@"Handled exception: {ExceptionType} with StatusCode {StatusCode}",
                      exception.GetType().Name, context.Response.StatusCode);
            }

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(problemDetails, jsonOptions);
            await context.Response.WriteAsync(json);
        }
    }
}
