using System.IO;
using System.Net;
using System.Net.Sockets;

namespace StandPoint.Net.Http.Abstractions
{
    public class TcpClientAdapter
    {
        private readonly TcpClient _tcpClient;

        public IPEndPoint LocalEndPoint
        {
            get;
            private set;
        }

        public IPEndPoint RemoteEndPoint
        {
            get;
            private set;
        }

        public TcpClientAdapter(TcpClient tcpClient)
        {
            this._tcpClient = tcpClient;

            LocalEndPoint = (IPEndPoint)tcpClient.Client.LocalEndPoint;
            RemoteEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
        }

        public Stream GetInputStream()
        {
            return this._tcpClient.GetStream();
        }

        public Stream GetOutputStream()
        {
            return this._tcpClient.GetStream();
        }

        public void Dispose()
        {
            this._tcpClient.Dispose();
        }
    }
}
