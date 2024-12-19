using Microsoft.Extensions.Options;
using SuperSocket.ProtoBase;
using SuperSocket.Server;
using SuperSocket.Server.Abstractions;

namespace SuperSocket.WebApp.Server;

public sealed class ChatServer(IServiceProvider serviceProvider, IOptions<ServerOptions> serverOptions)
    : SuperSocketService<TextPackageInfo>(serviceProvider, serverOptions)
{
    
}