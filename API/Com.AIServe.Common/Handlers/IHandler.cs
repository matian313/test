using Com.AIServe.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Com.AIServe.Common.Handlers;

/// <summary>
/// Handler 自动发现特性
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class HandlerAttribute : Attribute
{
    public string ActionPrefix { get; }

    public HandlerAttribute(string actionPrefix)
    {
        ActionPrefix = actionPrefix;
    }
}

public interface IHandler
{
    Task<ApiResponse> HandleRequestAsync(HttpContext context, IQueryCollection query, string body);
}
