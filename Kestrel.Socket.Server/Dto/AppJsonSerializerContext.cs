using System.Text.Json.Serialization;

namespace Kestrel.Socket.Server.Dto;

[JsonSerializable(typeof(ConnectionResponse))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}