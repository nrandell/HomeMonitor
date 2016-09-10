using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OilPalScraper
{
    public class OilLevel
    {
        public DateTime Day { get; set; }
        public int Litres { get; set; }

        public override string ToString() => $"{Day} = {Litres}";
    }
}
