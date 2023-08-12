using Nebular.Bot.Game.Extension;
using Nebular.Bot.Game.Extention;
using Nebular.Bot.Game.Inventory;
using Nebular.Bot.Game.Spells;
using Nebular.Bot.Game.World;
using Nebular.Bot.Hook;
using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Entity
{
    public class Character : Entity, INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string Name { get; internal set; }
        public Dofus Client { get; private set; }

        private CharacterState characterState = CharacterState.CONNECTED_IDLE;

        public event PropertyChangedEventHandler PropertyChanged;
        public GeneralInventory Inventory { get; set; }

        public CharacterState CharacterState
        {
            get { return characterState; }
            set
            {
                if (characterState != value)
                {
                    characterState = value;
                    OnPropertyChanged(nameof(CharacterState));
                }
            }
        }
        public bool IsConnected { get; internal set; } = false;
        public byte Level { get; internal set; } = 0;
        public byte RaceID { get; internal set; } = 0;
        public byte Gender { get; internal set; } = 0;
        public Int64 Kamas { get; internal set; } = 0;
        private Dictionary<short, Spell> Spells { get; set; } = new Dictionary<short, Spell>();
        public int CharacteristicPoints { get; internal set; } = 0;
        public Characteristics Characteristics { get; private set; } = new Characteristics();
        public Cell Cell { get; set; }
        public Map Map { get; internal set; }
        public int LastMapID { get; internal set; }
        public bool IsOnDrago { get; internal set; } = false;
        public bool UsingDrago { get; internal set; }
        public MovementExtension Movement { get; internal set; }
        public GatheringExtension Gathering { get; internal set; }
        public JobsExtension Jobs { get; private set; }
        internal FightExtension Fight { get; private set; }


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Character(Dofus dofus, int id, string name, Cell cell)
        {
            ID = id;
            Name = name;
            if (dofus != null)
            {
                Client = dofus;


                Inventory = new GeneralInventory(Client.Account);
                Fight = new FightExtension(dofus);
                Gathering = new GatheringExtension(this);
                Jobs = new JobsExtension(dofus);
                Map = new Map(dofus.Account);
                LastMapID = Map.Id;
                Movement = new MovementExtension(Client.Account, Map, this);
            }
            this.Cell = cell;
        }

        public double LastExperienceNextLevel = -1;
        public async void CheckLevelUp()
        {
            if(LastExperienceNextLevel != Characteristics.ExperienceNextLevel)
            {
                LastExperienceNextLevel = Characteristics.ExperienceNextLevel;
                Level++;
                characterState = CharacterState.LOADING;
                await Task.Delay(1000);
                Client.WindowManager.Click(375, 265);
                await Task.Delay(100);
                Client.WindowManager.Click(550, 370);
                characterState = CharacterState.CONNECTED_IDLE;
            }
        }
        public void UpdateCharacteristics(string packet)
        {
            string[] _loc3 = packet.Substring(2).Split('|');
            string[] _loc5 = _loc3[0].Split(',');

            Characteristics.ExperienceActual = double.Parse(_loc5[0]);
            Characteristics.ExperienceMinLevel = double.Parse(_loc5[1]);
            Characteristics.ExperienceNextLevel = double.Parse(_loc5[2]);
            CheckLevelUp();
            Kamas = Int64.Parse(_loc3[1]);
            CharacteristicPoints = int.Parse(_loc3[2]);

            _loc5 = _loc3[5].Split(',');
            Characteristics.CurrentVitality = int.Parse(_loc5[0]);
            Characteristics.MaxVitality = int.Parse(_loc5[1]);

            _loc5 = _loc3[6].Split(',');
            Characteristics.CurrentEnergy = int.Parse(_loc5[0]);
            Characteristics.MaxEnergy = int.Parse(_loc5[1]);
            Characteristics.Initiative.BaseCharacter = int.Parse(_loc3[7]);
            Characteristics.Prospecting.BaseCharacter = int.Parse(_loc3[8]);

            for (int i = 9; i < 19; ++i)
            {
                _loc5 = _loc3[i].Split(',');
                int baseCharacter = int.Parse(_loc5[0]);
                int equipment = int.Parse(_loc5[1]);
                int gifts = int.Parse(_loc5[2]);
                int boost = int.Parse(_loc5[3]);

                switch (i)
                {
                    case 9:
                        Characteristics.ActionPoints.UpdateStats(baseCharacter, equipment, gifts, boost);
                        break;

                    case 10:
                        Characteristics.MovementPoints.UpdateStats(baseCharacter, equipment, gifts, boost);
                        break;

                    case 11:
                        Characteristics.Strength.UpdateStats(baseCharacter, equipment, gifts, boost);
                        break;

                    case 12:
                        Characteristics.Vitality.UpdateStats(baseCharacter, equipment, gifts, boost);
                        break;

                    case 13:
                        Characteristics.Wisdom.UpdateStats(baseCharacter, equipment, gifts, boost);
                        break;

                    case 14:
                        Characteristics.Luck.UpdateStats(baseCharacter, equipment, gifts, boost);
                        break;

                    case 15:
                        Characteristics.Agility.UpdateStats(baseCharacter, equipment, gifts, boost);
                        break;

                    case 16:
                        Characteristics.Intelligence.UpdateStats(baseCharacter, equipment, gifts, boost);
                        break;

                    case 17:
                        Characteristics.Range.UpdateStats(baseCharacter, equipment, gifts, boost);
                        break;

                    case 18:
                        Characteristics.SummonableCreatures.UpdateStats(baseCharacter, equipment, gifts, boost);
                        break;
                }
            }
        }

        public void UpdateSpells(string packet)
        {
            Spells.Clear();

            string[] limitador = packet.Split(';'), separador;
            Spell spell;
            short spellId;

            for (int i = 0; i < limitador.Length - 1; ++i)
            {
                separador = limitador[i].Split('~');
                spellId = short.Parse(separador[0]);

                spell = Spell.GetSpell(spellId);
                spell.Level = byte.Parse(separador[1]);

                Spells.Add(spellId, spell);
            }
        }



        public bool IsDisconnected() => CharacterState == CharacterState.DISCONNECTED;
        public bool IsChangingToGame() => CharacterState == CharacterState.CHANGING_TO_GAME;
        public bool IsInDialogue() => CharacterState == CharacterState.STORAGE || CharacterState == CharacterState.DIALOGUE || CharacterState == CharacterState.TRADING || CharacterState == CharacterState.BUYING || CharacterState == CharacterState.SELLING;
        public bool IsFighting() => CharacterState == CharacterState.FIGHTING;
        public bool IsGathering() => CharacterState == CharacterState.GATHERING;
        public bool IsMoving() => CharacterState == CharacterState.MOVING;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
