using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OilPalClient
{
    public class Reading
    {
        private string data;
        private string device;
        private string flow;
        private string receiveCount;
        private string rtAddress;
        private string rxTime;

        public Reading(string device, string rtAddress, string receiveCount, string rxTime, string data, string flow)
        {
            this.device = device;
            this.rtAddress = rtAddress;
            this.receiveCount = receiveCount;
            this.rxTime = rxTime;
            this.data = data;
            this.flow = flow;
        }

        public override string ToString() => $"{device} {rtAddress} {receiveCount} {rxTime} {receiveCount} {data} {flow}";
    }
}
