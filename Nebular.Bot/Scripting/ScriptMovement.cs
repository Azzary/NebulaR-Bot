using Nebular.Bot.Game.World;
using Nebular.Bot.Game.World.Interactive;
using Nebular.Bot.Game.World.PathFinder;
using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Scripting
{
    internal class ScriptMovement : ScriptObject
    {
        private Dofus Client { get; set; }
        private Map Map => Client.Account.Character.Map;
        private List<Cell> CellUse = new List<Cell>();
        private LuaScript LuaScript { get; set; }

        private List<int> InteractToUse = new List<int>();
        public ScriptMovement(Dofus Client, LuaScript luaScript)
        {
            this.Client = Client;
            this.LuaScript = luaScript;
            InteractToUse = luaScript.GetListObject<int>("gather_elements");
        }

        public async Task<bool> Move(string direction)
        {
            Cell teleportCell = null;
            switch (direction.ToLower())
            {
                case "top":
                    teleportCell = Map.Cells.Where(cell => cell.IsTeleport()).OrderBy(cell => cell.Y).FirstOrDefault();
                    break;
                case "right":
                    teleportCell = Map.Cells.Where(cell => cell.IsTeleport()).OrderByDescending(cell => cell.X).FirstOrDefault();
                    break;
                case "left":
                    teleportCell = Map.Cells.Where(cell => cell.IsTeleport()).OrderBy(cell => cell.X).FirstOrDefault();
                    break;
                case "bot":
                case "bottom":
                    teleportCell = Map.Cells.Where(cell => cell.IsTeleport()).OrderByDescending(cell => cell.Y).FirstOrDefault();
                    break;
                default:
                    if(short.TryParse(direction, out short cellID))
                    {
                        teleportCell = Map.GetCellById(cellID);
                    }
                    break;
            }
            if (teleportCell != null)
            {
                teleportCell.Click();
                return true;
            }
            return false;
        }

        public List<Cell> GetCellAround(Cell centre, int size, CellType cellType = CellType.Any)
        {
            char[] dirs = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', };
            var res = new List<Cell>()
            {
                centre
            };
            for (int a = 0; a < size; a++)
            {
                List<Cell> cases2 = new List<Cell>(res);
                foreach (Cell aCell in cases2)
                {
                    foreach (char d in dirs)
                    {
                        int id = PathFinderUtil.GetCaseIDFromDirrection(aCell.Id, d, Map, false);
                        if (!(Map.Cells.Count() > id) || !(id > -1))
                            continue;
                        Cell cell = Map.Cells[id];
                        if ((cellType == CellType.Any || cell.Type == cellType) && !res.Contains(cell))
                            res.Add(cell);
                    }
                }
            }

            return res;
        }
        
        public bool Harvest()
        {
            try
            {
                List<Cell> CellsInteracts = Map.Interactives
                .Where(interactive => InteractToUse.Contains(interactive.Value.Gfx)
                || Client.Account.Character.Jobs.GetAvailableGatheringSkills().Any(x => interactive.Value.Model.Abilities.Contains(x)))
                .Where(interactive => interactive.Value.IsUsable && !CellUse.Contains(interactive.Value.Cell))
                .Where(interactive => GetCellAround(interactive.Value.Cell, 3, CellType.WalkableCell).Count > 1)
                .Select(interactive => interactive.Value.Cell)
                .OrderBy(cell => cell.GetDistance(Client.Account.Character.Cell))
                .ToList();
                if (CellsInteracts.Count == 0)
                    return false;
                CellsInteracts[0].Interact();
                CellUse.Add(CellsInteracts[0]);
                return true;
            }
            catch (Exception)
            {
            }
            return false;

        }

        public void ChangeMap()
        {
            CellUse.Clear();
        }
    }

}
