using AIServe.API.Handlers;
using Com.AIServe.Common.Handlers;
using Com.AIServe.Common.Models;
using Com.AIServe.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIServe.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiServiceController : ControllerBase
{
    private readonly HandlerFactory _handlerFactory;

    public ApiServiceController(HandlerFactory handlerFactory)
    {
        _handlerFactory = handlerFactory;
    }

    [HttpGet]
    public async Task<ApiResponse> Get()
    {
        string? method = Request.Query["action"];
        string uid = Guid.NewGuid().ToString();

        if (string.IsNullOrEmpty(method))
        {
            LogHelper.Info($"ApiServiceController.Get - 找不到指定的方法, uid={uid}");
            return ApiResponse.Fail("找不到指定的方法");
        }

        try
        {
            LogHelper.Info($"ApiServiceController.Get【请求】方法:{method};请求唯一Id:【{uid}】;请求时间:{DateTime.Now:HH:mm:ss.fff}");

            var handler = _handlerFactory.GetHandler(method);
            ApiResponse? result;

            if (handler != null)
            {
                result = await handler.HandleRequestAsync(Request.HttpContext, Request.Query, string.Empty);
            }
            else
            {
                result = ApiResponse.Fail("无效请求");
            }

            LogHelper.Info($"ApiServiceController.Get【响应】方法:{method};请求唯一Id:【{uid}】;响应时间:{DateTime.Now:HH:mm:ss.fff}");
            return result;
        }
        catch (Exception e)
        {
            LogHelper.Info($"ApiServiceController.Get【异常】方法:{method};请求唯一Id:【{uid}】;异常:{e.Message}");
            return ApiResponse.Fail(e.Message);
        }
    }

    [HttpPost]
    public async Task<ApiResponse> Post()
    {
        string? method = Request.Query["action"];
        string uid = Guid.NewGuid().ToString();

        if (string.IsNullOrEmpty(method))
        {
            LogHelper.Info($"ApiServiceController.Post - 找不到指定的方法, uid={uid}");
            return ApiResponse.Fail("找不到指定的方法");
        }

        try
        {
            LogHelper.Info($"ApiServiceController.Post【请求】方法:{method};请求唯一Id:【{uid}】;请求时间:{DateTime.Now:HH:mm:ss.fff}");

            using var reader = new StreamReader(Request.Body);
            string content = await reader.ReadToEndAsync();

            var handler = _handlerFactory.GetHandler(method);
            ApiResponse? result;

            if (handler != null)
            {
                result = await handler.HandleRequestAsync(Request.HttpContext, Request.Query, content);
            }
            else
            {
                result = ApiResponse.Fail("无效请求");
            }

            LogHelper.Info($"ApiServiceController.Post【响应】方法:{method};请求唯一Id:【{uid}】;响应时间:{DateTime.Now:HH:mm:ss.fff}");
            return result;
        }
        catch (Exception e)
        {
            LogHelper.Info($"ApiServiceController.Post【异常】方法:{method};请求唯一Id:【{uid}】;异常:{e.Message}");
            return ApiResponse.Fail(e.Message);
        }
    }
}
