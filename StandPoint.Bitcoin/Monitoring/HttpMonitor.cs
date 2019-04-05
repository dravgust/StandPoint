using System;
using System.Threading.Tasks;
using NBitcoin;
using QBitNinja.Client;
using StandPoint.Bitcoin.Balances;
using StandPoint.Bitcoin.Histories;

namespace StandPoint.Bitcoin.Monitoring
{
    public class HttpMonitor : Monitor
    {
        protected readonly QBitNinjaClient Client;

        public HttpMonitor(string baseAddress, Network network) : base(network)
        {
            if (string.IsNullOrEmpty(baseAddress))
                throw new ArgumentNullException(nameof(baseAddress));

            Client = new QBitNinjaClient(baseAddress, _Network);
        }

        public override AddressBalanceInfo GetAddressBalanceInfo(string address)
        {
            AssertNetwork(new BitcoinPubKeyAddress(address).Network);

            var confirmedBalance = 0m;
            var unconfirmedBalance = 0m;

            foreach (var record in GetAddressHistory(address).Records)
                if (record.Confirmed)
                    confirmedBalance += record.Amount;
                else unconfirmedBalance += record.Amount;

            return new AddressBalanceInfo(address, unconfirmedBalance, confirmedBalance);
        }

        public override async Task<TransactionInfo> GetTransactionInfoAsync(string transactionId)
        {
            // TODO AssertNetwork(can you get network from transactionId?);

            var transactionIdUint256 = new uint256(transactionId);
            var transactionResponse = await Client.GetTransaction(transactionIdUint256).ConfigureAwait(false);
            
            return new TransactionInfo(transactionResponse, Network);
        }

        public override AddressHistory GetAddressHistory(string address)
        {
            var nBitcoinAddress = new BitcoinPubKeyAddress(address);
            AssertNetwork(nBitcoinAddress.Network);

            var operations = Client.GetBalance(new BitcoinPubKeyAddress(address)).Result.Operations;

            return new AddressHistory(address, operations);
        }
    }
}