using System;
using System.Net.Http.Headers;
using System.Text;

namespace StandPoint.Net.Http.Client
{
    public class BasicHttpClient : HttpClient
    {
        public string Username { set; get; }

        public string Password { set; get; }

        protected override void SetAuthenticationHeader(System.Net.Http.HttpClient httpClient)
        {
            var credentials = Encoding.ASCII.GetBytes($"{Username}:{Password}");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));
        }
    }
}
