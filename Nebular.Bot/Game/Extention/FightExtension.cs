using Nebular.Bot.Game.Entity;
using Nebular.Bot.Game.World;
using Nebular.Bot.Game.World.PathFinder;
using Nebular.Bot.Hook;
using Nebular.Core.ProcessHandler;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nebular.Core;
using Nebular.Bot.Scripting.API.fightAPI;
using Nebular.Bot.Scripting;
using Nebular.Core.Network;
using System.Windows.Forms;

namespace Nebular.Bot.Game.Extention
{
    public class FightExtension
    {
        public Dofus Client { get; }
        public FightState FightState { get; set; }
        public bool FightStarted { get; private set; }
        public List<Cell> PlacementCell { get; private set; } = new List<Cell>();
        private ConcurrentDictionary<int, Fighter> Fighters { get; set; } = new ConcurrentDictionary<int, Fighter>();
        private ConcurrentDictionary<int, Fighter> Enemies { get; set; } = new ConcurrentDictionary<int, Fighter>();
        private ConcurrentDictionary<int, Fighter> Allies{ get; set; } = new ConcurrentDictionary<int, Fighter>();
        public PlayerFighter PlayerFighter { get; set; }
        public Character Character => Client.Account.Character;
        public LuaScript LuaScript { get;set; }
        public FightExtension(Dofus client)
        {
            Client = client;
        }

        public void HandleCreatedFight(byte fightState)
        {
            Fighters.Clear();
            Enemies.Clear();
            Allies.Clear();
            Turn = 0;
            FightState = (FightState)fightState;
            Client.Account.Character.CharacterState = CharacterState.FIGHTING;
            FightStarted = true;
            Client.log("FIGHT", " New fight started");
            if (!string.IsNullOrEmpty(Client.PathIaFile))
                LuaScript = new LuaScript(Client.PathIaFile, ScriptType.IA, Client);
            else
                Console.WriteLine("[Fight] No IA");
        }

        public IEnumerable<Fighter> GetAllies => Allies.Values.Where(a => a.IsAlive);
        public IEnumerable<Fighter> GetEnemies => Enemies.Values.Where(e => e.IsAlive);
        public IEnumerable<Fighter> GetAllFighters => Fighters.Values.Where(f => f.IsAlive);
        public int GetDistanceFromEnemy(Cell currentCell) => GetEnemies.Sum(enemy => currentCell.GetDistance(enemy.Cell) - 1);


        public void AddFighter(Fighter fighter)
        {
            if (fighter.Id == Character.ID)
                PlayerFighter = new PlayerFighter(Character.Name, Character.Level, fighter);
            else
                Fighters.TryAdd(fighter.Id, fighter);

            SortFighters();
        }


        private void SortFighters()
        {
            if (PlayerFighter == null)
                return;

            foreach (Fighter fighter in GetAllFighters)
            {
                if (Allies.ContainsKey(fighter.Id) || Enemies.ContainsKey(fighter.Id))
                    continue;

                if (fighter.Team == PlayerFighter.Team)
                    Allies.TryAdd(fighter.Id, fighter);
                else
                    Enemies.TryAdd(fighter.Id, fighter);
            }
        }


        public short GetClosestOrFarthestCell(bool isClosest, IEnumerable<Cell> possibleCells)
        {
            short cellId = -1;
            int totalDistance = -1;

            foreach (Cell currentCell in possibleCells)
            {
                int temporalTotalDistance = GetDistanceFromEnemy(currentCell);

                if (cellId == -1 || (isClosest && temporalTotalDistance < totalDistance) || (!isClosest && temporalTotalDistance > totalDistance))
                {
                    cellId = currentCell.Id;
                    totalDistance = temporalTotalDistance;
                }
            }

            return cellId;
        }

        public Fighter GetFighterById(int fighterId)
        {
            if (Fighters.TryGetValue(fighterId, out Fighter fighter))
                return fighter;

            if (PlayerFighter != null && PlayerFighter.Id == fighterId)
                return PlayerFighter;

            return null;
        }

        public int Turn { get; private set; } = 0;

        internal async Task GetTurnStarted()
        {

            if(Turn == 0)
            {
                await Task.Delay(100);
                Client.WindowManager.Click(688, 381);
                Console.WriteLine(Client.PathIaFile);
                if (!string.IsNullOrEmpty(Client.PathIaFile))
                {
                    LuaScript = new LuaScript(Client.PathIaFile, ScriptType.IA, Client);
                }
                await Task.Delay(100);
            }
            Turn++;
            await Task.Delay(100);
            if (LuaScript == null)
            {
                Console.WriteLine("[FIGHT] No IA");
                return;
            }
            try
            {
                lock (LuaScript)
                {
                    LuaScript.CallFunction("PlayTurn");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        internal async void CombatFinished()
        {
            FightState = FightState.Finished;
            await Hook.Client.WaitForPacket(2500, "GDK");
            await Task.Delay(1500);
            Client.WindowManager.Click(550, 370);
            await Task.Delay(100);
            Client.Account.Character.CharacterState = CharacterState.CONNECTED_IDLE;
        }
    }
}


public class PlayerFighter : Fighter
{
    public string Name { get; private set; }
    public byte Level { get; private set; }

    public PlayerFighter(string name, byte level, Fighter fighter) : base(fighter.Id, fighter.IsAlive, fighter.CurrentLife, fighter.ActionPoints, fighter.MovementPoints, fighter.Cell, fighter.MaxLife, fighter.Team, fighter.SummonerId, fighter.IsSummon)
    {
        Name = name;
        Level = level;
    }
}
