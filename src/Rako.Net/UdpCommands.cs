using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rako.Net
{
    public enum UdpCommands
    {
        Discover = 'D',
        RemoteCommand = 'R',
        Query = 'Q',
        SceneCache = 'C',
        Status = 'S'
    }
}
