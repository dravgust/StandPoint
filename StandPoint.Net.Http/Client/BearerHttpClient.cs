using System.Net.Http.Headers;

namespace StandPoint.Net.Http.Client
{
    public class BearerHttpClient : HttpClient
    {
        public string Token { set; get; }

        protected override void SetAuthenticationHeader(System.Net.Http.HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        }
    }
}
