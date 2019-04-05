using System;

namespace StandPoint.Net
{
    public interface ISocketContext : IDisposable
    {
        NetworkReader Request { get; }

        NetworkStreamWrtiter Response { get; }

        void Close();
    }
}
 