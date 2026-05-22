using Com.AIServe.Common.Handlers;

namespace AIServe.API.Handlers;

public class HandlerFactory
{
    private readonly Dictionary<string, Func<IServiceProvider, IHandler>> _handlerFactories = new();
    private readonly IServiceProvider _serviceProvider;

    public HandlerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        RegisterHandlers();
    }

    private void RegisterHandlers()
    {
        // Reservation handlers
        RegisterHandler("reservation_list", sp => sp.GetRequiredService<Com.AIServe.Handlers.Reservation.Handlers.ReservationHandler>());
        RegisterHandler("reservation_get", sp => sp.GetRequiredService<Com.AIServe.Handlers.Reservation.Handlers.ReservationHandler>());
        RegisterHandler("reservation_create", sp => sp.GetRequiredService<Com.AIServe.Handlers.Reservation.Handlers.ReservationHandler>());
        RegisterHandler("reservation_update", sp => sp.GetRequiredService<Com.AIServe.Handlers.Reservation.Handlers.ReservationHandler>());
        RegisterHandler("reservation_updatestatus", sp => sp.GetRequiredService<Com.AIServe.Handlers.Reservation.Handlers.ReservationHandler>());
        RegisterHandler("reservation_delete", sp => sp.GetRequiredService<Com.AIServe.Handlers.Reservation.Handlers.ReservationHandler>());

        // Setup handlers
        RegisterHandler("setup_getconfig", sp => sp.GetRequiredService<Com.AIServe.Handlers.Setup.Handlers.SetupHandler>());
        RegisterHandler("setup_updateconfig", sp => sp.GetRequiredService<Com.AIServe.Handlers.Setup.Handlers.SetupHandler>());
        RegisterHandler("setup_healthcheck", sp => sp.GetRequiredService<Com.AIServe.Handlers.Setup.Handlers.SetupHandler>());

        // Login handlers
        RegisterHandler("login_login", sp => sp.GetRequiredService<Com.AIServe.Handlers.Setup.Handlers.LoginHandler>());
        RegisterHandler("login_logout", sp => sp.GetRequiredService<Com.AIServe.Handlers.Setup.Handlers.LoginHandler>());
        RegisterHandler("login_register", sp => sp.GetRequiredService<Com.AIServe.Handlers.Setup.Handlers.LoginHandler>());
    }

    public void RegisterHandler(string action, Func<IServiceProvider, IHandler> handlerFactory)
    {
        _handlerFactories[action] = handlerFactory;
    }

    public IHandler? GetHandler(string action)
    {
        if (_handlerFactories.TryGetValue(action, out var factory))
        {
            return factory(_serviceProvider);
        }
        return null;
    }
}
