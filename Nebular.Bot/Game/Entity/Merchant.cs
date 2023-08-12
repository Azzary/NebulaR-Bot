using Nebular.Bot.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Entity
{
    public class Merchant : Entity
    {
        public int ID { get; set; }
        public Cell Cell { get; set; }
        private bool disposed;

        public Merchant(int id, Cell cell)
        {
            ID = id;
            Cell = cell;
        }

        #region Dispose Region
        ~Merchant() => Dispose(false);
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
