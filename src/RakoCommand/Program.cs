using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rako.Net;

namespace RakoCommand
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("usage: RakoCommand <room> <channel> <scene>");
            }
            else
            {
                try
                {
                    var room = int.Parse(args[0]);
                    var channel = int.Parse(args[1]);
                    var scene = int.Parse(args[2]);
                    RunAsync(room, channel, scene, CancellationToken.None).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

        }

        private static async Task RunAsync(int room, int channel, int scene, CancellationToken token)
        {
            using (var client = new RakoUdpClient("rako"))
            {
                var result = await client.SetScene(room, channel, scene);
                Console.WriteLine($"Got result as {result}");
            }
        }
    }
}
