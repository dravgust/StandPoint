using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using StandPoint.Security.Cryptography;
using StandPoint.Utilities;
using StandPoint.Utilities.Json;

namespace StandPoint.Blockchain
{
    public class BlockchainService
    {
        private List<Block> _blocks;
        private readonly ILogger<BlockchainService> _logger;

        public BlockchainService(ILogger<BlockchainService> logger)
        {
            Guard.NotNull(logger, nameof(logger));
            
            _blocks = new List<Block> {GetGenesisBlock()};
            _logger = logger;
        }

        public Block GetGenesisBlock()
        {
	        var block = new Block();

	        block.Head = 0;
	        block.Header.BlockTime = new DateTimeOffset(2017, 4, 1, 0, 0, 0, TimeSpan.Zero);
	        block.Header.HashPrevBlock = MultiHash.ComputeHash(Enumerable.Range(0, 256).Select(i => (byte) 0).ToArray());
	        block.Header.HashMerkleRoot = Enumerable.Range(0, 256).Select(i => (byte) 0).ToArray();
	        block.Data = null;

			return block;
        }

        public void Add(Block block)
        {
            _logger.LogDebug("add new block: " + block.ToJSON(new DataContractJsonSerializer()));
            _blocks.Add(block);
        }

        public List<Block> Get()
        {
            return _blocks;
        }

        public bool IsValidBlock(Block newBlock, Block previousBlock)
        {
            if (previousBlock.Head + 1 != newBlock.Head)
            {
                return false;
            }
            if (!previousBlock.Header.GetHash().Equals(newBlock.Header.HashPrevBlock))
            {
                return false;
            }
            return true;
        }

        public void ReplaceChain(List<Block> newBlocks)
        {
            if (IsValidChain(newBlocks) && newBlocks.Count > _blocks.Count)
            {
                _logger.LogDebug("Received blockchain is valid. Replacing current blockchain with received blockchain");
                _blocks = newBlocks;
                
            }
            else
            {
                throw new Exception("Received blockchain is invalid");
            }
        }

        public bool IsValidChain(List<Block> blockchainToValidate)
        {
	        var first = blockchainToValidate.DefaultIfEmpty(default(Block)).First();
	        var orig = GetGenesisBlock();

	        var eq = first.Equals(orig);


			if (!blockchainToValidate.DefaultIfEmpty(default(Block)).First().Equals(GetGenesisBlock()))
            {
                return false;
            }
            var tempBlocks = new List<Block> { blockchainToValidate.First() };
            for (var i = 0; i < blockchainToValidate.Count; i++)
            {
                if (IsValidBlock(blockchainToValidate[i], tempBlocks[i - 1]))
                {
                    tempBlocks.Add(blockchainToValidate[i]);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public Block GetLatestBlock()
        {
            return _blocks.Last();
        }

        public Block GenerateNextBlock(object blockData)
        {
            var previousBlock = GetLatestBlock();

            var block = new Block
            {
                Head = previousBlock.Head + 1,
                Header =
                {
                    BlockTime = DateTimeOffset.UtcNow,
                    HashPrevBlock = previousBlock.Header.GetHash(),
					HashMerkleRoot = Enumerable.Range(0, 256).Select(i => (byte)0).ToArray()
				},
                Data = blockData
            };

            return block;
        }
    }
}
