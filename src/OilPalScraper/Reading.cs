using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OilPalScraper
{
    public class Reading
    {
        public DateTime TimestampUtc { get; set; }
        public OilLevel[] Levels { get; set; }

        public override string ToString() => $"{TimestampUtc}: {String.Join<OilLevel>(", ", Levels)}";
    }
}
