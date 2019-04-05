using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using StandPoint.Net.Http.Abstractions;

namespace StandPoint.Net.Http
{
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

    public sealed class HttpListener : IDisposable
    {
        Task _listener;
        private readonly TcpListenerAdapter _tcpListener;
        CancellationTokenSource _cts;
        private bool disposedValue = false; // To detect redundant calls
        private bool _isListening;

        public event EventHandler<HttpListenerRequestEventArgs> Request;


        /// <summary>
        /// Gets the underlying Socket.
        /// </summary>
        public Socket Socket
        {
            get
            {
                return _tcpListener.Socket;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the HttpListener is running or not.
        /// </summary>
        public bool IsListening
        {
            get
            {
                return _isListening;
            }
        }

        /// <summary>
        /// Gets the local endpoint on which the listener is running.
        /// </summary>
        public IPEndPoint LocalEndpoint
        {
            get;
            private set;
        }

        private HttpListener()
        {
            _cts = null;
        }

        /// <summary>
        /// Initializes a HttpListener.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public HttpListener(IPAddress address, int port) : this()
        {
            LocalEndpoint = new IPEndPoint(
                address,
                port);

            _tcpListener = new TcpListenerAdapter(LocalEndpoint);
        }

        /// <summary>
        /// Initializes a HttpListener with an IPEndPoint.
        /// </summary>
        /// <param name="endpoint"></param>
        public HttpListener(IPEndPoint endpoint) : this()
        {
            _tcpListener = new TcpListenerAdapter(LocalEndpoint);
        }

        /// <summary>
        /// Starts the listener.
        /// </summary>
        public void Start()
        {
            if (disposedValue)
                throw new ObjectDisposedException("Object has been disposed.");

            if (_cts != null)
                throw new InvalidOperationException("HttpListener is already running.");

            _cts = new CancellationTokenSource();
            _isListening = true;
            _listener = Task.Run(listener, _cts.Token);
        }

        private async Task listener()
        {
            try
            {
                _tcpListener.Start();

                while (_isListening)
                {
                    // Await request.
                    var client = await _tcpListener.AcceptTcpClientAsync();

                    var request = new HttpListenerRequest(client);

                    // Handle request in a separate thread.
                    Task.Run(async () =>
                    {
                        // Process request.
                        var response = new HttpListenerResponse(request, client);

                        try
                        {
                            await request.ProcessAsync();

                            response.Initialize();

                            if (Request == null)
                            {
                                // No Request handlers exist. Respond with "Not Found".
                                response.NotFound();
                                response.Close();
                            }
                            else
                            {
                                // Invoke Request handlers.
                                Request(this, new HttpListenerRequestEventArgs(request, response));
                            }
                        }
                        catch (Exception)
                        {
                            response.CloseSocket();
                        }
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _isListening = false;
                _cts = null;
            }
        }

        /// <summary>
        /// Closes the listener.
        /// </summary>
        public void Close()
        {
            if (_cts == null)
                throw new InvalidOperationException("HttpListener is not running.");

            Request = null;
            _cts.Cancel();
            _cts = null;
            _isListening = false;
            _tcpListener.Stop();
        }

        /// <summary>
        /// Awaits the next HTTP request and returns its context.
        /// </summary>
        /// <returns></returns>
        public Task<HttpListenerContext> GetContextAsync()
        {
            // Await a Request and return the context to caller.

            var tcs = new TaskCompletionSource<HttpListenerContext>();
            EventHandler<HttpListenerRequestEventArgs> requestHandler = null;
            requestHandler = (sender, evArgs) =>
            {
                var context = new HttpListenerContext(evArgs.Request, evArgs.Response);
                tcs.SetResult(context);
                Request -= requestHandler;
            };
            Request += requestHandler;
            return tcs.Task;
        }

        #region IDisposable Support

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                Close();

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~HttpListener() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class HttpListenerContext
    {
        private readonly HttpListenerRequest _request;
        private readonly HttpListenerResponse _response;

        public HttpListenerContext(HttpListenerRequest request, HttpListenerResponse response)
        {
            this._request = request;
            this._response = response;
        }

        public HttpListenerRequest Request
        {
            get
            {
                return _request;
            }
        }

        public HttpListenerResponse Response
        {
            get
            {
                return _response;
            }
        }
    }
}
