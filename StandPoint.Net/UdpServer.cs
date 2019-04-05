using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using StandPoint.Reflection;
using StandPoint.Threading;

namespace StandPoint.Net
{
    public class UdpServer<T> : IDisposable where T : ISocketController
    {
        private readonly UdpClient _udpLClient;
        private CancellationTokenSource _cts = null;
        private bool _isDisposed;

        public IPEndPoint LocalEndPoint { get; }

        public UdpServer(IPAddress address, int port)
        {
            this.LocalEndPoint = new IPEndPoint(address, port);

            _udpLClient = new UdpClient(LocalEndPoint);
        }

        public UdpServer(int port) : this(IPAddress.Any, port){}

        public void Start()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(UdpServer<T>));

            if (_cts != null)
                throw new InvalidOperationException("UdpListener is already running.");

            this._cts = new CancellationTokenSource();
            Task.Run(Listener, _cts.Token).HandleExceptions();
        }

        private async Task Listener()
        {
            try
            {
                var token = _cts.Token;

                while (true)
                {
                    await Task.Delay(10, token);
                    token.ThrowIfCancellationRequested();

                    HandleUdpConnection(await _udpLClient.ReceiveAsync(), token);
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) when (this._isDisposed) { }
            finally
            {
                this._udpLClient.Close();

                this._cts.Dispose();
                this._cts = null;
            }
        }

        private static void HandleUdpConnection(UdpReceiveResult result, CancellationToken token)
        {
            Task.Run(async () =>
            {
                try
                {
                    byte[] endMarker = null;
                    byte[] startMarker = null;
                    if (typeof(T).HasAttribute(out MessageFormatAttribute format))
                    {
                        endMarker = format.EndMarker;
                        startMarker = format.StartMarker;
                    }

                    //using (var context = new UdpSocketContext(result, endMarker: endMarker, startMarker: startMarker))
                    //{
                    //    var controller = Activator.CreateInstance<T>();
                    //    await (Task)controller.InvokeAsync(context, token);
                    //}
                }
                catch (Exception e)
                {
                    if (typeof(T).HasAttribute<SocketExceptionAttribute>(out var customAttribute))
                    {
                        customAttribute.OnException(e);
                    }
                }
            }, token);
        }

        public void Stop()
        {
            this._cts.Cancel();
            this._udpLClient.Close();
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            this._isDisposed = true;

            this.Stop();
        }
    }
}

