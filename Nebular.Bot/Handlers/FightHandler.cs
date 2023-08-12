using Nebular.Bot.Game.World;
using Nebular.Bot.Game;
using Nebular.Bot.Network;
using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nebular.Bot.Game.Entity;
using Nebular.Bot.Game.Extention;
using Nebular.Core.Cryptography;

namespace Nebular.Bot.Handlers
{
    public class FightHandler
    {

        [Packet("GP")]
        public Task GetFightCellPosition(Dofus client, string packet) => Task.Run(async () =>
        {
            Character account = client.Account.Character;
            Map map = account.Map;
            FightExtension fight = account.Fight;
            fight.PlacementCell.Clear();
            for (int i = 0; i < 10; i++)
            {
                if (account.IsFighting() && fight.PlayerFighter != null)
                    break;
                    await Task.Delay(100);
            }
            if (!account.IsFighting())
                return;
            await Task.Delay(500);
            account.Fight.FightState = FightState.Positioning;
            string[] cellsData = packet.Substring(2).Split('|');
            int placement = 0;
            for (int i = 0; i < cellsData[1].Length; i += 2)
            {
                if(map.GetCellById((short)((Hash.GetHash(cellsData[0][i]) << 6) + Hash.GetHash(cellsData[0][i + 1]))) == fight.PlayerFighter.Cell)
                {
                    placement++;
                    break;
                }
            }
            for (int i = 0; i < cellsData[placement].Length; i += 2)
            {
                fight.PlacementCell.Add(map.GetCellById((short)((Hash.GetHash(cellsData[0][i]) << 6) + Hash.GetHash(cellsData[0][i + 1]))));
            }

            short targetCell = fight.GetClosestOrFarthestCell(true, fight.PlacementCell);
            client.WindowManager.ActivateWindow();
            try
            {
                fight.LuaScript.CallFunction("Placement");
            }
            catch (Exception)
            {

            }
            client.WindowManager.Press(Core.VirtualKeys.F1);
        });

        [Packet("GJK")]
        public void JoinFight(Dofus client, string packet)
        {
            string[] separator = packet.Substring(3).Split('|');
            byte fightState = byte.Parse(separator[0]);
            client.Account.Character.Fight.HandleCreatedFight(fightState);
        }

        [PacketAttribute("GTM")]
        public async void GetCombatInfoStats(Dofus client, string packet)
        {
            Account account = client.Account;
            if(account.Character == null)
                await Task.Delay(2000);
            if (!account.Character.IsFighting())
                return;

            string[] monsters = packet.Substring(4).Split('|');
            Map map = client.Account.Character.Map;

            foreach (string monster in monsters)
            {
                string[] data = monster.Split(';');
                int id = int.Parse(data[0]);
                Fighter fighter = account.Character.Fight.GetFighterById(id);

                if (data.Length > 0 && fighter != null)
                {
                    bool isAlive = data[1].Equals("0");
                    if (isAlive)
                    {
                        int currentLife = int.Parse(data[2]);
                        byte ap = byte.Parse(data[3]);
                        byte mp = byte.Parse(data[4]);
                        short cellId = short.Parse(data[5]);
                        int maxLife = int.Parse(data[7]);

                        if (cellId > 0) // They are spectators
                        {
                            fighter.CurrentLife = currentLife;
                            fighter.MaxLife = maxLife;
                            fighter.ActionPoints = ap;
                            fighter.MovementPoints = mp;
                            fighter.Cell = map.GetCellById(cellId);
                        }
                    }
                    fighter.IsAlive = isAlive;
                }
            }
        }

        [PacketAttribute("GTR")]
        public void GetCombatTurnReady(Dofus client, string packet)
        {
            Account account = client.Account;
            int id = int.Parse(packet.Substring(3));
        }


        [PacketAttribute("GS")]
        public void GetFightStart(Dofus client, string packet)
        {
            client.Account.Character.Fight.FightState = FightState.Fight;
        }

        [PacketAttribute("GTS")]
        public void GetCombatTurnStart(Dofus client, string packet) => Task.Run(async () =>
        {
            Account account = client.Account;
            account.Character.Fight.FightState = FightState.Fight;
            string[] separator = packet.Substring(3).Split('|');
            int id = int.Parse(separator[0]);
            if (account.IsFirstFight)
            {
                await Task.Delay(100);
                account.Client.WindowManager.Click(15, 57);
                await Task.Delay(100);
                account.Client.WindowManager.Click(660, 377);
                await Task.Delay(100);
                account.IsFirstFight = false;
            }

            if (account.Character.ID == id)
                await account.Character.Fight.GetTurnStarted();
        });

        [PacketAttribute("GE")]
        public void GetCombatFinished(Dofus client, string packet)
        {
            if (packet.StartsWith("GET"))
                return;
            if (client.Account.Character != null)
            {
                client.Account.Character.Fight.CombatFinished();

            }
        }





    }
}
