
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using EpicChain.Cryptography.MPTTrie;
using EpicChain.IO;
using EpicChain.SmartContract;
using static EpicChain.BlockchainToolkit.Constants;

namespace EpicChain.BlockchainToolkit
{
    public static class Utility
    {

        public static bool TryParseRpcUri(string value, [NotNullWhen(true)] out Uri? uri)
        {
            if (value.Equals("mainnet", StringComparison.OrdinalIgnoreCase))
            {
                uri = new Uri(MAINNET_RPC_ENDPOINTS[0]);
                return true;
            }

            if (value.Equals("testnet", StringComparison.OrdinalIgnoreCase))
            {
                uri = new Uri(TESTNET_RPC_ENDPOINTS[0]);
                return true;
            }

            return Uri.TryCreate(value, UriKind.Absolute, out uri)
                && uri is not null
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        public static (StorageKey key, byte[] value) VerifyProof(UInt256 rootHash, byte[]? proof)
        {
            ArgumentNullException.ThrowIfNull(proof);

            var proofs = new HashSet<byte[]>();

            using MemoryStream stream = new(proof, false);
            using BinaryReader reader = new(stream, EpicChain.Utility.StrictUTF8);

            var keyBuffer = reader.ReadVarBytes(Node.MaxKeyLength);

            var count = reader.ReadVarInt();
            for (ulong i = 0; i < count; i++)
            {
                proofs.Add(reader.ReadVarBytes());
            }

            var value = Trie.VerifyProof(rootHash, keyBuffer, proofs);
            if (value is null) throw new Exception("Verification failed");

            // Note, StorageKey.Deserialized was removed in EpicChain 3.3.0
            //       so VerifyProof has to deserialize StorageKey directly
            //       https://github.com/epicchainlabs/epicchain/issues/
            var key = new StorageKey()
            {
                Id = BinaryPrimitives.ReadInt32LittleEndian(keyBuffer),
                Key = keyBuffer.AsMemory(4)
            };
            return (key, value);
        }
    }
}
