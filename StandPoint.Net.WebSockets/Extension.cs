using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StandPoint.Utilities.Json;

namespace StandPoint.Net.WebSockets
{
    public static class Extension
    {
        //public static async Task WriteAsync(this WebSocket ws, byte[] message)
        //{
        //    var buffer = new ArraySegment<byte>(message);
        //    await ws.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
        //}
        private const int ChunkSize = 4096;

        public static async Task WriteAsync(this WebSocket ws, string message)
        {
            if(ws.State != WebSocketState.Open)
                throw new InvalidOperationException("Connection is not open");

            var buffer = Encoding.Default.GetBytes(message);
            var messageCount = (int) Math.Ceiling((double) buffer.Length / 4096);

            for (var i = 0; i < messageCount; i++)
            {
                var offset = ChunkSize * i;
                var count = ChunkSize;
                var lastMessage = (i + 1) == messageCount;

                if ((count * (i + 1)) > buffer.Length)
                {
                    count = buffer.Length - offset;
                }

                await ws.SendAsync(new ArraySegment<byte>(buffer, offset, count), WebSocketMessageType.Text, lastMessage, CancellationToken.None);
            }
            //await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public static async Task WriteAsync<T>(this WebSocket ws, T message) where T: class 
        {
            await WriteAsync(ws, message.ToJSON(new DataContractJsonSerializer()));
        }

        public static async Task ReceiveAsync(this WebSocket ws, IWebSocketHandler handler)
        {
            try
            {
                while (ws.State == WebSocketState.Open)
                {
                    var stringResult = new StringBuilder();
                    WebSocketReceiveResult received;
                    var buffer = new ArraySegment<byte>(new byte[ChunkSize]);
                    var webSocket = ws;
                    do
                    {
                        received = await ws.ReceiveAsync(buffer, CancellationToken.None);
                        switch (received.MessageType)
                        {
                            case WebSocketMessageType.Close:
                                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                                await handler.OnDisconected(webSocket);
                                break;
                            case WebSocketMessageType.Binary:
                                break;
                            case WebSocketMessageType.Text:
                                var data = Encoding.Default.GetString(buffer.Array, buffer.Offset, buffer.Count);
                                stringResult.Append(data.TrimEnd('\0'));
                                break;
                            default:
                                throw new ArgumentOutOfRangeException($"Unknown WebSocket MessageType: {received.MessageType}");
                        }
                    } while (!received.EndOfMessage);

                    RunInTask(async ()=> await handler.OnMessage(webSocket, stringResult.ToString()));
                }
            }
            catch (Exception e)
            {
                await handler.OnError(e);
                await handler.OnDisconected(ws);
            }
            finally
            {
                ws.Dispose();
            }
        }

        private static void RunInTask(Action action)
        {
            Task.Factory.StartNew(action);
        }
    }
}
