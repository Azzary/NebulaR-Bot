using Nebular.Bot.Game.Entity;
using Nebular.Bot.Game.World;
using Nebular.Bot.Scripting.API.GeneralAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Scripting.API.fightAPI
{
    public class FighterAPI
    {

        private Fighter Fighter { get; set; }
        public FighterAPI(Fighter fighter) 
        {
            Fighter = fighter;
        }

        public int Id() => Fighter.Id; 
        public CellAPI Cell() => Fighter.Cell.CellAPI;
        public byte Team() => Fighter.Team;
        public bool IsAlive() => Fighter.IsAlive;
        public int CurrentLife() => Fighter.CurrentLife;
        public int MaxLife() => Fighter.MaxLife;
        public byte ActionPoints() => Fighter.ActionPoints;
        public byte MovementPoints() => Fighter.MovementPoints;
        public int SummonerId() => Fighter.SummonerId;
        public bool IsSummon() => Fighter.IsSummon;
        public int LifePercentage() => Fighter.LifePercentage;


    }
}
