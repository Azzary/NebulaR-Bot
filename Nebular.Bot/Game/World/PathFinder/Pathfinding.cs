using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.World.PathFinder
{
    public class Pathfinding
    {
        public Map Map { get; set; }

        public List<CellPathfinding> Cells = new List<CellPathfinding>();
        public static int CELL_DISTANCE_VALUE = 10;

        private List<CellPathfinding> openList = new List<CellPathfinding>();
        private List<CellPathfinding> closedList = new List<CellPathfinding>();
        private bool fightPath;
        public Pathfinding(Map map, bool fightPath)
        {
            this.fightPath = fightPath;
            this.Map = map;
        }

        private void initialize()
        {
            this.openList = new List<CellPathfinding>();
            this.closedList = new List<CellPathfinding>();
            Cells.Clear();
            Map.Cells.ToList().ForEach(x => Cells.Add(new CellPathfinding(x)));
        }

        public CellPathfinding GetCell(int cell)
        {
            return this.Cells.FirstOrDefault(x => x.cell.Id == cell);
        }

        public List<int> GetCells(List<int> cells, List<int> directions)
        {
            List<int> traversedCells = new List<int>();

            if (cells.Count < 2 || directions.Count < 2)
                return traversedCells;



            for (int i = 0; i < directions.Count && i + 1 < directions.Count; i++)
            {
                if (getDirBetweenTwoCase(cells[i], cells[i + 1]) != directions[0])
                    return new List<int>();

                traversedCells.Add(cells[i]);
            }

            return traversedCells;
        }

        public int getDirBetweenTwoCase(int cell1ID, int cell2ID)
        {
            List<char> dirs = new List<char>();
            dirs.Add('b');
            dirs.Add('d');
            dirs.Add('f');
            dirs.Add('h');
            if (!fightPath)
            {
                dirs.Add('a');
                dirs.Add('b');
                dirs.Add('c');
                dirs.Add('d');
            }
            foreach (char c in dirs)
            {
                int cell = cell1ID;
                for (int i = 0; i <= 64; i++)
                {
                    if (GetCaseIDFromDirrection(cell, c) == cell2ID)
                        return GetDirNum(c);
                    cell = GetCaseIDFromDirrection(cell, c);
                }
            }
            return -100;
        }

        public int NextCell(int cell, int dir)
        {
            switch (dir)
            {
                case 0:
                    return cell + 1;

                case 1:
                    return cell + Map.Width;

                case 2:
                    return cell + (Map.Width * 2) - 1;

                case 3:
                    return cell + Map.Width - 1;

                case 4:
                    return cell - 1;

                case 5:
                    return cell - Map.Width;

                case 6:
                    return cell - (Map.Width * 2) + 1;

                case 7:
                    return cell - Map.Width + 1;

            }
            return -1;
        }

        public List<int> GetJoinCell(int cell, bool inFight = true)
        {
            List<int> cells = new List<int>();

            if (!inFight)
            {
                cells.Add(NextCell(cell, 6));//356
                cells.Add(NextCell(cell, 5));//370
                cells.Add(NextCell(cell, 4));//384
                cells.Add(NextCell(cell, 3));//399
                cells.Add(NextCell(cell, 2));//414
                cells.Add(NextCell(cell, 1));//400
                cells.Add(NextCell(cell, 0));//386
                cells.Add(NextCell(cell, 7));//371
            }
            else
            {
                cells.Add(NextCell(cell, 1));
                cells.Add(NextCell(cell, 3));
                cells.Add(NextCell(cell, 5));
                cells.Add(NextCell(cell, 7));
            }
            return cells;
        }
        public static int GetDirNum(char DirChar)
        {
            string hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
            return hash.IndexOf(DirChar);
        }

        public int GetCaseIDFromDirrection(int CaseID, char Direction)
        {
            if (Map == null)
                return -1;
            switch (Direction)
            {
                case 'a':
                    return fightPath ? -1 : CaseID + 1;
                case 'b':
                    return CaseID + Map.Width;
                case 'c':
                    return fightPath ? -1 : CaseID + (Map.Width * 2 - 1);
                case 'd':
                    return CaseID + (Map.Width - 1);
                case 'e':
                    return fightPath ? -1 : CaseID - 1;
                case 'f':
                    return CaseID - Map.Width;
                case 'g':
                    return fightPath ? -1 : CaseID - (Map.Width * 2 - 1);
                case 'h':
                    return CaseID - Map.Width + 1;
            }
            return -1;
        }

        public bool IsPathValid(List<int> path, List<int> DinamicObstable)
        {
            if (path == null || path.Count < 2)
            {
                return false;
            }

            for (int i = 0; i < path.Count - 1; i++)
            {
                int currentCellId = path[i];
                int nextCellId = path[i + 1];

                CellPathfinding currentCell = GetCell(currentCellId);
                if (currentCell == null)
                {
                    return false;
                }
                List<CellPathfinding> neighbors = getNeighbours(currentCell, DinamicObstable);

                if (!neighbors.Exists(neighbor => neighbor.cell.Id == nextCellId))
                {
                    return false;
                }
            }
            return true;
        }


        public List<Cell> FindShortestPath(int startCell, int endCell, List<int> dynObstacles)
        {
            this.initialize();
            try
            {
                var finalPath = new List<CellPathfinding>();
                CellPathfinding startNode = this.GetCell(startCell);
                CellPathfinding endNode = this.GetCell(endCell);

                this.addToOpenList(this.GetCell(startCell));
                CellPathfinding currentNode = null;
                while (this.openList.Count > 0)
                {
                    currentNode = this.getCurrentNode();
                    if (currentNode == endNode)
                        break;

                    this.addToCloseList(currentNode);
                    var neighbours = this.getNeighbours(currentNode, dynObstacles);
                    var maxi = neighbours.Count;
                    for (int i = 0; i < maxi; i++)
                    {
                        var node = neighbours[i];
                        if (node == null || this.closedList.Contains(node) ||endNode == null)
                            continue;

                        var newG = node.Parent.g + CELL_DISTANCE_VALUE;
                        var newH = (Math.Abs(endNode.cell.X - node.cell.X) + Math.Abs(endNode.cell.Y - node.cell.Y));
                        var newF = newH + newG;
                        if (this.openList.Contains(node))
                        {
                            if (newG < node.g)
                            {
                                node.Parent = currentNode;
                                node.g = newG;
                                node.h = newH;
                                node.f = newF;
                            }
                        }
                        else
                        {
                            addToOpenList(node);
                            node.Parent = currentNode;
                            node.g = newG;
                            node.h = newH;
                            node.f = newF;
                        }
                    }
                }

                if (this.openList.Count == 0)
                    return finalPath.Select(x => x.cell).ToList();

                var lastNode = this.openList.FirstOrDefault(x => x.cell.Id == endCell);
                while (lastNode != startNode)
                {
                    finalPath.Add(lastNode);
                    lastNode = lastNode.Parent;
                }

                finalPath.Reverse();
                return finalPath.Select(x => x.cell).ToList();
            }
            catch (Exception e)
            {
                return new List<Cell>();
            }
        }


        private void addToCloseList(CellPathfinding cell)
        {
            this.openList.Remove(cell);
            this.closedList.Add(cell);
        }

        private void addToOpenList(CellPathfinding cell)
        {
            this.closedList.Remove(cell);
            this.openList.Add(cell);
        }

        private CellPathfinding getCurrentNode()
        {
            var tmpList = new List<Cell>();
            var maximum = this.openList.Count;
            var minF = 1000000;
            CellPathfinding curNode = null;
            for (int i = 0; i < maximum; i++)
            {
                var node = this.openList[i];
                if (node.f < minF)
                {
                    minF = node.f;
                    curNode = node;
                }
            }
            return curNode;
        }

        private List<CellPathfinding> getNeighbours(CellPathfinding cell, List<int> dyn)
        {
            var neigh = new List<CellPathfinding>();
            var tmpCell = GetJoinCell(cell.cell.Id);
            foreach (var c in tmpCell)
            {
                if (cell.cell.IsWalkable() && (dyn == null || !dyn.Contains(c))) { neigh.Add(this.GetCell(c)); }
            }
            return neigh;
        }
    }

    public class CellPathfinding
    {
        public CellPathfinding Parent { get; set; }

        public Cell cell { get; set; }
        public int f = 0;
        public int g = 0;
        public int h = 0;

        public CellPathfinding(Cell _cell)
        {
            Parent = this;
            cell = _cell;
        }

    }
}
