using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            var multicastAddress = IPAddress.Parse("224.192.32.19");
            using (var client = new UdpClient(22600))
            {
                client.JoinMulticastGroup(multicastAddress);
                var remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
                while (true)
                {
                    var data = await client.ReceiveAsync();
                    var ascii = Encoding.ASCII.GetString(data.Buffer);
                    try
                    {
                        var record = Parse(ascii);
                        Console.WriteLine(record);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to parse: {0}", ex.Message);
                        Console.WriteLine($"Received {data.Buffer.Length} from {data.RemoteEndPoint}: {ascii}");
                    }
                }

            }
        }

        private static Data Parse(string xml)
        {
            var doc = XDocument.Parse(xml);
            var electricity = doc.Root;
            var id = electricity.Attribute("id");
            var unixTimestamp = (long)electricity.Element("timestamp");
            var time = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;
            var signal = electricity.Element("signal");
            var rssi = signal.Attribute("rssi");
            var lqi = signal.Attribute("lqi");
            var battery = electricity.Element("battery");
            var levelPercent = (string)battery.Attribute("level");
            var level = int.Parse(levelPercent.Substring(0, levelPercent.Length - 1), NumberStyles.Integer);
            var channels = electricity.Element("channels");
            var channel = channels.Elements().First(); ;
            var current = (double)channel.Element("curr");
            var day = (double)channel.Element("day");


            return new Data
            {
                Id = (string)id,
                Timestamp = time,
                Rssi = (int)rssi,
                Lqi = (int)lqi,
                Battery = level,
                Watts = current,
                DailyWattHours = day

            };

        }
        private static async Task OldRun(CancellationToken cancel)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                var multicastAddress = IPAddress.Parse("224.192.32.19");
                var localAddress = IPAddress.Parse("192.168.100.131");
                var localEndpoint = new IPEndPoint(localAddress, 22600);
                socket.Bind(localEndpoint);

                var multicastOption = new MulticastOption(multicastAddress, localAddress);
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, multicastOption);

                var buffer = new byte[10240];
                var remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
                var segment = new ArraySegment<byte>(buffer);
                Console.WriteLine("Starting loop");
                while (!cancel.IsCancellationRequested)
                {
                    var result = await socket.ReceiveFromAsync(segment, SocketFlags.None, remoteEndpoint);
                    var ascii = Encoding.ASCII.GetString(buffer, 0, result.ReceivedBytes);
                    Console.WriteLine($"Received {result.ReceivedBytes} from {result.RemoteEndPoint}: {ascii}");
                    Console.Out.Flush();
                }
                Console.WriteLine("Stopping loop");


            }
        }
    }
}
