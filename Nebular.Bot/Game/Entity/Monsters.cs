using Nebular.Bot.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Entity
{
    public class Monsters : Entity
    {
        public int ID { get; set; } = 0;
        public int TemplateId { get; set; } = 0;
        public Cell Cell { get; set; }
        public int Level { get; set; }

        public List<Monsters> MonstersInsideGroup { get; set; }
        public Monsters GroupLeader { get; set; }
        bool disposed;

        public int TotalMonsters => MonstersInsideGroup.Count + 1;
        public int TotalGroupLevel => GroupLeader.Level + MonstersInsideGroup.Sum(f => f.Level);

        public Monsters(int id, int template, Cell cell, int level)
        {
            ID = id;
            TemplateId = template;
            Cell = cell;
            MonstersInsideGroup = new List<Monsters>();
            Level = level;
        }

        public bool ContainsMonster(int id)
        {
            if (GroupLeader.TemplateId == id)
                return true;

            for (int i = 0; i < MonstersInsideGroup.Count; i++)
            {
                if (MonstersInsideGroup[i].TemplateId == id)
                    return true;
            }
            return false;
        }

        public void Dispose() => Dispose(true);
        ~Monsters() => Dispose(false);

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                MonstersInsideGroup.Clear();
                MonstersInsideGroup = null;
                disposed = true;
            }
        }
    }

}
