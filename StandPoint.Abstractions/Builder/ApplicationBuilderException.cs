using System;

namespace StandPoint.Abstractions.Builder
{
    public class ApplicationBuilderException : Exception
    {
        public ApplicationBuilderException(string message) : base(message)
        {
        }
    }
}
