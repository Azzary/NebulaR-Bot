using Nebular.Bot.Game.Entity;
using Nebular.Bot.Game.Inventory;
using Nebular.Bot.Hook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Scripting
{
    public class Banque
    {
        private Character Character;

        public Banque(Character character)
        { this.Character = character; }

        public void GiveItemToBanque()
        {
            foreach (var npc in Character.Map.GetNpcs())
            {
                int tryNpcSpeak = 0;
                var WindowManager = Character.Client.WindowManager;
                if (npc.ModelId == 100)
                {
                speakToNpc:
                    var task = Hook.Client.WaitForPacket(1000, "DCK" + npc.ID);
                    npc.Cell.RightClick();
                    task.Wait();
                    tryNpcSpeak++;
                    if (task.Result == false && tryNpcSpeak != 5)
                        goto speakToNpc;
                    Task.Delay(1000).Wait();
                    WindowManager.Click(131, 248);
                    Client.WaitForPacket(1000, "ECK").Wait();
                    Task.Delay(2000).Wait();
                    for (int i = 0; i < 1; i++)
                    {
                        WindowManager.Click(600, 125);
                        var chunks = SplitItemList<InventoryItems>(Character.Inventory.Items
                            .Where(x => x.InventoryType == Game.Inventory.Enum.InventoryItemTypes.RESOURCES).ToList(), 10);

                        foreach (var chunk in chunks)
                        {
                            foreach (var item in chunk.OrderByDescending(x => x.Index))
                            {
                                if (MainUI.Script.ScriptManager.ItemsToKeep.Contains(item.ModelId))
                                    continue;
                                SetInBanque(item);
                            }
                        }
                    }
                CloseBanque:
                    task = Client.WaitForPacket(1000, "EV");
                    WindowManager.Click(709, 100);
                    task.Wait();
                    if (task.Result == false)
                        goto CloseBanque;
                    Task.Delay(400).Wait();
                    return;
                }
            }
        }

        static List<List<T>> SplitItemList<T>(List<T> source, int chunkSize)
        {
            return source
                .Select((value, index) => new { value, index })
                .GroupBy(x => x.index / chunkSize)
                .Select(group => group.Select(x => x.value).ToList())
                .ToList();
        }

        private void SetInBanque(InventoryItems items)
        {
            var WindowManager = Character.Client.WindowManager;
            int x = 557;
            int y = 178;
            int xToAdd = 34;
            int yToAdd = 30;
            int clickX = x + (items.Index % 4) * xToAdd;
            int clickY = y + (items.Index / 4) * yToAdd;
            Task.Delay(200);
            WindowManager.MouseDown(clickX, clickY);
            Task.Delay(100).Wait();
            WindowManager.MouseUp(162, 178);
            Task.Delay(200).Wait();
            WindowManager.Click(57, 166);
            Task.Delay(200).Wait();
            var task = Client.WaitForPacket(1500, "EsKO+", "OR" + items.InventoryId);
            WindowManager.PressEnter();
            Task.Delay(300).Wait();
            task.Wait();
        }

    }
}
