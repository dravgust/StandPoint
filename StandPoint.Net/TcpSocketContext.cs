using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace StandPoint.Net
{
    public class TcpSocketContext : ISocketContext
    {
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _networkStream;
        private readonly CancellationTokenSource _tokenSource;

        public readonly ConcurrentDictionary<string, object> Cache;
        public NetworkReader Request { get; }
        public NetworkStreamWrtiter Response { get; }

        public IPEndPoint RemoteEndPoint => (IPEndPoint)_tcpClient.Client.RemoteEndPoint;

        public TcpSocketContext(TcpClient tcpClient, byte[] endMarker = null, byte[] startMarker = null)
        {
            _tcpClient = tcpClient ?? throw new ArgumentNullException(nameof(tcpClient));
            _networkStream = tcpClient.GetStream();
            _tokenSource = new CancellationTokenSource();

            this.Cache = new ConcurrentDictionary<string, object>();

            this.Request = new NetworkStreamReader(
                stream: _networkStream,
                token: _tokenSource.Token,
                endMarker: endMarker,
                startMarker: startMarker,
                bufferSize: _tcpClient.ReceiveBufferSize);

            this.Response = new NetworkStreamWrtiter(
                stream: _networkStream,
                token: _tokenSource.Token,
                endMarker: endMarker,
                startMarker: startMarker,
                bufferSize: _tcpClient.SendBufferSize);
        }

        public void Close()
        {
            _tokenSource.Cancel();
            _tcpClient.Close();
        }

        public void Dispose()
        {
            this.Request.Dispose();
            _networkStream.Dispose();
            _tokenSource.Dispose();
        }
    }
}
