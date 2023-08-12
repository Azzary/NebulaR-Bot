using Nebular.Core;
using Nebular.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nebular.Core.Extensions;
using System.Reflection;

namespace Nebular.IO.Network.Messages
{
    public class ProtocolMessageManager
    {


        //private static readonly Type[] HandlerMethodParameterTypes = new Type[] { typeof(NetworkMessage), typeof(Client) };

        private static readonly Dictionary<uint, Delegate> Handlers = new Dictionary<uint, Delegate>();

        private static readonly Dictionary<ushort, Type> Messages = new Dictionary<ushort, Type>();

        private static readonly Dictionary<ushort, Func<NetworkMessage>> Constructors = new Dictionary<ushort, Func<NetworkMessage>>();

        public static void Initialize(Assembly messagesAssembly)
        {
            foreach (var type in messagesAssembly.GetTypes().Where(x => x.IsSubclassOf(typeof(NetworkMessage))))
            {
                FieldInfo field = type.GetField("Id");
                if (field != null)
                {
                    ushort num = (ushort)field.GetValue(type);
                    if (Messages.ContainsKey(num))
                    {
                        throw new AmbiguousMatchException(string.Format("MessageReceiver() => {0} item is already in the dictionary, old type is : {1}, new type is  {2}",
                            num, Messages[num], type));
                    }
                    Messages.Add(num, type);
                    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                    if (constructor == null)
                    {
                        throw new Exception(string.Format("'{0}' doesn't implemented a parameterless constructor", type));
                    }
                    Constructors.Add(num, constructor.CreateDelegate<Func<NetworkMessage>>());
                }
            }

            Logger.Write(Messages.Count + " Message(s) Loaded | " + Handlers.Count + " Handler(s) Loaded", Channels.Log);
        }

        public static NetworkMessage BuildMessage2(BigEndianReader reader, string type)
        {
            // Vérification de la disponibilité des données pour l'entête du paquet
            if (reader.BytesAvailable < 2)
                return null;

            ushort header = reader.ReadUShort();
            ushort packetId = (ushort)(header >> 2);
            byte lengthType = (byte)(header & 0x03);
            if (type == "Client")
            {
                uint t = reader.ReadUInt();
            }
            int packetLength = 0;

            // Vérification de la disponibilité des données pour la longueur du paquet
            switch (lengthType)
            {
                case 0: // 1 octet de longueur
                    packetLength = reader.ReadByte();
                    break;

                case 1: // 2 octets de longueur
                    packetLength = reader.ReadUShort();
                    break;

                case 2: // 3 octets de longueur
                    byte[] lengthBytes = reader.ReadBytes(3);
                    packetLength = (lengthBytes[0] << 16) | (lengthBytes[1] << 8) | lengthBytes[2];
                    break;
                default:
                    throw new Exception("Type de longueur de paquet inconnu : " + lengthType);
            }

            // Vérification de la disponibilité des données pour le contenu du paquet
            if (reader.BytesAvailable < packetLength)
            {
                reader.Seek((int)reader.Position - 2);
                return null;
            }
            if(Constructors.ContainsKey(packetId))
            return Constructors[packetId]();
            return null;
        }



        public static NetworkMessage BuildMessage(BigEndianReader reader, string type)
        {
            var messagePart = new MessagePart();
            if (messagePart.Build(reader, type == "Client"))
            {
                try
                {
                    BigEndianReader finalReader = null;

                    ushort messageId = (ushort)messagePart.MessageId.Value;

                    if (!Messages.ContainsKey(messageId))
                    {
                        return null;
                    }
                    NetworkMessage message = Constructors[messageId]();

                    if (message == null)
                    {
                        return null;
                    }

                    if (IsIPCMessage(message)) // something wrong here.
                    {
                        reader.Seek(NetworkMessage.MESSAGE_ID_SIZE + NetworkMessage.ComputeTypeLen(messagePart.LengthBytesCount.Value), SeekOrigin.Begin);
                        finalReader = reader;
                    }
                    else
                    {
                        finalReader = new BigEndianReader(messagePart.Data);
                    }

                    message.Unpack(finalReader);
                    return message;
                }
                catch (Exception ex)
                {
                    Logger.Write("Unable to build message : " + ex.Message, Channels.Warning);
                    return null;
                }
            }
            else
                return null;

        }
        public static ushort[] GetMessageIds()
        {
            return Messages.Keys.ToArray();
        }
        private static bool IsIPCMessage(NetworkMessage message)
        {
            return message is IPCMessage;
        }
    }
}
