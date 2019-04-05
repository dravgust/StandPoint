using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace StandPoint.Security.Cryptography
{
    /// <summary>
    ///   Provides data for the unknown hashing algorithm event.
    /// </summary>
    public class UnknownHashingAlgorithmEventArgs : EventArgs
    {
        /// <summary>
        ///   The <see cref="HashingAlgorithm"/> that is defined for the
        ///   unknown hashing number.
        /// </summary>
        public HashingAlgorithm Algorithm { get; set; }
    }

    /// <summary>
    ///   Metadata and implementation of an hashing algorithm.
    /// </summary>
    public class HashingAlgorithm
    {
        internal static Dictionary<string, HashingAlgorithm> Names = new Dictionary<string, HashingAlgorithm>();
        internal static HashingAlgorithm[] Codes = new HashingAlgorithm[0x100];

        /// <summary>
        ///   The name of the algorithm.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///   The number assigned to the hashing algorithm.
        /// </summary>
        /// <remarks>
        ///   0x00-0x0f reserved for application specific functions. <br/>
        ///   0x10-0x3f reserved for SHA standard functions.
        /// </remarks>
        public byte Code { get; private set; }

        /// <summary>
        ///   The size, in bytes, of the digest value.
        /// </summary>
        public byte DigestSize { get; private set; }

        /// <summary>
        ///   Returns a cryptographic hash algorithm that can compute
        ///   a hash (digest).
        /// </summary>
        public Func<HashAlgorithm> Hasher { get; private set; }

        /// <summary>
        ///   Use <see cref="Register"/> to create a new instance of a <see cref="HashingAlgorithm"/>.
        /// </summary>
        private HashingAlgorithm()
        {
        }

        /// <summary>
        ///   The <see cref="Name"/> of the hashing algorithm.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        ///   Register a new hashing algorithm.
        /// </summary>
        /// <param name="name">
        ///   The name of the algorithm.
        /// </param>
        /// <param name="code">
        ///   The IPFS number assigned to the hashing algorithm.
        /// </param>
        /// <param name="digestSize">
        ///   The size, in bytes, of the digest value.
        /// </param>
        /// <param name="hasher">
        ///   A <c>Func</c> that a <see cref="HashAlgorithm"/>.  If not specified, then a <c>Func</c> is created to
        ///   return a <see cref="NotImplementedException"/>.
        /// </param>
        /// <returns>
        ///   A new <see cref="HashingAlgorithm"/>.
        /// </returns>
        public static HashingAlgorithm Register(string name, byte code, byte digestSize, Func<HashAlgorithm> hasher = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (Names.ContainsKey(name))
                throw new ArgumentException(string.Format("The hashing algorithm '{0}' is already defined.", name));
            if (Codes[code] != null)
                throw new ArgumentException(string.Format("The hashing algorithm code 0x{0:x2} is already defined.", code));
            if (hasher == null)
                hasher = () => throw new NotImplementedException(string.Format("The hashing algorithm '{0}' is not implemented.", name));

            var a = new HashingAlgorithm
            {
                Name = name,
                Code = code,
                DigestSize = digestSize,
                Hasher = hasher
            };
            Names[name] = a;
            Codes[code] = a;

            return a;
        }

        /// <summary>
        ///   A sequence consisting of all <see cref="HashingAlgorithm">hashing algorithms</see>.
        /// </summary>
        public static IEnumerable<HashingAlgorithm> All => Names.Values;
    }
}
