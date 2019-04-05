using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace StandPoint.Net.WebSockets
{
    public interface IConnectionManager: IDisposable
    {
        void AddSocket(WebSocket webSocket);

        Task RemoveSocket(string id);

        WebSocket GetSocketById(string id);

        ConcurrentDictionary<string, WebSocket> GetAll();

        string GetId(WebSocket webSocket);
    }
}
