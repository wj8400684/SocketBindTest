using System.Net;
using System.Net.Sockets;

var ipAddress = Dns.GetHostAddresses(Dns.GetHostName()).Where(ip => !ip.Equals(IPAddress.Loopback)).ToArray();

foreach (var ip in ipAddress)
{
    Console.WriteLine("本地ip:{0}", ip.ToString());
}

var bindIp = new IPEndPoint(ipAddress.First(), Random.Shared.Next(10000, 65535));

using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

socket.ExclusiveAddressUse = false;
socket.Bind(bindIp);

Console.WriteLine("正在连接");

await socket.ConnectAsync(new DnsEndPoint("www.baidu.com", 80, AddressFamily.InterNetwork));

Console.WriteLine("连接成功");

Console.ReadKey();