using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nebular.Bot.Game.World.PathFinder
{
    public class FightNode
    {
        public Cell Cell { get; private set; }
        public int AvailablePm { get; private set; }
        public int AvailablePa { get; private set; }
        public int Distance { get; private set; }

        public FightNode(Cell cell, int availablePm, int availablePa, int distance)
        {
            Cell = cell;
            AvailablePm = availablePm;
            AvailablePa = availablePa;
            Distance = distance;
        }
    }

}
