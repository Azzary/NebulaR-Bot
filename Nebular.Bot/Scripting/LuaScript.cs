using Nebular.Bot.Game.Extention;
using Nebular.Bot.Hook;
using Nebular.Bot.Network;
using Nebular.Bot.Scripting.API.fightAPI;
using Nebular.Bot.Scripting.API.GeneralAPI;
using Nebular.Bot.Scripting.API.TrajetsAPI;
using Nebular.Core.ProcessHandler;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Resources.ResXFileRef;

namespace Nebular.Bot.Scripting
{
    public class LuaScript : NLua.Lua
    {
        public ScriptType ScriptType { get; private set; }
        public ScriptPacketHandlerAPI ScriptPacketHandlerAPI { get; private set; }
        public bool AllGood { get; internal set; }

        public object[] CallFunction(string name)
        {
            var res = new object[0];
            try
            {
                res = base.GetFunction(name).Call();
                if(AllGood)
                    AllGood = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                AllGood = false;
            }
            MainUI.GetInstance().UpdateUI();
            return res;
        }

        public LuaScript(string path, ScriptType scriptType, Dofus client)
        {
            try
            {
                LoadCLRPackage();
                ScriptType = scriptType;
                DoFile(path);
                this["Client"] = new ClientAPI(client.WindowManager);
                ScriptPacketHandlerAPI = new ScriptPacketHandlerAPI(this, client);
                this["PacketHandler"] = ScriptPacketHandlerAPI;
                foreach (Core.VirtualKeys key in Enum.GetValues(typeof(Core.VirtualKeys)))
                {
                    this[key.ToString()] = key.ToString();
                }
                if (scriptType == ScriptType.IA)
                {
                    FightExtension fightExtension = client.Account.Character.Fight;
                    this["Character"] = new CharacterAPI(fightExtension, client);
                    this["Fight"] = new FightAPI(fightExtension, client);
                }
                else
                {
                    this["Character"] = new API.TrajetsAPI.CharaterAPI(client.Account.Character);
                    this["World"] = new WorldAPI(client.Account.Character);
                }
                try
                {
                    var luaFunction = GetFunction("start").Call();
                }
                catch (Exception ex)
                {
                }
                AllGood = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                AllGood = false;
            }
            finally
            {
                MainUI.GetInstance().UpdateUI(scriptType);
            }

        }



        public List<T> GetListObject<T>(string name)
        {
            List<T> list = new List<T>();
            if (this[name] == null)
                return list;
            try
            {
                foreach (var item in ((LuaTable)this[name]).Values)
                {
                    if (typeof(T) == typeof(int))
                    {
                        list.Add((T)Convert.ChangeType(int.Parse(item.ToString()), typeof(T)));
                    }
                    else
                    {
                        list.Add((T)item);
                    }
                }
            }
            catch (Exception)
            {

            }

            return list;
        }


    }

    public enum ScriptType
    {
        IA,
        TRAVEL,
        NONE
    }
}
