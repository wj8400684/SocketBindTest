using KestrelFramework;
using WebApplicationTarget.Middlewares;

namespace WebApplicationTarget.Extensions;

internal static class ConnectionContainerExtensions
{
    public static void AddConnectionContainer(this IServiceCollection services)
    {
        services.AddSingleton<InProcConnectionContainerMiddleware>();
        services.AddSingleton<IKestrelMiddleware>(s => s.GetRequiredService<InProcConnectionContainerMiddleware>());
        services.AddSingleton<IConnectionContainer>(s => s.GetRequiredService<InProcConnectionContainerMiddleware>());
    }
}