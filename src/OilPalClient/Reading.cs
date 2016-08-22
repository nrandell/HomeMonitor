using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OilPalClient
{
    public class Reading
    {
        public string Data {  get; }
        public string Device { get; }
        public string Flow { get; }
        public string ReceiveCount { get; }
        public string RfAddress { get; }
        public string RxTime { get; }

        public Reading(string device, string rfAddress, string receiveCount, string rxTime, string data, string flow)
        {
            this.Device = device;
            this.RfAddress = rfAddress;
            this.ReceiveCount = receiveCount;
            this.RxTime = rxTime;
            this.Data = data;
            this.Flow = flow;
        }

        public override string ToString() => $"{Device} {RfAddress} {ReceiveCount} {RxTime} {ReceiveCount} {Data} {Flow}";
    }
}
