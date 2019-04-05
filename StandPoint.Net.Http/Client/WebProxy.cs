using System;
using System.Net;

namespace StandPoint.Net.Http.Client
{
    public class WebProxy : IWebProxy
    {
        private readonly Uri _proxyUri;

        public WebProxy(string proxyUrl, string username = null, string password = null)
        {
            _proxyUri = new Uri(proxyUrl);
            Credentials = new NetworkCredential(username, password); ;
        }

        public Uri GetProxy(Uri destination)
        {
            return _proxyUri;
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }

        public ICredentials Credentials { get; set; }
    }
}
