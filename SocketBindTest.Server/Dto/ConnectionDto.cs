namespace SocketBindTest.Server.Dto;

public sealed class ConnectionDto
{
    public int? Count { get; set; }

    public required string Msg { get; set; }
}