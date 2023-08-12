using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Spells
{
    public class SpellEffect
    {
        public int Id { get; set; }
        public Zones EffectZone { get; set; }

        public SpellEffect(int id, Zones zone)
        {
            Id = id;
            EffectZone = zone;
        }
    }

}
