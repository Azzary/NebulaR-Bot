using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Entity
{
    public class StatsBase
    {
        public int BaseCharacter { get; set; }
        public int Equipment { get; set; }
        public int Gifts { get; set; }
        public int Boost { get; set; }

        public StatsBase(int baseCharacter) => BaseCharacter = baseCharacter;
        public StatsBase(int baseCharacter, int equipment, int gifts, int boost) => UpdateStats(baseCharacter, equipment, gifts, boost);
        public int TotalStats => BaseCharacter + Equipment + Gifts + Boost;

        public void UpdateStats(int baseCharacter, int equipment, int gifts, int boost)
        {
            BaseCharacter = baseCharacter;
            Equipment = equipment;
            Gifts = gifts;
            Boost = boost;
        }

        public void Clear()
        {
            BaseCharacter = 0;
            Equipment = 0;
            Gifts = 0;
            Boost = 0;
        }
    }

}
