using System;
using EpicChain.Persistence;

namespace EpicChain.BlockchainToolkit.Persistence
{
    public interface ICheckpointStore : IReadOnlyStore
    {
        ProtocolSettings Settings { get; }
    }
}