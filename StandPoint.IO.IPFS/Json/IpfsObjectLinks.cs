using System.Collections.Generic;

namespace StandPoint.IO.IPFS.Json
{
    public class Link
    {
        public string Name { get; set; }
        public string Hash { get; set; }
        public int Size { get; set; }
    }

    public class IpfsObjectLinks
    {
        public string Hash { get; set; }
        public List<Link> Links { get; set; }
    }
}
