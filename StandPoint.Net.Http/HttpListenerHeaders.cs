using System;
using System.Collections.Generic;
using System.Text;

namespace StandPoint.Net.Http
{
    public class HttpListenerHeaders : Dictionary<string, string>
    {
        private HttpListenerHeaderValueCollection<string> _contentType;
        private HttpListenerHeaderValueCollection<string> _connection;
        private string _contentMd5;
        private bool? _isRequestHeaders;

        private HttpListenerResponse _response;
        private HttpListenerRequest _request;

        internal void ParseHeaderLines(IEnumerable<string> lines)
        {
            foreach (var headerLine in lines)
            {
                var parts = headerLine.Split(':');
                var key = parts[0];
                var value = parts[1].Trim();
                Add(key, value);
            }
        }

        private string MakeHeaderString()
        {
            var sb = new StringBuilder();
            foreach (var header in this)
            {
                sb.Append($"{header.Key}: {header.Value}\r\n");
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return MakeHeaderString();
        }

        public HttpListenerHeaderValueCollection<string> Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new HttpListenerHeaderValueCollection<string>(this, "Connection");
                }
                return _connection;
            }
        }

        #region Content Headers

        public HttpListenerHeaderValueCollection<string> ContentType
        {
            get
            {
                if (_contentType == null)
                {
                    _contentType = new HttpListenerHeaderValueCollection<string>(this, "Content-Type");
                }
                return _contentType;
            }
        }

        public long ContentLength
        {
            get
            {
                if (IsRequestHeaders)
                {
                    string headerValue;
                    if (TryGetValue("Content-Length", out headerValue))
                    {
                        return int.Parse((string)headerValue);
                    }
                    return -1;
                }
                else
                {
                    var response = this.GetResponse();
                    return response.OutputStream.Length;
                }
            }
        }

        public string ContentMd5
        {
            get
            {
                if (_contentMd5 == null)
                {
                    if (TryGetValue("Content-MD5", out _contentMd5))
                    {
                        return _contentMd5;
                    }
                }
                return null;
            }

            #endregion
        }

        private bool IsRequestHeaders
        {
            get
            {
                if (_isRequestHeaders == null)
                {
                    _isRequestHeaders = this is HttpListenerRequestHeaders;
                }
                return _isRequestHeaders.GetValueOrDefault();
            }
        }

        private void ThrowIfSetterNotSupported()
        {
            if (IsRequestHeaders)
                throw new NotSupportedException("This header cannot be set for requests.");
        }

        private HttpListenerRequest GetRequest()
        {
            if (_response == null)
            {
                var headers = this as HttpListenerRequestHeaders;
                _request = headers.Request;
            }
            return _request;
        }

        private HttpListenerResponse GetResponse()
        {
            if (_response == null)
            {
                var headers = this as HttpListenerResponseHeaders;
                _response = headers.Response;
            }
            return _response;
        }
    }
}