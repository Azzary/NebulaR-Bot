using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Core.Network
{
    public class TcpServer
    {
        public event Action OnServerStarted;
        public event Action<Exception> OnServerFailedToStart;
        public event Action<Socket> OnSocketConnected;

        public Socket Socket
        {
            get;
            private set;
        }

        public IPEndPoint EndPoint
        {
            get;
            set;
        }

        public TcpServer(string ip, int port)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void Start()
        {
            try
            {
                Socket.Bind(EndPoint);
            }
            catch (Exception ex)
            {
                //OnServerFailedToStart(ex);
                return;
            }
            Socket.Listen(100);
            StartAccept(null);
            OnServerStarted?.Invoke();
        }
        protected void StartAccept(SocketAsyncEventArgs args)
        {
            if (args == null)
            {
                args = new SocketAsyncEventArgs();
                args.Completed += AcceptEventCompleted;
            }
            else
            {
                args.AcceptSocket = null;
            }

            bool willRaiseEvent = Socket.AcceptAsync(args);
            if (!willRaiseEvent)
            {
                ProcessAccept(args);
            }
        }
        private void AcceptEventCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }
        public void Stop()
        {
            Socket.Shutdown(SocketShutdown.Both);
        }
        void ProcessAccept(SocketAsyncEventArgs args)
        {
            OnSocketConnected(args.AcceptSocket);
            StartAccept(args);
        }


    }
}
