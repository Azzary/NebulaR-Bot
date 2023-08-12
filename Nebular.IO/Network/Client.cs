using Nebular.Core.Network.Messages;
using Nebular.IO;
using Nebular.IO.Network.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Core.Network
{
    public abstract class Client : TcpClient
    {
        public Client()
        {

        }
        public Client(Socket socket) : base(socket)
        {

        }
        public abstract void OnMessageReceived(NetworkMessage message);

        public abstract void OnMessageUnhandled(NetworkMessage message);

        public abstract void OnHandlingError(NetworkMessage message, Delegate handler, Exception ex);

        public void Send(NetworkMessage message)
        {
            if (Socket != null && Socket.Connected)
            {
                try
                {
                    using (var writer = new BigEndianWriter())
                    {
                        message.Pack(writer);
                        Socket.BeginSend(writer.Data, 0, writer.Data.Length, SocketFlags.None, OnSended, message);
                    }
                }
                catch
                {
                    Disconnect();
                }
            }
        }
        protected override void OnDataArrival(int dataSize)
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
                    // ICI
                    NetworkMessage message = ProtocolMessageManager.BuildMessage(reader, "");

                    if (message != null)
                    {
                        OnMessageReceived(message);
                    }
                    else
                    {
                        Disconnect();
                        BufferPosition = 0;
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