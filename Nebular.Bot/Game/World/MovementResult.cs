using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.World
{
    public enum MovementResult
    {
        SUCCESS,
        SAME_CELL,
        FAILURE,
        PATHFINDING_ERROR
    }
}
