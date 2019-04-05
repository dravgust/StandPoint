using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using StandPoint.Utilities.Json;

namespace StandPoint.Net.Http.Client
{
    public class HttpClient
    {
        public string BaseUrl { set; get; }

        public TimeSpan? Timeout { set; get; }

        public IWebProxy Proxy { set; get; }

        public HttpClient()
        {

        }

        public async Task<T> PostJsonAsync<T>(string apiUrl, object objectBody)
        {
            using (var responseMessage = await PostJsonAsync(apiUrl, objectBody))
            {
                responseMessage.EnsureSuccessStatusCode();

                using (HttpContent httpContent = responseMessage.Content)
                {
                    var responseData = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
                    return responseData.FromJSON<T>();
                }
            }
        }

        public async Task<T> GetAsync<T>(string apiUrl)
        {
            using (var responseMessage = await GetAsync(apiUrl))
            {
                responseMessage.EnsureSuccessStatusCode();

                using (HttpContent httpContent = responseMessage.Content)
                {
                    var responseData = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
                    return responseData.FromJSON<T>();
                }
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string apiUrl)
        {
            return await InvokeRequestAsync(apiUrl, async (uri, httpClient) => await httpClient.GetAsync(uri).ConfigureAwait(false));
        }

        public async Task<HttpResponseMessage> PostJsonAsync(string apiUrl, object objectBody)
        {
            if (objectBody == null)
                throw new ArgumentNullException(nameof(objectBody), "objectBody required");

            return await InvokeRequestAsync(apiUrl, async (uri, httpClient) =>
                   {
                       using (var resquest = new StringContent(objectBody.ToJSON(), Encoding.UTF8, "application/json"))
                       {
                           //resquest.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                           return await httpClient.PostAsync(uri, resquest).ConfigureAwait(false);
                       }
                   });
        }

        protected virtual void SetAuthenticationHeader(System.Net.Http.HttpClient httpClient)
        {

        }

        private async Task<HttpResponseMessage> InvokeRequestAsync(string apiUrl, Func<Uri, System.Net.Http.HttpClient, Task<HttpResponseMessage>> funcAsync)
        {
            if (string.IsNullOrEmpty(apiUrl))
                throw new ArgumentNullException(nameof(apiUrl), "apiUrl required");

            var handler = new HttpClientHandler()
            {

            };

            if (Proxy != null)
            {
                handler.UseProxy = true;
                handler.PreAuthenticate = true;
                handler.UseDefaultCredentials = false;
                handler.Proxy = Proxy;
            }

            using (var httpClient = new System.Net.Http.HttpClient(handler))
            {
                if (Timeout != null)
                {
                    httpClient.Timeout = Timeout.Value;
                }

                this.SetAuthenticationHeader(httpClient);

                return await funcAsync(new Uri($"{BaseUrl}{apiUrl}"), httpClient);
            }
        }
    }
}
