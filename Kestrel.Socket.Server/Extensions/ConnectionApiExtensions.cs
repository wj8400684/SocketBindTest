using Kestrel.Socket.Server.Dto;
using Kestrel.Socket.Server.Middlewares;

namespace Kestrel.Socket.Server.Extensions;

internal static class ConnectionApiExtensions
{
    public static void MapConnectionApi(this WebApplication app)
    {
        var connection = app.MapGroup("api/connection");

        connection.MatGetConnectionCount();
    }

    private static void MatGetConnectionCount(this RouteGroupBuilder group)
    {
        group.MapGet("count", (IConnectionContainer container) => new ConnectionResponse(
            Msg: "操作成功",
            RefreshTime: DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Count: container.GetConnectionCount()));
    }
}