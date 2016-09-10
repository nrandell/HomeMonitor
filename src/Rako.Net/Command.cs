using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rako.Net
{
    public class Command : RakoCommand
    {
        public Command() { }
        public Command(byte[] buffer)
        {
            var numberOfBytes = UdpProtocol.VerifyMessage(buffer, UdpCommands.Status, 4);

            Room = (int)((uint)(buffer[2] << 8) + (uint)buffer[3]);
            Channel = buffer[4] & 0x0FF;
            Instruction = buffer[5] & 0x0FF;
            if (numberOfBytes > 5)
            {
                Data = new byte[numberOfBytes - 5];
                Array.Copy(buffer, 6, Data, 0, numberOfBytes - 5);
            }
        }

        public int Room { get; set; }
        public int Channel { get; set; }
        public int Instruction { get; set; }
        public byte[] Data { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Room).Append(".").Append(Channel).Append(": ").Append(Instruction);
            if (Data != null)
            {
                foreach (var parameter in Data)
                {
                    sb.Append(" ").Append(parameter);
                }
            }
            return sb.ToString();
        }
    }
}
