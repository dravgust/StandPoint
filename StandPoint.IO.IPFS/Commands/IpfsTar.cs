using System;
using System.Net.Http;
using StandPoint.Utilities.Json;

namespace StandPoint.IO.IPFS.Commands
{
    public class IpfsTar : IpfsCommand
    {
        internal IpfsTar(Uri commandUri, HttpClient httpClient, IJsonSerializer jsonSerializer) : base(commandUri, httpClient, jsonSerializer)
        {
        }

        //public async Task<HttpContent> Add()
        //{

        //}

        //public async Task<HttpContent> Cat()
        //{

        //}
    }
}
