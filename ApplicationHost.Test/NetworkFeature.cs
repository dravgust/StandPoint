using System;
using System.Collections.Generic;
using ApplicationHost.Test.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StandPoint.Abstractions.Builder;
using StandPoint.Abstractions.Builder.Feature;
using StandPoint.Net;
using StandPoint.Utilities;

namespace ApplicationHost.Test
{
	public class NetworkOptions
	{
		public int ListeningPort { get; set; }
	}

	public class NetworkFeature : ApplicationFeature
    {
	    private readonly ILogger _logger;

		private readonly List<IDisposable> _resources;
	    private readonly NetworkOptions _options;

	    private readonly TcpServer _tcpServer;

		public NetworkFeature(
			IOptions<NetworkOptions> options, 
			TcpServer tcpServer,
			ILoggerFactory loggerFatcory)
	    {
		    Guard.NotNull(options, nameof(options));
		    Guard.NotNull(tcpServer, nameof(tcpServer));
	        Guard.NotNull(loggerFatcory, nameof(loggerFatcory));

		    _logger = loggerFatcory.CreateLogger<NetworkFeature>();

		    _options = options.Value;
			_resources = new List<IDisposable>();

	        _tcpServer = tcpServer;
	        _resources.Add(_tcpServer);
        }

		public override void Start()
	    {
		    _tcpServer.Start(_options.ListeningPort);

		    _logger.LogInformation($"Network Listening on a TCP port: {_options.ListeningPort}");
		}

	    public override void Stop()
	    {
		    foreach (var disposable in _resources)
		    {
			    disposable.Dispose();
		    }
	    }
	}

	internal static class NetworkFeatureExtensions
	{
		public static IServiceCollection AddNetworkOptions(this IServiceCollection services,
			IConfigurationSection configuration)
		{
			Guard.NotNull(configuration, nameof(configuration));

			services.Configure<NetworkOptions>(config =>
			{
				var listeningPort = configuration[nameof(NetworkOptions.ListeningPort)];
				if (listeningPort != null)
				{
					config.ListeningPort = Convert.ToInt32(listeningPort);
				}
				else
				{
					config.ListeningPort = 9999;
				}
			});

			return services;
		}

		public static IApplicationBuilder UseNetwork(this IApplicationBuilder applicationBuilder,
			IConfiguration configuration)
		{
			Guard.NotNull(configuration, nameof(configuration));

			applicationBuilder.ConfigureFeature(features =>
			{
				features.AddFeature<NetworkFeature>()
					.FeatureServices(services =>
					{
						var networkSection = configuration.GetSection(nameof(NetworkFeature));
						services.AddNetworkOptions(networkSection);

					    services
                        .AddSingleton<TcpServer>()
						.AddSingleton<IConnectionManagement, ConnectionManagement>()
                        .AddTransient<ISocketController, NetworkController>();
					});
			});

			return applicationBuilder;
		}
	}
}
