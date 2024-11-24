using System.Buffers;
using System.Collections.Generic;
using MessagePack;
using EpicChain.VM;
using StackItem = EpicChain.VM.Types.StackItem;

namespace EpicChain.BlockchainToolkit.TraceDebug
{
    [MessagePackObject]
    public class ResultsRecord : ITraceDebugRecord
    {
        public const int RecordKey = 3;

        [Key(0)]
        public readonly VMState State;
        [Key(1)]
        public readonly long EpicPulseConsumed;
        [Key(2)]
        public readonly IReadOnlyList<StackItem> ResultStack;

        public ResultsRecord(VMState vmState, long c, IReadOnlyList<StackItem> resultStack)
        {
            State = vmState;
            EpicPulseConsumed = epicpulseConsumed;
            ResultStack = resultStack;
        }

        public static void Write(IBufferWriter<byte> writer, MessagePackSerializerOptions options, VMState vmState, long epicpulseConsumed, IReadOnlyCollection<StackItem> resultStack)
        {
            var mpWriter = new MessagePackWriter(writer);
            Write(ref mpWriter, options, vmState, epicpulseConsumed, resultStack);
            mpWriter.Flush();
        }

        public static void Write(ref MessagePackWriter writer, MessagePackSerializerOptions options, VMState vmState, long epicpulseConsumed, IReadOnlyCollection<StackItem> resultStack)
        {
            writer.WriteArrayHeader(2);
            writer.WriteInt32(RecordKey);
            writer.WriteArrayHeader(3);
            options.Resolver.GetFormatterWithVerify<VMState>().Serialize(ref writer, vmState, options);
            writer.Write(EpicPulseConsumed);
            options.Resolver.GetFormatterWithVerify<IReadOnlyCollection<StackItem>>().Serialize(ref writer, resultStack, options);
        }
    }
}
