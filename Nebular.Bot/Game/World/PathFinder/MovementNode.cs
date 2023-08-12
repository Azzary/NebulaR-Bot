using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.World.PathFinder
{
    public class MovementNode
    {
        public short InitialCell { get; private set; }
        public bool Reachable { get; private set; }
        public FightPath Path { get; set; }

        public MovementNode(short initialCell, bool reachable)
        {
            InitialCell = initialCell;
            Reachable = reachable;
        }
    }

}
