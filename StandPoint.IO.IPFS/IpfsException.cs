using System;

namespace StandPoint.IO.IPFS
{
    public class IpfsException : Exception
    {
        internal IpfsException()
        {
        }

        internal IpfsException(string message) : base(message)
        {
        }

        internal IpfsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
