using Nebular.Core.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nebular.Bot.Game.World.PathFinder
{
    internal class PathFinderUtil
    {
        private static readonly Dictionary<AnimationType, AnimationDuration> animationDurations = new Dictionary<AnimationType, AnimationDuration>()
    {
        { AnimationType.MOUNT, new AnimationDuration(135, 200, 120) },
        { AnimationType.RUNNING, new AnimationDuration(180, 270, 160) },
        { AnimationType.WALKING, new AnimationDuration(480, 510, 425) },
        { AnimationType.GHOST, new AnimationDuration(57, 85, 50) }
    };

        public static int GetTravelTime(List<Cell> cells)
        {
            if (cells.Count < 2) return 0;
            Cell StartCell = cells[0];
            int totalTravelTime = 0;

            for (int i = 1; i < cells.Count; i++)
            {
                Cell currentCell = cells[i - 1];
                Cell nextCell = cells[i];
                int cellTravelTime = GetCellTravelTime(currentCell, nextCell, cells.Count - 1);
                totalTravelTime += cellTravelTime;
            }

            return totalTravelTime;
        }

        public static int GetCellTravelTime(Cell currentCell, Cell target, int TotalLenght, bool withMount = false)
        {
            int movementTime = 20;
            AnimationDuration animationType;

            if (withMount)
                animationType = animationDurations[AnimationType.MOUNT];
            else
                animationType = TotalLenght >= 3 ? animationDurations[AnimationType.RUNNING] : animationDurations[AnimationType.WALKING];

            Cell nextCell;

            nextCell = target;

            if (currentCell.Y == nextCell.Y)
                movementTime += animationType.Horizontal;
            else if (currentCell.X == nextCell.Y)
                movementTime += animationType.Vertical;
            else
                movementTime += animationType.Linear;

            if (currentCell.GroundLevel < nextCell.GroundLevel)
                movementTime += 100;
            else if (nextCell.GroundLevel > currentCell.GroundLevel)
                movementTime -= 100;
            else if (currentCell.GroundSlope != nextCell.GroundSlope)
            {
                if (currentCell.GroundSlope == 1)
                    movementTime += 100;
                else if (nextCell.GroundSlope == 1)
                    movementTime -= 100;
            }

            return movementTime;
        }

        public static List<Cell> getPathFromStrPath(string packet, int startCell, Map map)
        {
            List<Cell> cells = new List<Cell>();
            var message = new MapMouvementPacket(packet);
            List<int> ListCells = new List<int>()
            {
                startCell
            };
            message.Cells.Insert(0, startCell);
            for (int i = 0; i <= message.Cells.Count() - 2; i++)
            {
                ListCells.RemoveAt(ListCells.Count - 1);
                ListCells.AddRange(GetCellsInDirection(message.Cells[i], message.Cells[i + 1], message.Directions[i], map));
            }

            foreach (var item in ListCells)
            {
                cells.Add(map.Cells[item]);
            }
            return cells;
        }


        public static List<int> GetCellsInDirection(int startCell, int targetCell, int intDirection, Map map)
        {
            char direction = Hash.GetDirChar(intDirection);
            var Cells = new List<int>
            {
                startCell
            };
            int i = 0;
            while (targetCell != startCell)
            {
                int newCell = GetCaseIDFromDirrection(startCell, direction, map, false);
                if (newCell < 0 || i > 600)
                    throw new Exception("Invalid Cell");
                Cells.Add(newCell);
                startCell = newCell;
            }
            return Cells;
        }

        public static int GetCaseIDFromDirrection(int CaseID, char Direction,
                                  Map map, bool Combat)
        {
            if (map == null)
                return -1;
            switch (Direction)
            {
                case 'a':
                    return Combat ? -1 : CaseID + 1;
                case 'b':
                    return CaseID + map.Width;
                case 'c':
                    return Combat ? -1 : CaseID + (map.Width * 2 - 1);
                case 'd':
                    return CaseID + (map.Width - 1);
                case 'e':
                    return Combat ? -1 : CaseID - 1;
                case 'f':
                    return CaseID - map.Width;
                case 'g':
                    return Combat ? -1 : CaseID - (map.Width * 2 - 1);
                case 'h':
                    return CaseID - map.Width + 1;
            }
            return -1;
        }

        public static string GetCleanPathfinding(List<Cell> path)
        {
            Cell destinationCell = path.Last();

            if (path.Count < 3)
                return destinationCell.GetDirection(path.First()) + Hash.GetCellChar(destinationCell.Id);

            StringBuilder pathfinder = new StringBuilder();
            char previousDirection = path[1].GetDirection(path.First()), currentDirection;

            for (int i = 2; i < path.Count; i++)
            {
                Cell currentCell = path[i];
                Cell previousCell = path[i - 1];
                currentDirection = currentCell.GetDirection(previousCell);

                if (previousDirection != currentDirection)
                {
                    pathfinder.Append(previousDirection);
                    pathfinder.Append(Hash.GetCellChar(previousCell.Id));

                    previousDirection = currentDirection;
                }
            }

            pathfinder.Append(previousDirection);
            pathfinder.Append(Hash.GetCellChar(destinationCell.Id));
            return pathfinder.ToString();
        }
    }

}
