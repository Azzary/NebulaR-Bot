using Nebular.Bot.Game.World.PathFinder;
using Nebular.Bot.Game.World;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace Nebular.Bot.Game.World.PathFinder
{
    public class WorldPathfinder
    {

        public static Dictionary<string, List<ScriptedCell>> MapScriptedCells = new Dictionary<string, List<ScriptedCell>>();
        public static Dictionary<string, List<int>> PosToMapID = new Dictionary<string, List<int>>();
        public static Dictionary<string, int[]> MapIDPos = new Dictionary<string, int[]>();

        public static void ConvertDict()
        {
            foreach (KeyValuePair<string, List<int>> entry in PosToMapID)
            {
                string[] coords = entry.Key.Split(',');
                int[] intCoords = { Int32.Parse(coords[0]), Int32.Parse(coords[1]) };

                foreach (int id in entry.Value)
                {
                    if (!MapIDPos.ContainsKey(id.ToString()))
                    {
                        MapIDPos[id.ToString()] = intCoords;
                    }
                }
            }
        }
        public static void LoadPosToMapID()
        {
            string jsonData = Properties.Resources.PosToMapIDs;
            JObject jsonObject = JObject.Parse(jsonData);

            PosToMapID = jsonObject.ToObject<Dictionary<string, List<int>>>();
            ConvertDict();
        }
        public static void LoadScriptedCells()
        {
            LoadPosToMapID();
            MapScriptedCells.Clear();
            JObject jsonData = JObject.Parse(Properties.Resources.ScriptedCell);
            Map map = new Map(null);
            foreach (KeyValuePair<string, JToken> mapPair in jsonData)
            {
                string mapID = mapPair.Key;
                map.UpdateMapData(mapID);
                JObject cells = (JObject)mapPair.Value;
                List<ScriptedCell> cellList = new List<ScriptedCell>();
                foreach (KeyValuePair<string, JToken> cellPair in cells)
                {
                    string cellID = cellPair.Key;
                    JObject cellData = (JObject)cellPair.Value;
                    if(map.Cells.Length < short.Parse(cellID))
                        continue;
                    Cell cell = map.GetCellById(short.Parse(cellID));
                    if (((string)cellData["ActionsArgs"]).Contains(","))
                        cellList.Add(new ScriptedCell(mapID, cellID, cellData, cell.X, cell.Y));
                }
                MapScriptedCells.Add(mapID, cellList);
            }
        }
       

        private Dictionary<string, string> cameFrom = new Dictionary<string, string>();
        private Dictionary<string, double> costSoFar = new Dictionary<string, double>();
        private Map Map = new Map(null);
        private Pathfinding Pathfinding;
        private HashSet<ScriptedCell> UsedScriptedCells = new HashSet<ScriptedCell>();
        private string CurrentMap = "";
        private string TargetMap = "";
        private int LastCell = 0;
        List<ScriptedCell> Path = new List<ScriptedCell>();

        public List<ScriptedCell> FindWorldPath(string startMapID, string startCellID, string targetMapID)
        {
            if (MapScriptedCells.Count() == 0)
                LoadScriptedCells();
            InitializePathFinding(startMapID, startCellID, targetMapID);
            var temp = GetNearestScriptedCellToTarget();
            while (temp != null)
            {
                Path.Add(temp);
                if (RecalculateWorldPath() != null)
                    return Path;
                UpdateCurrentMapAndLastCell();
                Path.Clear();
                temp = GetNearestScriptedCellToTarget();
            }
            return Path;
        }

        private void InitializePathFinding(string startMapID, string startCellID, string targetMapID)
        {
            Pathfinding = new Pathfinding(Map, false);
            UsedScriptedCells.Clear();
            Path.Clear();

            CurrentMap = startMapID;
            TargetMap = targetMapID;
            LastCell = int.Parse(startCellID);
        }

        private void UpdateCurrentMapAndLastCell()
        {
            CurrentMap = Path.Last().TpMapID;
            LastCell = int.Parse(Path.Last().TpCellID);
        }

        private List<ScriptedCell> RecalculateWorldPath()
        {
            ScriptedCell checkCell = null;
            UpdateCurrentMapAndLastCell();
            while ((checkCell = GetNearestScriptedCellToTarget()) != null)
            {
                if (checkCell.MapID == TargetMap)
                {
                    return Path;
                }
                Path.Add(checkCell);
                if (RecalculateWorldPath() != null)
                {
                    return Path;
                }
                Path.RemoveAt(Path.Count - 1);
            }
            return null;
        }

        public ScriptedCell GetNearestScriptedCellToTarget()
        {
            List<ScriptedCell> cellsOnStartMap = MapScriptedCells[CurrentMap];
            ScriptedCell nearestCell = null;
            double minDistance = double.MaxValue;
            Map.UpdateMapData(CurrentMap);

            foreach (ScriptedCell cell in cellsOnStartMap)
            {
                if (!CellIsAccessible(cell))
                    continue;
                double distance = CalculateDistanceToTarget(cell);
                if (distance < minDistance)
                {
                    nearestCell = cell;
                    minDistance = distance;
                }
            }
            if (nearestCell != null)
            {
                UsedScriptedCells.Add(nearestCell);
                var TpMap = MapScriptedCells[nearestCell.TpMapID];
                UsedScriptedCells.Add(getCellTp(TpMap, short.Parse(nearestCell.TpCellID)));
            }
            return nearestCell;
        }

        private ScriptedCell getCellTp(List<ScriptedCell> TpMaps, short nearestCellID)
        {
            var currentMap = new Map(null);
            currentMap.UpdateMapData(TpMaps[0].MapID);
            var cell = currentMap.GetCellById(nearestCellID);
            ScriptedCell returnCell = null;
            float distance = int.MaxValue;
            foreach (var SunCell in TpMaps)
            {
                var curentCellCheck = currentMap.GetCellById(short.Parse(SunCell.CellID));
                var newDistance = curentCellCheck.GetDistance(cell);
                if (newDistance < distance) 
                {
                    distance = newDistance;
                    returnCell = SunCell;
                }
            }
            return returnCell;
        }

        private bool CellIsAccessible(ScriptedCell cell)
        {
            return (Map.GetCellById(short.Parse(cell.CellID)).IsWalkable() || Map.GetCellById(short.Parse(cell.CellID)).IsWalkableInteractive()) && !UsedScriptedCells.Contains(cell) && Pathfinding.FindShortestPath(LastCell, int.Parse(cell.CellID), null).Count() > 0;
        }

        private double CalculateDistanceToTarget(ScriptedCell cell)
        {
            return Distance(MapIDPos[cell.TpMapID], MapIDPos[TargetMap]);
        }

        private double Distance(int[] pos1, int[] pos2)
        {
            return Math.Sqrt(Math.Pow(pos1[0] - pos2[0], 2) + Math.Pow(pos1[1] - pos2[1], 2));
        }


    }
}

public class ScriptedCell
{
    public string TpMapID { get; set; }
    public string TpCellID { get; set; }
    public string MapID { get; set; }
    public string CellID { get; set; }
    public bool Condition { get; set; } = false;
    public int X { get; set; }
    public int Y { get; set; }

    public ScriptedCell(string mapID, string cellID, JObject data, int x, int y)
    {
        X = x;
        Y = y;
        MapID = mapID;
        CellID = cellID;
        if(data == null)
        {

            TpMapID = "";
            TpCellID = "";

            Condition = false;
            return;
        }
        string actionsArgs = (string)data["ActionsArgs"];
        string[] splitArgs = actionsArgs.Split(',');

        TpMapID = splitArgs[0];
        TpCellID = splitArgs[1];

        Condition = (bool)data["Conditions"];
    }

}



public class PriorityQueue<T>
{
    private List<KeyValuePair<T, double>> elements = new List<KeyValuePair<T, double>>();

    public T First()
    {
        var bestIndex = 0;

        for (var i = 0; i < elements.Count; i++)
        {
            if (elements[i].Value < elements[bestIndex].Value)
            {
                bestIndex = i;
            }
        }

        return elements[bestIndex].Key;
    }

    public int Count
    {
        get { return elements.Count; }
    }

    public void Enqueue(T item, double priority)
    {
        elements.Add(new KeyValuePair<T, double>(item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Value < elements[bestIndex].Value)
            {
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].Key;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}










//public static Dictionary<string, string> GetWorldPath(string StartMapID, string TargetMapID)
//{
//    Map currentMapata = new Map(null);
//    Pathfinding pathfinding = new Pathfinding(currentMapata, false);


//    // Initialize a dictionary to store the shortest distances to each map
//    Dictionary<string, int> distances = new Dictionary<string, int>();
//    foreach (var map in MapScriptedCells.Keys)
//    {
//        distances[map] = int.MaxValue;
//    }

//    // The distance from the start map to itself is 0
//    distances[StartMapID] = 0;

//    // Initialize a dictionary to store the previous map in the shortest path to each map
//    Dictionary<string, string> previous = new Dictionary<string, string>();
//    foreach (var map in MapScriptedCells.Keys)
//    {
//        previous[map] = null;
//    }

//    // Create a set of all the unvisited maps
//    HashSet<string> unvisited = new HashSet<string>(MapScriptedCells.Keys);

//    // While there are still maps to visit
//    while (unvisited.Count > 0)
//    {
//        // Select the unvisited map with the shortest known distance from the start map
//        string currentMap = null;
//        foreach (var map in unvisited)
//        {
//            if (currentMap == null || distances[map] < distances[currentMap])
//            {
//                currentMap = map;
//            }
//        }

//        // If all remaining maps are inaccessible, stop
//        if (distances[currentMap] == int.MaxValue)
//        {
//            break;
//        }

//        // If the target map has been visited, stop
//        if (currentMap == TargetMapID)
//        {
//            break;
//        }

//        // Remove the current map from the set of unvisited maps
//        unvisited.Remove(currentMap);

//        // Update the distances to each of the current map's neighbors
//        foreach (var cell in MapScriptedCells[currentMap])
//        {
//            string neighborMap = cell.TpMapID;
//            if (unvisited.Contains(neighborMap))
//            {
//                int altDistance = distances[currentMap] + 1;  // The distance to a neighbor is always 1 in this context
//                if (altDistance < distances[neighborMap])
//                {
//                    distances[neighborMap] = altDistance;
//                    previous[neighborMap] = currentMap;
//                }
//            }
//        }
//    }

//    // Build the shortest path from the start map to the target map
//    Dictionary<string, string> path = new Dictionary<string, string>();
//    string mapID = TargetMapID;
//    while (mapID != null)
//    {
//        string prevMapID = previous[mapID];
//        if (prevMapID != null)
//        {
//            // Find the cell in the previous map that teleports to the current map
//            foreach (var cell in MapScriptedCells[prevMapID])
//            {
//                if (cell.TpMapID == mapID)
//                {
//                    path[prevMapID] = cell.CellID;
//                    break;
//                }
//            }
//        }
//        mapID = prevMapID;
//    }

//    return path;
//}