using System.Text.Json.Serialization;

namespace WebApplicationTarget.Dto;

[JsonSerializable(typeof(ConnectionResponse))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}