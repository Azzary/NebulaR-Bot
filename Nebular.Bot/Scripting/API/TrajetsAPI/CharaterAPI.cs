using Nebular.Bot.Game.Entity;
using Nebular.Bot.Game.Inventory;
using Nebular.Bot.Hook;
using Nebular.Bot.Scripting.API.GeneralAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nebular.Bot.Scripting.API.TrajetsAPI
{
    public class CharaterAPI
    {
        private Character Character { get; set; }
        public CharaterAPI(Character character)
        {
            Character = character;
        }

        public int LastMapID() => Character.LastMapID;
        public string LastMapIDStr() => Character.LastMapID.ToString();
        public int GetMapID() => Character.Map.Id;
        public int ID() => Character.ID;
        public string Name() => Character.Name;
        public bool IsConnected() => Character.IsConnected;
        public byte Level() => Character.Level;
        public byte RaceID() => Character.RaceID;
        public byte Gender() => Character.Gender;
        public Int64 Kamas() => Character.Kamas;
        public CellAPI Cell() => Character.Cell.CellAPI;
        public bool IsDisconnected() => Character.IsDisconnected();
        public bool IsChangingToGame() => Character.IsChangingToGame();
        public bool IsInDialogue() => Character.IsInDialogue();
        public bool IsFighting() => Character.IsFighting();
        public bool IsGathering() => Character.IsGathering();
        public bool IsMoving() => Character.IsMoving();

        
    }
}
