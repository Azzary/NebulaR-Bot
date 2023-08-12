using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Entity
{
    public class Characteristics
    {
        public double ExperienceActual { get; set; }
        public double ExperienceMinLevel { get; set; }
        public double ExperienceNextLevel { get; set; }
        public int CurrentEnergy { get; set; }
        public int MaxEnergy { get; set; }
        public int CurrentVitality { get; set; }
        public int MaxVitality { get; set; }
        public StatsBase Initiative { get; set; }
        public StatsBase Prospecting { get; set; }
        public StatsBase ActionPoints { get; set; }
        public StatsBase MovementPoints { get; set; }
        public StatsBase Vitality { get; set; }
        public StatsBase Wisdom { get; set; }
        public StatsBase Strength { get; set; }
        public StatsBase Intelligence { get; set; }
        public StatsBase Luck { get; set; }
        public StatsBase Agility { get; set; }
        public StatsBase Range { get; set; }
        public StatsBase SummonableCreatures { get; set; }
        public int PercentageLife => MaxVitality == 0 ? 0 : (int)((double)CurrentVitality / MaxVitality * 100);

        public Characteristics()
        {
            Initiative = new StatsBase(0, 0, 0, 0);
            Prospecting = new StatsBase(0, 0, 0, 0);
            ActionPoints = new StatsBase(0, 0, 0, 0);
            MovementPoints = new StatsBase(0, 0, 0, 0);
            Vitality = new StatsBase(0, 0, 0, 0);
            Wisdom = new StatsBase(0, 0, 0, 0);
            Strength = new StatsBase(0, 0, 0, 0);
            Intelligence = new StatsBase(0, 0, 0, 0);
            Luck = new StatsBase(0, 0, 0, 0);
            Agility = new StatsBase(0, 0, 0, 0);
            Range = new StatsBase(0, 0, 0, 0);
            SummonableCreatures = new StatsBase(0, 0, 0, 0);
        }

        public void Clear()
        {
            ExperienceActual = 0;
            ExperienceMinLevel = 0;
            ExperienceNextLevel = 0;
            CurrentEnergy = 0;
            MaxEnergy = 0;
            CurrentVitality = 0;
            MaxVitality = 0;

            Initiative.Clear();
            Prospecting.Clear();
            ActionPoints.Clear();
            MovementPoints.Clear();
            Vitality.Clear();
            Wisdom.Clear();
            Strength.Clear();
            Intelligence.Clear();
            Luck.Clear();
            Agility.Clear();
            Range.Clear();
            SummonableCreatures.Clear();
        }
    }

}
