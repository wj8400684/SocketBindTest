using System.Buffers;
using System.Buffers.Binary;
using Google.Protobuf;
using SuperSocket.ProtoBase;

namespace SocketBindTest.Client;

public sealed class CommandPackageEncoder : IPackageEncoder<CommandPackage>
{
    public int Encode(IBufferWriter<byte> writer, CommandPackage pack)
    {
        var total = CommandPackage.BodyHeader.Length;
        writer.Write(CommandPackage.BodyHeader.Span);

        var bodyLength = pack.CalculateSize();
        BinaryPrimitives.WriteInt32BigEndian(writer.GetSpan(), bodyLength);
        total += bodyLength;

        pack.WriteTo(writer);

        return total;
    }
}