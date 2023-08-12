using System;
using System.Collections.Generic;
using Nebular.Bot.Scripting.API.TrajetsAPI;
using Nebular.Core.ProcessHandler;
using NLua;

namespace Nebular.Bot.Scripting
{
    public class ScriptManager
    {
        public LuaScript LuaScript {get; private set;}

        public bool Fight { get; private set; }
        public bool Harvest { get; private set; }
        public bool Donjon { get; private set; }
        public string ChangeMap { get; private set; }
        public string Custom { get; private set; }
        public Dofus Client { get; private set; }
        public int MinLife { get; internal set; } = 0;
        public List<int> ItemsToKeep { get; set; } = new List<int>();
        public int MaxPods { get; internal set; } = 111;

        public ScriptManager(Dofus Client, string path)
        {
            this.Client = Client;
            LuaScript = new LuaScript(path, ScriptType.TRAVEL, Client);
            if (LuaScript["min_life"] != null)
            {
                var tmp = int.Parse(LuaScript["min_life"].ToString());
                MinLife = (tmp > 0 && tmp <= 100) ? tmp : MinLife;
            }
            if (LuaScript["max_pods"] != null)
            {
                var tmp = int.Parse(LuaScript["max_pods"].ToString());
                MaxPods = (tmp > 0 && tmp <= 100) ? tmp : MaxPods;
            }
            
            ItemsToKeep = LuaScript.GetListObject<int>("items_to_keep");
        }

        public bool ExecuteMoveFunction(string mapId, string y, string x, string fonctionName = "move")
        {
            try
            {
                Fight = false;
                Harvest = false;
                Donjon = false;
                Custom = "";
                var result = LuaScript.CallFunction(fonctionName);

                if (result.Length > 0 && result[0] is LuaTableData)
                {
                    var luaTableData = (LuaTableData)result[0];
                    ChangeMap = luaTableData.changeMap;
                    return true;
                }
                string XandY = $"{x};{y}";
                foreach (LuaTable outerTable in result)
                {
                    foreach (LuaTable table in outerTable.Values)
                    {
                        string map = table["map"]?.ToString();
                        string ScriptX = table["x"]?.ToString();
                        string ScriptY = table["y"]?.ToString();
                        if (map != mapId && (x != ScriptX || ScriptY != y))
                            continue;
                        bool? fightNullable = (bool?)table["fight"];
                        bool? harvestNullable = (bool?)table["harvest"];
                        bool? donjonNullable = (bool?)table["donjon"];
                        Fight = fightNullable.GetValueOrDefault(false);
                        Harvest = harvestNullable.GetValueOrDefault(false);
                        Donjon = donjonNullable.GetValueOrDefault(false);
                        ChangeMap = table["changeMap"]?.ToString();
                        Custom = table["custom"]?.ToString();
                        Console.WriteLine($"Map: {map}, Harvest: {Harvest}, Fight: {Fight}, ChangeMap: {ChangeMap}");
                        return true;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        internal bool CallLost(string mapId, string y, string x)
        {
            try
            {

                if (true)
                {
                    var result = LuaScript.CallFunction("lost");

                    if (result.Length > 0 && result[0] is LuaTableData)
                    {
                        var luaTableData = (LuaTableData)result[0];
                        Fight = false;
                        Harvest = false;
                        ChangeMap = luaTableData.changeMap;
                        return true;
                    }
                    foreach (LuaTable outerTable in result)
                    {
                        foreach (LuaTable table in outerTable.Values)
                        {
                            string map = table["map"]?.ToString();
                            string ScriptX = table["x"]?.ToString();
                            string ScriptY = table["y"]?.ToString();
                            if (map != mapId && (x != ScriptX || ScriptY != y))
                                continue;
                            Fight = false;
                            Harvest = false;
                            Donjon = false;
                            ChangeMap = table["changeMap"]?.ToString();
                            Console.WriteLine($"Map: {map}, Harvest: {Harvest}, Fight: {Fight}, ChangeMap: {ChangeMap}");
                            return true;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing Lua script: {ex.Message}");
            }
            return false;
        }
    }
}
