using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Reflection;
using Nebular.Zaap.Network;
using Nebular.Core;
using Nebular.Core.Network;

namespace Nebular.Zaap.ZaapAPI
{
    [Obfuscation(Exclude = true)]

    public class ZaapServer
    {
        public static DofusAccount SelectedAccount { get; set; }
        public static int port {  get; private set; }
        private TcpServer Server
        {
            get;
            set;
        }

        public void Start()
        {
            port = 26117;
            bool portTrouve = false;
            while (!portTrouve)
            {
                this.Server = new TcpServer("127.0.0.1", port);
                this.Server.OnSocketConnected += Server_OnSocketConnected;
                Server.Start();
                portTrouve = true;
            }

            Logger.Write($"Zaap Server start on port: {port}");
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(60 * 60 * 1000);
                }
            });
        }


        private void Server_OnSocketConnected(System.Net.Sockets.Socket obj)
        {
            Task.Run(() =>
            {
                var zaapClient = new ZaapClient(obj, SelectedAccount);
            });
        }

    }
}
