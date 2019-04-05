﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StandPoint.Security.Cryptography.Serialization
{
    internal class MerkleNodeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MerkleNode);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            MerkleNode merkleNode = new MerkleNode();

            int startingDepth = reader.Depth;
            while (!(reader.TokenType == JsonToken.EndObject && startingDepth == reader.Depth))
            {
                reader.Read();

                if (reader.TokenType == JsonToken.PropertyName)
                {
                    string propertyName = reader.Value.ToString();

                    if (String.Equals(propertyName, "Data"))
                    {
                        reader.Read();
                        merkleNode.Data = serializer.Deserialize<byte[]>(reader);
                    }
                    else if (String.Equals(propertyName, "Hash"))
                    {
                        reader.Read();
                        merkleNode.Hash = serializer.Deserialize<MultiHash>(reader);
                    }
                    else if (String.Equals(propertyName, "Links"))
                    {
                        reader.Read();
                        merkleNode.Links = serializer.Deserialize<IEnumerable<MerkleNode>>(reader);
                    }
                    else if (String.Equals(propertyName, "Name"))
                    {
                        reader.Read();
                        merkleNode.Name = serializer.Deserialize<string>(reader);
                    }
                    else if (String.Equals(propertyName, "Size"))
                    {
                        reader.Read();
                        merkleNode.Size = serializer.Deserialize<long?>(reader);
                    }
                }
            }

            return merkleNode;
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if(value == null)
            {
                writer.WriteNull();
                return;
            }

            MerkleNode mn = value as MerkleNode;

            writer.WriteStartObject();

            writer.WritePropertyName("Data");
            serializer.Serialize(writer, mn.Data);

            writer.WritePropertyName("Hash");
            serializer.Serialize(writer, mn.Hash);

            writer.WritePropertyName("Links");
            serializer.Serialize(writer, mn.Links);

            writer.WritePropertyName("Name");
            serializer.Serialize(writer, mn.Name);

            writer.WritePropertyName("Size");
            serializer.Serialize(writer, mn.Size);

            writer.WriteEndObject();
        }
    }
}
