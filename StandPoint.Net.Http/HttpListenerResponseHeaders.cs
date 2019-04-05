using System;

namespace StandPoint.Net.Http
{
    public sealed class HttpListenerResponseHeaders : HttpListenerHeaders
    {
        private HttpListenerHeaderValueCollection<string> _contentEncoding;
        private Uri _location;

        public HttpListenerResponseHeaders(HttpListenerResponse response)
        {
            Response = response;
        }

        public Uri Location
        {
            get
            {
                if (_location == null)
                {
                    string locationString;
                    if (TryGetValue("Location", out locationString))
                    {
                        _location = new Uri(locationString);
                    }
                }
                return _location;
            }

            set
            {
                if (!value.Equals(_location))
                {
                    _location = value;
                    if (_location == null)
                        return;
                    this["Location"] = _location.ToString();
                }
            }
        }

        #region Content Headers

        public HttpListenerHeaderValueCollection<string> ContentEncoding
        {
            get
            {
                if (_contentEncoding == null)
                {
                    _contentEncoding = new HttpListenerHeaderValueCollection<string>(this, "Content-Encoding");
                }
                return _contentEncoding;
            }
        }

        internal HttpListenerResponse Response { get; set; }

        #endregion
    }
}
