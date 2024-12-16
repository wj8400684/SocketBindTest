using System.Text.Json.Serialization;

namespace SocketBindTest.Server.Dto;

[JsonSerializable(typeof(ConnectionDto))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}