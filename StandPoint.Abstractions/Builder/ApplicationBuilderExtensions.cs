using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace StandPoint.Abstractions.Builder
{
    /// <summary>
    /// A class providing extension methods for <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseConfiguration(this IApplicationBuilder builder, IConfiguration configuration)
        {
            foreach (KeyValuePair<string, string> keyValuePair in configuration.AsEnumerable())
                builder.UseSetting(keyValuePair.Key, keyValuePair.Value);

            return builder;
        }
    }
}
