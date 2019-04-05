using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StandPoint.Reflection;
using StandPoint.Threading;
using StandPoint.Utilities;

namespace StandPoint.Net
{
    public class ConnectionManagement : IConnectionManagement
    {
	    private readonly IServiceProvider _serviceProvider;

		public ConnectionManagement(IServiceProvider serviceProvider)
		{
			Guard.NotNull(serviceProvider, nameof(serviceProvider));
			_serviceProvider = serviceProvider;
		}

	    public void Push(TcpClient client, CancellationToken token)
	    {
		    Task.Run(async () =>
			{
				try
				{
				    var controller = _serviceProvider.GetService<ISocketController>();
				    if (controller == null)
					    throw new InvalidOperationException($"{nameof(ISocketController)} must be set.");

				    byte[] endMarker = null;
				    byte[] startMarker = null;

				    if (controller.GetType().HasAttribute(out MessageFormatAttribute format))
				    {
					    endMarker = format.EndMarker;
					    startMarker = format.StartMarker;
				    }

				    using (var context = new TcpSocketContext(client, endMarker: endMarker, startMarker: startMarker))
				    {
					    try
					    {
						    await (Task)controller.InvokeAsync(context, token);
					    }
					    catch (Exception e)
					    {
						    if (controller.GetType().HasAttribute<SocketExceptionAttribute>(out var customAttribute))
						    {
							    customAttribute.OnException(e);
						    }
					    }
				    }
				}
				finally
				{
				    client.Close();
				}
			}, token)
			.HandleExceptions();
	    }
	}
}
