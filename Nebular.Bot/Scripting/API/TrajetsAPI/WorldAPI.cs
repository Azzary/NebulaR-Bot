using Nebular.Bot.Game.Entity;
using Nebular.Bot.Game.World.PathFinder;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Scripting.API.TrajetsAPI
{
    public class WorldAPI
    {


        private string StartMap { get; set; } = "";
        private string TargetMap { get; set; } = "";
        private Character Character { get; set; }
        private List<ScriptedCell> Path { get; set; } = new List<ScriptedCell>();


        public WorldAPI(Character character)
        {
            Character = character;
        }

        public LuaTableData GoTo(int X, int Y)
        {
            if (WorldPathfinder.PosToMapID.ContainsKey($"{X},{Y}"))
            {
                return GoTo(WorldPathfinder.PosToMapID[$"{X},{Y}"][0]);
            }
            else
                throw new Exception($"Impossible de trouver la mapID corespondante {X},{Y}");

        }
        int pathIndex = 0;
        public LuaTableData GoTo(int targetMapID)
        {
            return GoTo(targetMapID.ToString());
        }
        public LuaTableData GoTo(string targetMapID)
        {
            if (Path.Count() < pathIndex && pathIndex > 0 && Path[pathIndex].MapID != Character.Map.Id.ToString() && Path[pathIndex-1].MapID == Character.Map.Id.ToString())
            {
                pathIndex--;
            }

            if (TargetMap != targetMapID || Path.Count() <= pathIndex || Path[pathIndex].MapID != Character.Map.Id.ToString())
            {
                pathIndex = 0;
                Console.WriteLine("[WorldPathfinderAPI] Find Path...");
                TargetMap = targetMapID;
                Path = new WorldPathfinder().FindWorldPath(Character.Map.Id.ToString(), Character.Cell.Id.ToString(), targetMapID);
            }
            Console.WriteLine("[WorldPathfinderAPI] Go next map");
            if(Character.Map.Id.ToString() == targetMapID)
            {
                throw new Exception("Le persoonage est deja sur cette carte");
            }
            if(Path.Count() <= pathIndex)
            {
                throw new Exception("Impossible de trouver un chemain...");
            }
            string cell = Path[pathIndex].CellID.ToString();
            pathIndex++;
            return new LuaTableData { map = Character.Map.Id.ToString(), changeMap = cell };
        }

    }
    public class LuaTableData
    {
        public string map { get; set; }
        public string changeMap { get; set; }
    }

}
