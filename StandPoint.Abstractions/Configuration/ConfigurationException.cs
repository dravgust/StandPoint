using System;

namespace StandPoint.Abstractions.Configuration
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message)
        {

        }
    }
}
