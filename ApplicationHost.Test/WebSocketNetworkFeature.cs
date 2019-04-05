using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StandPoint.Abstractions.Builder;
using StandPoint.Abstractions.Builder.Feature;
using StandPoint.Net.WebSockets;
using StandPoint.Utilities;

namespace ApplicationHost.Test
{
    public class WebSocketNetworkFeature : ApplicationFeature
    {
        private readonly List<IDisposable> _resources;

        private readonly IConnectionManager _connectionManager;
        private readonly IWebSocketHandler _socketHandler;
        private readonly WebSocketManagerOptions _options;

        public WebSocketNetworkFeature(IConnectionManager connectionManager, IWebSocketHandler socketHandler, IOptions<WebSocketManagerOptions> options)
        {
            Guard.NotNull(connectionManager, nameof(connectionManager));
            Guard.NotNull(socketHandler, nameof(socketHandler));
            Guard.NotNull(options, nameof(options));

            this._resources = new List<IDisposable>();
            this._connectionManager = connectionManager;
            this._socketHandler = socketHandler;
            this._options = options.Value;

            _resources.Add(this._connectionManager);
        }

        public override void Start()
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseIISIntegration()
                .UseUrls($"http://{_options.Listen.Address}:{_options.Listen.Port}")
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IWebSocketHandler>(_socketHandler);
                })
                .UseStartup<Startup>()
                .Build();

            host.Start();

            this._resources.Add(host);

            _options.EndPoints.ForEach(async endPoint =>
            {
                var ws = new ClientWebSocket();
                ws.Options.KeepAliveInterval = TimeSpan.FromSeconds(20);
                await ws.ConnectAsync(new Uri($"ws://{endPoint.Address}:{endPoint.Port}"), CancellationToken.None);

                _connectionManager.AddSocket(ws);
                await _socketHandler.OnStart(ws);
                await ws.ReceiveAsync(_socketHandler);
            });
        }

        public override void Stop()
        {
            foreach (var disposable in _resources)
            {
               disposable.Dispose();;
            }
        }
    }

    internal static class WebSocketNetworkFeatureExtensions
    {
        public static IApplicationBuilder UseWebSocketNetworkFeature(this IApplicationBuilder applicationBuilder, IConfiguration configuration)
        {
            applicationBuilder.ConfigureFeature(features =>
            {
                features.AddFeature<WebSocketNetworkFeature>()
                    .FeatureServices(services =>
                    {
                        services.AddSingleton<IConnectionManager, WebSocketManager>();

                        var connectonManagerSection = configuration.GetSection(nameof(WebSocketManager));
                        services.AddConnectionManagerOptions(connectonManagerSection);

                        services.AddSingleton<IWebSocketHandler, ConnectionHandler>();
                    });
            });

            return applicationBuilder;
        }
    }
}
