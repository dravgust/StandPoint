using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StandPoint.Utilities;

namespace ApplicationHost.Test
{
    public class WebSocketManagerOptions
    {
        public List<IPEndPoint> EndPoints
        {
            get; 
        } = new List<IPEndPoint>();

        public IPEndPoint Listen { get; set; }
    }

    public static class ConnectionManagerOptionsExtensions
    {
        public static IServiceCollection AddConnectionManagerOptions(this IServiceCollection services,
            IConfigurationSection configuration)
        {
            Guard.NotNull(configuration, nameof(configuration));

            services.Configure<WebSocketManagerOptions>(config =>
            {
                var endPointsString = configuration[nameof(WebSocketManagerOptions.EndPoints)];
                if (endPointsString != null)
                {
                    var endPoints = endPointsString.Split(new char[','], StringSplitOptions.RemoveEmptyEntries);
                    foreach (var endPoint in endPoints)
                    {
                        var data = endPoint.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        config.EndPoints.Add(new IPEndPoint(IPAddress.Parse(data[0]), data.Length > 1 ? Convert.ToInt32(data[1]) : 9999));
                    }
                }
                var listen = configuration[nameof(WebSocketManagerOptions.Listen)];
                if (listen != null)
                {
                    var data = listen.Split(new []{ ":" }, StringSplitOptions.RemoveEmptyEntries);
                    config.Listen = new IPEndPoint(IPAddress.Parse(data[0]), data.Length > 1 ? Convert.ToInt32(data[1]) : 9999);
                }
                else
                {
                    config.Listen = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 9999);
                }
                
            });

            return services;
        }
    }
}
