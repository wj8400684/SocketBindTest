using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Google.Protobuf;
using SuperSocket.ProtoBase;

namespace SocketBindTest.Client;

public partial class CommandPackage : IKeyedPackageInfo<CommandPackage.Types.CommandType>
{
    private static readonly ConcurrentDictionary<Type, MessageParser> MessageParsersFactory = new();

    internal const int BodySize = 4;
    internal const int BodyFlagLength = 12;
    internal const int BodyHeaderLength = 16;
    internal static readonly ReadOnlyMemory<byte> BodyHeader = new([
        0x01, 0x1a, 0x2b, 0x3c, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00
    ]);

    public Types.CommandType Key
    {
        get => (Types.CommandType)command_;
        init => command_ = (int)value;
    }

    public static CommandPackage New(Types.CommandType key, IMessage message)
    {
        return new CommandPackage
        {
            Key = key,
            Content = message.ToByteString(),
        };
    }

    public bool TryParse<TPackage>([MaybeNullWhen(returnValue: false)] out TPackage package)
        where TPackage : IMessage<TPackage>?
    {
        try
        {
            var parser = MessageParsersFactory.GetOrAdd(typeof(TPackage),
                _ => new MessageParser<TPackage>(Activator.CreateInstance<TPackage>));

            package = (TPackage?)parser.ParseFrom(Content);
            return package != null;
        }
        catch (Exception)
        {
            package = default;
            return false;
        }
    }
}