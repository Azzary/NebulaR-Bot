using Nebular.Bot.Scripting.API.TrajetsAPI;
using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Scripting.API.GeneralAPI
{
    public class ScriptPacketHandlerAPI
    {
        private List<ScriptHandler> handlers { get; set; } = new List<ScriptHandler>();
        private LuaScript Lua { get; set; }
        private Dofus Client { get; set; }
        public ScriptPacketHandlerAPI(LuaScript lua, Dofus client)
        {
            Lua = lua;
            Client = client;
        }

        public void Add(string Packet, string CallBack)
        {
            if (handlers.FirstOrDefault(handler => handler.Packet == Packet) == null)
                handlers.Add(new ScriptHandler(Packet, CallBack));
        }

        public void Remove(string Packet)
        {
            int index = handlers.RemoveAll(handler => handler.Packet == Packet);
            if (index != -1)
            {
                handlers.RemoveAt(index);
            }
        }

        public bool callCallback(string Packet)
        {
            var handler = handlers.FirstOrDefault(h => Packet.StartsWith(h.Packet));

            if (handler != null)
            {
                try
                {
                    Lua.GetFunction(handler.CallBack).Call(Packet);
                    Client.log("[SCRIPT]", "Call custom handler for: " + handler.Packet);
                }
                catch (NLua.Exceptions.LuaScriptException e)
                {
                    Console.WriteLine($"Erreur lors de l'appel de la fonction Callback dans le script Lua : {e.Message}");
                }
            }
            return handler != null;
        }

    }

    class ScriptHandler
    {
        public string Packet { get; private set; }
        public string CallBack { get; private set; }

        public ScriptHandler(string Packet, string CallBack)
        {
            this.Packet = Packet;
            this.CallBack = CallBack;
        }
    }
}
