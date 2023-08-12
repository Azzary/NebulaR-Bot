using Nebular.Core.Network.Messages;
using Nebular.Core.Network.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Core.NebularProtocol.IO
{
    public class MessageSerializer
    {
            public static byte[] Serialize(Message packet)
            {
                Type packetType = packet.GetType();
                PropertyInfo[] properties = packetType.GetProperties();
                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(stream))
                    {
                        var idProperty = packet.GetType().GetProperty("Id");
                        binaryWriter.Write((int)idProperty.GetValue(null));
                        foreach (PropertyInfo property in properties)
                        {
                            if (property.CanRead && property.CanWrite)
                            {
                                var value = property.GetValue(packet);
                                WriteValue(binaryWriter, value);
                            }
                        }
                    }

                    byte[] serializedData = stream.ToArray();
                    byte[] packetSizeBytes = BitConverter.GetBytes(serializedData.Length);
                    var baseByte = packetSizeBytes.Concat(serializedData).ToArray();
                    Console.WriteLine("size: " + baseByte.Length);
                    return baseByte;
                }
            }


            public static Message Deserialize(byte[] data)
            {
                using (MemoryStream stream = new MemoryStream(data))
                {
                    using (BinaryReader binaryReader = new BinaryReader(stream))
                    {
                        int id = binaryReader.ReadInt32();
                        Message packet = MessageHandler.CreatePacketInstance(id);
                        if (packet != null)
                        {
                            Type packetType = packet.GetType();
                            PropertyInfo[] properties = packetType.GetProperties();
                            foreach (PropertyInfo property in properties)
                            {
                                if (property.CanRead && property.CanWrite)
                                {
                                    object value = ReadValue(binaryReader, property.PropertyType);
                                    property.SetValue(packet, value);
                                }
                            }
                        }
                        return packet;
                    }
                }
            }


            private static void WriteValue(BinaryWriter writer, object value)
            {
                Type valueType = value.GetType();

                if (valueType == typeof(int))
                {
                    writer.Write((int)value);
                }
                else if (value is Message)
                {
                    writer.Write(Serialize((Message)value));
                }
                else if (valueType == typeof(uint))
                {
                    writer.Write((uint)value);
                }
                else if (valueType == typeof(short))
                {
                    writer.Write((short)value);
                }
                else if (valueType == typeof(ushort))
                {
                    writer.Write((ushort)value);
                }
                else if (valueType == typeof(string))
                {
                    writer.Write((string)value);
                }
                else if (valueType == typeof(bool))
                {
                    writer.Write((bool)value);
                }
                else if (valueType == typeof(byte))
                {
                    writer.Write((byte)value);
                }
                else if (valueType == typeof(sbyte))
                {
                    writer.Write((sbyte)value);
                }
                else if (valueType == typeof(char))
                {
                    writer.Write((char)value);
                }
                else if (valueType == typeof(double))
                {
                    writer.Write((double)value);
                }
                else if (valueType == typeof(float))
                {
                    writer.Write((float)value);
                }
                else if (valueType == typeof(long))
                {
                    writer.Write((long)value);
                }
                else if (valueType == typeof(ulong))
                {
                    writer.Write((ulong)value);
                }
                else if (valueType == typeof(decimal))
                {
                    writer.Write((decimal)value);
                }
                else if (valueType == typeof(byte[]))
                {
                    var array = (byte[])value;
                    writer.Write(array.Length);
                    writer.Write((byte[])value);
                }
                else if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var elementType = valueType.GetGenericArguments()[0];
                    var writeListMethod = typeof(MessageSerializer).GetMethod("WriteList").MakeGenericMethod(elementType);
                    writeListMethod.Invoke(null, new object[] { writer, value });
                }
                else
                {
                    throw new NotSupportedException($"Type not supported: {valueType}");
                }
            }

            public static void WriteList<T>(BinaryWriter writer, List<T> value)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryWriter _writer = new BinaryWriter(stream))
                    {
                        foreach (var item in value)
                        {
                            WriteValue(_writer, item);
                        }
                    }

                    byte[] serializedData = stream.ToArray();
                    byte[] packetSizeBytes = BitConverter.GetBytes(serializedData.Length);

                    writer.Write(packetSizeBytes.Concat(serializedData).ToArray());
                }
            }

            private static object ReadValue(BinaryReader reader, Type valueType)
            {
                if (valueType == typeof(int))
                {
                    return reader.ReadInt32();
                }
                else if (valueType.IsSubclassOf(typeof(Message)))
                {
                    int size = reader.ReadInt32();
                    return Deserialize(reader.ReadBytes(size));
                }
                else if (valueType == typeof(uint))
                {
                    return reader.ReadUInt32();
                }
                else if (valueType == typeof(short))
                {
                    return reader.ReadInt16();
                }
                else if (valueType == typeof(ushort))
                {
                    return reader.ReadUInt16();
                }
                else if (valueType == typeof(string))
                {
                    return reader.ReadString();
                }
                else if (valueType == typeof(bool))
                {
                    return reader.ReadBoolean();
                }
                else if (valueType == typeof(byte))
                {
                    return reader.ReadByte();
                }
                else if (valueType == typeof(sbyte))
                {
                    return reader.ReadSByte();
                }
                else if (valueType == typeof(char))
                {
                    return reader.ReadChar();
                }
                else if (valueType == typeof(double))
                {
                    return reader.ReadDouble();
                }
                else if (valueType == typeof(float))
                {
                    return reader.ReadSingle();
                }
                else if (valueType == typeof(long))
                {
                    return reader.ReadInt64();
                }
                else if (valueType == typeof(ulong))
                {
                    return reader.ReadUInt64();
                }
                else if (valueType == typeof(decimal))
                {
                    return reader.ReadDecimal();
                }
                else if (valueType == typeof(byte[]))
                {
                    var len = reader.ReadInt32();
                    return reader.ReadBytes(len);
                }
                else if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var elementType = valueType.GetGenericArguments()[0];
                    var readListMethod = typeof(MessageSerializer).GetMethod("ReadList").MakeGenericMethod(elementType);
                    return readListMethod.Invoke(null, new object[] { reader });
                }
                else
                {
                    throw new NotSupportedException($"Type not supported: {valueType}");
                }
            }

            public static List<T> ReadList<T>(BinaryReader reader)
            {
                List<T> result = new List<T>();

                int packetSize = reader.ReadInt32();
                long startPosition = reader.BaseStream.Position;

                while (reader.BaseStream.Position - startPosition < packetSize)
                {
                    result.Add((T)ReadValue(reader, typeof(T)));
                }

                return result;
            }


    }
}
