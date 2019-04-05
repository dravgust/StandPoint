﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using StandPoint.Security.Cryptography;
using StandPoint.Utilities.Json;

namespace StandPoint.IO.IPFS.Commands
{
    public class IpfsRepo : IpfsCommand
    {
        internal IpfsRepo(Uri commandUri, HttpClient httpClient, IJsonSerializer jsonSerializer) : base(commandUri, httpClient, jsonSerializer)
        {
        }

        /// <summary>
        /// Perform a garbage collection sweep on the repo
        /// 
        /// 'ipfs repo gc' is a plumbing command that will sweep the local
        /// set of stored objects and remove ones that are not pinned in
        /// order to reclaim hard disk space.
        /// </summary>
        /// <param name="quiet">Write minimal output</param>
        /// <returns>An enumerable of multihashes that were cleared. The enumerable will be empty if no entries were removed</returns>
        public async Task<IEnumerable<MultiHash>> GC(bool quiet = false)
        {
            var flags = new Dictionary<string, string>();

            if(quiet)
            {
                flags.Add("quiet", "true");
            }

            HttpContent content = await ExecuteGetAsync("gc", flags);

            string json = await content.ReadAsStringAsync();

            if (String.IsNullOrEmpty(json))
            {
                return Enumerable.Empty<MultiHash>();
            }

            Dictionary<string, string> keys = _jsonSerializer.Deserialize<Dictionary<string, string>>(json);

            return keys.Values.Select(x => new MultiHash(x));
        }
    }
}
