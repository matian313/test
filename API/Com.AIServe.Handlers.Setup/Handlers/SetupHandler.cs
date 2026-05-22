using Com.AIServe.Common.Handlers;
using Com.AIServe.Common.Models;
using Com.AIServe.Handlers.Setup.Models;
using Com.AIServe.Utils;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Com.AIServe.Handlers.Setup.Handlers;

public class SetupHandler : IHandler
{
    private static SystemConfig _config = new();
    private static readonly System.Text.Json.JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
    };

    public async Task<ApiResponse> HandleRequestAsync(HttpContext context, IQueryCollection query, string body)
    {
        var action = query["action"].ToString();
        LogHelper.Info($"SetupHandler.HandleRequest - action: {action}");

        return action switch
        {
            "setup_getconfig" => await Task.FromResult(GetConfig()),
            "setup_updateconfig" => await Task.FromResult(UpdateConfig(body)),
            "setup_healthcheck" => await Task.FromResult(HealthCheck()),
            _ => await Task.FromResult(ApiResponse.Fail("无效的请求")),
        };
    }

    private ApiResponse GetConfig()
    {
        LogHelper.Info("获取系统配置");
        return ApiResponse.Ok(_config);
    }

    private ApiResponse UpdateConfig(string body)
    {
        try
        {
            var config = JsonSerializer.Deserialize<SystemConfig>(body, _jsonOptions);
            if (config == null)
            {
                return ApiResponse.Fail("无效的请求数据");
            }

            _config = config;
            LogHelper.Info("更新系统配置");
            return ApiResponse.Ok();
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"更新配置失败: {ex.Message}");
        }
    }

    private ApiResponse HealthCheck()
    {
        return ApiResponse.Ok(new { Status = "OK", Time = DateTime.Now });
    }
}
