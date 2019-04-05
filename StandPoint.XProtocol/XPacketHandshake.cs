using StandPoint.XProtocol.Serializer;

namespace StandPoint.XProtocol
{
    public class XPacketHandshake
    {
        [XField(1)]
        public int MagicHandshakeNumber;
    }
}
