using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rako.Net
{
    public class RoomCache
    {
        public int Room { get; set; }
        public int Scene { get; set; }

        public override string ToString() => $"{Room} = {Scene}";
    }
}
