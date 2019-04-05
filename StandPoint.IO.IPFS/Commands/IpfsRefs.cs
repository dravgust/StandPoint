﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using StandPoint.Security.Cryptography;
using StandPoint.Utilities.Json;

namespace StandPoint.IO.IPFS.Commands
{
    public class IpfsRefs : IpfsCommand
    {
        internal IpfsRefs(Uri commandUri, HttpClient httpClient, IJsonSerializer jsonSerializer) : base(commandUri, httpClient, jsonSerializer)
        {
        }

        /// <summary>
        /// Lists all local references
        /// Displays the hashes of all local objects.
        /// </summary>
        /// <returns>Returns an enumerable of multihashes, or an empty enumerable if nothing is found.</returns>
        public async Task<IEnumerable<MultiHash>> Local()
        {
            HttpContent content = await ExecuteGetAsync("local");

            //DOESN'T return json!
            //Returns a list of multihashes separated by "\n"
            var stringContent = await content.ReadAsStringAsync();

            if(String.IsNullOrEmpty(stringContent))
            {
                return Enumerable.Empty<MultiHash>();
            }

            return stringContent
                .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => new MultiHash(x));
        }
    }
}
