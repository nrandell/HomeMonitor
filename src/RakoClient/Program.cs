using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rako.Net;

namespace RakoClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                RunAsync(CancellationToken.None).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private static async Task RunAsync(CancellationToken token)
        {
            var client = new UdpProtocol();
            while (!token.IsCancellationRequested)
            {
                var command = await client.ReceiveMessageAsync(token);
                Console.WriteLine($"{DateTime.UtcNow}: {command}");
            }

        }
    }
}
