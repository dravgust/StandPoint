using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace StandPoint.Net.WebSockets
{
    public interface IWebSocketHandler
    {
        Task OnError(Exception exception);
        Task OnMessage(WebSocket webSocket, string message);
        Task OnStart(WebSocket ws);
        Task OnConnected(WebSocket webSocket);
        Task OnDisconected(WebSocket webSocket);
    }
}
