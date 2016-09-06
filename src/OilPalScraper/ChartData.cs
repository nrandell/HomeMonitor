using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace OilPalScraper
{
    [DataContract]
    public class ChartData
    {
        [DataMember(Name = "Label")]
        public string[] Label { get; set; }

        [DataMember(Name = "Data")]
        public string[] Data { get; set; }
    }
}
