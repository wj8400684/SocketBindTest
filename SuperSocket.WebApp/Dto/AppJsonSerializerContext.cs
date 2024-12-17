using System.Text.Json.Serialization;

namespace SuperSocket.WebApp.Dto;

[JsonSerializable(typeof(ConnectionResponse))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}