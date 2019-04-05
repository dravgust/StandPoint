using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StandPoint.Net.WebSockets;

namespace ApplicationHost.Test
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            app.UseWebSockets();
            app.Use(async (httpContext, next) =>
            {
                if (httpContext.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();
                    var socketHandler = httpContext.RequestServices.GetRequiredService<IWebSocketHandler>();

                    if (socketHandler == null)
                        throw new InvalidOperationException($"{nameof(IWebSocketHandler)} not registered, configuration required");

                    await socketHandler.OnConnected(webSocket);
                    await webSocket.ReceiveAsync(socketHandler);
                }
                else
                {
                    await next();
                }
            });
        }
    }
}
