using System.Text.Json.Serialization;

namespace Kestrel.Socket.Server.Dto;

[JsonSerializable(typeof(ConnectionDto))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}