using Nebular.Bot.Game.Entity;
using Nebular.Bot.Game.Extention;
using Nebular.Bot.Game.World.PathFinder;
using Nebular.Bot.Game.World;
using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nebular.Bot.Scripting.API.GeneralAPI;

namespace Nebular.Bot.Scripting.API.fightAPI
{
    public class FightAPI
    {

        private FightExtension FightExtension { get; set; }

        private Dofus Client { get; set; }

        public FightAPI(FightExtension FightExtension, Dofus client)
        {
            this.FightExtension = FightExtension;
            this.Client = client;
        }

        public int GetTurn()
        {
            return FightExtension.Turn;
        }

        public IEnumerable<Fighter> GetAllEntitys() => FightExtension.GetAllFighters;

        public int GetPathDistanceBetween(CellAPI start, CellAPI end)
        {
            Pathfinding pathfinding = new Pathfinding(Client.Account.Character.Map, true);
            List<Cell> path = pathfinding.FindShortestPath(start.ID(), end.ID(), FightExtension.GetAllFighters.Where(x => x.Cell.Id != end.ID()).Select(x => (int)x.Cell.Id).ToList());
            if (path.Count <= 1)
                return 0;
            path.RemoveAt(path.Count - 1);
            return path.Count;
        }

        public CellAPI GetSafestCell()
        {
            short cellId = -1;
            int totalDistance = -1;
            Cell safestCell = null;

            foreach (Cell currentCell in Client.Account.Character.Map.Cells)
            {
                if (currentCell.IsWalkable() && FightExtension.GetEnemies.FirstOrDefault(x => x.Cell.Id == currentCell.Id) == null)
                {
                    int temporalTotalDistance = FightExtension.GetDistanceFromEnemy(currentCell);

                    if (cellId == -1 || (false && temporalTotalDistance < totalDistance) || (!false && temporalTotalDistance > totalDistance))
                    {
                        cellId = currentCell.Id;
                        safestCell = currentCell;
                        totalDistance = temporalTotalDistance;
                    }
                }
            }
            return safestCell.CellAPI;
        }

        public FighterAPI GetClosestEnemy()
        {
            float distance = -1, tempDistance;
            Fighter closestEnemy = null;

            foreach (Fighter enemy in FightExtension.GetEnemies)
            {
                if (!enemy.IsAlive)
                    continue;

                tempDistance = FightExtension.PlayerFighter.Cell.GetDistance(enemy.Cell);
                if (FightExtension.PlayerFighter.Cell.X == enemy.Cell.X)
                    tempDistance += 0.5f;
                else if (FightExtension.PlayerFighter.Cell.Y == enemy.Cell.Y)
                    tempDistance += 0.5f;

                if (distance == -1 || tempDistance < distance)
                {
                    distance = tempDistance;
                    closestEnemy = enemy;
                }
            }
            return closestEnemy.FighterAPI;
        }

    }
}
