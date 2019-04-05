using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using StandPoint.Utilities;
using StandPoint.Utilities.Encoders;

namespace StandPoint.Security.Cryptography
{
    /// <summary> 
    ///   A protocol for differentiating outputs from various well-established cryptographic hash functions, 
    ///   addressing size + encoding considerations.
    /// </summary>
    /// <seealso href="https://github.com/jbenet/multihash"/>
    public class MultiHash : IEquatable<MultiHash>
    {
        /// <summary>
        ///   Register the standard hash algorithms.
        /// </summary>
        static MultiHash()
        {
            HashingAlgorithm.Register("sha1", 0x11, 20, SHA1.Create);
            HashingAlgorithm.Register("sha2-256", 0x12, 32, SHA256.Create);
            HashingAlgorithm.Register("sha2-512", 0x13, 64, SHA512.Create);
            //HashingAlgorithm.Register("sha3-512", 0x14, 64, () => { return new SHA3.SHA3Managed(512); });
            //HashingAlgorithm.Register("blake2b", 0x40, 64);
            //HashingAlgorithm.Register("blake2s", 0x41, 32);
        }

        /// <summary>
        ///   The default hashing algorithm is "sha2-256".
        /// </summary>
        public const string DefaultAlgorithmName = "sha2-256";

        public byte[] Digest { get; }

        /// <summary>
        ///   The hashing algorithm.
        /// </summary>
        public HashingAlgorithm Algorithm { get; private set; }

        /// <summary>
        ///   Gets the <see cref="HashAlgorithm"/> with the specified multi-hash name.
        /// </summary>
        public static HashAlgorithm GetHashAlgorithm(string name = DefaultAlgorithmName)
        {
            return HashingAlgorithm.Names[name].Hasher();
        }

        /// <summary>
        ///   Occurs when an unknown hashing algorithm number is parsed.
        /// </summary>
        public static EventHandler<UnknownHashingAlgorithmEventArgs> UnknownHashingAlgorithm;

        /// <summary>
        ///   Creates a new instance of the <see cref="MultiHash"/> class with the
        ///   specified <see cref="HashingAlgorithm">Algorithm name</see> and <see cref="Digest"/> value.
        /// </summary>
        /// <param name="algorithmName">
        ///   A valid IPFS hashing algorithm name, e.g. "sha2-256" or "sha2-512".
        /// </param>
        /// <param name="digest">
        ///    The digest value as a byte array.
        /// </param>
        public MultiHash(string algorithmName, byte[] digest)
        {
            Guard.NotNull(algorithmName, nameof(algorithmName));
            Guard.NotNull(digest, nameof(digest));

            HashingAlgorithm a;
            if (!HashingAlgorithm.Names.TryGetValue(algorithmName, out a))
                throw new ArgumentException(string.Format("The hashing algorithm '{0}' is unknown.", algorithmName));
            Algorithm = a;

            if (Algorithm.DigestSize != digest.Length)
                throw new ArgumentException(string.Format("The digest size for '{0}' is {1} bytes, not {2}.", algorithmName, Algorithm.DigestSize, digest.Length));
            Digest = digest;
        }

        public MultiHash(string base58) : this(Encoders.Base58.Decode(base58)) { }

        public MultiHash(byte[] data)
        {
            Guard.NotNull(data, nameof(data));
            
            if (data.Length < 2)
                throw new ArgumentOutOfRangeException(nameof(data));

            var fnCode = data[0];
            var size = data[1];

            ValidateAlgorithm(fnCode, size);

            Digest = data.Skip(2).ToArray();
        }

        public MultiHash(byte fnCode, byte size, byte[] digest)
        {
            Guard.NotNull(digest, nameof(digest));

            ValidateAlgorithm(fnCode, size);

            Digest = digest;
        }

        private void ValidateAlgorithm(byte fnCode, byte size)
        {
            Algorithm = HashingAlgorithm.Codes[fnCode];

            if (Algorithm == null)
            {
                Algorithm = HashingAlgorithm.Register("custom-" + fnCode, fnCode, size);
                RaiseUnknownHashingAlgorithm(Algorithm);
            }
            else if (size != Algorithm.DigestSize)
            {
                throw new InvalidDataException(string.Format("The digest size {0} is wrong for {1}; it should be {2}.", size, Algorithm.Name, Algorithm.DigestSize));
            }
        }

        /// <summary>
        ///   Generate the multihash for the specified data. 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="algorithmName">
        ///   The name of the hashing algorithm to use; defaults to <see cref="DefaultAlgorithmName"/>.
        /// </param>
        /// <returns>
        ///   A <see cref="MultiHash"/> for the <paramref name="data"/>.
        /// </returns>
        public static MultiHash ComputeHash(byte[] data, string algorithmName = DefaultAlgorithmName)
        {
            using (var algorithm = GetHashAlgorithm(algorithmName))
            {
                var hash = algorithm.ComputeHash(data);
                return new MultiHash(algorithmName, hash);
            }         
        }

        public bool Equals(MultiHash other)
        {
            return other != null && Equals(other.Digest, Digest);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this)) return true;

            var other = obj as MultiHash;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Digest.GetHashCode();
        }

        public static explicit operator string(MultiHash multiHash)
        {
            return multiHash.ToString();
        }

        public override string ToString()
        {
            return Encoders.Base58.Encode(Digest);
        }

        private void RaiseUnknownHashingAlgorithm(HashingAlgorithm algorithm)
        {
            //"Unknown hashing algorithm number 0x{0:x2}.", algorithm.Code
            var handler = UnknownHashingAlgorithm;
            if (handler != null)
            {
                var args = new UnknownHashingAlgorithmEventArgs { Algorithm = algorithm };
                handler(this, args);
            }
        }
    }
}
