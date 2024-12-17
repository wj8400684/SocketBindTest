using System.Text.Json.Serialization;

namespace Kestrel.Socket.Server.Dto;

[JsonSerializable(typeof(ConnectionDto))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}