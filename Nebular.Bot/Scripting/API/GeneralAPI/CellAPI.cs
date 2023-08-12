using Nebular.Bot.Game.World;
using Nebular.Bot.Game.World.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Scripting.API.GeneralAPI
{
    public class CellAPI
    {

        private Cell Cell { get; set; }
        public CellAPI(Cell cell)
        {
            Cell = cell;
        }

        public int ID() => Cell.Id;
        public void Click() => Cell.Click();
        public void Interact() => Cell.Interact();
        public int GetX() => Cell.X;
        public int GetY() => Cell.Y;
        public bool IsTeleport() => Cell.IsTeleport();
        public bool IsInteractive() => Cell.IsInteractive();
        public bool IsWalkable() => Cell.IsWalkable();
        public bool IsWalkableInteractive() => Cell.IsWalkableInteractive();
    }
}
