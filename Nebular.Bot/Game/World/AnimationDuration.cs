using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.World
{
    internal class AnimationDuration
    {
        public int Linear { get; private set; }
        public int Horizontal { get; private set; }
        public int Vertical { get; private set; }

        public AnimationDuration(int linear, int horizontal, int vertical)
        {
            Linear = linear;
            Horizontal = horizontal;
            Vertical = vertical;
        }
    }

}
