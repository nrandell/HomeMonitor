using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mios.Net
{
    public class Device
    {
        public string Name { get; set; }
        public string AltId { get; set; }
        public int Id { get; set; }
        public int Category { get; set; }
        public int SubCategory { get; set; }
        public int Room { get; set; }
        public int Parent { get; set; }
        public string CommFailure { get; set; }
        public string Level { get; set; }
        public string Status { get; set; }
        public int? State { get; set; }
        public string Comment { get; set; }
        public string Watts { get; set; }
        public string Kwh { get; set; }
        public string Mode { get; set; }
        public string Setpoint { get; set; }
        public string Heat { get; set; }
        public string Cool { get; set; }
        public string Commands { get; set; }
        public string BatteryLevel { get; set; }
        public string Light { get; set; }
        public string Temperature { get; set; }
        public string Armed { get; set; }
        public string ArmedTripped { get; set; }
        public string LastTrip { get; set; }
        public string Tripped { get; set; }


    }
}
