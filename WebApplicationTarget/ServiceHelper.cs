using ServiceSelf;

namespace WebApplicationTarget;

internal static class ServiceHelper
{
    public static bool UseServiceSelf(string[] args)
    {
        const string serviceName = "socketServer";
        var serviceOptions = new ServiceOptions
        {
            Description = "基于kestrel的socket+websocket服务器",
        };

        var workingDirectory = Environment.CurrentDirectory;
        var execStart = string.Join("/", workingDirectory, "WebApplicationTarget");

        Console.WriteLine($"execStart: {execStart}");
        
        serviceOptions.Linux.Service["WorkingDirectory"] = workingDirectory;
        serviceOptions.Linux.Service["ExecStart"] = execStart;
        // serviceOptions.Linux.Service["Environment"] = "ASPNETCORE_ENVIRONMENT=Production";
        // serviceOptions.Linux.Service["User"] = "root";
        // serviceOptions.Linux.Service["LimitMEMORY"] = "5120M";
        // serviceOptions.Linux.Service["LimitNOFILE"] = "50000";
        // serviceOptions.Linux.Service["LimitCPU"] = "100%";
        // serviceOptions.Linux.Service.Restart = "always";
        // serviceOptions.Linux.Service.RestartSec = "10";

        serviceOptions.Linux.Install.WantedBy = "WantedBy=multi-user.target";

        return Service.UseServiceSelf(args, serviceName, serviceOptions);
    }
}