using Nebular.Bot.Game.World;
using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nebular.Bot.Scripting
{
    public class Script
    {
        public Dofus Client { get; private set; }

        private CancellationTokenSource cancellationTokenSource;
        private readonly object lockObject = new object(); // Lock object for thread safety
        public ScriptManager ScriptManager { get; set; }
        private ScriptMovement ScriptMovement { get; set; }
        private ScriptFight ScriptFight { get; set; }
        private Banque Banque { get; set; }
        public LuaScript LuaScript => ScriptManager != null ? ScriptManager.LuaScript : null;
        public int Recolte = 0;
        public int Fight = 0;
        public Script(Dofus client)
        {
            Client = client;
        }

        private CancellationToken cancellationToken;
        public bool IsRunning => cancellationTokenSource != null;

        private async Task WaitCharacterIDLE()
        {
            while (!cancellationToken.IsCancellationRequested 
                && (Client.Account.Character.CharacterState != Game.Entity.CharacterState.CONNECTED_IDLE || Client.Account.Character.Cell.IsTeleport()))
            {
                await Task.Delay(500, cancellationToken);
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
                if (Client.Account.Character.CharacterState != Game.Entity.CharacterState.CONNECTED_IDLE)
                {
                    await WaitCharacterIDLE();
                }
            }
        }

        public void Start(string luaFile)
        {
            lock (lockObject)
            {
                ScriptManager = new ScriptManager(Client, luaFile);
                ScriptMovement = new ScriptMovement(Client, ScriptManager.LuaScript);
                ScriptFight = new ScriptFight(Client, ScriptManager.LuaScript);
                Banque = new Banque(Client.Account.Character);
                if (!IsRunning)
                {
                    Client.log("[Script]", "Start");
                    Client.Account.Character.LastMapID = 0;
                    cancellationTokenSource = new CancellationTokenSource();
                    cancellationToken = cancellationTokenSource.Token;
                    Task.Run(Loop, cancellationToken);
                }
            }
        }

        public void Stop()
        {
            lock (lockObject)
            {
                if (IsRunning)
                {
                    Client.log("[Script]", "Stop");
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource = null;
                }
            }
        }

        private async Task checkLife()
        {
            int pourcentageLife = ScriptManager.MinLife;
            double currentVitality = Client.Account.Character.Characteristics.CurrentVitality;
            double totalVitality = Client.Account.Character.Characteristics.MaxVitality;

            double remainingPercentage = (currentVitality / totalVitality) * 100;

            if (remainingPercentage < pourcentageLife)
            {
                Client.WindowManager.Click(58, 435);
                await Hook.Client.WaitForPacket(1000, "eUK"+Client.Account.Character.ID);
                Client.log("[SCRIPT]", "Wait for life...");
                await Task.Delay((int)(1000*(totalVitality - currentVitality)));
                Client.Account.Character.Characteristics.CurrentVitality = Client.Account.Character.Characteristics.MaxVitality;
            }
        }

        private List<string> Packets = new List<string>();
        public void AddPacket(string packet) {
            if (!IsRunning)
                return;
            lock(this)
                Packets.Add(packet);
        }

        public bool CheckPackets()
        {
            if(!IsRunning) return false;
            bool flag = false;
            if (ScriptManager.LuaScript.ScriptPacketHandlerAPI == null)
                return false; 
            lock (this)
            {
                foreach (var packet in Packets)
                {
                    if (ScriptManager.LuaScript.ScriptPacketHandlerAPI.callCallback(packet))
                    {
                        flag = true;
                    }
                }
                Packets.Clear();
            }
            return flag;
        }

        private async Task Loop()
        {
            ScriptFight.ChangeMap();
            ScriptMovement.ChangeMap();
            Packets.Clear();
            bool actionDo = false;
            bool handlerPacket = false;
            short Try = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                if (handlerPacket)
                    continue;
                await checkLife();
                var mapid = Client.Account.Character.Map.Id;
                if (Client.Account.Character.Inventory.PodsPercentage >= 100)
                {
                    Client.log("[Script]", "Pods >= 100");
                    Stop();
                    return;
                }
                string fonctionName = Client.Account.Character.Inventory.PodsPercentage >= ScriptManager.MaxPods ? "banque" : "move";
                if (Client.Account.Character.Inventory.PodsPercentage >= ScriptManager.MaxPods)
                {
                    Client.log("[Script]", "Go banque");
                }
                if (!ScriptManager.ExecuteMoveFunction(mapid.ToString(), Client.Account.Character.Map.Y.ToString(), Client.Account.Character.Map.X.ToString(), fonctionName))
                {
                    if (ScriptManager.CallLost(mapid.ToString(), Client.Account.Character.Map.Y.ToString(), Client.Account.Character.Map.X.ToString()))
                    {
                        Console.WriteLine("[Script] Lost...");
                    }
                    else
                    {
                        Client.log("[Script]", "Aucune action trouver pour " + mapid.ToString());
                        Stop();
                        return;
                    }
                }
                if (!string.IsNullOrEmpty(ScriptManager.Custom))
                {
                    if(ScriptManager.Custom == "bank")
                    {
                        Banque.GiveItemToBanque();
                        await WaitCharacterIDLE();
                        if (string.IsNullOrEmpty(ScriptManager.ChangeMap))
                            continue;
                    }
                    else
                        ScriptManager.LuaScript.CallFunction(ScriptManager.Custom);
                    await WaitCharacterIDLE();
                }
                if (ScriptManager.Donjon)
                {
                    while (!ScriptFight.Fight(true) && !cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("[SCRIPT] No figt foud wait.");
                        await Task.Delay(500);
                    }
                    await WaitCharacterIDLE();
                    Try = 0;
                    continue;
                }
                if (ScriptManager.Harvest)
                {
                    actionDo = (ScriptMovement.Harvest());
                    Recolte++;
                    Client.log("[SCRIPT]", "Recolte: " + Recolte);
                    await Task.Delay(500);
                }
                if(!actionDo && ScriptManager.Fight)
                {
                    while (!Client.Account.Character.IsFighting() && !ScriptFight.Fight())
                    {
                        await WaitCharacterIDLE();

                    }
                    if (Client.Account.Character.IsFighting())
                    {
                        actionDo = true;
                        Fight++;
                    }
                }

                await WaitCharacterIDLE();
                if (actionDo == false && !string.IsNullOrEmpty(ScriptManager.ChangeMap))
                {
                    short lastMap = Client.Account.Character.Map.Id;
                    await ScriptMovement.Move(ScriptManager.ChangeMap);
                    await WaitCharacterIDLE();
                    if (lastMap == Client.Account.Character.Map.Id)
                    {
                        Try++;
                        Client.log("[SCRIPT]", "Imposible de changer de map try: " + (Try + 1) + "/4");
                        if (Try == 3)
                        {
                            Stop();
                            return;
                        }
                    }
                    else
                    {
                        ScriptFight.ChangeMap();
                        ScriptMovement.ChangeMap();
                        Try = 0;
                    }

                }
                else if (string.IsNullOrEmpty(ScriptManager.ChangeMap))
                {
                    Stop();
                }
                actionDo = false;
            }
        }
    }


}
