using Nebular.Bot.Game.Entity;
using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game
{
    public class Account
    {
        internal GameServer GameServers { get; set; }
        public Dofus Client { get; set; }
        public List<Entity.Character> Characters { get; private set; } = new List<Entity.Character>();
        public Character Character => SelectedIndex == -1 ? null : Characters[SelectedIndex];

        public bool IsFirstFight = true;

        private int SelectedIndex = -1;
        public Account(Dofus Client)
        {
            this.Client = Client;
            GameServers = new GameServer(Client);
        }

        public void SetIndexOfCharacter(string CharacterName)
        {
            SelectedIndex = Characters.FindIndex(x => x.Name.ToLower() == CharacterName.ToLower());
        }
        internal async void SelectCharacter(string CharacterName)
        {
            SetIndexOfCharacter(CharacterName);
            Client.WindowManager.Click(125 *(SelectedIndex + 1) , 300);
            await Task.Delay(300);
            Client.WindowManager.Click(370, 440);
        }

    }
}
