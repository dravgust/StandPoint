using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using StandPoint.Threading;
using StandPoint.Utilities;

namespace StandPoint.Net
{
    public class TcpServer : IDisposable
    {
        private readonly IConnectionManagement _connectionManagement;
        private TcpListener _tcpListener;
        private CancellationTokenSource _cts = null;
        private bool _isDisposed;

        public TcpServer(IConnectionManagement connectionManagement)
        {
            Guard.NotNull(connectionManagement, nameof(connectionManagement));
            _connectionManagement = connectionManagement;
        }

        public void Start(int port)
        {
            this.Start(IPAddress.Any, port);
        }

        public void Start(IPAddress address, int port)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(TcpServer));

            if (_cts != null)
                throw new InvalidOperationException("TcpListener is already running.");

            Guard.NotNull(address, nameof(address));

            var localEndPoint = new IPEndPoint(address, port);
            _tcpListener = new TcpListener(localEndPoint);
            _cts = new CancellationTokenSource();

            Task.Run(Listener, _cts.Token)
                .HandleExceptions();
        }

        private async Task Listener()
        {
            try
            {
                _tcpListener.Start();
                var token = _cts.Token;

                while (true)
                {
                    await Task.Delay(10, token);
                    token.ThrowIfCancellationRequested();
	                var tcpClient = await _tcpListener.AcceptTcpClientAsync();
					_connectionManagement.Push(tcpClient, token);
				}
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) when (this._isDisposed) { }
            finally
            {
                _tcpListener.Stop();

                _cts.Dispose();
                _cts = null;
            }
        }

        public void Stop()
        {
            _cts.Cancel();
            _tcpListener.Stop();
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            this.Stop();
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SocketExceptionAttribute : Attribute
    {
        public virtual void OnException(Exception e)
        {
            Trace.TraceError($"{e.GetType().Name} {e.Message}\r\n{e.StackTrace}");
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MessageFormatAttribute : Attribute
    {
        public byte[] EndMarker { set; get; }
        public byte[] StartMarker { set; get; } = null;
    }
}

