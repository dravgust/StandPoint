using System.Collections.Generic;
using Newtonsoft.Json;

namespace StandPoint.Security.Cryptography.Serialization
{
    public class JsonSerializer : Utilities.Json.JsonSerializer
    {
        public JsonSerializer() : base(new List<JsonConverter>
        {
            new MultiHashConverter(),
            new MerkleNodeConverter(),
        }){ }
    }
}
