using Nebular.Bot.Game.World;
using Nebular.Bot.Scripting.API.fightAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Entity
{
    public class Fighter
    {
        public int Id { get; set; }
        public Cell Cell { get; set; }
        public byte Team { get; set; }
        public bool IsAlive { get; set; }
        public int CurrentLife { get; set; }
        public int MaxLife { get; set; }
        public byte ActionPoints { get; set; }
        public byte MovementPoints { get; set; }
        public int SummonerId { get; set; }
        public bool IsSummon { get; set; }
        public int LifePercentage => (int)((double)CurrentLife / MaxLife * 100);

        public FighterAPI FighterAPI { get; set; } 

        public Fighter(int id, bool isAlive, int currentLife, byte actionPoints, byte movementPoints, Cell cell, int maxLife, byte team, int summonerId, bool isSummon)
        {
            Id = id;
            IsAlive = isAlive;
            CurrentLife = currentLife;
            ActionPoints = actionPoints;
            MovementPoints = movementPoints;
            Cell = cell;
            MaxLife = maxLife;
            Team = team;
            SummonerId = summonerId;
            IsSummon = isSummon;
            FighterAPI = new FighterAPI(this);
        }

        public void UpdateFightPoints(int actionId, string value)
        {
            switch (actionId)
            {
                /** Life loss **/
                case 100:
                    byte lifeLost = byte.Parse(value.Substring(1));
                    CurrentLife -= lifeLost;
                    break;

                /** Life gain **/
                case 108:
                case 110:
                    byte lifeGained = byte.Parse(value);
                    CurrentLife += lifeGained;
                    if (CurrentLife > MaxLife)
                        CurrentLife = MaxLife;
                    break;

                /** Action Points loss **/
                case 101:
                case 102:
                case 168:
                    byte apLost = byte.Parse(value.Substring(1));
                    ActionPoints -= apLost;
                    break;

                case 120://Action Points gain
                    byte apGained = byte.Parse(value);
                    ActionPoints += apGained;
                    break;

                /** Movement Points loss **/
                case 127:
                case 77:
                case 129:
                case 169:
                    byte mpLost = byte.Parse(value.Substring(1));
                    MovementPoints -= mpLost;
                    break;

                /** Movement Points gain **/
                case 78:
                case 128:
                    byte mpGained = byte.Parse(value);
                    MovementPoints += mpGained;
                    break;
            }
        }
    }

}
