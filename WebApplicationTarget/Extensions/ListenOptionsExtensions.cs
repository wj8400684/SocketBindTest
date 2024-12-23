using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using WebApplicationTarget.Middlewares;

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
            .UseTlsDetection()
            .UseConnectionHandler<TelnetConnectionHandler>();
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