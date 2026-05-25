using System.Reflection;
using Com.AIServe.Common.Handlers;
using Com.AIServe.Handlers.Reservation.Handlers;
using Com.AIServe.Handlers.Setup.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace AIServe.API.Handlers;

public static class HandlerServiceExtensions
{
    /// <summary>
    /// 注册所有 Handler
    /// </summary>
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        var handlerMap = new Dictionary<string, Type>();

        // 注册所有 Handler 并建立映射
        RegisterHandler<ReservationHandler>(services, handlerMap, "reservation");
        RegisterHandler<SetupHandler>(services, handlerMap, "setup");
        RegisterHandler<LoginHandler>(services, handlerMap, "login");
        RegisterHandler<ReportHandler>(services, handlerMap, "report");

        // 注册 Handler 工厂，传入映射关系
        services.AddScoped<HandlerFactory>(sp => new HandlerFactory(sp, handlerMap));

        return services;
    }

    private static void RegisterHandler<T>(IServiceCollection services, Dictionary<string, Type> handlerMap, string prefix)
        where T : class, IHandler
    {
        services.AddScoped<T>();
        handlerMap[prefix] = typeof(T);
    }
}

/// <summary>
/// Handler 工厂
/// </summary>
public class HandlerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _handlerMap;

    public HandlerFactory(IServiceProvider serviceProvider, Dictionary<string, Type> handlerMap)
    {
        _serviceProvider = serviceProvider;
        _handlerMap = handlerMap;
    }

    /// <summary>
    /// 根据 action 名称获取对应的 Handler
    /// </summary>
    public IHandler? GetHandler(string action)
    {
        foreach (var (prefix, handlerType) in _handlerMap)
        {
            if (action.StartsWith(prefix + "_"))
            {
                return _serviceProvider.GetService(handlerType) as IHandler;
            }
        }
        return null;
    }
}
