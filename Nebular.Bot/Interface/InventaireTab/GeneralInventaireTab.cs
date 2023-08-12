using Nebular.Bot.Game.Inventory;
using Nebular.Bot.Game.Inventory.Enum;
using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Nebular.Bot.Interface.InventaireTab
{
    public class InventaireControl : UserControl
    {
        private TabControl GeneralInventaireTab;
        private TabPage EquipementTab;
        private TabPage UsableTab;
        private List<InventoryItemDiplay> InventoryConverter = new List<InventoryItemDiplay>();
        public int IndexEquiped = 0;
        private InventoryItemDiplay CeintureItem;
        private InventoryItemDiplay BoteItem;
        private InventoryItemDiplay CacItem;
        private InventoryItemDiplay Anneau1Item;
        private InventoryItemDiplay BoubouItem;
        private InventoryItemDiplay Anneau2Item;
        private InventoryItemDiplay CapeItem;
        private InventoryItemDiplay HatItem;
        private InventoryItemDiplay MontureItem;
        private InventoryItemDiplay FamilierItem;
        private InventoryItemDiplay dofus1Item;
        private InventoryItemDiplay dofus2Item;
        private InventoryItemDiplay dofus4Item;
        private InventoryItemDiplay dofus3Item;
        private InventoryItemDiplay dofus6Item;
        private InventoryItemDiplay dofus5Item;
        private InventoryItemDiplay AmulleteItem;
        private TabPage ResourceTab;
        private Dictionary<InventoryPositions, InventoryItemDiplay> positionToItemDisplayMap;
        private Dofus Client { get;set;}
        private GeneralInventory GeneralInventory => Client.Account.Character.Inventory;

        public void SetClientDofus(Dofus client)
        {
            Client = client;
        }
        public InventaireControl()
        {
            InitializeComponent();
            positionToItemDisplayMap = new Dictionary<InventoryPositions, InventoryItemDiplay>
            {
            { InventoryPositions.AMULET, AmulleteItem },
            { InventoryPositions.WEAPON, CacItem },
            { InventoryPositions.LEFT_RING, Anneau1Item },
            { InventoryPositions.BELT, CeintureItem },
            { InventoryPositions.RIGHT_RING, Anneau2Item },
            { InventoryPositions.BOOTS, BoteItem },
            { InventoryPositions.HAT, HatItem },
            { InventoryPositions.CAPE, CapeItem },
            { InventoryPositions.PET, FamilierItem },
            { InventoryPositions.DOFUS1, dofus1Item },
            { InventoryPositions.DOFUS2, dofus2Item },
            { InventoryPositions.DOFUS3, dofus3Item },
            { InventoryPositions.DOFUS4, dofus4Item },
            { InventoryPositions.DOFUS5, dofus5Item },
            { InventoryPositions.DOFUS6, dofus6Item },
            { InventoryPositions.SHIELD, BoubouItem }
        };
        }

        internal void Clear()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(Clear));
                return;
            }
            InventoryConverter.Clear();
            EquipementTab.Controls.Clear();
            UsableTab.Controls.Clear();
            ResourceTab.Controls.Clear();
            foreach (var item in positionToItemDisplayMap.Values)
            {
                item.Clear();
            }
        }

        private TabPage GetTargetTab(InventoryItemTypes type)
        {
            switch (type)
            {
                case InventoryItemTypes.EQUIPMENT:
                    return EquipementTab;
                case InventoryItemTypes.RESOURCES:
                    return ResourceTab;
                case InventoryItemTypes.DIVERSE:
                    return UsableTab;
                case InventoryItemTypes.USABLE:
                    //return UsableTab;
                default:
                    return null;
            }
        }

        public void RefreshLayout()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(RefreshLayout));
                return;
            }
            lock (this)
            {
                Clear();
                int x = 10;  // Coordonnée initiale X
                int y = 10;  // Coordonnée initiale Y
                int width = 72;  // Largeur de l'élément (vous pouvez ajuster selon vos besoins)
                int height = 85;  // Hauteur de l'élément (vous pouvez ajuster selon vos besoins)
                foreach (var item in GeneralInventory.Items)
                {
                    if (positionToItemDisplayMap.ContainsKey(item.Position))
                    {
                        positionToItemDisplayMap[item.Position].SetItem(item);
                    }
                    else
                    {
                        var tab = GetTargetTab(item.InventoryType); if (tab == null) continue;
                        Control itemBox = new InventoryItemDiplay(item);
                        x = 10 + (item.Index % 4) * (width + 2);
                        y = 10 + (int)Math.Floor((double)item.Index / 4) * (height);
                        itemBox.Location = new Point(x, y);
                        tab.Controls.Add(itemBox);
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            this.GeneralInventaireTab = new System.Windows.Forms.TabControl();
            this.EquipementTab = new System.Windows.Forms.TabPage();
            this.UsableTab = new System.Windows.Forms.TabPage();
            this.Anneau1Item = new Nebular.Bot.Interface.InventaireTab.InventoryItemDiplay();
            this.BoubouItem = new Nebular.Bot.Interface.InventaireTab.InventoryItemDiplay();
            this.Anneau2Item = new Nebular.Bot.Interface.InventaireTab.InventoryItemDiplay();
            this.CacItem = new Nebular.Bot.Interface.InventaireTab.InventoryItemDiplay();
            this.BoteItem = new Nebular.Bot.Interface.InventaireTab.InventoryItemDiplay();
            this.CeintureItem = new Nebular.Bot.Interface.InventaireTab.InventoryItemDiplay();
            this.AmulleteItem = new Nebular.Bot.Interface.InventaireTab.InventoryItemDiplay();
            this.CapeItem = new Nebular.Bot.Interface.InventaireTab.InventoryItemDiplay();
            this.HatItem = new Nebular.Bot.Interface.InventaireTab.InventoryItemDiplay();
            this.MontureItem = new Nebular.Bot.Interface.InventaireTab.InventoryItemDiplay();
            this.FamilierItem = new Nebular.Bot.Interface.InventaireTab.InventoryItemDiplay();
            this.dofus1Item = new Nebular.Bot.Interface.InventaireTab.InventoryItemDiplay();
            this.dofus2Item = new Nebular.Bot.Interface.InventaireTab.InventoryItemDiplay();
            this.dofus4Item = new Nebular.Bot.Interface.InventaireTab.InventoryItemDiplay();
            this.dofus3Item = new Nebular.Bot.Interface.InventaireTab.InventoryItemDiplay();
            this.dofus6Item = new Nebular.Bot.Interface.InventaireTab.InventoryItemDiplay();
            this.dofus5Item = new Nebular.Bot.Interface.InventaireTab.InventoryItemDiplay();
            this.ResourceTab = new System.Windows.Forms.TabPage();
            this.GeneralInventaireTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // GeneralInventaireTab
            // 
            this.GeneralInventaireTab.Controls.Add(this.EquipementTab);
            this.GeneralInventaireTab.Controls.Add(this.UsableTab);
            this.GeneralInventaireTab.Controls.Add(this.ResourceTab);
            this.GeneralInventaireTab.Location = new System.Drawing.Point(511, 3);
            this.GeneralInventaireTab.Name = "GeneralInventaireTab";
            this.GeneralInventaireTab.SelectedIndex = 0;
            this.GeneralInventaireTab.Size = new System.Drawing.Size(329, 563);
            this.GeneralInventaireTab.TabIndex = 0;
            // 
            // EquipementTab
            // 
            this.EquipementTab.Location = new System.Drawing.Point(4, 22);
            this.EquipementTab.Name = "EquipementTab";
            this.EquipementTab.Padding = new System.Windows.Forms.Padding(3);
            this.EquipementTab.Size = new System.Drawing.Size(321, 537);
            this.EquipementTab.TabIndex = 0;
            this.EquipementTab.Text = "Equipement";
            this.EquipementTab.UseVisualStyleBackColor = true;
            // 
            // UsableTab
            // 
            this.UsableTab.Location = new System.Drawing.Point(4, 22);
            this.UsableTab.Name = "UsableTab";
            this.UsableTab.Padding = new System.Windows.Forms.Padding(3);
            this.UsableTab.Size = new System.Drawing.Size(321, 537);
            this.UsableTab.TabIndex = 1;
            this.UsableTab.Text = "Usable";
            this.UsableTab.UseVisualStyleBackColor = true;
            // 
            // Anneau1Item
            // 
            this.Anneau1Item.Location = new System.Drawing.Point(31, 245);
            this.Anneau1Item.Name = "Anneau1Item";
            this.Anneau1Item.Size = new System.Drawing.Size(72, 85);
            this.Anneau1Item.TabIndex = 6;
            // 
            // BoubouItem
            // 
            this.BoubouItem.Location = new System.Drawing.Point(31, 109);
            this.BoubouItem.Name = "BoubouItem";
            this.BoubouItem.Size = new System.Drawing.Size(72, 85);
            this.BoubouItem.TabIndex = 5;
            // 
            // Anneau2Item
            // 
            this.Anneau2Item.Location = new System.Drawing.Point(254, 245);
            this.Anneau2Item.Name = "Anneau2Item";
            this.Anneau2Item.Size = new System.Drawing.Size(72, 85);
            this.Anneau2Item.TabIndex = 4;
            // 
            // CacItem
            // 
            this.CacItem.Location = new System.Drawing.Point(254, 109);
            this.CacItem.Name = "CacItem";
            this.CacItem.Size = new System.Drawing.Size(72, 85);
            this.CacItem.TabIndex = 3;
            // 
            // BoteItem
            // 
            this.BoteItem.Location = new System.Drawing.Point(139, 314);
            this.BoteItem.Name = "BoteItem";
            this.BoteItem.Size = new System.Drawing.Size(72, 85);
            this.BoteItem.TabIndex = 2;
            // 
            // CeintureItem
            // 
            this.CeintureItem.Location = new System.Drawing.Point(139, 184);
            this.CeintureItem.Name = "CeintureItem";
            this.CeintureItem.Size = new System.Drawing.Size(72, 85);
            this.CeintureItem.TabIndex = 1;
            // 
            // AmulleteItem
            // 
            this.AmulleteItem.Location = new System.Drawing.Point(139, 57);
            this.AmulleteItem.Name = "AmulleteItem";
            this.AmulleteItem.Size = new System.Drawing.Size(72, 85);
            this.AmulleteItem.TabIndex = 0;
            // 
            // CapeItem
            // 
            this.CapeItem.Location = new System.Drawing.Point(366, 161);
            this.CapeItem.Name = "CapeItem";
            this.CapeItem.Size = new System.Drawing.Size(72, 85);
            this.CapeItem.TabIndex = 8;
            // 
            // HatItem
            // 
            this.HatItem.Location = new System.Drawing.Point(366, 70);
            this.HatItem.Name = "HatItem";
            this.HatItem.Size = new System.Drawing.Size(72, 85);
            this.HatItem.TabIndex = 7;
            // 
            // MontureItem
            // 
            this.MontureItem.Location = new System.Drawing.Point(366, 348);
            this.MontureItem.Name = "MontureItem";
            this.MontureItem.Size = new System.Drawing.Size(72, 85);
            this.MontureItem.TabIndex = 10;
            // 
            // FamilierItem
            // 
            this.FamilierItem.Location = new System.Drawing.Point(366, 257);
            this.FamilierItem.Name = "FamilierItem";
            this.FamilierItem.Size = new System.Drawing.Size(72, 85);
            this.FamilierItem.TabIndex = 9;
            // 
            // dofus1Item
            // 
            this.dofus1Item.Location = new System.Drawing.Point(20, 495);
            this.dofus1Item.Name = "dofus1Item";
            this.dofus1Item.Size = new System.Drawing.Size(72, 85);
            this.dofus1Item.TabIndex = 11;
            // 
            // dofus2Item
            // 
            this.dofus2Item.Location = new System.Drawing.Point(98, 495);
            this.dofus2Item.Name = "dofus2Item";
            this.dofus2Item.Size = new System.Drawing.Size(72, 85);
            this.dofus2Item.TabIndex = 12;
            // 
            // dofus4Item
            // 
            this.dofus4Item.Location = new System.Drawing.Point(252, 495);
            this.dofus4Item.Name = "dofus4Item";
            this.dofus4Item.Size = new System.Drawing.Size(72, 85);
            this.dofus4Item.TabIndex = 14;
            // 
            // dofus3Item
            // 
            this.dofus3Item.Location = new System.Drawing.Point(176, 495);
            this.dofus3Item.Name = "dofus3Item";
            this.dofus3Item.Size = new System.Drawing.Size(72, 85);
            this.dofus3Item.TabIndex = 13;
            // 
            // dofus6Item
            // 
            this.dofus6Item.Location = new System.Drawing.Point(408, 495);
            this.dofus6Item.Name = "dofus6Item";
            this.dofus6Item.Size = new System.Drawing.Size(72, 85);
            this.dofus6Item.TabIndex = 16;
            // 
            // dofus5Item
            // 
            this.dofus5Item.Location = new System.Drawing.Point(330, 495);
            this.dofus5Item.Name = "dofus5Item";
            this.dofus5Item.Size = new System.Drawing.Size(72, 85);
            this.dofus5Item.TabIndex = 15;
            // 
            // ResourceTab
            // 
            this.ResourceTab.Location = new System.Drawing.Point(4, 22);
            this.ResourceTab.Name = "ResourceTab";
            this.ResourceTab.Padding = new System.Windows.Forms.Padding(3);
            this.ResourceTab.Size = new System.Drawing.Size(321, 537);
            this.ResourceTab.TabIndex = 2;
            this.ResourceTab.Text = "Ressources";
            this.ResourceTab.UseVisualStyleBackColor = true;
            // 
            // InventaireControl
            // 
            this.Controls.Add(this.dofus6Item);
            this.Controls.Add(this.dofus5Item);
            this.Controls.Add(this.dofus4Item);
            this.Controls.Add(this.dofus3Item);
            this.Controls.Add(this.dofus2Item);
            this.Controls.Add(this.dofus1Item);
            this.Controls.Add(this.MontureItem);
            this.Controls.Add(this.FamilierItem);
            this.Controls.Add(this.CapeItem);
            this.Controls.Add(this.HatItem);
            this.Controls.Add(this.Anneau1Item);
            this.Controls.Add(this.BoubouItem);
            this.Controls.Add(this.Anneau2Item);
            this.Controls.Add(this.CacItem);
            this.Controls.Add(this.BoteItem);
            this.Controls.Add(this.CeintureItem);
            this.Controls.Add(this.AmulleteItem);
            this.Controls.Add(this.GeneralInventaireTab);
            this.Name = "InventaireControl";
            this.Size = new System.Drawing.Size(843, 630);
            this.GeneralInventaireTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }


    }

}
