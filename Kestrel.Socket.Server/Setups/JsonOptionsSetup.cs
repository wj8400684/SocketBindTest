using Kestrel.Socket.Server.Dto;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace Kestrel.Socket.Server.Setups;

internal sealed class JsonOptionsSetup : IConfigureOptions<JsonOptions>
{
    public void Configure(JsonOptions options)
    {
        options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    }
}