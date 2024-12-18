using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using WebApplicationTarget.Dto;

namespace WebApplicationTarget.Setups;

internal sealed class JsonOptionsSetup : IConfigureOptions<JsonOptions>
{
    public void Configure(JsonOptions options)
    {
        options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    }
}