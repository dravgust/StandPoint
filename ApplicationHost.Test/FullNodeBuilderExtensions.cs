using Microsoft.Extensions.Configuration;
using StandPoint.Abstractions.Builder;
using StandPoint.Abstractions.Builder.Feature;

namespace ApplicationHost.Test
{
    public static class FullNodeBuilderExtensions
    {
        public static IApplicationBuilder UseConfiguration(this IApplicationBuilder builder, IConfiguration configuration)
        {
            ApplicationBuilderExtensions.UseConfiguration(builder, configuration);

            //builder.UseWebSocketNetworkFeature(configuration);
	        builder.UseNetwork(configuration);

            builder.UseBaseFeature();

            return builder;
        }
    }
}
