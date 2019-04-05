﻿using System.Collections.Generic;
using System.Linq;
using QBitNinja.Client.Models;

namespace StandPoint.Bitcoin.Histories
{
    public class AddressHistory : History
    {
        public AddressHistory(string address, IEnumerable<BalanceOperation> operations) :
            base(operations.Select(operation => new AddressHistoryRecord(address, operation)).ToList())
        {
            Address = address;
        }

        public string Address { get; }
    }
}