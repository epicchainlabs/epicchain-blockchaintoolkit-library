using System;
using EpicChain.BlockchainToolkit;
using EpicChain.VM;

namespace MessagePack.Formatters.EpicChain.BlockchainToolkit
{
    public class ScriptFormatter : IMessagePackFormatter<Script>
    {
        public static readonly ScriptFormatter Instance = new ScriptFormatter();

        public Script Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var bytes = options.Resolver.GetFormatter<byte[]>().Deserialize(ref reader, options);
            return new Script(bytes);
        }

        public void Serialize(ref MessagePackWriter writer, Script value, MessagePackSerializerOptions options)
        {
            writer.Write(value.AsSpan());
        }
    }
}
