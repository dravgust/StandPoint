using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace StandPoint.Net.WebSockets
{
    public class EchoSocketHandler : IWebSocketHandler
    {

        public async Task OnError(Exception exception)
        {
            await Task.Yield();
        }

        public async Task OnMessage(WebSocket webSocket, string message)
        {
            await webSocket.WriteAsync(message);
        }

        public async Task OnStart(WebSocket ws)
        {
            await Task.Yield();
        }

        public async Task OnConnected(WebSocket webSocket)
        {
            await Task.Yield();
        }

        public async Task OnDisconected(WebSocket webSocket)
        {
            await Task.Yield();
        }
    }
}
