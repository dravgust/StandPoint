using System.Collections.Generic;

namespace StandPoint.IO.IPFS.Json
{
    public class IpfsBitSwapStat
    {
        public int ProvideBufLen { get; set; }
        public object Wantlist { get; set; }
        public List<string> Peers { get; set; }
        public int BlocksReceived { get; set; }
        public int DupBlksReceived { get; set; }
        public int DupDataReceived { get; set; }
    }
}
