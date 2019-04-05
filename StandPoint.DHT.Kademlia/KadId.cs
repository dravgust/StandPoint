using StandPoint.Utilities;

namespace StandPoint.DHT.Kademlia
{
    public class KadId
    {
        public const int HASH_SIZE = 256;
        public const int TOTAL_BITS = HASH_SIZE * 8;

        private byte[] _id;

        public KadId()
        {
            _id = new byte[HASH_SIZE];
        }

        public byte[] Value
        {
            get => _id;
        }

        public void Set(int index, byte value)
        {
            _id[index] = value;
        }

        /// <summary>
        /// returns the distance between the two nodes using the kademlia XOR-metric
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static KadId Distance(KadId n1, KadId n2)
        {
            Guard.NotNull(n1, nameof(n1));
            Guard.NotNull(n2, nameof(n2));

            var ret = new KadId();
            for (var i = 0; i < HASH_SIZE; i++)
            {
                ret.Set(i, (byte)(n1.Value[i] ^ n2.Value[i]));
            }
            return ret;
        }

        /// <summary>
        /// returns -1 if: distance(n1, target) &lt; distance(n2, target) 
        /// returns 1 if: distance(n1, target) &gt; distance(n2, target) 
        /// returns 0 if: distance(n1, target) == distance(n2, target)
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int DistanceTarget(KadId n1, KadId n2, KadId target)
        {
            for (var i = 0; i != HASH_SIZE; i++)
            {
                var lhs = (n1.Value[i] ^ target.Value[i]) & 0xFF;
                var rhs = (n2.Value[i] ^ target.Value[i]) & 0xFF;

                if (lhs < rhs) return -1;
                if (lhs > rhs) return 1;
            }

            return 0;
        }

        /// <summary>
        /// returns n in: 2^n &gt;= distance(n1, n2) &gt; 2^(n+1) useful for finding out which bucket a node belongs to
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static int DistanceExp(KadId n1, KadId n2)
        {
            var bt = HASH_SIZE - 1;
            for (var i = 0; i != HASH_SIZE; i++, bt--)
            {
                Guard.Assert(bt < 0);
                var t = (n1.Value[i] ^ n2.Value[i]) & 0xFF;
                if(t == 0) continue;
                Guard.Assert(t < 0);
                // we have found the first non-zero byte
                // return the bit-number of the first bit
                // that differs
                var bit = bt * 8;
                for (var b = 7; b >= 0; b--)
                {
                    if (t >= (1 << b)) return bit + b;
                }
                return bit;
            }
            return 0;
        }
    }
}
