using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mios.Net
{
    public class SummaryData
    {
        public string Full { get; set; }
        public string Version { get; set; }
        public string Model { get; set; }
        public string ZWaveHeal { get; set; }
        public string Temperature { get; set; }
        public string SerialNumber { get; set; }
        public string Fwd1 { get; set; }
        public string Fwd2 { get; set; }
        public int Mode { get; set; }
        public string Ir { get; set; }
        public string IrTx { get; set; }
        public long LoadTime { get; set; }
        public long DataVersion { get; set; }
        public string State { get; set; }
        public string Comment { get; set; }

        public Section[] Sections { get; set; }
        public Room[] Rooms { get; set; }
        public Scene[] Scenes { get; set; }
        public JObject[] Devices { get; set; }
        public Category[] Categories { get; set; }


    }
}
