using System.Collections.Generic;
using StandPoint.Security.Cryptography;

namespace StandPoint.IO.IPFS
{
    public class IpfsID
    {
        public MultiHash ID { get; set; }
        public string PublicKey { get; set; }
        public List<MultiAddress> Addresses { get; set; }
        public string AgentVersion { get; set; }
        public string ProtocolVersion { get; set; }
    }
}
