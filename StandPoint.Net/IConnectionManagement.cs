using System.Net.Sockets;
using System.Threading;

namespace StandPoint.Net
{
    public interface IConnectionManagement
    {
	    void Push(TcpClient client, CancellationToken token);
    }
}
