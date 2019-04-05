using System;
using System.IO;
using StandPoint.Security.Cryptography;
using StandPoint.Utilities;
using StandPoint.Utilities.Encoders;

namespace StandPoint.Blockchain
{
    [BlockchainSerializable]
    public class BlockHeader
    {
        public static int CURRENT_VERSION = 1;

        private int _nVersion;
        private uint _nNonce;
        private uint _nTime;
        private byte[] _hashMerkleRoot;

        public int Version
        {
            get => _nVersion;
            set => _nVersion = value;
        }

        public uint Nonce
        {
            get => _nNonce;
            set => _nNonce = value;
        }

        public uint Time
        {
            get => _nTime;
            set => _nTime = value;
        }
        public DateTimeOffset BlockTime
        {
            get => DateTimeUtils.UnixTimeToDateTime(this._nTime);
            set => _nTime = DateTimeUtils.DateTimeToUnixTime(value);
        }

        public MultiHash HashPrevBlock { get; set; }

        public byte[] HashMerkleRoot
        {
            get => _hashMerkleRoot;
            set => _hashMerkleRoot = value;
        }

	    public BlockHeader()
	    {
		    
	    }

        public BlockHeader(string hex)
            : this(Encoders.Hex.Decode(hex))
        {

        }

        public BlockHeader(byte[] bytes)
        {
            using (var stream = new BlockchainStream(bytes))
            {
                stream.Read(out this._nVersion);
                var hash = new byte[256];
                stream.Read(ref hash);
                this.HashPrevBlock = new MultiHash(hash);
                stream.Read(ref this._hashMerkleRoot);
                stream.Read(out this._nTime);
                stream.Read(out this._nNonce);
            }
        }

        public MultiHash GetHash()
        {
            using (var ms = new MemoryStream())
            {
                using (var stream = new BlockchainStream(ms))
                {
                    stream.Write(this._nVersion);
                    stream.Write(this.HashPrevBlock.Digest);
                    stream.Write(this._hashMerkleRoot);
                    stream.Write(this._nTime);
                    stream.Write(this._nNonce);

                    return MultiHash.ComputeHash(ms.ToArray());
                }
            }
        }

        public override string ToString()
        {
            return this.GetHash().ToString();
        }
    }
}
