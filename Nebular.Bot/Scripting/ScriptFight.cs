using Nebular.Bot.Hook;
using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nebular.Bot.Scripting
{
    public class ScriptFight : ScriptObject
    {

        public Dofus Client { get; }
        int MinMonsters { get; set; } = 1;
        int MaxMonsters { get; set; } = int.MaxValue;
        int MinLevel { get; set; } = 0;
        int MaxLevel { get; set; } = int.MaxValue;
        List<int> prohibitedMonsters { get; set; } = new List<int>();
        List<int> requiredMonsters { get; set; } = new List<int>();
        private bool AllReadyFightInMap { get; set; } = false;

        public ScriptFight(Dofus client, LuaScript lua)
        {
            Client = client;
            int tmp;


            if (lua["min_monsters"] != null)
            {
                tmp = int.Parse(lua["min_monsters"].ToString());
                MinMonsters = (0 <= tmp) ? tmp : MinMonsters;
            }

            if(lua["max_monsters"] != null)
            {
                tmp = int.Parse(lua["max_monsters"].ToString());
                MaxMonsters = (0 <= tmp) ? tmp : MaxMonsters;
            }

            if (lua["min_level"] != null)
            {
                tmp = int.Parse(lua["min_level"].ToString());
                MinLevel = (0 <= tmp) ? tmp : MinLevel;
            }

            if (lua["max_level"] != null)
            {
                tmp = int.Parse(lua["max_level"].ToString());
                MaxLevel = (0 <= tmp) ? tmp : MaxLevel;
            }

            prohibitedMonsters = lua.GetListObject<int>("avoid_monsters");
            requiredMonsters = lua.GetListObject<int>("required_monsters");
        }



        internal bool Fight(bool donjon = false)
        {
            if(donjon)
            {
                foreach (var mobs in Client.Account.Character.Map.GetMonsterGroups(1, int.MaxValue, 0, int.MaxValue, new List<int>(), new List<int>()))
                {
                    mobs.Cell.Click();
                    return true;
                }
                return false;
            }

            if(AllReadyFightInMap) return false;
            
            foreach (var mobs in Client.Account.Character.Map.GetMonsterGroups(MinMonsters, MaxMonsters, MinLevel, MaxLevel,  prohibitedMonsters, requiredMonsters))
            {
                mobs.Cell.Click();
                AllReadyFightInMap = true;
                return true;
            }
            return false;
        }

        public void ChangeMap()
        {
            AllReadyFightInMap = false;
        }
    }
}
