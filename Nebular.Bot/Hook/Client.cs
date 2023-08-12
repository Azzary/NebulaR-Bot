using Nebular.Core.Network.Messages;
using Nebular.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Nebular.IO;
using Nebular.Bot.Network;
using System.Threading.Tasks;
using Nebular.Core.Network;
using Nebular.Bot.Scripting;
using Nebular.IO.Network.Messages;
using Nebular.Core.NebularProtocol.Messages;
using System.Reflection;

namespace Nebular.Bot.Hook
{
    [Obfuscation(Exclude = true)]
    public class Client
    {
        public static Dictionary<string, Client> Clients = new Dictionary<string, Client>();
        public const int BUFFER_LENGTH = 8192;
        private EnumClientType clientType;
        public string UID { get; private set; }

        public Client(EnumClientType clientType, string uid) 
        {
            this.clientType = clientType;
            UID = uid;
        }

        public void AddNewConnection(System.Net.Sockets.TcpClient clientConnection, System.Net.Sockets.TcpClient serverConnection)
        {
            ThreadPool.QueueUserWorkItem(_ => TransferData(clientConnection, serverConnection, clientType, "Client"));
            ThreadPool.QueueUserWorkItem(_ => TransferData(serverConnection, clientConnection, clientType, "Server"));
        }


        private static List<PacketWaiter> waitingPackets = new List<PacketWaiter>();

        public static async Task<bool> WaitForPacket(int timeout, params string[] packets)
        {
            PacketWaiter packetWaiter = new PacketWaiter(packets.ToList());
            lock (waitingPackets)
                waitingPackets.Add(packetWaiter);
            bool allRecv = await packetWaiter.WaitForPacket(timeout);
            lock (waitingPackets)
                waitingPackets.Remove(packetWaiter);
            return allRecv;
        }


        private void TransferData(System.Net.Sockets.TcpClient source, System.Net.Sockets.TcpClient destination, EnumClientType clientVersion, string type)
        {
            byte[] buffer = new byte[BUFFER_LENGTH];
            byte[] dataRecvbuffer = new byte[BUFFER_LENGTH];
            int BufferPosition = 0;
            try
            {
                using (NetworkStream sourceStream = source.GetStream(), destStream = destination.GetStream())
                {
                    int bytesRead;
                    byte[] dataForNext = new byte[0];
                    while ((bytesRead = sourceStream.Read(dataRecvbuffer, 0, dataRecvbuffer.Length)) != 0)
                    {
                        destStream.Write(dataRecvbuffer, 0, bytesRead);
                        BufferPosition = 0;
                        if (clientVersion == EnumClientType.DofusRetro)
                        {
                            string stringData = Encoding.UTF8.GetString(dataRecvbuffer, 0, bytesRead);
                            if(type == "Client")
                                stringData = stringData.Split('ù')[stringData.Split('ù').Count() - 1];
                            string[] splitData = stringData.Split('\0');
                            foreach (string item in splitData)
                            {
                                Console.WriteLine($"[{type}] : {item}");
                                PacketHandler.Receive(MainUI.client, item, type);
                                lock (waitingPackets)
                                {
                                    foreach (PacketWaiter waitingPacket in waitingPackets)
                                        waitingPacket.CheckPacket(item);
                                }
                                if(MainUI.Script != null)
                                {
                                    MainUI.Script.AddPacket(item);
                                    MainUI.Script.CheckPackets();
                                }
                                    
                            }
                        }
                        else
                        {
                            OnDataArrival(ref dataRecvbuffer, bytesRead, ref BufferPosition, type);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Write("Client IOException");
            }
            finally
            {
                source.Close();
                destination.Close();
            }
        }

        private void OnDataArrival(ref byte[] Buffer, int dataSize, ref int BufferPosition, string type)
        {
            if (Buffer.Max() == 0)
            {
                BufferPosition = dataSize;
            }
            else
            {
                BufferPosition += dataSize;

                if (BufferPosition > BUFFER_LENGTH)
                {
                    throw new Exception("Too large amount of data."); // todo copy in new buffer
                }
            }

            while (BufferPosition > 0)
            {
                using (BigEndianReader reader = new BigEndianReader(Buffer))
                {
                    int lastBufferPosition = BufferPosition;
                    NetworkMessage message = ProtocolMessageManager.BuildMessage2(reader, type);

                    if (message != null)
                    {
                        Logger.Write(message.GetType().Name);
                    }
                    else
                    {
                        BufferPosition = lastBufferPosition;
                        return;
                    }

                    var newBuffer = new byte[BUFFER_LENGTH];
                    Array.Copy(Buffer, reader.Position, newBuffer, 0, newBuffer.Length - reader.Position);

                    Buffer = newBuffer;

                    BufferPosition -= (int)reader.Position;

                }
            }


        }


    }
}
