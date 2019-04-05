using System;
using Microsoft.Extensions.Configuration;
using StandPoint.Utilities;

namespace StandPoint.Abstractions.Configuration
{
    public class ApplicationOptions
    {
        public string ApplicationName { get; set; }

        public ApplicationOptions()
        {
            
        }

        public ApplicationOptions(IConfiguration configuration)
        {
            Guard.NotNull(configuration, nameof(configuration));

            this.ApplicationName = configuration[nameof(ApplicationOptions.ApplicationName)];
        }

        private static bool ParseBool(IConfiguration configuration, string key)
        {
            if (!string.Equals("true", configuration[key], StringComparison.OrdinalIgnoreCase))
                return string.Equals("1", configuration[key], StringComparison.OrdinalIgnoreCase);
            return true;
        }
    }
}
