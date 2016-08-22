using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OwlClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Run(CancellationToken.None).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running: {ex.Message}");
            }
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private static async Task Run(CancellationToken cancel)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                var multicastAddress =IPAddress.Parse("224.192.32.19");
                var localAddress = IPAddress.Parse("192.168.100.69");
                var localEndpoint = new IPEndPoint(localAddress, 22600);
                socket.Bind(localEndpoint);

                var multicastOption = new MulticastOption(multicastAddress, localAddress);
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, multicastOption);

                var buffer = new byte[10240];
                var remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
                var segment = new ArraySegment<byte>(buffer);
                while (!cancel.IsCancellationRequested)
                {
                    var result = await socket.ReceiveFromAsync(segment, SocketFlags.None, remoteEndpoint);
                    var ascii = Encoding.ASCII.GetString(buffer, 0, result.ReceivedBytes);
                    Console.WriteLine($"Received {result.ReceivedBytes} from {result.RemoteEndPoint}: {ascii}");
                }


            }
        }
    }
}
