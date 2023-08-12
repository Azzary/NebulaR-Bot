using Nebular.Bot.Game;
using Nebular.Bot.Game.Entity;
using Nebular.Bot.Interface;
using Nebular.Bot.Network;
using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Handlers
{
    internal class CharacterHandler
    {

        [Packet("ALK")]
        public async void GetCharacterList(Dofus client, string packet)
        {
            Account account = client.Account;
            string[] data = packet.Substring(3).Split('|');
            int counter = 2, characterId = 0;

            while (counter < data.Length)
            {
                string[] characterData = data[counter].Split(';');
                characterId = int.Parse(characterData[0]);
                string name = characterData[1];
                if(account.Characters.FirstOrDefault(x => x.Name == data[1]) == null)
                    account.Characters.Add(new Character(client, characterId, name, null));
                counter++;
            }
            await Task.Delay(1000);
            account.SelectCharacter(DofusLoginForm.SelectedAccount.Speudo);

        }

        [Packet("ASK")]
        public void GetSelectedCharacter(Dofus client, string packet) 
        {
            Account account = client.Account;
            string[] data = packet.Substring(4).Split('|');
            if(account.Character == null)
            {
                if(account.Characters.FirstOrDefault(x => x.Name == data[1]) == null)
                    account.Characters.Add(new Character(client, int.Parse(data[0]), data[1], null));
                account.SetIndexOfCharacter(data[1]);
            }

            Character character = account.Character;
            character.ID = int.Parse(data[0]);
            character.Name = data[1];
            character.Level = byte.Parse(data[2]);
            character.RaceID = byte.Parse(data[3]);
            character.Gender = byte.Parse(data[4]);
            character.IsConnected = true;

            character.Inventory.AddItems(data[9]);
        }

        [Packet("As")]
        public void GetUpdateStat(Dofus client, string packet)
        { 
            Character character = client.Account.Character;

            character.UpdateCharacteristics(packet);
        }

        [Packet("SL")]
        public void GetSpellList(Dofus client, string packet)
        {
            if (!packet[2].Equals('o'))
                client.Account.Character.UpdateSpells(packet.Substring(2));
        }


        [PacketAttribute("Ow")]
        public Task GetInventoryUpdate(Dofus client, string packet) => Task.Run(() =>
        {
            string[] inventoryPods = packet.Substring(2).Split('|');
            short currentPods = short.Parse(inventoryPods[0]);
            short maximumPods = short.Parse(inventoryPods[1]);
            Character character = client.Account.Character;

            character.Inventory.CurrentPods = currentPods;
            character.Inventory.MaximumPods = maximumPods;
        });

        [PacketAttribute("OAKO")]
        public Task AddItems(Dofus client, string packet) => Task.Run(() => client.Account.Character.Inventory.AddItems(packet.Substring(4)));

        [PacketAttribute("OR")]
        public Task RemoveItem(Dofus client, string packet) => Task.Run(() => client.Account.Character.Inventory.DeleteItem(uint.Parse(packet.Substring(2)), 1, false));

        [PacketAttribute("OQ")]
        public Task ModifyItemQuantity(Dofus client, string packet) => Task.Run(() => client.Account.Character.Inventory.ModifyItems(packet.Substring(2)));

        [PacketAttribute("OM", true)]
        public Task EquipeItem(Dofus client, string packet) => Task.Run(() => client.Account.Character.Inventory.EquipItem(packet.Substring(2)));



    }
}
