using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StandPoint.Blockchain
{
    [DataContract]
    public class ProtocolMessage
    {
        [DataMember, JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type { set; get; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<Block> Data { set; get; }

        public ProtocolMessage()
        {
            
        }

        public ProtocolMessage(MessageType type)
        {
            Type = type;
        }

        public ProtocolMessage(List<Block> chain)
        {
            Type = MessageType.RESPONSE_BLOCKCHAIN;
            Data = chain;
        }

        public static ProtocolMessage QueryChainLengthBlockchainMessage =>
            new ProtocolMessage(MessageType.QUERY_LATEST);

        public static ProtocolMessage QueryAllBlockchainMessage =>
            new ProtocolMessage(MessageType.QUERY_ALL);

        public static ProtocolMessage ResponseLatestMessage(Block block) =>
            new ProtocolMessage(new List<Block> { block });

        public static ProtocolMessage ResponseChainMessage(List<Block> chain) =>
            new ProtocolMessage(chain);
    }
}
