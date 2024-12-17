using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using SuperSocket.WebApp.Dto;

namespace SuperSocket.WebApp.Setups;

internal sealed class JsonOptionsSetup : IConfigureOptions<JsonOptions>
{
    public void Configure(JsonOptions options)
    {
        options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    }
}