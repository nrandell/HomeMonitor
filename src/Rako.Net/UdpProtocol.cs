using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Rako.Net
{
    public class UdpProtocol
    {

        public static int VerifyMessage(byte[] buffer, UdpCommands command, int minimumLength)
        {
            if (buffer[0] != (byte)command)
            {
                throw new InvalidOperationException($"Invalid command buffer: {(char)buffer[0]}");
            }
            var numberOfBytes = (int)buffer[1];
            if (numberOfBytes < minimumLength)
            {
                throw new InvalidOperationException($"Invalid number of bytes: {numberOfBytes}");
            }
            return numberOfBytes;
        }

        private UdpClient _client;
        private readonly UdpDecoder _decoder = new UdpDecoder();

        public UdpProtocol()
        {
            _client = new UdpClient(9761);


        }


        public async Task<RakoCommand> ReceiveMessageAsync(CancellationToken cancel)
        {
            while (!cancel.IsCancellationRequested)
            {
                try
                {
                    var result = await _client.ReceiveAsync();
                    var command = _decoder.Decode(result.Buffer);
                    return command;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            cancel.ThrowIfCancellationRequested();
            return null;
        }

    }
}
