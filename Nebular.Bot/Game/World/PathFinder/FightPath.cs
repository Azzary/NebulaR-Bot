using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.World.PathFinder
{
    public class FightPath
    {
        public List<short> AccessibleCells { get; set; }
        public List<short> UnreachableCells { get; set; }
        public Dictionary<short, int> AccessibleMap { get; set; }
        public Dictionary<short, int> UnreachableMap { get; set; }
    }

}
