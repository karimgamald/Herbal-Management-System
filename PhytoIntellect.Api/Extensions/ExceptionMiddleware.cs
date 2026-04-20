using System.Net;
using System.Text.Json;

namespace PhytoIntellect.Api.Extensions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // سيب الريكويست يكمل شغله العادي
            await _next(context);
        }
        catch (Exception ex)
        {
            // لو ضرب أي إيرور في أي مكان في السيستم، هيمسكه هنا
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // 👈 التريكة هنا: بنترجم نوع الإيرور لـ Status Code
        context.Response.StatusCode = exception switch
        {
            KeyNotFoundException => (int)HttpStatusCode.NotFound, // 404 (مش موجود)
            InvalidOperationException => (int)HttpStatusCode.BadRequest, // 400 (موجود قبل كده أو عملية مرفوضة)
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized, // 401 (مش مسجل دخول)
            _ => (int)HttpStatusCode.InternalServerError // 500 (إيرور في الكود نفسه)
        };

        // شكل الـ JSON اللي هيرجع للفرونت إند
        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message // دي الرسالة اللي إنت كاتبها بإيدك في الـ Service
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        return context.Response.WriteAsync(json);
    }
}