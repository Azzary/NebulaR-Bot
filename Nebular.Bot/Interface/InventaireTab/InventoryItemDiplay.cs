using Nebular.Bot.Game.Inventory;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nebular.Bot.Interface.InventaireTab
{
    public class InventoryItemDiplay : UserControl
    {
        private MaskedTextBox ItemName;
        private PictureBox ImageItem;

        public InventoryItemDiplay()
        {
            Item = null;
            InitializeComponent();
            this.ItemName.Text = "";
        }

        public void SetItem(InventoryItems item)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<InventoryItems>(SetItem), item);
                return;
            }
            if(item == null)
            {
                Clear();
                return;
            }
            _setItem(item);
        }

        private void _setItem(InventoryItems item)
        {
            this.Item = item;
            this.ItemName.Text = item.Name;
            try
            {
                ImageItem.Image = Image.FromFile($"Images/{item.ModelId}.png");

            }
            catch (Exception)
            {

            }
        }

        public InventoryItemDiplay(InventoryItems item)
        {
            InitializeComponent();
            _setItem(item);
        }

        public InventoryItems Item { get; internal set; }

        private void InitializeComponent()
        {
            this.ImageItem = new System.Windows.Forms.PictureBox();
            this.ItemName = new System.Windows.Forms.MaskedTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.ImageItem)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageItem
            // 
            this.ImageItem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ImageItem.Location = new System.Drawing.Point(3, 3);
            this.ImageItem.Name = "ImageItem";
            this.ImageItem.Size = new System.Drawing.Size(66, 56);
            this.ImageItem.TabIndex = 0;
            this.ImageItem.TabStop = false;
            // 
            // ItemName
            // 
            this.ItemName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ItemName.Location = new System.Drawing.Point(3, 62);
            this.ItemName.Name = "ItemName";
            this.ItemName.Size = new System.Drawing.Size(66, 20);
            this.ItemName.TabIndex = 2;
            // 
            // InventoryItem
            // 
            this.Controls.Add(this.ItemName);
            this.Controls.Add(this.ImageItem);
            this.Name = "InventoryItem";
            this.Size = new System.Drawing.Size(72, 85);
            ((System.ComponentModel.ISupportInitialize)(this.ImageItem)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        internal void UpdateItem(InventoryItems item)
        {
            if(item == null)
            {
                this.Item = null;
                this.ItemName.Text = "";
                return;
            }
            this.Item = item;
            this.ItemName.Text = item.Name;
        }

        internal void Clear()
        {
            this.Item = null;
            this.ItemName.Text = "";
            ImageItem.Image = null;
        }
    }
}
