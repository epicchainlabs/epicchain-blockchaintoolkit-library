﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Newtonsoft.Json;

namespace EpicChain.BlockchainToolkit.Models
{
    public class ExpressChain
    {
        private readonly static ImmutableHashSet<uint> KNOWN_NETWORK_NUMBERS = ImmutableHashSet.Create<uint>(
            /* EpicChain 2 MainNet */ 7630401,
            /* EpicChain 2 TestNet */ 1953787457,
            /* EpicChain 3 MainNet */ 860833102,
            /* EpicChain 3 T5 TestNet */ 894710606,
            /* EpicChain 3 T4 TestNet */ 877933390,
            /* EpicChain 3 RC3 TestNet */ 844378958,
            /* EpicChain 3 RC1 TestNet */ 827601742,
            /* EpicChain 3 Preview5 TestNet */ 894448462);

        public static uint GenerateNetworkValue()
        {
            var random = new Random();
            Span<byte> buffer = stackalloc byte[sizeof(uint)];
            while (true)
            {
                random.NextBytes(buffer);
                uint network = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(buffer);

                if (network > 0 && !KNOWN_NETWORK_NUMBERS.Contains(network))
                {
                    return network;
                }
            }
        }

        [JsonProperty("magic")]
        public uint Network { get; set; }

        [JsonProperty("address-version")]
        public byte AddressVersion { get; set; } = ProtocolSettings.Default.AddressVersion;

        [JsonProperty("consensus-nodes")]
        public List<ExpressConsensusNode> ConsensusNodes { get; set; } = new List<ExpressConsensusNode>();

        [JsonProperty("wallets")]
        public List<ExpressWallet> Wallets { get; set; } = new List<ExpressWallet>();

        [JsonProperty("settings")]
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
    }
}
