using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.World
{
    public enum CellType
    {
        NotWalkable = 0,
        InteractiveObject = 1,
        TeleportCell = 2,
        Unknown1 = 3,
        WalkableCell = 4,
        Unknown2 = 5,
        Path1 = 6,
        Path2 = 7,
        Any = 8
    }

}
