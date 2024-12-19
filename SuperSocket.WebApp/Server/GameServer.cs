using Microsoft.Extensions.Options;
using SuperSocket.ProtoBase;
using SuperSocket.Server;
using SuperSocket.Server.Abstractions;

namespace SuperSocket.WebApp;

public sealed class GameServer(IServiceProvider serviceProvider, IOptions<ServerOptions> serverOptions)
    : SuperSocketService<TextPackageInfo>(serviceProvider, serverOptions)
{
    
}