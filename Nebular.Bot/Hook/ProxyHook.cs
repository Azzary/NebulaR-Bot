using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using Nebular.Core;
using System.Reflection;

namespace Nebular.Bot.Hook
{
    [Obfuscation(Exclude = true)]
    public class ProxyHook
    {
        private TcpListener _socketToClient;
        private static readonly object lockObject = new object();
        public static int port { get; private set; }
        EnumClientType enumClientType;
        public void Start()
        {
            port = 8080;
            bool portTrouve = false;
            while (!portTrouve)
            {
                try
                {
                    _socketToClient = new TcpListener(IPAddress.Loopback, port);
                    _socketToClient.Start();
                    portTrouve = true;
                }
                catch (SocketException)
                {
                    port++;
                }
            }
            Console.WriteLine($"Server ProxyHook start on port {port}");

            Thread initHookThread = new Thread(InitHook);
            initHookThread.Start();
        }

        private void InitHook()
        {
            while (true)
            {
                TcpClient clientConnection = _socketToClient.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(_ => InitServerConnection(clientConnection));
            }
        }

        private void InitServerConnection(TcpClient clientConnection)
        {
            NetworkStream clientStream = clientConnection.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = clientStream.Read(buffer, 0, buffer.Length);
            string packet = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            string[] parts = packet.Split(' ');
            string[] addressParts = parts[1].Split(':');
            string ip = addressParts[0];
            int port = int.Parse(addressParts[1]);
            enumClientType = (EnumClientType)int.Parse(addressParts[2]);
            string uid = addressParts[3];
            TcpClient serverConnection = new TcpClient();
            bool success = true;

            try
            {
                serverConnection.Connect(ip, port);
            }
            catch
            {
                success = false;
            }

            byte[] responseBytes = Encoding.ASCII.GetBytes("HTTP/1.0 200 OK");
            clientStream.Write(responseBytes, 0, responseBytes.Length);

            if (!success) return;

            Client client;

            lock (lockObject) 
            {
                if (Client.Clients.ContainsKey(uid))
                {
                    client = Client.Clients[uid];
                }
                else
                {
                    client = new Client(enumClientType, uid);
                    Client.Clients.Add(uid, client);
                }
            }
            client.AddNewConnection(clientConnection, serverConnection);

        }


    }
}
