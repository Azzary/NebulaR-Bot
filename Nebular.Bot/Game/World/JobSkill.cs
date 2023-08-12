using Nebular.Bot.Game.World.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.World
{
    public class JobSkill
    {
        public short Id { get; private set; }
        public byte MinQuantity { get; private set; }
        public byte MaxQuantity { get; private set; }
        public InteractiveObjectModel InteractiveModel { get; private set; }
        public bool IsCraft { get; private set; } = true;
        public float Time { get; private set; }

        public JobSkill(short _id, byte _minQuantity, byte _maxQuantity, float _time)
        {
            Id = _id;
            MinQuantity = _minQuantity;
            MaxQuantity = _maxQuantity;

            InteractiveObjectModel _model = InteractiveObjectModel.GetModelByAbility(Id);
            if (_model != null)
            {
                InteractiveModel = _model;

                if (InteractiveModel.IsCollectible)
                    IsCraft = false;
            }
            Time = _time;
        }

        public void Update(short _id, byte _minQuantity, byte _maxQuantity, float _time)
        {
            Id = _id;
            MinQuantity = _minQuantity;
            MaxQuantity = _maxQuantity;
            Time = _time;
        }
    }

}
