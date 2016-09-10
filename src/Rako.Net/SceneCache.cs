using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rako.Net
{
    public class SceneCache : RakoCommand
    {
        public SceneCache() { }
        public SceneCache(byte[] buffer)
        {
            var numberOfBytes = UdpProtocol.VerifyMessage(buffer, UdpCommands.SceneCache, 3);
            var numberOfEntries = numberOfBytes / 2;
            var rooms = new RoomCache[numberOfEntries];
            for (var i = 0; i < numberOfEntries; i++)
            {
                var byte1 = buffer[2 + (2 * i)];
                var byte2 = buffer[3 + (2 * i)];
                var scene = (int)((byte1 >> 2) & 0x3F);
                var room = (int)((uint)(byte1 << 8) + (uint)byte2) & 0x3FF;
                rooms[i] = new RoomCache { Room = room, Scene = scene };
            }
            Rooms = rooms;

        }
        public RoomCache[] Rooms { get; set; }

        public override string ToString() => String.Join<RoomCache>(", ", Rooms);
    }
}
