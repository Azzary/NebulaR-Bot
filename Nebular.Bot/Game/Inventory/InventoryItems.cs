using Nebular.Bot.Game.Inventory.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nebular.Bot.Game.Inventory
{
    public class InventoryItems
    {
        public uint InventoryId { get; private set; }
        public int ModelId { get; private set; }
        public string Name { get; private set; } = "Unknown";
        public int Quantity { get; set; }
        public InventoryPositions Position { get; set; } = InventoryPositions.NOT_EQUIPPED;
        public short Pods { get; private set; }
        public short Level { get; private set; }
        public byte Type { get; private set; }

        public int Index { get; private set; } = 0;
        public short LifeRegenerated { get; }
        public InventoryItemTypes InventoryType { get; private set; } = InventoryItemTypes.UNKNOWN;
        private readonly XElement itemFile;

        public InventoryItems(string package)
        {
            string[] separator = package.Split('~');
            InventoryId = Convert.ToUInt32(separator[0], 16);
            ModelId = Convert.ToInt32(separator[1], 16);
            Quantity = Convert.ToInt32(separator[2], 16);

            if (!string.IsNullOrEmpty(separator[3]))
                Position = (InventoryPositions)Convert.ToSByte(separator[3], 16);

            string[] split = separator[4].Split(',');
            foreach (string stat in split)
            {
                string[] statsSeparator = stat.Split('#');
                string id = statsSeparator[0];

                if (string.IsNullOrEmpty(id))
                    continue;

                int statId = Convert.ToInt32(id, 16);
                if (statId == 110)
                    LifeRegenerated = Convert.ToInt16(statsSeparator[1], 16);
            }

            FileInfo itemFileDetails = new FileInfo($"items/{ModelId}.xml");
            if (itemFileDetails.Exists)
            {
                itemFile = XElement.Load(itemFileDetails.FullName);
                Name = itemFile.Element("Name").Value;
                Pods = short.Parse(itemFile.Element("Pods").Value);
                Type = byte.Parse(itemFile.Element("Type").Value);
                Level = short.Parse(itemFile.Element("Level").Value);
                InventoryType = InventoryUtils.GetInventoryItemsType(Type);

                itemFile = null;
            }
        }

        public void SetIndex(int index)
        {
            this.Index = index;
        }
        public bool IsItemEquipped() => Position > InventoryPositions.NOT_EQUIPPED;
    }

}
