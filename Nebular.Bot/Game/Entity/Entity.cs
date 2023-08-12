using Nebular.Bot.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Entity
{
    public interface Entity : IDisposable
    {
        int ID { get; set; }
        Cell Cell { get; set; }
    }
}
