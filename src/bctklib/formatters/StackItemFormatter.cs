using System;
using System.Buffers;
using System.Numerics;

using StackItem = EpicChain.VM.Types.StackItem;
using StackItemType = EpicChain.VM.Types.StackItemType;
using PrimitiveType = EpicChain.VM.Types.PrimitiveType;

using EpicChainArray = EpicChain.VM.Types.Array;
using EpicChainBoolean = EpicChain.VM.Types.Boolean;
using EpicChainBuffer = EpicChain.VM.Types.Buffer;
using EpicChainByteString = EpicChain.VM.Types.ByteString;
using EpicChainInteger = EpicChain.VM.Types.Integer;
using EpicChainInteropInterface = EpicChain.VM.Types.InteropInterface;
using TraceInteropInterface = EpicChain.BlockchainToolkit.TraceDebug.TraceInteropInterface;
using EpicChainMap = EpicChain.VM.Types.Map;
using EpicChainNull = EpicChain.VM.Types.Null;
using EpicChainPointer = EpicChain.VM.Types.Pointer;
using EpicChainStruct = EpicChain.VM.Types.Struct;

namespace MessagePack.Formatters.EpicChain.BlockchainToolkit
{
    public class StackItemFormatter : IMessagePackFormatter<StackItem>
    {
        public static readonly StackItemFormatter Instance = new StackItemFormatter();

        public StackItem Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var count = reader.ReadArrayHeader();
            if (count != 2) throw new MessagePackSerializationException($"Invalid StackItem Array Header {count}");

            var type = options.Resolver.GetFormatterWithVerify<StackItemType>().Deserialize(ref reader, options);
            switch (type)
            {
                case StackItemType.Any:
                    reader.ReadNil();
                    return StackItem.Null;
                case StackItemType.Boolean:
                    return reader.ReadBoolean() ? StackItem.True : StackItem.False;
                case StackItemType.Buffer:
                    {
                        var bytes = options.Resolver.GetFormatter<byte[]>().Deserialize(ref reader, options);
                        return new EpicChainBuffer(bytes);
                    }
                case StackItemType.ByteString:
                    {
                        var bytes = options.Resolver.GetFormatter<byte[]>().Deserialize(ref reader, options);
                        return new EpicChainByteString(bytes);
                    }
                case StackItemType.Integer:
                    {
                        var integer = options.Resolver.GetFormatterWithVerify<BigInteger>().Deserialize(ref reader, options);
                        return new EpicChainInteger(integer);
                    }
                case StackItemType.InteropInterface:
                    {
                        var typeName = options.Resolver.GetFormatterWithVerify<string>().Deserialize(ref reader, options);
                        return new TraceInteropInterface(typeName);
                    }
                case StackItemType.Pointer:
                    reader.ReadNil();
                    return new EpicChainPointer(Array.Empty<byte>(), 0);
                case StackItemType.Map:
                    {
                        var map = new EpicChainMap();
                        var mapCount = reader.ReadMapHeader();
                        for (int i = 0; i < mapCount; i++)
                        {
                            var key = (PrimitiveType)Deserialize(ref reader, options);
                            map[key] = Deserialize(ref reader, options);
                        }
                        return map;
                    }
                case StackItemType.Array:
                case StackItemType.Struct:
                    {
                        var array = type == StackItemType.Array
                            ? new EpicChainArray()
                            : new EpicChainStruct();
                        var arrayCount = reader.ReadArrayHeader();
                        for (int i = 0; i < arrayCount; i++)
                        {
                            array.Add(Deserialize(ref reader, options));
                        }
                        return array;
                    }
            }

            throw new MessagePackSerializationException($"Invalid StackItem {type}");
        }

        public void Serialize(ref MessagePackWriter writer, StackItem value, MessagePackSerializerOptions options)
        {
            var resolver = options.Resolver;
            var stackItemTypeResolver = resolver.GetFormatterWithVerify<StackItemType>();

            writer.WriteArrayHeader(2);
            switch (value)
            {
                case EpicChainBoolean _:
                    stackItemTypeResolver.Serialize(ref writer, StackItemType.Boolean, options);
                    writer.Write(value.GetBoolean());
                    break;
                case EpicChainBuffer buffer:
                    stackItemTypeResolver.Serialize(ref writer, StackItemType.Buffer, options);
                    writer.Write(buffer.InnerBuffer.Span);
                    break;
                case EpicChainByteString byteString:
                    stackItemTypeResolver.Serialize(ref writer, StackItemType.ByteString, options);
                    writer.Write(byteString.GetSpan());
                    break;
                case EpicChainInteger integer:
                    stackItemTypeResolver.Serialize(ref writer, StackItemType.Integer, options);
                    resolver.GetFormatterWithVerify<BigInteger>().Serialize(ref writer, integer.GetInteger(), options);
                    break;
                case EpicChainInteropInterface interopInterface:
                    {
                        stackItemTypeResolver.Serialize(ref writer, StackItemType.InteropInterface, options);
                        var typeName = interopInterface.GetInterface<object>().GetType().FullName ?? "<unknown InteropInterface>";
                        resolver.GetFormatterWithVerify<string>().Serialize(ref writer, typeName, options);
                    }
                    break;
                case EpicChainMap map:
                    stackItemTypeResolver.Serialize(ref writer, StackItemType.Map, options);
                    writer.WriteMapHeader(map.Count);
                    foreach (var kvp in map)
                    {
                        Serialize(ref writer, kvp.Key, options);
                        Serialize(ref writer, kvp.Value, options);
                    }
                    break;
                case EpicChainNull _:
                    stackItemTypeResolver.Serialize(ref writer, StackItemType.Any, options);
                    writer.WriteNil();
                    break;
                case EpicChainPointer _:
                    stackItemTypeResolver.Serialize(ref writer, StackItemType.Pointer, options);
                    writer.WriteNil();
                    break;
                case EpicChainArray array:
                    {
                        var stackItemType = array is EpicChainStruct ? StackItemType.Struct : StackItemType.Array;
                        stackItemTypeResolver.Serialize(ref writer, stackItemType, options);
                        writer.WriteArrayHeader(array.Count);
                        for (int i = 0; i < array.Count; i++)
                        {
                            Serialize(ref writer, array[i], options);
                        }
                        break;
                    }
                default:
                    throw new MessagePackSerializationException($"Invalid StackItem {value.GetType()}");
            }
        }
    }
}
