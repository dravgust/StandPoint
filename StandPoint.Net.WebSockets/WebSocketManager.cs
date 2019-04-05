using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using StandPoint.Utilities;

namespace StandPoint.Net.WebSockets
{
    public class WebSocketManager : IConnectionManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets;

        public WebSocketManager()
        {
            this._sockets = new ConcurrentDictionary<string, WebSocket>();
        }

        public void AddSocket(WebSocket webSocket)
        {
            Guard.NotNull(webSocket, nameof(webSocket));

            _sockets.TryAdd($"{Guid.NewGuid()}", webSocket);
        }

        public async Task RemoveSocket(string id)
        {
            Guard.NotEmpty(id, nameof(id));

            _sockets.TryRemove(id, out WebSocket webSocket);

            await webSocket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure, 
                statusDescription: "Closed by the WebSocketConnectionManager",
                cancellationToken: CancellationToken.None);
        }

        public WebSocket GetSocketById(string id)
        {
            Guard.NotEmpty(id, nameof(id));

            return _sockets.FirstOrDefault(s => s.Key == id).Value;
        }

        public ConcurrentDictionary<string, WebSocket> GetAll()
        {
            return _sockets;
        }

        public string GetId(WebSocket webSocket)
        {
            Guard.NotNull(webSocket, nameof(webSocket));

            return _sockets.FirstOrDefault(s => s.Value == webSocket).Key;
        }

        public void Dispose()
        {
            foreach (var item in _sockets)
            {
                _sockets.TryRemove(item.Key, out var webSocket);
                webSocket?.Dispose();
            }
        }
    }
}
    