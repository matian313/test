using Com.AIServe.Common.Models;
using Com.AIServe.Utils;
using System.Text.Json;

namespace AIServe.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // 记录详细错误日志
        var errorId = Guid.NewGuid().ToString();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;
        var queryString = context.Request.QueryString.ToString();

        var errorMessage = $"【全局异常捕获】错误ID: {errorId}\n" +
                           $"请求路径: {requestMethod} {requestPath}{queryString}\n" +
                           $"异常类型: {exception.GetType().Name}\n" +
                           $"异常消息: {exception.Message}\n" +
                           $"堆栈跟踪: {exception.StackTrace}";

        LogHelper.Error(errorMessage, exception);
        _logger.LogError(exception, "全局异常捕获: {ErrorId}", errorId);

        // 返回统一错误响应
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = ApiResponse.Fail($"服务器内部错误 (错误ID: {errorId})");
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var json = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json);
    }
}
