using KestrelFramework;
using SocketBindTest.Server;
using SocketBindTest.Server.Dto;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddSingleton<ForwardOptionsDb>();
builder.Services.AddOptions<ForwardOptions>();
builder.Services.ConfigureOptions<KestrelOptionsSetup>();
builder.Services.ConfigureOptions<SocketTransportOptionsSetup>();

builder.Services.AddSingleton<InProcConnectionContainerMiddleware>();
builder.Services.AddSingleton<IKestrelMiddleware>(s => s.GetRequiredService<InProcConnectionContainerMiddleware>());
builder.Services.AddSingleton<IConnectionContainer>(s => s.GetRequiredService<InProcConnectionContainerMiddleware>());

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

app.MapConnectionApi();

app.Run();