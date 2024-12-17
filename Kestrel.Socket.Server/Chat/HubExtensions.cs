using Microsoft.AspNetCore.SignalR;

namespace Kestrel.Socket.Server.Chat;

public static class HubExtensions
{
    public static ISignalRServerBuilder AddContainer(this ISignalRServerBuilder serverBuilder)
    {
        serverBuilder.Services.AddSingleton<ChatConnectionContainer>();
        serverBuilder.Services.AddSingleton<IChatConnectionContainer>(s =>
            s.GetRequiredService<ChatConnectionContainer>());
        return serverBuilder;
    }
}