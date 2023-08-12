using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Spells
{
    public class Zones
    {
        public SpellZone Type { get; set; }
        public int Size { get; set; }

        public Zones(SpellZone type, int size)
        {
            Type = type;
            Size = size;
        }

        public static Zones Parse(string str)
        {
            if (str.Length != 2)
                throw new ArgumentException("Invalid zone");

            SpellZone type;

            switch (str[0])
            {
                case 'P':
                    type = SpellZone.Individual;
                    break;

                case 'C':
                    type = SpellZone.Circle;
                    break;

                case 'L':
                    type = SpellZone.Line;
                    break;

                case 'X':
                    type = SpellZone.Cross;
                    break;

                case 'O':
                    type = SpellZone.Ring;
                    break;

                case 'R':
                    type = SpellZone.Rectangle;
                    break;

                case 'T':
                    type = SpellZone.TLine;
                    break;

                default:
                    type = SpellZone.Individual;
                    break;
            }
            return new Zones(type, Core.Cryptography.Hash.GetHash(str[1]));
        }
    }

}
