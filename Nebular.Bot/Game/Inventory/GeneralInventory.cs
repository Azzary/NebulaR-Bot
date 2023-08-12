using Nebular.Bot.Game.Entity;
using Nebular.Bot.Game.Inventory.Enum;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Inventory
{
    public class GeneralInventory
    {
        private Account _account;
        private ConcurrentDictionary<uint, InventoryItems> _items;
        private bool _disposed;

        public int Kamas { get; private set; }
        public short CurrentPods { get; set; }
        public short MaximumPods { get; set; }

        public IEnumerable<InventoryItems> Items => _items.Values;
        public IEnumerable<InventoryItems> Equipment => Items.Where(o => o.InventoryType == InventoryItemTypes.EQUIPMENT);
        public IEnumerable<InventoryItems> Miscellaneous => Items.Where(o => o.InventoryType == InventoryItemTypes.DIVERSE);
        public IEnumerable<InventoryItems> Resources => Items.Where(o => o.InventoryType == InventoryItemTypes.RESOURCES);
        public IEnumerable<InventoryItems> QuestItems => Items.Where(o => o.InventoryType == InventoryItemTypes.USABLE);
        public int PodsPercentage => (int)((double)CurrentPods / MaximumPods * 100);

        public event Action InventoryUpdated;
        public event Action StorageOpened;
        public event Action StorageClosed;

        // Constructor
        internal GeneralInventory(Account account)
        {
            _account = account;
            _items = new ConcurrentDictionary<uint, InventoryItems>();
        }

        public InventoryItems GetItemByModelId(int gid) => Items.FirstOrDefault(f => f.ModelId == gid);
        public InventoryItems GetItemAtPosition(InventoryPositions position) => Items.FirstOrDefault(o => o.Position == position);

        public void AddItems(string package)
        {
            lock (this)
            {
                foreach (string item in package.Split(';'))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        string[] separator = item.Split('~');
                        uint inventoryId = Convert.ToUInt32(separator[0], 16);
                        try
                        {
                            InventoryItems newItem = new InventoryItems(item);
                            ChangeIndexToList(newItem, +1);
                            _items.TryAdd(inventoryId, newItem);
                        }
                        catch (Exception)
                        {
                        }

                    }
                }
                MainUI.GetInstance().InventaireControl.RefreshLayout();
            }
        }

        public void ChangeIndexToList(InventoryItems newItem, int value, bool CheckEquipedItem = true)
        {
            foreach (var oldItem in Items)
            {
                if(oldItem != newItem && oldItem.InventoryType == newItem.InventoryType && newItem.Index <= oldItem.Index && (!CheckEquipedItem  || oldItem.Position == InventoryPositions.NOT_EQUIPPED))
                    oldItem.SetIndex(oldItem.Index + value);
            }
        }

        public void ModifyItems(string package)
        {
            if (!string.IsNullOrEmpty(package))
            {
                string[] separator = package.Split('|');
                InventoryItems item = Items.FirstOrDefault(f => f.InventoryId == uint.Parse(separator[0]));

                if (item != null)
                {
                    int quantity = int.Parse(separator[1]);
                    InventoryItems updatedItem = item;
                    updatedItem.Quantity = quantity;
                    if (_items.TryUpdate(item.InventoryId, updatedItem, item))
                        InventoryUpdated?.Invoke();
                }
            }
        }

        public void DeleteItem(InventoryItems item, int quantity, bool deletePackage) => Task.Run(async () =>
        {
            if (item == null)
                return;

            quantity = 0;
            _items.TryRemove(item.InventoryId, out InventoryItems removedItem);
            ChangeIndexToList(removedItem, -1);
            if (deletePackage)
            {
                _account.Client.log("Inventory", $" {item.Name} deleted.");
            }
            MainUI.GetInstance().InventaireControl.RefreshLayout();
        });

        public void DeleteItem(uint inventoryId, int quantity, bool deletePackage)
        {
            if (!_items.TryGetValue(inventoryId, out InventoryItems item))
                return;

            DeleteItem(item, quantity, deletePackage);
        }

        public void EquipItem(string package)
        {
            if (!string.IsNullOrEmpty(package))
            {
                string[] separator = package.Split('|');
                InventoryItems item = Items.FirstOrDefault(f => f.InventoryId == uint.Parse(separator[0]));

                if (item != null)
                {
                    var newPosition = separator[1] == "" ? InventoryPositions.NOT_EQUIPPED : (InventoryPositions)int.Parse(separator[1]);
                    item.Position = newPosition;
                    lock (this)
                    {
                        if(newPosition == InventoryPositions.NOT_EQUIPPED)
                        {
                            ChangeIndexToList(item, +1, false);
                            var TabInvtoryItem = Items.Where(x => x.InventoryType == item.InventoryType);
                            if (item.Index > TabInvtoryItem.Count())
                                item.SetIndex(TabInvtoryItem.Count());
                        }
                        else
                        {
                            ChangeIndexToList(item, -1, false);
                        }
                    }
                    MainUI.GetInstance().InventaireControl.RefreshLayout();
                }
            }
        }

        public void StorageOpenedEvent() => StorageOpened?.Invoke();
        public void StorageClosedEvent() => StorageClosed?.Invoke();

    }
}
