using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace StandPoint.Net.Http.Abstractions
{
    public class TcpListenerAdapter
    {
        private TcpListener _tcpListener;

        public IPEndPoint LocalEndpoint { get; private set; }

        public Socket Socket => _tcpListener.Server;

        public TcpListenerAdapter(IPEndPoint localEndpoint)
        {
            LocalEndpoint = localEndpoint;

            Initialize();
        }

        private void Initialize()
        {
            _tcpListener = new TcpListener(LocalEndpoint);
        }

        public Task<TcpClientAdapter> AcceptTcpClientAsync()
        {
            return AcceptTcpClientAsyncInternal();
        }

        private async Task<TcpClientAdapter> AcceptTcpClientAsyncInternal()
        {
            var tcpClient = await _tcpListener.AcceptTcpClientAsync();
            return new TcpClientAdapter(tcpClient);
        }

        public void Start()
        {
            _tcpListener.Start();
        }

        public void Stop()
        {
            _tcpListener.Stop();
        }
    }
}