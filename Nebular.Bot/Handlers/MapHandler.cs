using Nebular.Bot.Game;
using Nebular.Bot.Game.Entity;
using Nebular.Bot.Game.Extention;
using Nebular.Bot.Game.World;
using Nebular.Bot.Network;
using Nebular.Core.Cryptography;
using Nebular.Core.ProcessHandler;
using System.Threading.Tasks;

namespace Nebular.Bot.Handlers
{
    public class MapHandler
    {

        public static async Task WaitMapLoad(Map map ,Account account)
        {
            if (account == null)
                return;
            while (!map.IsMapLoaded || account == null || account.Character == null || account.Character.Map == null)
            {
                await Task.Delay(100);
            }
        }

        [Packet("GM")]
        public async void GetCharactersMovements(Dofus client, string packet)
        {
            Account account = client.Account;
            Map map = account.Character.Map;
            string[] playerSeparator = packet.Substring(3).Split('|'), informations;
            string templateName, type;
            int id;
            await WaitMapLoad(map, account);

            foreach (string player in playerSeparator)
            {
                if (player.Length < 1)
                    continue;

                informations = player.Substring(1).Split(';');

                if (player[0].Equals('+'))
                {
                    if (informations.Length < 6)
                        continue;
                    Cell cell = map.GetCellById(short.Parse(informations[0]));
                    FightExtension fight = account.Character.Fight;
                    id = int.Parse(informations[3]);
                    templateName = informations[4];
                    type = informations[5];
                    if (type.Contains(","))
                        type = type.Split(',')[0];

                    switch (int.Parse(type))
                    {
                        case -1: // Creature
                        case -2: // Monster
                            if (!account.Character.IsFighting())
                                return;

                            int monsterLife = int.Parse(informations[12]);
                            byte monsterPA = byte.Parse(informations[13]);
                            byte monsterPM = byte.Parse(informations[14]);
                            byte monsterTeam;

                            if (informations.Length > 18)
                                monsterTeam = byte.Parse(informations[22]);
                            else
                                monsterTeam = byte.Parse(informations[15]);

                            fight.AddFighter(new Fighter(id, true, monsterLife, monsterPA, monsterPM, cell, monsterLife, monsterTeam, 0, false));
                            break;

                        case -3: // Group of Monsters
                            string[] templates = templateName.Split(',');
                            string[] levels = informations[7].Split(',');

                            Monsters monster = new Monsters(id, int.Parse(templates[0]), cell, int.Parse(levels[0]));
                            monster.GroupLeader = monster;

                            for (int m = 1; m < templates.Length; ++m)
                                monster.MonstersInsideGroup.Add(new Monsters(id, int.Parse(templates[m]), cell, int.Parse(levels[m])));

                            map.Entities.TryAdd(id, monster);
                            break;

                        case -4: // NPC's
                            map.Entities.TryAdd(id, new Npc(id, int.Parse(templateName), cell));
                            break;

                        case -5: // Merchants
                            map.Entities.TryAdd(id, new Merchant(id, cell));
                            break;

                        case -6: // Collectors
                            if (account.Character.IsFighting())
                            {
                                byte collectorLife = byte.Parse(informations[9]);
                                byte collectorPA = byte.Parse(informations[9]);
                                byte collectorPM = byte.Parse(informations[10]);
                                byte collectorTeam = byte.Parse(informations[18]);

                                fight.AddFighter(new Fighter(id, true, collectorLife, collectorPA, collectorPM, cell, collectorLife, collectorTeam, 0, false));
                            }
                            break;

                        case -7:
                        case -8:
                        case -9:
                        case -10:
                            break;

                        default: // Players
                            if (account.Character.IsFighting())
                            {
                                int life = int.Parse(informations[14]);
                                byte pa = byte.Parse(informations[15]);
                                byte pm = byte.Parse(informations[16]);
                                byte team = byte.Parse(informations[24]);
                                fight.AddFighter(new Fighter(id, true, life, pa, pm, cell, life, team, 0, false));
                            }
                            else
                            {
                                if (account.Character.ID == id)
                                {
                                    account.Character.Cell = cell;
                                    //account.Character.Restrictions.SetRestrictions(int.Parse(informations[18]));
                                }
                                else
                                    map.Entities.TryAdd(id, new Character(null, id, templateName, cell));//, byte.Parse(informations[7].ToString()), int.Parse(informations[18])));
                            }
                            break;
                    }
                }

                if (player[0].Equals('-'))
                {
                    if (!account.Character.IsFighting())
                    {
                        id = int.Parse(player.Substring(1));
                        map.Entities.TryRemove(id, out Entity entity);
                        //map.GetUpdatedEntity();

                        //if (GlobalConf.ShowDebugMessages)
                        //    account.Logger.LogInformation("DEBUG", $"Entity {entity.Id} removed from cell {entity.Cell.Id}");
                    }
                }
            }
            client.Account.Character.Map.UpdateInterface();
        }



        [Packet("GDM")]
        public void GetNewMap(Dofus client, string packet)
        {
            if(!client.Account.Character.IsFighting())
                client.Account.Character.LastMapID = client.Account.Character.Map.Id;
            string[] parts = packet.Substring(4).Split('|');
            short Id = short.Parse(parts[0]);
            client.Account.Character.Map.UpdateMapData(Id);
            client.Account.Character.Map.UpdateInterface();
            client.Account.Character.CharacterState = CharacterState.LOADING;
        }
        
        [Packet("GDK")]
        public async void EndGetNewMapData(Dofus client, string packet)
        {
            await Task.Delay(300);
                client.Account.Character.CharacterState = CharacterState.CONNECTED_IDLE;
        }


        [PacketAttribute("GDF")]
        public void GetInteractiveState(Dofus client, string packet) 
        {
            foreach (string interactiveData in packet.Substring(4).Split('|'))
            {
                string[] separator = interactiveData.Split(';');
                Account account = client.Account;
                short cellId = short.Parse(separator[0]);
                byte state = byte.Parse(separator[1]);
                if (!account.Character.Map.Interactives.ContainsKey(cellId))
                    continue;
                switch (state)
                {
                    case 2:
                        account.Character.Map.Interactives[cellId].IsUsable = false;
                        break;

                    case 3:
                        if (account.Character.Map.Interactives.TryGetValue(cellId, out var interactiveCell))
                            interactiveCell.IsUsable = false;
                        break;
                    case 4:
                        account.Character.Map.Interactives[cellId].IsUsable = false;
                        break;
                    case 5:
                        account.Character.Map.Interactives[cellId].IsUsable = true;
                        break;
                }
            }

            client.Account.Character.Map.UpdateInterface();
        }


        [PacketAttribute("GA")]
        public Task GetAction(Dofus client, string packet) => Task.Run(async () =>
        {
            if (client.Account.Character.Map == null)
                await Task.Delay(1500);
            string[] separator = packet.Substring(2).Split(';');
            int actionId = int.Parse(separator[1]);
            int entityId = int.Parse(separator[2]);
            Account account = client.Account;
            Character character = account.Character;
            Cell cell;
            Map map = account.Character.Map;
            await WaitMapLoad(map, account);
            FightExtension fight = account.Character.Fight;
            Fighter fighter = fight.GetFighterById(entityId);
            switch (actionId)
            {
                case 0:
                    switch (account.Character.CharacterState)
                    {
                        case CharacterState.MOVING:
                            await account.Character.Movement.MovementFinishedEvent(packet);
                            break;

                        case CharacterState.FIGHTING:
                            //await account.FightExtension.ProcessActionWithoutCastingSpell();
                            break;
                    }
                    break;

                case 1:
                    cell = map.GetCellById(Hash.GetCellIdFromHash(separator[3].Substring(separator[3].Length - 2)));
                    byte.TryParse(separator[0], out byte gkkMovementType);

                    if (account.Character.IsFighting() && fighter != null)
                        fighter.Cell = cell;
                    
                    if (entityId == character.ID && cell.Id > 0)
                    {
                        if (client.ScriptCreator.Create && !account.Character.IsFighting())
                        {
                            client.ScriptCreator.AddMove(map.Id, cell.Id.ToString());
                        }
                        await account.Character.Movement.MovementFinishedEvent(packet);
                        account.Character.Cell = cell;
                    }
                    if (map.Entities.TryGetValue(entityId, out Entity entity))
                        entity.Cell = cell;
                    //map.GetUpdatedEntity();

                    //if (GlobalConf.ShowDebugMessages)
                    //    account.Client.log("DEBUG", "Entity movement to cell: " + cell.Id);
                    break;

                case 4:
                    separator = separator[3].Split(',');
                    cell = map.GetCellById(short.Parse(separator[1]));

                    if (account.Character.IsFighting() && fighter != null)
                    {
                        fighter.Cell = cell;
                    }
                    else
                    if (entityId == character.ID && character.Cell.Id != cell.Id)
                    {
                        character.Cell = cell;
                    }

                    //map.GetUpdatedEntity();
                    break;

                case 5:
                    if (!account.Character.IsFighting())
                        return;

                    byte.TryParse(separator[0], out byte gkk);
                    separator = separator[3].Split(',');
                    fighter = fight.GetFighterById(int.Parse(separator[0]));

                    if (fighter != null)
                        fighter.Cell = map.GetCellById(short.Parse(separator[1]));
                    break;
                // Modified fight points
                case 100:
                case 108:
                case 110:
                case 127:
                case 129:
                case 128:
                case 78:
                case 169:
                case 101:
                case 102:
                case 111:
                case 120:
                case 168:
                    if (!account.Character.IsFighting() /*|| fighter == null*/)
                        return;

                    string[] _loc43_ = separator[3].Split(',');

                    fighter.UpdateFightPoints(actionId, _loc43_[1]);
                    break;

                case 103:
                    if (!account.Character.IsFighting())
                        return;

                    int deadId = int.Parse(separator[3]);
                    fighter = fight.GetFighterById(deadId);

                    if (fighter != null)
                    {
                        fighter.CurrentLife = 0;
                        fighter.IsAlive = false;
                    }
                    break;
                case 151: // Invisible obstacles
                    if (!account.Character.IsFighting() /*|| fighter == null*/)
                        return;

                    //account.Logger.LogError("FIGHT", "This action cannot be performed due to an invisible obstacle.");
                    //if (fighter.Id == character.Id)
                    //{
                    //    await Task.Delay(1000);
                    //    await account.FightExtension.ProcessActionWithoutCastingSpell();
                    //}
                    break;

                // Summoning effect
                case 180:
                case 181:
                    cell = map.GetCellById(short.Parse(separator[3].Substring(1)));
                    short fighterId = short.Parse(separator[6]);
                    short life = short.Parse(separator[15]);
                    byte ap = byte.Parse(separator[16]);
                    byte mp = byte.Parse(separator[17]);
                    byte team;

                    if (separator.Length > 19)
                        team = byte.Parse(separator[25]);
                    else
                        team = byte.Parse(separator[18]);

                    fight.AddFighter(new Fighter(fighterId, true, life, ap, mp, cell, life, team, fighterId, true));
                    break;

                case 300: // Successful spell cast
                    if (!account.Character.IsFighting() || entityId != character.ID)
                        return;

                    short spellId = short.Parse(separator[3].Split(',')[0]);
                    short cellId = short.Parse(separator[3].Split(',')[1]);
                    //fight.UpdateSpellSuccess(spellId, cellId);
                    break;

                case 302: // Critical failure
                    if (!account.Character.IsFighting() || entityId != character.ID)
                        return;

                    //await account.Character.FightExtension.ProcessActionWithoutCastingSpell();
                    break;

                case 501:
                    int harvestTime = int.Parse(separator[3].Split(',')[1]);
                    cell = map.GetCellById(short.Parse(separator[3].Split(',')[0]));
                    byte gkkHarvestType = byte.Parse(separator[0]);

                    await account.Character.Gathering.HarvestStarted(entityId, harvestTime, cell.Id, gkkHarvestType);
                    break;

                case 900:
                    //await account.Connection.Send("GA902" + entityId);
                    //account.Logger.LogInformation("INFORMATION", "Challenge of character id: " + entityId + " canceled");
                    break;
            }

            client.Account.Character.Map.UpdateInterface();
        });


        [PacketAttribute("GKE0")]
        public void StopMovement(Dofus client, string packet)
        {
            if (int.TryParse(packet.Split('|')[1], out int cellID))
            {
                client.Account.Character.Movement.TryCancelToken();
                client.Account.Character.Cell = client.Account.Character.Map.Cells[cellID];
            }

            client.Account.Character.Map.UpdateInterface();
        }


    }
}
