using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Rako.Net
{
    public class RakoUdpClient : IDisposable
    {
        public string BridgeAddress { get; }
        private readonly UdpClient _client;

        public RakoUdpClient(string bridgeAddress)
        {
            BridgeAddress = bridgeAddress;
            _client = new UdpClient();
            _client.Client.ReceiveTimeout = 2000;
            _client.Client.SendTimeout = 1000;
        }

        public Task<bool> SetScene(int room, int channel, int scene) =>
            SendCommand(room, channel, 49, 0, (byte)scene);
            //SendCommand(room, channel, 49, 1, (byte)scene, 0, 0, 0);

        public async Task<bool> SendCommand(int room, int channel, params byte[] data)
        {
            var buffer = new byte[data.Length + 6];
            buffer[0] = (byte)UdpCommands.RemoteCommand;
            buffer[1] = (byte)(data.Length + 4);
            buffer[2] = (byte)(room >> 8);
            buffer[3] = (byte)room;
            buffer[4] = (byte)channel;
            Array.Copy(data, 0, buffer, 5, data.Length);
            byte total = 0;
            for (var i = 1; i < buffer.Length - 1; i++)
            {
                total += buffer[i];
            }
            buffer[buffer.Length - 1] = (byte)(256 - total);
            Console.WriteLine(BitConverter.ToString(buffer));
            await _client.SendAsync(buffer, buffer.Length, BridgeAddress, 9761);
            var result = await _client.ReceiveAsync();

            Console.WriteLine(BitConverter.ToString(result.Buffer));
            var message = System.Text.Encoding.ASCII.GetString(result.Buffer, 1, result.Buffer.Length - 3);
            Console.WriteLine(message);
            return message.Equals("OK");
        }

        public void Dispose() => _client.Dispose();
    }
}
