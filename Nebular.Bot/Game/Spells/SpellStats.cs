using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Spells
{
    public class SpellStats
    {
        public byte CostPA { get; set; }
        public byte MinRange { get; set; }
        public byte MaxRange { get; set; }

        public bool IsLineCast { get; set; }
        public bool RequiresLineOfSight { get; set; }
        public bool RequiresFreeCell { get; set; }
        public bool IsModifiableRange { get; set; }

        public byte CastsPerTurn { get; set; }
        public byte CastsPerTarget { get; set; }
        public byte Interval { get; set; }

        public List<SpellEffect> NormalEffects { get; private set; }
        public List<SpellEffect> CriticalEffects { get; private set; }

        public SpellStats()
        {
            NormalEffects = new List<SpellEffect>();
            CriticalEffects = new List<SpellEffect>();
        }

        public void AddEffect(SpellEffect effect, bool isCritical)
        {
            if (!isCritical)
                NormalEffects.Add(effect);
            else
                CriticalEffects.Add(effect);
        }
    }

}
