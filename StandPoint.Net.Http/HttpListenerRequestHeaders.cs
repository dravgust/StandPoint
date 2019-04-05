using System;

namespace StandPoint.Net.Http
{
    public sealed class HttpListenerRequestHeaders : HttpListenerHeaders
    {
        private HttpListenerHeaderValueCollection<string> _accept;
        private HttpListenerHeaderValueCollection<string> _acceptCharset;
        private HttpListenerHeaderValueCollection<string> _acceptLanguage;
        private HttpListenerHeaderValueCollection<string> _acceptEncoding;
        private DateTime _accepDatetime;
        private string _host;

        public HttpListenerRequestHeaders(HttpListenerRequest request)
        {
            Request = request;
        }

        public string Host
        {
            get
            {
                if (_host == null)
                {
                    string hostString;
                    if (TryGetValue("Host", out hostString))
                    {
                        _host = hostString;
                    }
                }
                return _host;
            }
        }

        #region Accept Headers

        public HttpListenerHeaderValueCollection<string> Accept
        {
            get
            {
                if (_accept == null)
                {
                    _accept = new HttpListenerHeaderValueCollection<string>(this, "Accept");
                }
                return _accept;
            }
        }

        public HttpListenerHeaderValueCollection<string> AcceptEncoding
        {
            get
            {
                if (_acceptEncoding == null)
                {
                    _acceptEncoding = new HttpListenerHeaderValueCollection<string>(this, "Accept-Encoding");
                }
                return _acceptEncoding;
            }
        }

        public HttpListenerHeaderValueCollection<string> AcceptCharset
        {
            get
            {
                if (_acceptCharset == null)
                {
                    _acceptCharset = new HttpListenerHeaderValueCollection<string>(this, "Accept-Charset");
                }
                return _acceptCharset;
            }
        }

        public HttpListenerHeaderValueCollection<string> AcceptLanguage
        {
            get
            {
                if (_acceptLanguage == null)
                {
                    _acceptLanguage = new HttpListenerHeaderValueCollection<string>(this, "Accept-Language");
                }
                return _acceptLanguage;
            }
        }

        public DateTime AcceptDateTime
        {
            get
            {
                if (_accepDatetime == default(DateTime))
                {
                    string headerValue;
                    if(TryGetValue("Accept-Datetime", out headerValue))
                    {
                        _accepDatetime = DateTime.Parse(headerValue);
                    }
                }
                return _accepDatetime;
            }
        }

        internal HttpListenerRequest Request { get; set; }

        #endregion
    }
}
