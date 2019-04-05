using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using StandPoint.Threading;

namespace StandPoint.Net
{
    public delegate void OnCancellation();

    public class NetworkStreamReader : NetworkReader
    {
        private readonly int _bufferSize;

        public readonly NetworkStream BaseStream;

        public NetworkStreamReader(NetworkStream stream, CancellationToken token, byte[] endMarker = null, byte[] startMarker = null, int bufferSize = 1024)
            : base(token, endMarker, startMarker)
        {
            this.BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));
            _bufferSize = bufferSize;

            MessageListenerInvoke(token);
        }

        private void MessageListenerInvoke(CancellationToken token)
        {
            Task.Run(async () =>
            {
                var buffer = new byte[_bufferSize];
                while (true)
                {
                    if (!this.HasMessageHandlers)
                    {
                        this.StartReadingEvent.Reset();
                    }

                    this.StartReadingEvent.Wait(token);

                    if (BaseStream.DataAvailable)
                    {
                        var length = await BaseStream.ReadAsync(buffer, 0, buffer.Length, token)
                            .ConfigureAwait(false);

                        this.Push(buffer, 0, length);
                    }
                    else
                    {
                        await Task.Delay(1000, token);
                    }
                }
            }, token)
            .HandleExceptions().ContinueWith(task =>
            {}, TaskContinuationOptions.OnlyOnCanceled);
        }
    }
}
