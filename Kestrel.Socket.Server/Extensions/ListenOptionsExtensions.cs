using Kestrel.Socket.Server.ConnectionHandlers;
using Kestrel.Socket.Server.Middlewares;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Kestrel.Socket.Server.Extensions;

internal static partial class ListenOptionsExtensions
{
    /// <summary>
    /// 使用RedisConnectionHandler
    /// </summary>
    /// <param name="listen"></param>
    public static ListenOptions UseTelnet(this ListenOptions listen)
    {
        listen.UseConnectionHandler<TelnetConnectionHandler>();
        
        return listen;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="listen"></param>
    /// <returns></returns>
    public static ListenOptions UseForward(this ListenOptions listen)
    {
        listen.UseConnectionHandler<ForwardConnectionHandler>();
        
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