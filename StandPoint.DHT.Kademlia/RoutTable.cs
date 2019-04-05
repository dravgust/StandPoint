using System;
using System.Collections.Generic;
using StandPoint.Utilities;

namespace StandPoint.DHT.Kademlia
{
    public class RoutTable
    {
        private List<KBucket> _buckets;
        private KadId _self;

        public RoutTable(KadId self)
        {
            Guard.NotNull(self, nameof(self));

            _buckets = new List<KBucket>();
            _self = self;
        }

        public int FindBucket(KadId id)
        {
            int numBukets = _buckets.Count;
            if (numBukets == 0)
            {
                _buckets.Add(new KBucket());
                numBukets++;
            }

            int bucketIndex = Math.Min(KadId.TOTAL_BITS - 1 - KadId.DistanceExp(_self, id), numBukets - 1);
            Guard.Assert(bucketIndex < _buckets.Count);
            Guard.Assert(bucketIndex >= 0);
            return bucketIndex;
        }
    }
}
