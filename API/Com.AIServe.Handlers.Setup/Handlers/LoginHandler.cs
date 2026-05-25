using Com.AIServe.Common.Handlers;
using Com.AIServe.Common.Models;
using Com.AIServe.Handlers.Setup.Models;
using Com.AIServe.Utils;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Com.AIServe.Handlers.Setup.Handlers;

[Handler("login")]
public class LoginHandler : IHandler
{
    private static readonly Dictionary<string, string> _users = new()
    {
        { "admin", "admin123" }
    };
    private static readonly HashSet<string> _activeTokens = new();
    private static readonly System.Text.Json.JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
    };

    public async Task<ApiResponse> HandleRequestAsync(HttpContext context, IQueryCollection query, string body)
    {
        var action = query["action"].ToString();
        LogHelper.Info($"LoginHandler.HandleRequest - action: {action}");

        return action switch
        {
            "login_login" => await Task.FromResult(Login(body)),
            "login_logout" => await Task.FromResult(Logout(body)),
            "login_register" => await Task.FromResult(Register(body)),
            _ => await Task.FromResult(ApiResponse.Fail("无效的请求")),
        };
    }

    private ApiResponse Login(string body)
    {
        try
        {
            var request = JsonSerializer.Deserialize<LoginRequest>(body, _jsonOptions);
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return ApiResponse.Fail("用户名和密码不能为空");
            }

            LogHelper.Info($"用户登录: {request.Username}");

            if (_users.TryGetValue(request.Username, out var password) && password == request.Password)
            {
                var token = Guid.NewGuid().ToString();
                _activeTokens.Add(token);
                var response = new LoginResponse
                {
                    Token = token,
                    Username = request.Username
                };
                LogHelper.Info($"用户 {request.Username} 登录成功");
                return ApiResponse.Ok(response);
            }
            else
            {
                LogHelper.Info($"用户 {request.Username} 登录失败：用户名或密码错误");
                return ApiResponse.Fail("用户名或密码错误");
            }
        }
        catch (Exception ex)
        {
            LogHelper.Info($"登录异常: {ex.Message}");
            return ApiResponse.Fail($"登录失败: {ex.Message}");
        }
    }

    private ApiResponse Logout(string body)
    {
        try
        {
            var request = JsonSerializer.Deserialize<LogoutRequest>(body, _jsonOptions);
            if (request == null || string.IsNullOrEmpty(request.Token))
            {
                return ApiResponse.Fail("Token不能为空");
            }

            LogHelper.Info($"用户退出登录");

            if (_activeTokens.Remove(request.Token))
            {
                LogHelper.Info($"用户退出成功");
                return ApiResponse.Ok(new { Message = "退出成功" });
            }
            else
            {
                return ApiResponse.Fail("Token无效或已过期");
            }
        }
        catch (Exception ex)
        {
            LogHelper.Info($"退出登录异常: {ex.Message}");
            return ApiResponse.Fail($"退出登录失败: {ex.Message}");
        }
    }

    private ApiResponse Register(string body)
    {
        try
        {
            var request = JsonSerializer.Deserialize<RegisterRequest>(body, _jsonOptions);
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return ApiResponse.Fail("用户名和密码不能为空");
            }

            LogHelper.Info($"用户注册: {request.Username}");

            if (_users.ContainsKey(request.Username))
            {
                LogHelper.Info($"注册失败：用户名 {request.Username} 已存在");
                return ApiResponse.Fail("用户名已存在");
            }

            _users[request.Username] = request.Password;
            LogHelper.Info($"用户 {request.Username} 注册成功");
            return ApiResponse.Ok(new { Message = "注册成功" });
        }
        catch (Exception ex)
        {
            LogHelper.Info($"注册异常: {ex.Message}");
            return ApiResponse.Fail($"注册失败: {ex.Message}");
        }
    }
}

public class LogoutRequest
{
    public string Token { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
