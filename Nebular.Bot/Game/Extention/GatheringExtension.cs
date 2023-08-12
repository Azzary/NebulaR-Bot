using Nebular.Bot.Game.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Extension
{
    public class GatheringExtension
    {

        public Character Character { get; private set; }
        public GatheringExtension(Character character) 
        {
            Character = character;
        }

        internal async Task HarvestStarted(int entityId, int harvestTime, short id, byte gkkHarvestType)
        {
            if(entityId == Character.ID)
            {
                Character.CharacterState = CharacterState.GATHERING;
                await Task.Delay(harvestTime);
                if(Character.CharacterState == CharacterState.GATHERING)
                    Character.CharacterState = CharacterState.CONNECTED_IDLE;
            }
        }
    }
}
