using System.Buffers;
using System.Runtime.CompilerServices;
using SuperSocket.Connection;
using SuperSocket.ProtoBase;

namespace SocketBindTest.Client;

public sealed class CommandPackageProtocol(IConnection connection)
    : IPackageDecoder<CommandPackage>, IPackageEncoder<CommandPackage>
{
    private readonly IAsyncEnumerator<CommandPackage> _packageStream =
        connection.GetPackageStream(new CommandPackagePipeLineFilter());


    CommandPackage IPackageDecoder<CommandPackage>.Decode(ref ReadOnlySequence<byte> buffer, object context)
    {
        throw new NotImplementedException();
    }

    int IPackageEncoder<CommandPackage>.Encode(IBufferWriter<byte> writer, CommandPackage pack)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<CommandPackage> SendEnterRoomAsync(EnterRoomPackage package,
        CancellationToken cancellationToken = default)
    {
        var commandPackage = CommandPackage.New(CommandPackage.Types.CommandType.EnterRoom, package);
        return await RequestAsync(commandPackage, cancellationToken);
    }

    private async ValueTask<CommandPackage> RequestAsync(
        CommandPackage package,
        CancellationToken cancellationToken = default)
    {
        await connection.SendAsync(this, package, cancellationToken);
        return await _packageStream.ReceiveAsync();
    }

    public async IAsyncEnumerable<CommandPackage> ReceiveAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = await _packageStream.ReceiveAsync();
            if (result == null)
                yield break;
            
            yield return result;
        }
    }
}