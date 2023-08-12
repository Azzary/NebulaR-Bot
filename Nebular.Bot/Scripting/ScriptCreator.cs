using Nebular.Bot.Game.Entity;
using Nebular.Core.ProcessHandler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Nebular.Bot.Scripting
{
    public class ScriptCreator
    {
        private Dofus client;
        public Character Character => client.Account.Character;
        public string Name { get; set; } = "";
        public bool Fight { get; internal set; } = false;
        public bool Harvest { get; internal set; } = false;
        public bool Create { get; internal set; } = false;

        private List<ScriptPath> LuaScript = new List<ScriptPath>();

        public ScriptCreator(Dofus ofus)
        {
            client = ofus;
        }

        public void AddMove(int mapId, string cellId)
        {
            var key = mapId.ToString();
            var lastMap = LuaScript.FirstOrDefault(x => x.MapId == key) != null ? Character.LastMapID.ToString() : "";
            LuaScript.Add(new ScriptPath(key, cellId, Fight, Harvest, lastMap));
        }

        public void Start()
        {
            LuaScript.Clear();
            Create = true;
        }

        public void Save()
        {
            Create = false;
            var sb = new StringBuilder();
            sb.AppendLine("function move()");

            // Ajout des chemins avec 'LastMap' en premier

            foreach (var scriptPath in LuaScript.Where(sp => !string.IsNullOrEmpty(sp.LastMap)))
            {
                sb.AppendLine("    " + scriptPath.ToLua());
            }

            // Ajout des chemins sans 'LastMap'
            sb.AppendLine("    return {");
            foreach (var scriptPath in LuaScript.Where(sp => string.IsNullOrEmpty(sp.LastMap)))
            {
                sb.AppendLine("        " + scriptPath.ToLua());
            }
            sb.AppendLine("    }");
            sb.AppendLine("end");

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", $"{Name}.lua");
            File.WriteAllText(path, sb.ToString());
        }
    }

    class ScriptPath
    {
        public string MapId;
        public string CellId;
        public string LastMap;
        public bool Fight;
        public bool Harvest;

        public ScriptPath(string mapId, string cellId, bool fight, bool harvest, string lastMap = "")
        {
            MapId = mapId;
            CellId = cellId;
            LastMap = lastMap;
            Fight = fight;
            Harvest = harvest;
        }

        public string ToLua()
        {

            if (!string.IsNullOrEmpty(LastMap))
            {
                return $"if(Character:LastMapID() == {LastMap}) then\nreturn {{ {{ map = \"{MapId}\" , fight = {Fight.ToString().ToLower()} ,  harvest = {Harvest.ToString().ToLower()}, changeMap = \"{CellId}\" }},}}\nend";
            }

            var result = $"{{ map = \"{MapId}\", changeMap = \"{CellId}\"";

            if (Fight)
            {
                result += ", fight = true";
            }

            if (Harvest)
            {
                result += ", harvest = true";
            }

            result += " },";

            return result;
        }
    }
}
