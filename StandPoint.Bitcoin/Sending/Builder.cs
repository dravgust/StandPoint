﻿using System;
using System.Collections.Generic;
using StandPoint.Bitcoin.Interfaces;

namespace StandPoint.Bitcoin.Sending
{
    public abstract class Builder : IAssertNetwork
    {
        // ReSharper disable once InconsistentNaming
        protected readonly NBitcoin.Network _Network;

        protected Builder(Network network)
        {
            _Network = network.ToNBitcoinNetwork();
        }

        public Network Network => _Network.ToHiddenBitcoinNetwork();

        public void AssertNetwork(Network network)
        {
            if (network != Network)
                throw new Exception("Wrong network");
        }

        public void AssertNetwork(NBitcoin.Network network)
        {
            if (network != _Network)
                throw new Exception("Wrong network");
        }

        public TransactionInfo BuildSpendAllTransaction(List<string> fromPrivateKeys, string toAddress,
            FeeType feeType = FeeType.Fastest, string message = "")
        {
            var addressAmountPair = new AddressAmountPair
            {
                Address = toAddress,
                Amount = 0 // doesn't matter, we send all
            };

            return BuildTransaction(
                fromPrivateKeys,
                new List<AddressAmountPair> {addressAmountPair},
                feeType,
                message: message,
                spendAll: true
                );
        }

        /// <summary>
        /// </summary>
        /// <param name="fromPrivateKeys"></param>
        /// <param name="to"></param>
        /// <param name="feeType"></param>
        /// <param name="changeAddress"></param>
        /// <param name="message"></param>
        /// <param name="spendAll">If true changeAddress and amounts of to does not matter, we send them all</param>
        /// <returns></returns>
        public abstract TransactionInfo BuildTransaction(List<string> fromPrivateKeys, List<AddressAmountPair> to,
            FeeType feeType = FeeType.Fastest, string changeAddress = "", string message = "", bool spendAll = false);
    }
}