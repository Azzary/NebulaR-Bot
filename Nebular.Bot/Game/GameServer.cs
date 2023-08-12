using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game
{
    internal class GameServer
    {
        private Dofus Dofus { get; set; }
        public List<int> ServersID { get; private set; } = new List<int>();
        public GameServer(Dofus window)
        {
            Dofus = window;
        }

        public async Task SelectServer(int ID)
        {
            int index = ServersID.IndexOf(ID);
            if (index == -1)
            {
                Dofus.log("Impossible de trouver le server avez vous un perssonage dessus ?");
            }
            else
            {
                index++;
                Dofus.WindowManager.Click(125 * index, 300);
                await Task.Delay(300);
                Dofus.WindowManager.Click(550, 440);
            }
        }
    }
}
