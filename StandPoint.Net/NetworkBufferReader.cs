using System;
using System.Threading;
using System.Threading.Tasks;
using StandPoint.Threading;

namespace StandPoint.Net
{
    public class NetworkBufferReader : NetworkReader
    {
        public readonly byte[] Buffer;

        public NetworkBufferReader(byte[] buffer, CancellationToken token, byte[] endMarker = null, byte[] startMarker = null)
            : base(token, endMarker, startMarker)
        {
            this.Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            ReadBufferAsync(token);
        }

        private void ReadBufferAsync(CancellationToken token)
        {
            Task.Run(() =>
            {
                for (var i = 0; i < Buffer.Length; i++)
                {
                    if (!this.HasMessageHandlers)
                    {
                        this.StartReadingEvent.Reset();
                    }

                    this.StartReadingEvent.Wait(token);

                    this.Push(this.Buffer, i, 1);
                }
            }, token).HandleExceptions().ContinueWith(task =>
            {

            }, TaskContinuationOptions.OnlyOnCanceled);
        }
    }
}
