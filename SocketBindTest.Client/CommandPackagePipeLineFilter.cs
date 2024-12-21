using System.Buffers;
using System.Buffers.Binary;
using SuperSocket.ProtoBase;

namespace SocketBindTest.Client;

public sealed class CommandPackagePipeLineFilter() : FixedHeaderPipelineFilter<CommandPackage>(CommandPackage.BodyHeaderLength)
{
    protected override CommandPackage DecodePackage(ref ReadOnlySequence<byte> buffer)
    {
        return CommandPackage.Parser.ParseFrom(buffer.Slice(CommandPackage.BodyHeaderLength));
    }

    protected override int GetBodyLengthFromHeader(ref ReadOnlySequence<byte> buffer)
    {
        var bodyLength = 0;

        if (buffer.IsSingleSegment)
            BinaryPrimitives.TryReadInt32BigEndian(buffer.FirstSpan.Slice(CommandPackage.BodyFlagLength, CommandPackage.BodySize), out bodyLength);
        else
        {
            var reader = new SequenceReader<byte>(buffer);
            reader.Advance(CommandPackage.BodyFlagLength);
            reader.TryReadBigEndian(out bodyLength);
        }

        return bodyLength;
    }
}