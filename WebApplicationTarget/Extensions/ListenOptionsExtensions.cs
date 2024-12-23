using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using WebApplicationTarget.Middlewares;
using WebApplicationTarget.Middlewares.Tls;

namespace WebApplicationTarget.Extensions;

internal static partial class ListenOptionsExtensions
{
    /// <summary>
    /// 使用TelnetConnectionHandler
    /// </summary>
    /// <param name="listen"></param>
    public static ListenOptions UseTelnet(this ListenOptions listen)
    {
        listen.UseContainer()
            .UseTls()
            .UseConnectionHandler<TelnetConnectionHandler>();
        return listen;
    }

    /// <summary>
    /// 使用tls
    /// </summary>
    /// <param name="listen"></param>
    /// <returns></returns>
    public static ListenOptions UseTls(this ListenOptions listen)
    {
        listen.Use<TlsInvadeMiddleware>();
        listen.UseHttps();
        listen.Use<TlsRestoreMiddleware>();
        return listen;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="listen"></param>
    public static ListenOptions UseContainer(this ListenOptions listen)
    {
        listen.Use(listen.ApplicationServices.GetRequiredService<InProcConnectionContainerMiddleware>());

        return listen;
    }
}