using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace StandPoint.Net
{
    public class UdpSocketContext : ISocketContext
    {
        private readonly UdpReceiveResult _result;
        private readonly CancellationTokenSource _tokenSource;

        public readonly ConcurrentDictionary<string, object> Cache;
        public NetworkReader Request { get; }
        public NetworkStreamWrtiter Response { get; }

        public IPEndPoint RemoteEndPoint => (IPEndPoint)_result.RemoteEndPoint;

        public UdpSocketContext(UdpReceiveResult result, byte[] endMarker = null, byte[] startMarker = null)
        {
            _result = result;
            var buffer = result.Buffer;
            _tokenSource = new CancellationTokenSource();

            this.Cache = new ConcurrentDictionary<string, object>();

            this.Request = new NetworkBufferReader(
                buffer: buffer,
                token: _tokenSource.Token,
                endMarker: endMarker,
                startMarker: startMarker);
        }

        public void Close()
        {
            _tokenSource.Cancel();
        }

        public void Dispose()
        {
            this.Request.Dispose();
            _tokenSource.Dispose();
        }
    }
}
