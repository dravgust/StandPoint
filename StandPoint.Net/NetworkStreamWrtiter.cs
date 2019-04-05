using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace StandPoint.Net
{
    public class NetworkStreamWrtiter
    {
        private readonly byte[] _startMarker;
        private readonly byte[] _endMarker;
        public readonly NetworkStream BaseStream;

        public NetworkStreamWrtiter(NetworkStream stream, CancellationToken token, byte[] endMarker = null, byte[] startMarker = null, int bufferSize = 1024)
        {
            this.BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));

            _endMarker = endMarker ?? new byte[0];
            _startMarker = startMarker ?? new[] { (byte)'\r', (byte)'\n' };
        }

        public async Task WriteLineAsync(byte[] message)
        {
            var buffer = new byte[_startMarker.Length + message.Length + _endMarker.Length];

            Array.Copy(_startMarker, 0, buffer, 0, _startMarker.Length);
            Array.Copy(message, 0, buffer, _startMarker.Length, message.Length);
            Array.Copy(_endMarker, 0, buffer, _startMarker.Length + message.Length, _endMarker.Length);

            await BaseStream.WriteAsync(buffer, 0, buffer.Length);
            await BaseStream.FlushAsync();
        }
    }
}
