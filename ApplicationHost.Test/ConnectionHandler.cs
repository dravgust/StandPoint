using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StandPoint.Blockchain;
using StandPoint.Net.WebSockets;
using StandPoint.Utilities;
using StandPoint.Utilities.Json;
using DataContractJsonSerializer = StandPoint.Utilities.Json.DataContractJsonSerializer;

namespace ApplicationHost.Test
{
	public class ConnectionHandler : IWebSocketHandler
    {
        private readonly BlockchainService _blockchain;
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<ConnectionHandler> _logger;

        public ConnectionHandler(IConnectionManager connectionManager, BlockchainService blockchain, ILogger<ConnectionHandler> logger)
        {
            Guard.NotNull(connectionManager, nameof(connectionManager));
            Guard.NotNull(blockchain, nameof(blockchain));
            Guard.NotNull(logger, nameof(logger));

            _connectionManager = connectionManager;
            _blockchain = blockchain;
            _logger = logger;
        }

        public async Task OnStart(WebSocket ws)
        {
            await ws.WriteAsync(ProtocolMessage.QueryChainLengthBlockchainMessage);
        }

        public Task OnError(Exception e)
        {
            _logger.LogError($"{e.Message}\r\n{e.StackTrace}");
            return Task.CompletedTask;
        }

        public async Task OnConnected(WebSocket webSocket)
        {
            
            await Task.CompletedTask;
        }

        public async Task OnDisconected(WebSocket webSocket)
        {
            await _connectionManager.RemoveSocket(_connectionManager.GetId(webSocket));
        }

        public async Task OnMessage(WebSocket ws, string message)
        {
	        var protocolMessage = message.FromJSON<ProtocolMessage>(new DataContractJsonSerializer());

			_logger.LogDebug($"Message: {message}");

            try
            {
                switch (protocolMessage.Type)
                {
                    case MessageType.QUERY_LATEST:
                        await ws.WriteAsync(ProtocolMessage.ResponseLatestMessage(_blockchain.GetLatestBlock()));
                        break;
                    case MessageType.QUERY_ALL:
                        await ws.WriteAsync(ProtocolMessage.ResponseChainMessage(_blockchain.Get()));
                        break;
                    case MessageType.RESPONSE_BLOCKCHAIN:
                        var reseivedBlocks = protocolMessage.Data;
                        reseivedBlocks.Sort();

                        var lastBlockReceived = reseivedBlocks.Last();
                        var latestBlockHeld = _blockchain.GetLatestBlock();

                        if (lastBlockReceived.Head > latestBlockHeld.Head)
                        {
                            if (_blockchain.IsValidBlock(lastBlockReceived, latestBlockHeld))
                            {
                                _logger.LogDebug("We can append the received block to our chain");
                                _blockchain.Add(lastBlockReceived);
                                await this.SendMessageToAllAsync(ProtocolMessage.ResponseLatestMessage(_blockchain.GetLatestBlock()).ToJSON(new DataContractJsonSerializer()));
                            }
                            else if (reseivedBlocks.Count == 1)
                            {
                                _logger.LogDebug("We have to query the chain from our peer");
                                await this.SendMessageToAllAsync(ProtocolMessage.QueryAllBlockchainMessage.ToJSON(new DataContractJsonSerializer()));
                            }
                            else
                            {
                                _logger.LogDebug("Received blockchain is longer than current blockchain");
                                _blockchain.ReplaceChain(reseivedBlocks);
                            }
                        }
                        else
                        {
                            _logger.LogDebug("Received blockchain is not longer than received blockchain. Do nothing");
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Unknown MessageType: {protocolMessage.Type}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\r\n{e.StackTrace}");
            }
        }

        public async Task SendMessageAsync(string socketId, string message)
        {
            await _connectionManager.GetSocketById(socketId).WriteAsync(message);
        }

        public async Task SendMessageToAllAsync(string message)
        {
            foreach (var pair in _connectionManager.GetAll())
            {
                await pair.Value.WriteAsync(message);
            }
        }
    }
}
