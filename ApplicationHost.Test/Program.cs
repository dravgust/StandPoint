using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StandPoint.Abstractions;
using StandPoint.Blockchain;
using ApplicationBuilder = StandPoint.Abstractions.Builder.ApplicationBuilder;

namespace ApplicationHost.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //NodeSettings nodeSettings = NodeSettings.FromArguments(args);
            var logger = new LoggerFactory().AddConsole(LogLevel.Trace, false);

				var configuration = new ConfigurationBuilder()
		        //.SetBasePath("")
		        //.AddJsonFile()
		        .AddInMemoryCollection(new List<KeyValuePair<string, string>>
		        {
			        new KeyValuePair<string, string>("ApplicationName","blockchain"),
			        new KeyValuePair<string, string>("NetworkFeature:ListeningPort","5555"),
		        })
		        .Build();

	        var fullNode = new ApplicationBuilder()
		        .UseLoggerFactory(logger)
		        .UseConfiguration(configuration)
		        .Build();

	        fullNode.Run();

			return;

                var configuration1 = new ConfigurationBuilder()
                    //.SetBasePath("")
                    //.AddJsonFile()
                    .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("ApplicationName","blockchain"),
                        new KeyValuePair<string, string>("WebSocketManager:Listen","0.0.0.0:9000"),
                    })
                    .Build();

                var node = new ApplicationBuilder()
                    .ConfigureServices(services =>
                    {
                        var blockChain = new BlockchainService(logger.CreateLogger<BlockchainService>());
                        blockChain.Add(blockChain.GenerateNextBlock("test data"));
                        services.AddSingleton(blockChain);
                    })
                    .UseLoggerFactory(logger)
                    .UseConfiguration(configuration1)
                    .Build();

                node.Start();

                var configuration2 = new ConfigurationBuilder()
                    .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("ApplicationName","blockchain"),
                        new KeyValuePair<string, string>("WebSocketManager:EndPoints","127.0.0.1:9000"),
                        new KeyValuePair<string, string>("WebSocketManager:Listen","0.0.0.0:9001"),
                    })
                    .Build();

                var node2 = new ApplicationBuilder()
                    .ConfigureServices(services =>
                    {
                        services.AddSingleton<BlockchainService>(new BlockchainService(logger.CreateLogger<BlockchainService>()));
                    })
                    .UseLoggerFactory(logger)
                    .UseConfiguration(configuration2)
                    .Build();




                node2.Start();

                Console.WriteLine("Press any key to stop");
                Console.ReadLine();
                node.Dispose();
                node2.Dispose();
            }
        }

}