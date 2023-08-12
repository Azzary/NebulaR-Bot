using Nebular.Bot.Game.Entity;
using Nebular.Bot.Game.Extention;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.World.PathFinder
{
    public class FightPathfinder
    {
        public static FightPath GetFightPath(short currentCell, short targetCell, Dictionary<short, MovementNode> cells)
        {
            if (!cells.ContainsKey(targetCell))
                return null;

            short actual = targetCell;
            List<short> accessibleCells = new List<short>();
            List<short> unreachableCells = new List<short>();
            Dictionary<short, int> accessibleMap = new Dictionary<short, int>();
            Dictionary<short, int> unreachableMap = new Dictionary<short, int>();
            byte distance = 0;

            while (actual != currentCell)
            {
                MovementNode cell = cells[actual];
                if (cell.Reachable)
                {
                    accessibleCells.Insert(0, actual);
                    accessibleMap.Add(actual, distance);
                }
                else
                {
                    unreachableCells.Insert(0, actual);
                    unreachableMap.Add(actual, distance);
                }

                actual = cell.InitialCell;
                distance += 1;
            }


            return new FightPath()
            {
                AccessibleCells = accessibleCells,
                UnreachableCells = unreachableCells,
                AccessibleMap = accessibleMap,
                UnreachableMap = unreachableMap,
            };
        }

        public static Dictionary<short, MovementNode> GetAccessibleCells(FightExtension fight, Map map, Cell currentCell)
        {
            Dictionary<short, MovementNode> cells = new Dictionary<short, MovementNode>();

            if (fight.PlayerFighter.MovementPoints <= 0)
                return cells;

            short maxPm = fight.PlayerFighter.MovementPoints;

            List<FightNode> allowedCells = new List<FightNode>();
            Dictionary<short, FightNode> prohibitedCells = new Dictionary<short, FightNode>();

            FightNode node = new FightNode(currentCell, maxPm, fight.PlayerFighter.ActionPoints, 1);
            allowedCells.Add(node);
            prohibitedCells[currentCell.Id] = node;

            while (allowedCells.Count > 0)
            {
                FightNode actual = allowedCells.Last();
                allowedCells.Remove(actual);
                Cell nodeCell = actual.Cell;
                List<Cell> adjacentCells = GetAdjacentCells(nodeCell, map.Cells);

                int i = 0;
                while (i < adjacentCells.Count)
                {
                    Fighter enemy = fight.GetEnemies.FirstOrDefault(f => f.Cell.Id == adjacentCells[i]?.Id);

                    if (adjacentCells[i] != null && enemy == null)
                    {
                        i++;
                        continue;
                    }
                    adjacentCells.RemoveAt(i);
                }

                int availablePm = actual.AvailablePm - 1;
                int availablePa = actual.AvailablePa;
                int distance = actual.Distance + 1;
                bool reachable = availablePm >= 0;

                for (i = 0; i < adjacentCells.Count; i++)
                {
                    if (prohibitedCells.ContainsKey(adjacentCells[i].Id))
                    {
                        FightNode previous = prohibitedCells[adjacentCells[i].Id];
                        if (previous.AvailablePm > availablePm)
                            continue;

                        if (previous.AvailablePm == availablePm && previous.AvailablePm >= availablePa)
                            continue;
                    }

                    if (!adjacentCells[i].IsWalkable())
                        continue;

                    cells[adjacentCells[i].Id] = new MovementNode(nodeCell.Id, reachable);
                    node = new FightNode(adjacentCells[i], availablePm, availablePa, distance);
                    prohibitedCells[adjacentCells[i].Id] = node;

                    if (actual.Distance < maxPm)
                        allowedCells.Add(node);
                }
            }

            foreach (short cell in cells.Keys)
                cells[cell].Path = GetFightPath(currentCell.Id, cell, cells);

            return cells;
        }

        // Fight doesn't use diagonals
        public static List<Cell> GetAdjacentCells(Cell node, Cell[] mapCells)
        {
            List<Cell> adjacentCells = new List<Cell>();

            Cell rightCell = mapCells.FirstOrDefault(n => n.X == node.X + 1 && n.Y == node.Y);
            Cell leftCell = mapCells.FirstOrDefault(n => n.X == node.X - 1 && n.Y == node.Y);
            Cell lowerCell = mapCells.FirstOrDefault(n => n.X == node.X && n.Y == node.Y + 1);
            Cell upperCell = mapCells.FirstOrDefault(n => n.X == node.X && n.Y == node.Y - 1);

            if (rightCell != null)
                adjacentCells.Add(rightCell);
            if (leftCell != null)
                adjacentCells.Add(leftCell);
            if (lowerCell != null)
                adjacentCells.Add(lowerCell);
            if (upperCell != null)
                adjacentCells.Add(upperCell);

            return adjacentCells;
        }
    }


}
