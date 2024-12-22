using SuperSocket.Kestrel.Protocol;
using System.Buffers;
using System.Text;

namespace WebApplicationTarget;

#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。

public sealed class LinePipelineProtocol : 
    IMessageReader<SuperSocket.ProtoBase.TextPackageInfo>, 
    IMessageWriter<SuperSocket.ProtoBase.TextPackageInfo>
{
    private readonly ReadOnlyMemory<byte> _terminator = new([13, 10]);

    public bool TryParseMessage(in ReadOnlySequence<byte> input, 
        ref SequencePosition consumed, 
        ref SequencePosition examined, 
        out SuperSocket.ProtoBase.TextPackageInfo message)
    {
        message = null;

        if(input.IsEmpty)
            return false;

        var reader = new SequenceReader<byte>(input);
        if (!reader.TryReadTo(out ReadOnlySequence<byte> sequence, _terminator.Span, advancePastDelimiter: false))
            return false;

        message = new SuperSocket.ProtoBase.TextPackageInfo
        { 
            Text = Encoding.UTF8.GetString(sequence.FirstSpan),
        };

        reader.Advance(_terminator.Length);

        examined = consumed = input.GetPosition(reader.Length);

        return true;
    }

    public void WriteMessage(SuperSocket.ProtoBase.TextPackageInfo message, IBufferWriter<byte> output)
    {
        output.Write(message.Text, Encoding.UTF8);
        output.Write(_terminator.Span);
    }
}


#pragma warning restore CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
