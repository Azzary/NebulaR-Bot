using Nebular.Bot.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Entity
{
    public class Npc : Entity
    {
        public int ID { get; set; }
        public int ModelId { get; private set; }
        public Cell Cell { get; set; }
        private bool disposed;

        public Npc(int id, int modelId, Cell cell)
        {
            ID = id;
            ModelId = modelId;
            Cell = cell;
        }

        #region Dispose Region
        ~Npc() => Dispose(false);
        public void Dispose() => Dispose(true);

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                Cell = null;
                disposed = true;
            }
        }
        #endregion
    }

}
