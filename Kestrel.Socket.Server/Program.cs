using Kestrel.Socket.Server.Dto;
using Kestrel.Socket.Server.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddConnectionContainer();

var app = builder.Build();

app.MapConnectionApi();

app.Run();
