using Nebular.Core.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.World.PathFinder
{
    public class MapMouvementPacket
    {
        public new const string Name = "GA001";
        public List<int> Directions { get; private set; }
        public List<int> Cells { get; set; }

        public MapMouvementPacket(string packet)
        {
            Directions = new List<int>();
            Cells = new List<int>();
            Deserialize(packet);
        }

        public void Deserialize(string packet)
        {
            if (!packet.StartsWith("GA0;1"))
            {
                throw new ArgumentException("Le paquet n'est pas au bon format");
            }

            string path = packet.Split(';')[3];
            if (path.Length % 3 != 0)
            {
                throw new ArgumentException("Le chemin n'est pas au bon format");
            }
            for (int i = 0; i < path.Length; i += 3)
            {
                int direction = Hash.GetDirNum(path.Substring(i, 1));
                int cellID = Hash.CharToCell(path.Substring(i + 1, 2));
                Directions.Add(direction);
                Cells.Add(cellID);
            }
        }

    }
}
