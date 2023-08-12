using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Core.Network
{
    public abstract class TcpClient
    {
        public const int BUFFER_LENGTH = 8192;

        protected Socket Socket
        {
            get;
            set;
        }

        protected byte[] Buffer
        {
            get;
            set;
        }

        protected int BufferPosition
        {
            get;
            set;
        }
        public IPEndPoint EndPoint
        {
            get
            {
                return Socket.RemoteEndPoint as IPEndPoint;
            }
        }
        public string Ip
        {
            get
            {
                if (EndPoint == null)
                {
                    return null;
                }
                return EndPoint.Address.ToString();
            }
        }
        public bool Connected
        {
            get
            {
                return Socket != null && Socket.Connected;
            }
        }

        public TcpClient()
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Buffer = new byte[BUFFER_LENGTH];
        }
        public TcpClient(Socket socket)
        {
            this.Buffer = new byte[BUFFER_LENGTH];
            this.Socket = socket;
            BeginReceive();
        }

        public abstract void OnConnectionClosed();

        public abstract void OnConnected();

        public abstract void OnDisconnected();


        public abstract void OnFailToConnect(Exception ex);

        protected abstract void OnDataArrival(int dataSize);

        public abstract void OnSended(IAsyncResult result);

        public void Connect(string host, int port)
        {
            Socket?.BeginConnect(new IPEndPoint(IPAddress.Parse(host), port), new AsyncCallback(OnConnectionResulted), Socket);
        }

        public void OnConnectionResulted(IAsyncResult result)
        {
            try
            {
                Socket.EndConnect(result);
                BeginReceive();
                OnConnected();
            }
            catch (Exception ex)
            {
                OnFailToConnect(ex);
            }
        }
        private void BeginReceive()
        {
            try
            {
                Socket?.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, OnReceived, null);
            }
            catch (Exception ex)
            {
                Logger.Write("Unable to receive from client " + ex, Channels.Warning);
                Disconnect();
            }
        }
        public void OnReceived(IAsyncResult result)
        {
            if (Socket == null)
            {
                return;
            }

            int size = 0;
            try
            {
                size = Socket.EndReceive(result);

                if (size == 0)
                {
                    Dispose();
                    OnConnectionClosed();
                    return;
                }

            }
            catch
            {
                Dispose();
                OnConnectionClosed();
                return;
            }

            OnDataArrival(size);
            BeginReceive();
        }
        public void Disconnect()
        {
            if (Socket != null)
            {
                Dispose();

                try
                {
                    OnDisconnected();
                }
                catch (Exception ex)
                {
                    Logger.Write("Unable to disconnect client : " + ex);
                }
            }
        }

        private void Dispose()
        {
            Socket?.Shutdown(SocketShutdown.Both);
            Socket?.Close();
            Socket?.Dispose();
            Socket = null;

        }
    }
}
