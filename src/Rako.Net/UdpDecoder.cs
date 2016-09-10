using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rako.Net
{
    public class UdpDecoder
    {
        public RakoCommand Decode(byte[] buffer)
        {
            var identifier = (UdpCommands)buffer[0];
            switch (identifier)
            {
                case UdpCommands.Discover:
                case UdpCommands.Query:
                case UdpCommands.RemoteCommand:
                    throw new InvalidOperationException($"Unsupported command: {identifier}");
                case UdpCommands.SceneCache: return new SceneCache(buffer);
                case UdpCommands.Status: return new Command(buffer);
                default: throw new InvalidOperationException($"Invalid command: {identifier}");

            }
        }
    }
}
