using System.Threading;
using System.Threading.Tasks;

namespace StandPoint.Net
{
    public interface ISocketController
    {
        Task InvokeAsync(ISocketContext context, CancellationToken token);
    }
}
