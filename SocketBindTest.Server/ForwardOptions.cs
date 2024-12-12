using System.Net;

namespace SocketBindTest.Server;

internal sealed class ForwardOptions
{
    public static ForwardOptions Load(IConfigurationSection section)
    {
        var options = new ForwardOptions();
        section.Bind(options);
        return options;
    }
    
    public string? ListenUrl { get; set; }

    public int ForwardPort { get; set; }

    public string? ForwardAddress { get; set; }

    public bool IsForward { get; set; }
    
    public string? ForwardName { get; set; }

    public IPAddress? ForwardIpAddress { get; set; }
}