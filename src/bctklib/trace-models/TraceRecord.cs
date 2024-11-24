using System;
using System.Buffers;
using System.Collections.Generic;
using MessagePack;
using EpicChain.VM;

namespace EpicChain.BlockchainToolkit.TraceDebug
{
    [MessagePackObject]
    public partial class TraceRecord : ITraceDebugRecord
    {
        public const int RecordKey = 0;

        [Key(0)]
        public readonly VMState State;
        [Key(1)]
        public readonly long EpicPulseConsumed;
        [Key(2)]
        public readonly IReadOnlyList<StackFrame> StackFrames;

        public TraceRecord(VMState state, long epicpulseConsumed, IReadOnlyList<StackFrame> stackFrames)
        {
            State = state;
            EpicPulseConsumed = epicpulseConsumed;
            StackFrames = stackFrames;
        }

        public static void Write(IBufferWriter<byte> writer,
                                 MessagePackSerializerOptions options,
                                 VMState vmState,
                                 long epicpulseConsumed,
                                 IReadOnlyCollection<ExecutionContext> contexts,
                                 Func<ExecutionContext, UInt160> getScriptIdentifier)
        {
            var mpWriter = new MessagePackWriter(writer);
            Write(ref mpWriter, options, vmState, epicpulseConsumed, contexts, getScriptIdentifier);
            mpWriter.Flush();
        }

        public static void Write(ref MessagePackWriter writer,
                                 MessagePackSerializerOptions options,
                                 VMState vmState,
                                 long epicpulseConsumed,
                                 IReadOnlyCollection<ExecutionContext> contexts,
                                 Func<ExecutionContext, UInt160> getScriptIdentifier)
        {
            writer.WriteArrayHeader(2);
            writer.WriteInt32(RecordKey);
            writer.WriteArrayHeader(3);
            options.Resolver.GetFormatterWithVerify<VMState>().Serialize(ref writer, vmState, options);
            writer.WriteInt64(epicpulseConsumed);
            writer.WriteArrayHeader(contexts.Count);
            foreach (var context in contexts)
            {
                StackFrame.Write(ref writer, options, context, getScriptIdentifier(context));
            }
        }
    }
}
