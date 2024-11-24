using System.Collections.Generic;

namespace EpicChain.BlockchainToolkit
{
    public static class Constants
    {
        public const string EXPRESS_EXTENSION = ".epicchain-express";
        public const string DEFAULT_EXPRESS_FILENAME = "default" + EXPRESS_EXTENSION;
        public const string WORKNET_EXTENSION = ".epicchain-worknet";
        public const string DEFAULT_WORKNET_FILENAME = "default" + WORKNET_EXTENSION;


        public static readonly IReadOnlyList<string> MAINNET_RPC_ENDPOINTS = new[]
        {
            "http://mainnet1-seed.epic-chain.org:10111",
            "http://mainnet2-seed.epic-chain.org:10111",
            "http://mainnet3-seed.epic-chain.org:10111",
            "http://mainnet4-seed.epic-chain.org:10111",
            "http://mainnet5-seed.epic-chain.org:10111"
        };

        public static readonly IReadOnlyList<string> TESTNET_RPC_ENDPOINTS = new[]
        {
            "http://testnet1-seed.epic-chain.org:20111",
            "http://testnet2-seed.epic-chain.org:20111",
            "http://testnet3-seed.epic-chain.org:20111",
            "http://testnet4-seed.epic-chain.org:20111",
            "http://testnet5-seed.epic-chain.org:20111"
        };
    }
}
