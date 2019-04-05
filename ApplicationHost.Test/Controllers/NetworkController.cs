using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StandPoint.Net;
using StandPoint.Utilities;

namespace ApplicationHost.Test.Controllers
{
    [DeviceException, MessageFormat(EndMarker = new[] { (byte)'\0' })]
    public class NetworkController : SocketController
	{
	    private readonly ILogger<NetworkController> _logger;

        public NetworkController(ILogger<NetworkController> logger)
	    {
	        Guard.NotNull(logger, nameof(logger));
	        _logger = logger;
	    }

        [OnRequest]
		public async Task<byte[]> OnRequest(byte[] message)
		{
            try
            {
                await Task.Delay(0);

                _logger.LogInformation($"GET: {Encoding.ASCII.GetString(message)}");

                
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                //Context.Close();
            }

		    return message;
		}
	}

    public class DeviceExceptionAttribute : SocketExceptionAttribute
    {
        public override void OnException(Exception e)
        {
            base.OnException(e);
        }
    }
}
