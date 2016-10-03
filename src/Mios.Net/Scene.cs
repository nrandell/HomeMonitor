using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mios.Net
{
    public class Scene
    {
        public string Active { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public int Room { get; set; }
        public int? State { get; set; }
        public string Comment { get; set; }

    }
}
