using Nebular.Bot.Game;
using Nebular.Bot.Interface;
using Nebular.Bot.Network;
using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Handlers
{
    public class LoginHandler
    {
        public static Dictionary<string, int> Servers = new Dictionary<string, int>
            {
                { "Boune", 612 },
                { "Galgarion", 614 },
                { "Craill", 613 },
                { "Eratz", 601 },
                { "Henual", 602 },
                { "Tournoi", 635 }
            };


        [Packet("AxK")]
        public Task GetServerList(Dofus client, string packet) => Task.Run(async () =>
        {
            MainUI.GetInstance().InventaireControl.Clear();
            Account account = client.Account;
            GameServer gameServer = account.GameServers;
            gameServer.ServersID.Clear();
            string[] loc5 = packet.Substring(3).Split('|');
            int counter = 1;
            bool selected = false;
            account.Characters.Clear();
            while (counter < loc5.Length && !selected)
            {
                string[] _loc10_ = loc5[counter].Split(',');
                int id = int.Parse(_loc10_[0]);
                gameServer.ServersID.Add(id);
                client.log("id: ", id);
                counter++;
            }
            await Task.Delay(1000);
            await gameServer.SelectServer(getServerIDByName(DofusLoginForm.SelectedAccount.Serveur));
        });

        private static int getServerIDByName(string name)
        {
            if(Servers.ContainsKey(name))
                return Servers[name];
            return 10;
        }

    }
}
