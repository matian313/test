using Com.AIServe.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Com.AIServe.Common.Handlers;

public interface IHandler
{
    Task<ApiResponse> HandleRequestAsync(HttpContext context, IQueryCollection query, string body);
}
