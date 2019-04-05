﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using StandPoint.IO.IPFS.Commands;
using StandPoint.IO.IPFS.Json;
using StandPoint.IO.IPFS.Utilities;
using StandPoint.Security.Cryptography;
using StandPoint.Utilities.Json;
using JsonSerializer = StandPoint.Security.Cryptography.Serialization.JsonSerializer;

namespace StandPoint.IO.IPFS
{
    public class IpfsClient : IDisposable
    {
        private static Uri DefaultUri
        {
            get { return new Uri("http://127.0.0.1:5001"); }
        }

        private static HttpClient DefaultHttpClient
        {
            get { return new HttpClient(); }
        }

        private static string DefaultApiPath
        {
            get { return "api/v0"; }
        }

        private static IJsonSerializer DefaultJsonSerializer
        {
            get { return new JsonSerializer(); }
        }

        private readonly Uri _apiUri;
        private readonly HttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;

        public IpfsClient() : this(DefaultUri, DefaultHttpClient, DefaultJsonSerializer) { }
        public IpfsClient(string address) : this(new Uri(address), DefaultHttpClient, DefaultJsonSerializer) { }
        public IpfsClient(Uri address) : this(address, DefaultHttpClient, DefaultJsonSerializer) { }
        public IpfsClient(Uri address, HttpClient httpClient) : this(address, httpClient, DefaultJsonSerializer) { }

        public IpfsClient(Uri address, HttpClient httpClient, IJsonSerializer jsonSerializer)
        {
            _apiUri = UriHelper.AppendPath(address, DefaultApiPath);
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
        }

        /// <summary>
        /// Root commands.
        /// Gives access to top level commands like 'ipfs add' and 'ipfs cat'
        /// Also availible at the client level with aliases defined below
        /// </summary>
        private IpfsRoot _root;
        public IpfsRoot Root
        {
            get
            {
                if (_root == null)
                {
                    _root = new IpfsRoot(_apiUri, _httpClient, _jsonSerializer);
                }

                return _root;
            }
        }

        /// <summary>
        /// A set of commands to manipulate the bitswap agent
        /// </summary>
        private IpfsBitSwap _bitSwap;
        public IpfsBitSwap BitSwap
        {
            get
            {
                if (_bitSwap == null)
                {
                    Uri commandUri = UriHelper.AppendPath(_apiUri, "bitswap");
                    _bitSwap = new IpfsBitSwap(commandUri, _httpClient, _jsonSerializer);
                }

                return _bitSwap;
            }
        }

        /// <summary>
        /// Block subcommands
        /// </summary>
        private IpfsBlock _block;
        public IpfsBlock Block
        {
            get
            {
                if (_block == null)
                {
                    Uri commandUri = UriHelper.AppendPath(_apiUri, "block");
                    _block = new IpfsBlock(commandUri, _httpClient, _jsonSerializer);
                }

                return _block;
            }
        }

        /// <summary>
        /// Bootstrap subcommands
        /// </summary>
        private IpfsBootstrap _bootstrap;
        public IpfsBootstrap Bootstrap
        {
            get
            {
                if (_bootstrap == null)
                {
                    Uri commandUri = UriHelper.AppendPath(_apiUri, "bootstrap");
                    _bootstrap = new IpfsBootstrap(commandUri, _httpClient, _jsonSerializer);
                }

                return _bootstrap;
            }
        }

        /// <summary>
        /// Config subcommands
        /// </summary>
        private IpfsConfig _config;
        public IpfsConfig Config
        {
            get
            {
                if (_config == null)
                {
                    Uri commandUri = UriHelper.AppendPath(_apiUri, "config");
                    _config = new IpfsConfig(commandUri, _httpClient, _jsonSerializer);
                }

                return _config;
            }
        }

        /// <summary>
        /// Dht subcommands
        /// </summary>
        private IpfsDht _dht;
        public IpfsDht Dht
        {
            get
            {
                if (_dht == null)
                {
                    Uri commandUri = UriHelper.AppendPath(_apiUri, "dht");
                    _dht = new IpfsDht(commandUri, _httpClient, _jsonSerializer);
                }

                return _dht;
            }
        }

        /// <summary>
        /// Diag subcommands
        /// </summary>
        private IpfsDiag _diag;
        public IpfsDiag Diag
        {
            get
            {
                if (_diag == null)
                {
                    Uri commandUri = UriHelper.AppendPath(_apiUri, "diag");
                    _diag = new IpfsDiag(commandUri, _httpClient, _jsonSerializer);
                }

                return _diag;
            }
        }

        /// <summary>
        /// Interact with ipfs objects representing Unix filesystems
        /// </summary>
        private IpfsFile _file;
        public IpfsFile File
        {
            get
            {
                if (_file == null)
                {
                    Uri commandUri = UriHelper.AppendPath(_apiUri, "file");
                    _file = new IpfsFile(commandUri, _httpClient, _jsonSerializer);
                }

                return _file;
            }
        }

        /// <summary>
        /// Log subcommands
        /// </summary>
        private IpfsLog _log;
        public IpfsLog Log
        {
            get
            {
                if (_log == null)
                {
                    Uri commandUri = UriHelper.AppendPath(_apiUri, "log");
                    _log = new IpfsLog(commandUri, _httpClient, _jsonSerializer);
                }

                return _log;
            }
        }

        /// <summary>
        /// Name subcommands
        /// </summary>
        private IpfsName _name;
        public IpfsName Name
        {
            get
            {
                if (_name == null)
                {
                    Uri commandUri = UriHelper.AppendPath(_apiUri, "name");
                    _name = new IpfsName(commandUri, _httpClient, _jsonSerializer);
                }

                return _name;
            }
        }

        /// <summary>
        /// Object subcommands
        /// </summary>
        private IpfsObject _object;
        public IpfsObject Object
        {
            get
            {
                if (_object == null)
                {
                    Uri commandUri = UriHelper.AppendPath(_apiUri, "object");
                    _object = new IpfsObject(commandUri, _httpClient, _jsonSerializer);
                }

                return _object;
            }
        }

        /// <summary>
        /// Pin subcommands
        /// </summary>
        private IpfsPin _pin;
        public IpfsPin Pin
        {
            get
            {
                if (_pin == null)
                {
                    Uri commandUri = UriHelper.AppendPath(_apiUri, "pin");
                    _pin = new IpfsPin(commandUri, _httpClient, _jsonSerializer);
                }

                return _pin;
            }
        }

        /// <summary>
        /// Refs subcommands
        /// </summary>
        private IpfsRefs _refs;
        public IpfsRefs Refs
        {
            get
            {
                if (_refs == null)
                {
                    Uri commandUri = UriHelper.AppendPath(_apiUri, "refs");
                    _refs = new IpfsRefs(commandUri, _httpClient, _jsonSerializer);
                }

                return _refs;
            }
        }

        /// <summary>
        /// Repo subcommands
        /// </summary>
        private IpfsRepo _repo;
        public IpfsRepo Repo
        {
            get
            {
                if (_repo == null)
                {
                    Uri commandUri = UriHelper.AppendPath(_apiUri, "repo");
                    _repo = new IpfsRepo(commandUri, _httpClient, _jsonSerializer);
                }

                return _repo;
            }
        }

        /// <summary>
        /// Query IPFS statistics
        /// </summary>
        private IpfsStats _stats;
        public IpfsStats Stats
        {
            get
            {
                if (_stats == null)
                {
                    Uri commandUri = UriHelper.AppendPath(_apiUri, "stats");
                    _stats = new IpfsStats(commandUri, _httpClient, _jsonSerializer);
                }

                return _stats;
            }
        }

        /// <summary>
        /// Swarm subcommands
        /// </summary>
        private IpfsSwarm _swarm;
        public IpfsSwarm Swarm
        {
            get
            {
                if (_swarm == null)
                {
                    Uri commandUri = UriHelper.AppendPath(_apiUri, "swarm");
                    _swarm = new IpfsSwarm(commandUri, _httpClient, _jsonSerializer);
                }

                return _swarm;
            }
        }

        /// <summary>
        /// utility functions for tar files in ipfs
        /// </summary>
        private IpfsTar _tar;
        public IpfsTar Tar
        {
            get
            {
                if (_tar == null)
                {
                    Uri commandUri = UriHelper.AppendPath(_apiUri, "tar");
                    _tar = new IpfsTar(commandUri, _httpClient, _jsonSerializer);
                }

                return _tar;
            }
        }

        /// <summary>
        /// Tour subcommands
        /// </summary>
        private IpfsTour _tour;
        public IpfsTour Tour
        {
            get
            {
                if (_tour == null)
                {
                    Uri commandUri = UriHelper.AppendPath(_apiUri, "tour");
                    _tour = new IpfsTour(commandUri, _httpClient, _jsonSerializer);
                }

                return _tour;
            }
        }

        #region Root command aliases

        ///// <summary>
        ///// Add an object to ipfs.
        ///// Adds contents of <path> to ipfs. Use -r to add directories.
        ///// Note that directories are added recursively, to form the ipfs
        ///// MerkleDAG.A smarter partial add with a staging area(like git)
        ///// remains to be implemented
        ///// </summary>
        ///// <param name="path">The path to a file to be added to IPFS</param>
        ///// <param name="recursive">Add directory paths recursively</param>
        ///// <param name="quiet">Write minimal output</param>
        ///// <param name="progress">Stream progress data</param>
        ///// <param name="wrapWithDirectory">Wrap files with a directory object</param>
        ///// <param name="trickle">Use trickle-dag format for dag generation</param>
        ///// <returns></returns>
        //public async Task<IList<MerkleNode>> Add(IEnumerable<Tuple<string,Stream>> files, bool recursive = false, bool quiet = false, bool progress = false, bool wrapWithDirectory = false, bool trickle = false)
        //{
        //    return await Root.Add(files, recursive, quiet, progress, wrapWithDirectory, trickle);
        //}

        /// <summary>
        /// Add an object to ipfs.
        /// Adds contents of <path> to ipfs. Use -r to add directories.
        /// Note that directories are added recursively, to form the ipfs
        /// MerkleDAG.A smarter partial add with a staging area(like git)
        /// remains to be implemented
        /// </summary>
        /// <param name="path">The path to a file to be added to IPFS</param>
        /// <param name="recursive">Add directory paths recursively</param>
        /// <param name="quiet">Write minimal output</param>
        /// <param name="progress">Stream progress data</param>
        /// <param name="wrapWithDirectory">Wrap files with a directory object</param>
        /// <param name="trickle">Use trickle-dag format for dag generation</param>
        /// <returns></returns>
        public async Task<MerkleNode> Add(IpfsStream file, bool recursive = false, bool quiet = false, bool wrapWithDirectory = false, bool trickle = false)
        {
            return await Root.Add(file, recursive, quiet, wrapWithDirectory, trickle);
        }

        /// <summary>
        /// Show IPFS object data
        /// Retrieves the object named by <ipfs-path> and outputs the data
        /// it contains.
        /// </summary>
        /// <param name="ipfsPath">The path to the IPFS object(s) to be outputted</param>
        /// <returns></returns>
        public async Task<Stream> Cat(string ipfsPath)
        {
            return await Root.Cat(ipfsPath);
        }

        /// <summary>
        /// List all available commands.
        /// Lists all available commands (and subcommands) and exits.
        /// </summary>
        /// <returns></returns>
        public async Task<Json.IpfsCommand> Commands()
        {
            return await Root.Commands();
        }

        /// <summary>
        /// get and set IPFS config values
        ///   ipfs config controls configuration variables. It works
        ///   much like 'git config'. The configuration values are stored in a config
        ///   file inside your IPFS repository.
        /// </summary>
        /// <param name="key">The key of the config entry (e.g. "Addresses.API")</param>
        /// <param name="value">The value to set the config entry to</param>
        /// <param name="bool">Set a boolean value</param>
        /// <returns></returns>
        public async Task<HttpContent> ConfigCommand(string key, string value = null, bool @bool = false)
        {
            return await Root.ConfigCommand(key, value, @bool);
        }

        /// <summary>
        /// Download IPFS objects
        ///
        /// Retrieves the object named by <ipfs-path> and stores the data to disk.
        ///
        /// By default, the output will be stored at./<ipfs-path>, but an alternate path
        ///
        /// can be specified with '--output=<path>' or '-o=<path>'.
        ///
        /// To output a TAR archive instead of unpacked files, use '--archive' or '-a'.
        ///
        /// To compress the output with GZIP compression, use '--compress' or '-C'. You
        /// may also specify the level of compression by specifying '-l=<1-9>'.
        /// </summary>
        /// <param name="ipfsPath">The path to the IPFS object(s) to be outputted</param>
        /// <param name="output">The path where output should be stored</param>
        /// <param name="archive">Output a TAR archive</param>
        /// <param name="compress">Compress the output with GZIP compression</param>
        /// <param name="compressionLevel">The level of compression (1-9)</param>
        /// <returns></returns>
        public async Task<HttpContent> Get(string ipfsPath, string output = null, bool archive = false, bool compress = false, int? compressionLevel = null)
        {
            return await Root.Get(ipfsPath, output, archive, compress, compressionLevel);
        }

        /// <summary>
        /// Show IPFS Node ID info
        ///
        /// Prints out information about the specified peer,
        /// if no peer is specified, prints out local peers info.
        ///
        /// ipfs id supports the format option for output with the following keys:
        /// <id> : the peers id
        /// <aver>: agent version
        /// <pver>: protocol version
        /// <pubkey>: public key
        /// </summary>
        /// <param name="peerId">peer.ID of node to look up</param>
        /// <param name="format">optional output format</param>
        /// <returns></returns>
        public async Task<IpfsID> Id(string peerId = null, string format = null)
        {
            return await Root.Id(peerId, format);
        }

        /// <summary>
        /// List links from an object.
        ///
        ///  Retrieves the object named by <ipfs-path> and displays the links
        ///  it contains, with the following format:
        ///
        /// <link base58 hash> <link size in bytes> <link name>
        /// </summary>
        /// <param name="path">The path to the IPFS object(s) to list links from</param>
        /// <returns></returns>
        public async Task<IList<MerkleNode>> Ls(string path)
        {
            return await Root.Ls(path);
        }

        /// <summary>
        /// Mounts IPFS to the filesystem (read-only)
        ///
        /// Mount ipfs at a read-only mountpoint on the OS (default: /ipfs and /ipns).
        /// All ipfs objects will be accessible under that directory.Note that the
        /// root will not be listable, as it is virtual. Access known paths directly.
        ///
        /// You may have to create /ipfs and /ipfs before using 'ipfs mount'
        /// </summary>
        /// <param name="f">The path where IPFS should be mounted</param>
        /// <param name="n">The path where IPNS should be mounted</param>
        /// <returns></returns>
        public async Task<HttpContent> Mount(string f = null, string n = null)
        {
            return await Root.Mount(f, n);
        }

        /// <summary>
        /// send echo request packets to IPFS hosts
        ///
        /// ipfs ping is a tool to test sending data to other nodes. It finds nodes
        /// via the routing system, send pings, wait for pongs, and print out round-
        /// trip latency information.
        /// </summary>
        /// <param name="peerId">ID of peer to be pinged</param>
        /// <param name="count">number of ping messages to send</param>
        /// <returns></returns>
        public async Task<IpfsPingResult> Ping(string peerId, int? count = null)
        {
            return await Root.Ping(peerId, count);
        }

        /// <summary>
        /// Lists links (references) from an object
        /// Retrieves the object named by <ipfs-path> and displays the link
        /// hashes it contains, with the following format:
        ///  <link base58 hash>
        /// </summary>
        /// <param name="ipfsPath">Path to the object(s) to list refs from</param>
        /// <param name="format">Emit edges with given format. tokens: <src> <dst> <linkname></param>
        /// <param name="edges">Emit edge format: `<from> -> <to>`</param>
        /// <param name="unique">Omit duplicate refs from output</param>
        /// <param name="recursive">Recursively list links of child nodes</param>
        /// <returns></returns>
        public async Task<HttpContent> RefsCommand(string ipfsPath, string format = null, bool edges = false, bool unique = false, bool recursive = false)
        {
            return await Root.RefsCommand(ipfsPath, format, edges, unique, recursive);
        }

        /// <summary>
        /// An introduction to IPFS
        ///
        /// This is a tour that takes you through various IPFS concepts,
        /// features, and tools to make sure you get up to speed with
        /// IPFS very quickly
        /// </summary>
        /// <param name="id">The id of the topic you would like to tour</param>
        /// <returns></returns>
        public async Task<Stream> TourCommand(string id)
        {
            return await Root.TourCommand(id);
        }

        public async Task<IpfsVersion> Version(bool number = false)
        {
            return await Root.Version(number);
        }

        #endregion Root command aliases

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_httpClient != null) _httpClient.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
