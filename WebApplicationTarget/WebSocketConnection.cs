using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Features;
using SuperSocket.Kestrel.Protocol;
using SuperSocket.ProtoBase;
using System.Net;
using System.Runtime.CompilerServices;

namespace WebApplicationTarget;

public sealed class WebSocketConnection(ConnectionContext connectionContext) : IAsyncDisposable
{
    private readonly static LinePipelineProtocol Potocol = new();
    private readonly ProtocolReader _reader = connectionContext.CreateReader();
    private readonly ProtocolWriter _writer = connectionContext.CreateWriter();

    public IDictionary<object, object?> Items { get; } = connectionContext.Items;

    public IFeatureCollection Features { get; } = connectionContext.Features;

    public EndPoint? RemoteEndPoint { get; } = connectionContext.RemoteEndPoint;

    public EndPoint? LocalEndPoint { get; } = connectionContext.LocalEndPoint;

    public ValueTask SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        return _writer.WriteAsync(data, cancellationToken);
    }

    public ValueTask SendAsync(TextPackageInfo packageInfo, CancellationToken cancellationToken = default)
    {
        return _writer.WriteAsync(Potocol, packageInfo, cancellationToken);
    }

    public async IAsyncEnumerable<TextPackageInfo> RunAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (true)
        {
            var readResult = await _reader.ReadAsync(Potocol, cancellationToken);
            if (readResult.IsCanceled)
                throw new OperationCanceledException();

            if (readResult.IsCompleted)
                yield break;

            yield return readResult.Message;

            _reader.Advance();
        }
    }

    public void Close()
    {
        connectionContext.Abort();
    }

    public async ValueTask DisposeAsync()
    {
        await _writer.DisposeAsync();
        await _reader.DisposeAsync();
        await connectionContext.DisposeAsync();
    }
}
