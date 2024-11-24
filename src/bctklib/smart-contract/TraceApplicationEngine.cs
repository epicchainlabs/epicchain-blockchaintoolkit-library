using System;
using System.Collections.Immutable;
using EpicChain.BlockchainToolkit.TraceDebug;
using EpicChain.Network.P2P.Payloads;
using EpicChain.Persistence;
using EpicChain.SmartContract;
using EpicChain.SmartContract.Native;
using EpicChain.VM;

namespace EpicChain.BlockchainToolkit.SmartContract
{
    public class TraceApplicationEngine : ApplicationEngine
    {
        readonly ITraceDebugSink traceDebugSink;
        ImmutableDictionary<UInt160, string> contractNameMap = ImmutableDictionary<UInt160, string>.Empty;

        public TraceApplicationEngine(ITraceDebugSink traceDebugSink, TriggerType trigger, IVerifiable container,
                                      DataCache snapshot, Block? persistingBlock, ProtocolSettings settings, long epicpulse,
                                      IDiagnostic? diagnostic = null)
            : base(trigger, container, snapshot, persistingBlock, settings, epicpulse, diagnostic)
        {
            this.traceDebugSink = traceDebugSink;

            Log += OnLog!;
            Notify += OnNotify!;
        }

        public override void Dispose()
        {
            Log -= OnLog!;
            Notify -= OnNotify!;
            traceDebugSink.Dispose();
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        private string GetContractName(UInt160 scriptId)
        {
            return ImmutableInterlocked.GetOrAdd(ref contractNameMap, scriptId,
                k => NativeContract.ContractManagement.GetContract(Snapshot, k)?.Manifest.Name ?? string.Empty);
        }

        private void OnNotify(object sender, NotifyEventArgs args)
        {
            if (ReferenceEquals(sender, this))
            {
                traceDebugSink.Notify(args, GetContractName(args.ScriptHash));
            }
        }

        private void OnLog(object sender, LogEventArgs args)
        {
            if (ReferenceEquals(sender, this))
            {
                traceDebugSink.Log(args, GetContractName(args.ScriptHash));
            }
        }

        public override VMState Execute()
        {
            traceDebugSink.ProtocolSettings(ProtocolSettings.Network, ProtocolSettings.AddressVersion);
            traceDebugSink.Script(CurrentContext?.Script ?? Array.Empty<byte>());
            traceDebugSink.Trace(State, EpicPulseConsumed, InvocationStack);
            WriteStorages(CurrentScriptHash);

            return base.Execute();
        }

        protected override void PostExecuteInstruction(Instruction instruction)
        {
            base.PostExecuteInstruction(instruction);

            if (State == VMState.HALT)
            {
                traceDebugSink.Results(State, EpicPulseConsumed, ResultStack);
            }
            traceDebugSink.Trace(State, EpicPulseConsumed, InvocationStack);
            WriteStorages(CurrentScriptHash);
        }

        protected override void OnFault(Exception e)
        {
            base.OnFault(e);
            traceDebugSink.Fault(e);
            traceDebugSink.Trace(State, EpicPulseConsumed, InvocationStack);
        }

        private void WriteStorages(UInt160 scriptHash)
        {
            if (scriptHash != null)
            {
                var contractState = NativeContract.ContractManagement.GetContract(Snapshot, scriptHash);
                if (contractState != null)
                {
                    var storages = Snapshot.Find(StorageKey.CreateSearchPrefix(contractState.Id, default));
                    traceDebugSink.Storages(scriptHash, storages);
                }
            }
        }
    }
}
