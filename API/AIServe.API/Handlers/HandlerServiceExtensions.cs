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
        // 显式注册所有 Handler
        services.AddScoped<ReservationHandler>();
        services.AddScoped<SetupHandler>();
        services.AddScoped<LoginHandler>();
        services.AddScoped<ReportHandler>();

        // 注册 Handler 工厂
        services.AddScoped<HandlerFactory>();

        return services;
    }
}

/// <summary>
/// Handler 工厂
/// </summary>
public class HandlerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public HandlerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 根据 action 名称获取对应的 Handler
    /// </summary>
    public IHandler? GetHandler(string action)
    {
        if (action.StartsWith("reservation_"))
        {
            return _serviceProvider.GetService<ReservationHandler>();
        }
        else if (action.StartsWith("setup_"))
        {
            return _serviceProvider.GetService<SetupHandler>();
        }
        else if (action.StartsWith("login_"))
        {
            return _serviceProvider.GetService<LoginHandler>();
        }
        else if (action.StartsWith("report_"))
        {
            return _serviceProvider.GetService<ReportHandler>();
        }

        return null;
    }
}
