namespace Kestrel.Socket.Server.Dto;

internal sealed class ConnectionDto
{
    public int? Count { get; set; }

    public required string Msg { get; set; }
}