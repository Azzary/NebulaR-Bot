using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Spells
{
    public class Spell
    {
        public short Id { get; private set; }
        public string Name { get; private set; }
        public byte Level { get; set; }
        public Dictionary<byte, SpellStats> SpellStatsDictionary; // Level, information

        public static Dictionary<short, Spell> LoadedSpells = new Dictionary<short, Spell>();

        public Spell(short id, string name)
        {
            Id = id;
            Name = name;
            SpellStatsDictionary = new Dictionary<byte, SpellStats>();

            LoadedSpells.Add(Id, this);
        }

        public void AddSpellStats(byte level, SpellStats stats)
        {
            if (SpellStatsDictionary.ContainsKey(level))
                SpellStatsDictionary.Remove(level);

            SpellStatsDictionary.Add(level, stats);
        }

        public SpellStats GetStats() => SpellStatsDictionary[Level];

        public static Spell GetSpell(short id) => LoadedSpells[id];
    }

}
