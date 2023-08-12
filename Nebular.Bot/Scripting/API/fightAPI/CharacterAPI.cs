using Nebular.Bot.Game;
using Nebular.Bot.Game.Entity;
using Nebular.Bot.Game.Extention;
using Nebular.Bot.Game.World;
using Nebular.Bot.Game.World.PathFinder;
using Nebular.Bot.Scripting.API.GeneralAPI;
using Nebular.Core;
using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Scripting.API.fightAPI
{
    public class CharacterAPI
    {

        private PlayerFighter PlayerFighter { get; set; }
        private FightExtension FightExtension { get;set; }
        private Dofus Client { get; set; }

        public CharacterAPI(FightExtension fightExtension, Dofus client) 
        {
            this.FightExtension = fightExtension;
            this.PlayerFighter = fightExtension.PlayerFighter;
            this.Client = client;
        }

        public IEnumerable<Fighter> GetAllies() => FightExtension.GetAllies;
        public IEnumerable<Fighter> GetEnemies() => FightExtension.GetEnemies;
        public bool PlaceOn(short cellID)
        {
            return PlaceOn(Client.Account.Character.Map.GetCellById(cellID).CellAPI);
        }

        public bool PlaceOn(CellAPI cell, int nbTry = 0)
        {
            int i = 0;
            while (FightExtension.FightState != FightState.Positioning || PlayerFighter == null)
            {
                Task.Delay(1000).Wait();
                i++;
                if (i == 10)
                    return false;
            }
            Client.WindowManager.ActivateWindow();
            if (cell.ID() == FightExtension.PlayerFighter.Cell.Id)
            {
                return true;
            }
            var waitTask = Hook.Client.WaitForPacket(1000, "GIC|"+PlayerFighter.Id+";"+cell.ID());
            cell.Click();
            waitTask.Wait();
            if (waitTask.Result == true)
                return true;
            if(nbTry == 5) return false;
            nbTry++;
            return PlaceOn(cell, nbTry);

        }
        // GIC|120653514;408
        public void PlaceCloser()
        {
            var targetCell = FightExtension.GetClosestOrFarthestCell(true, FightExtension.PlacementCell);
            PlaceOn(FightExtension.Character.Map.GetCellById(targetCell).CellAPI);
        }
        public void PlaceFarest()
        {
            var targetCell = FightExtension.GetClosestOrFarthestCell(false, FightExtension.PlacementCell);
            PlaceOn(FightExtension.Character.Map.GetCellById(targetCell).CellAPI);
        }
        public CellAPI GetCell() => PlayerFighter.Cell.CellAPI;
        public int GetLife() => PlayerFighter.CurrentLife;
        public int GetMaxLife() => PlayerFighter.MaxLife;
        public int GetPA() => PlayerFighter.ActionPoints;
        public int GetPM() => PlayerFighter.MovementPoints;

        public void EndTurn() => Client.WindowManager.Press(Core.VirtualKeys.F1);

        public int Move(short cellID, int nbTry = 0)
        {
            var cell = FightExtension.Character.Map.GetCellById(cellID).CellAPI;
            Pathfinding pathfinding = new Pathfinding(Client.Account.Character.Map, true);
            List<Cell> path = pathfinding.FindShortestPath(PlayerFighter.Cell.Id, cell.ID(), FightExtension.GetAllFighters.Where(x => x.Cell.Id != cell.ID()).Select(x => (int)x.Cell.Id).ToList());
            if (path.Count < 1)
                return 0;
            if (PlayerFighter.MovementPoints == 0)
                return path.Count();

            int distance = 0;
            Cell CellTarget;
            if (path.Count > PlayerFighter.MovementPoints)
            {
                CellTarget = path[PlayerFighter.MovementPoints - 1];
                distance = path.Count() - (path.IndexOf(CellTarget) + 1);
            }
            else
            {
                CellTarget = path.Last();
                distance = 0;
            }
            if (CellTarget.Click())
            {
                nbTry++;
                return Move(cellID, nbTry);
            }

            Task.Delay(220 * (path.IndexOf(CellTarget) + 1) + PathFinderUtil.GetTravelTime(path.GetRange(0, path.IndexOf(CellTarget)))).Wait(); // Attendez que la tâche soit terminée (blocage ici, à utiliser avec précaution)
            return distance;
        }
        public int Move(CellAPI cell, int nbTry = 0)
        {
            Pathfinding pathfinding = new Pathfinding(Client.Account.Character.Map, true);
            List<Cell> path = pathfinding.FindShortestPath(PlayerFighter.Cell.Id, cell.ID(), FightExtension.GetAllFighters.Where(x => x.Cell.Id != cell.ID()).Select(x => (int)x.Cell.Id).ToList());
            if (path.Count <= 1)
                return 0;
            path.RemoveAt(path.Count - 1);
            if (PlayerFighter.MovementPoints == 0)
                return path.Count();

            int distance = 0;
            Cell CellTarget;
            if (path.Count > PlayerFighter.MovementPoints)
            {
                CellTarget = path[PlayerFighter.MovementPoints - 1];
                distance = path.Count() - (path.IndexOf(CellTarget) + 1);
            }
            else
            {
                CellTarget = path.Last();
                distance = 0;
            }
            if (CellTarget.Click())
            {
                nbTry++;
                return Move(cell, nbTry);
            }

            Task.Delay(220 * (path.IndexOf(CellTarget)+1) + PathFinderUtil.GetTravelTime(path.GetRange(0, path.IndexOf(CellTarget)))).Wait(); // Attendez que la tâche soit terminée (blocage ici, à utiliser avec précaution)
            return distance;
        }

        public bool LauchSpell(string key, CellAPI Target)
        {
            if (Enum.TryParse(key, out VirtualKeys virtualKey))
                return LauchSpell(virtualKey, Target, 0);
            else
                Console.WriteLine($"La touche '{key}' n'est pas une valeur valide de l'enum VirtualKeys.");
            return false;
        }
        public bool LauchSpell(string key, short cellID) => LauchSpell(key, FightExtension.Character.Map.GetCellById(cellID).CellAPI);

        public bool LauchSpell(string key, CharacterAPI Target) => LauchSpell(key, Target.GetCell());

        public bool LauchSpell(string key, FighterAPI Target) => LauchSpell(key, Target.Cell());

        private bool LauchSpell(Core.VirtualKeys key, CellAPI Target, short TotalTry = 0)
        {
            TotalTry++;
            if (TotalTry > 4 || FightExtension.FightState != FightState.Fight)
                return false;
            Client.WindowManager.ActivateWindow();
            Task.Delay(100).Wait();
            Client.WindowManager.Press(key);
            Task.Delay(100).Wait();
            var res = Hook.Client.WaitForPacket(3500, "GA;300;");
            Target.Click();
            res.Wait();
            Console.WriteLine(res.Result.ToString());
            //Task.Delay(200).Wait();
            if (res.Result == false)
            {
                TotalTry++;
                Client.log("[SPELL]", "Spell non lancer.. " + TotalTry);
                return LauchSpell(key, Target, TotalTry);
            }
            return true;
        }

    }
}
