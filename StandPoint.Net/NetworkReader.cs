using System;
using System.Threading;
using System.Threading.Tasks;
using StandPoint.Utilities;

namespace StandPoint.Net
{
    public abstract class NetworkReader : DataReader, IDisposable
    {
        protected readonly ManualResetEventSlim StartReadingEvent;
        private readonly CancellationToken _cancellationToken;
        protected CancellationTokenRegistration TokenRegistration;

        private event OnCancellation OnCancellation;

        protected NetworkReader(CancellationToken token, byte[] endMarker = null, byte[] startMarker = null)
            : base(endMarker, startMarker)
        {
            StartReadingEvent = new ManualResetEventSlim(true);
            _cancellationToken = token;

            TokenRegistration = token.Register(() =>
            {
                this.OnCancellation?.Invoke();
            });
        }

        public async Task<byte[]> ReadLineAsync(CancellationToken? token = null)
        {
            try
            {
                return await ReadDataTask().ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        private Task<byte[]> ReadDataTask()
        {
            var completionSource = new TaskCompletionSource<byte[]>();

            void OnCancellationHandler()
            {
                if (!completionSource.TrySetCanceled()) return;
                this.OnCancellation -= OnCancellationHandler;
                this.OnMessage -= OnMessageHandler;
                this.OnException -= OnExceptionHandler;
            }
            void OnExceptionHandler(Exception e)
            {
                if (!completionSource.TrySetException(e)) return;
                this.OnCancellation -= OnCancellationHandler;
                this.OnMessage -= OnMessageHandler;
                this.OnException -= OnExceptionHandler;
            }
            void OnMessageHandler(byte[] data)
            {
                if (!completionSource.TrySetResult(data)) return;
                this.OnCancellation -= OnCancellationHandler;
                this.OnMessage -= OnMessageHandler;
                this.OnException -= OnExceptionHandler;
            }

            this.OnCancellation += OnCancellationHandler;
            this.OnException += OnExceptionHandler;
            this.OnMessage += OnMessageHandler;

            this.StartReadingEvent.Set();

            return completionSource.Task;
        }

        public void Dispose()
        {
            TokenRegistration.Dispose();
            StartReadingEvent.Dispose();
        }
    }
}
