namespace Kestrel.Socket.Server.Dto;

public sealed class ConnectionDto
{
    public int? Count { get; set; }

    public required string Msg { get; set; }
}