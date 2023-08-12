using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.World.Interactive
{
    public class InteractiveObject
    {
        public short Gfx { get; private set; }
        public Cell Cell { get; private set; }
        public InteractiveObjectModel Model { get; private set; }
        public bool IsUsable { get; set; } = true;

        public InteractiveObject(short gfx, Cell cell)
        {
            Gfx = gfx;
            Cell = cell;

            InteractiveObjectModel model = InteractiveObjectModel.GetModelByGfx(Gfx);

            if (model != null)
            {
                Model = model;
                IsUsable = true;
            }
        }
    }


}
