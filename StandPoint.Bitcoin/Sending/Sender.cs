using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.Protocol;
using QBitNinja.Client;
using StandPoint.Bitcoin.Monitoring;

namespace StandPoint.Bitcoin.Sending
{
    public abstract class Sender
    {
        internal static List<Transaction> BuiltTransactions = new List<Transaction>();

        public static async Task SendAsync(string baseAddress, ConnectionType connectionType, TransactionInfo transactionInfo, int tryTimes = 1)
        {
            var monitor = new HttpMonitor(baseAddress, transactionInfo.Network);

            if (connectionType == ConnectionType.Http)
            {
                var client = new QBitNinjaClient(baseAddress, transactionInfo.Network.ToNBitcoinNetwork());
                var transaction = FindTransaction(transactionInfo);

                var broadcastResponse = await client.Broadcast(transaction).ConfigureAwait(false);
                if (!broadcastResponse.Success)
                    throw new Exception($"ErrorCode: {broadcastResponse.Error.ErrorCode}" + Environment.NewLine
                                        + broadcastResponse.Error.Reason);
            }
            if (connectionType == ConnectionType.RandomNode)
            {
                var parameters = new NodeConnectionParameters();
                var group = new NodesGroup(transactionInfo.Network.ToNBitcoinNetwork(), parameters, new NodeRequirement
                {
                    RequiredServices = NodeServices.Nothing
                })
                { MaximumNodeConnection = 1 };
                group.Connect();

                while (group.ConnectedNodes.Count == 0)
                    await Task.Delay(100).ConfigureAwait(false);

                var transaction = FindTransaction(transactionInfo);
                var payload = new TxPayload(transaction);
                group.ConnectedNodes.First().SendMessage(payload);
            }

            for (var i = 0; i < 10; i++)
            {
                try
                {
                    var result = await monitor.GetTransactionInfoAsync(transactionInfo.Id);
                }
                catch (NullReferenceException exception)
                {
                    if (exception.Message != "Transaction does not exists") throw;
                    await Task.Delay(1000).ConfigureAwait(false);
                    continue;
                }
                if (i == 10)
                {
                    if (tryTimes == 1)
                        throw new Exception("Transaction has not been broadcasted, try again!");
                    await SendAsync(baseAddress, connectionType, transactionInfo, tryTimes - 1)
                        .ConfigureAwait(false);
                }
                break;
            }
        }

        protected static Transaction FindTransaction(TransactionInfo transactionInfo)
        {
            var tx = BuiltTransactions.FirstOrDefault(transaction => transaction.GetHash() == new uint256(transactionInfo.Id));
            if (tx != null) return tx;
            throw new Exception("Transaction has not been created");
        }
    }
}