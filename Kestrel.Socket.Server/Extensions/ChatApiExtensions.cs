using Kestrel.Socket.Server.Chat;
using Kestrel.Socket.Server.Dto;
using Kestrel.Socket.Server.Middlewares;

namespace Kestrel.Socket.Server.Extensions;

internal static class ChatApiExtensions
{
    public static void MapChatWebApi(this WebApplication app)
    {
        var connection = app.MapGroup("api/chat");

        connection.MatGetConnectionCount();
    }

    private static void MatGetConnectionCount(this RouteGroupBuilder group)
    {
        group.MapGet("count", (IChatConnectionContainer container) => new ConnectionResponse(
            Msg: "操作成功",
            RefreshTime: DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Count: container.GetConnectionCount()));
    }
}