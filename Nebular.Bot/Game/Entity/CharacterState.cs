using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Entity
{
    public enum CharacterState
    {
        CONNECTING,
        CHANGING_TO_GAME,
        CONNECTED_IDLE,
        DISCONNECTED,
        MOVING,
        FIGHTING,
        GATHERING,
        DIALOGUE,
        STORAGE,
        TRADING,
        BUYING,
        SELLING,
        REGENERATING,
        MAP_CHANGE,
        LOADING
    }

}
