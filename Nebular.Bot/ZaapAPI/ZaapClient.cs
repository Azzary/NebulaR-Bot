using Nebular.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Reflection;
using Nebular.Zaap.Network;
using Nebular.Zaap.ZaapAPI.TProtocol;
using Nebular.Zaap.ZaapAPI.Protocol;
using Nebular.Core;

namespace Nebular.Zaap.ZaapAPI
{
    [Obfuscation(Exclude = true)]

    public class ZaapClient : Core.Network.TcpClient
    {
        public DofusAccount Account
        {
            get;
            private set;
        }
        public string Password
        {
            get;
            private set;
        }
        private TProtocol.TProtocol TProtocol
        {
            get;
            set;
        }
        public ZaapClient(Socket socket, DofusAccount account) : base(socket)
        {
            this.TProtocol = new TProtocol.TProtocol();

            this.Account = account;
        }
        public override void OnConnected()
        {
            throw new NotImplementedException();
        }

        public override void OnConnectionClosed()
        {
            Console.WriteLine("Client disconnected.");
        }

        public void Send(ZaapMessage message)
        {
            var tMessage = new TMessage()
            {
                Name = "success",
                Type = (int)TMessageType.REPLY,
                SequenceId = 0, // good idea ?
            };

            using (BigEndianWriter writer = new BigEndianWriter())
            {
                TProtocol.WriteMessageBegin(tMessage, writer);

                message.Serialize(TProtocol, writer);

                Socket.BeginSend(writer.Data, 0, writer.Data.Length, SocketFlags.None, OnSended, message);
            }
        }

        protected async override void OnDataArrival(int dataSize)
        {
            ZaapMessage message = null;

            // Retro client
            if(Encoding.ASCII.GetString(Buffer, 0, dataSize).Contains("connect retro"))
            {
                string apiKey = await ZaapConnect.get_ApiKey(Account.Email, Account.Password, Account.Key);
                string token = await ZaapConnect.get_Token(apiKey, EnumClientType.DofusRetro);
                byte[] data = Encoding.UTF8.GetBytes($"auth_getGameToken {token}\0");
                Socket.BeginSend(data, 0, data.Length, SocketFlags.None, OnSended, message);
                return;
            }

            using (BigEndianReader reader = new BigEndianReader(Buffer))
            {
                TMessage tMessage = TProtocol.ReadMessageBegin(reader);

                switch (tMessage.Name)
                {
                    case "connect":
                        message = new ConnectArgs();
                        break;
                    case "settings_get":
                        message = new SettingsGet();
                        break;
                    case "userInfo_get":
                        message = new UserInfoGet();
                        break;
                    case "auth_getGameToken":
                        message = new AuthGetGameToken();
                        break;
                    default:
                        Logger.Write("No message: " + tMessage.Name);
                        return;
                }
                Logger.Write("Received : " + message.GetType().Name);
                message.Deserialize(TProtocol, reader);
            }

            MessagesHandler.Handle(this, message);
        }

        public override void OnDisconnected()
        {
            Console.WriteLine("Zaap client disconnected.");
        }

        public override void OnFailToConnect(Exception ex)
        {
            throw new NotImplementedException();
        }

        public override void OnSended(IAsyncResult result)
        {
            if(result.AsyncState != null)
                Console.WriteLine("Send : " + result.AsyncState.GetType().Name.ToString());
        }
    }
}
