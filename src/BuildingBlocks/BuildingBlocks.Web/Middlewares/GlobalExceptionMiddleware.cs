using System.Text.Json;
using System.Text.Json.Serialization; // Cần thêm namespace này
using BuildingBlocks.Shared.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Web.Middlewares;

public class GlobalExceptionMiddleware(RequestDelegate _next, ILogger<GlobalExceptionMiddleware> _logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (ex is DomainException || ex is FluentValidationException)
            {
                _logger.LogWarning(
                    "Domain or Validation exception occurred. Path: {RequestPath}, Method: {Method}, Exception: {ExceptionType}, Message: {Message}", 
                    context.Request.Path, 
                    context.Request.Method,
                    ex.GetType().Name,
                    ex.Message);
            }
            else
            {
                _logger.LogError(ex, 
                    "Unhandled exception occurred. RequestPath: {RequestPath}, Method: {Method}, User: {User}, Exception: {ExceptionType}, Message: {Message}", 
                    context.Request.Path, 
                    context.Request.Method,
                    context.User?.Identity?.Name ?? "Anonymous",
                    ex.GetType().Name,
                    ex.Message);
            }
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var exceptionDetail = GetExceptionDetails(exception);
        
        context.Response.StatusCode = exceptionDetail.StatusCode;
        
        var options = new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(exceptionDetail, options));
    }

    private ExceptionDetail GetExceptionDetails(Exception ex)
    {
        // Dùng switch expression để return luôn, code cực gọn và an toàn
        var (statusCode, message, errors) = ex switch
        {
            DomainException domain => (StatusCodes.Status400BadRequest, domain.Message, (object?)null),
            FluentValidationException fluent => (StatusCodes.Status400BadRequest, fluent.Message, (object?)fluent.Errors),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred", (object?)null)
        };

        return new ExceptionDetail
        {
            StatusCode = statusCode,
            Message = message,
            Errors = errors
        };
    }
}

public class ExceptionDetail
{
    [JsonIgnore]
    public int StatusCode { get; set; }
    
    public string Message { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Errors { get; set; } = null;
}
