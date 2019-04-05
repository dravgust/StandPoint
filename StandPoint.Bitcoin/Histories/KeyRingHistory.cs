using System.Collections.Generic;
using System.Linq;
using StandPoint.Bitcoin.KeyManagement;

namespace StandPoint.Bitcoin.Histories
{
    public class KeyRingHistory : History
    {
        public KeyRingHistory(KeyRing keyRing, IEnumerable<AddressHistory> addressHistories)
            : base(addressHistories.SelectMany(addressHistory => addressHistory.Records).ToList())
        {
            KeyRing = keyRing;
        }

        public KeyRing KeyRing { get; }
    }
}