using Nebular.Bot.Game.Entity;
using Nebular.Bot.Game.World.Interactive;
using Nebular.Bot.Handlers;
using Nebular.Core.Cryptography;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace Nebular.Bot.Game.World
{
    public class Map : IDisposable
    {
        public short Id { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public Account Account { get; set; }
        public sbyte X { get; set; }
        public sbyte Y { get; set; }
        public bool IsMapLoaded { get; private set; }
        public Cell[] Cells;

        /** Concurrent to force thread-safety **/
        public ConcurrentDictionary<int, Nebular.Bot.Game.Entity.Entity> Entities;
        public ConcurrentDictionary<int, InteractiveObject> Interactives;

        private bool disposed;

        public Map(Account account)
        {
            Account = account;
            Entities = new ConcurrentDictionary<int, Nebular.Bot.Game.Entity.Entity>();
            Interactives = new ConcurrentDictionary<int, InteractiveObject>();
            IsMapLoaded = false;
        }

        public void UpdateInterface()
        {
            MainUI.mapControl.UpdateMap(this);
        }

        public void UpdateMapData(string _id)
        {
            UpdateMapData(short.Parse(_id));
        }

        public void UpdateMapData(short _id)
        {
            Entities.Clear();
            Interactives.Clear();

            Id = _id;

            FileInfo mapFile = new FileInfo($"maps/{Id}.json");
            if (!mapFile.Exists)
            {
                //Account.Client.log("MAP", "Error loading the map, it does not exist");
                return;
            }

            string text = File.ReadAllText($"maps/{Id}.json");
            MapJson mapData = JsonSerializer.Deserialize<MapJson>(text);

            Width = mapData.Width;
            Height = mapData.Height;
            X = mapData.X;
            Y = mapData.Y;

            DecompressMap(mapData.MapData);

            mapData = null;
            IsMapLoaded = true;
        }

        public string Coordinates => $"[{X},{Y}]";
        public Cell GetCellById(short cellId)
        {
            if( Account != null)
                MapHandler.WaitMapLoad(this, Account).Wait();
            return Cells[cellId];
        }
        public bool IsOnMap(string coordinates) => coordinates == Id.ToString() || coordinates == Coordinates;
        public bool IsOnMap(short id) => Id == id;
        public Cell GetCellByCoordinates(int x, int y) => Cells.FirstOrDefault(cell => cell.X == x && cell.Y == y);
        public bool CanFightMonsterGroup(int minMonsters, int maxMonsters, int minLevel, int maxLevel, List<int> prohibitedMonsters, List<int> requiredMonsters) => GetMonsterGroups(minMonsters, maxMonsters, minLevel, maxLevel, prohibitedMonsters, requiredMonsters).Count > 0;
        public List<Cell> OccupiedCells() => Entities.Values.Select(e => e.Cell).Where(c => !c.IsTeleport()).ToList();
        public List<Npc> GetNpcs() => Entities.Values.Where(e => e is Npc).Select(e => e as Npc).ToList();
        public List<Monsters> GetMonsters() => Entities.Values.Where(e => e is Monsters).Select(e => e as Monsters).ToList();
        public List<Character> GetCharacters() => Entities.Values.Where(e => e is Character).Select(e => e as Character).ToList();
        public List<Merchant> GetMerchants() => Entities.Values.Where(e => e is Merchant).Select(e => e as Merchant).ToList();

        public List<Monsters> GetMonsterGroups(int minMonsters, int maxMonsters, int minLevel, int maxLevel, List<int> prohibitedMonsters, List<int> requiredMonsters)
        {
            List<Monsters> availableMonsterGroups = new List<Monsters>();

            foreach (Monsters monsterGroup in GetMonsters())
            {
                if (monsterGroup.TotalMonsters < minMonsters || monsterGroup.TotalMonsters > maxMonsters)
                    continue;

                if (monsterGroup.TotalGroupLevel < minLevel || monsterGroup.TotalGroupLevel > maxLevel)
                    continue;

                if (monsterGroup.Cell.IsTeleport())
                    continue;

                bool isValid = true;

                if (prohibitedMonsters != null)
                {
                    for (int i = 0; i < prohibitedMonsters.Count; i++)
                    {
                        if (monsterGroup.ContainsMonster(prohibitedMonsters[i]))
                        {
                            isValid = false;
                            break;
                        }
                    }
                }

                if (requiredMonsters != null && isValid)
                {
                    for (int i = 0; i < requiredMonsters.Count; i++)
                    {
                        if (!monsterGroup.ContainsMonster(requiredMonsters[i]))
                        {
                            isValid = false;
                            break;
                        }
                    }
                }

                if (isValid)
                    availableMonsterGroups.Add(monsterGroup);
            }
            return availableMonsterGroups;
        }
        public async Task<bool> WaitForMapChanged(int delay)
        {
            bool mapChanged = false;

            void MapChanged()
            {
                mapChanged = true;
            }


            //&& Account.Script.IsRunning
            for (int i = 0; i < delay && !mapChanged && !Account.Character.IsFighting() ; i++)
                await Task.Delay(1000);


            return mapChanged;
        }

        public InteractiveObject GetInteractive(int cellId)
        {
            if (Interactives.TryGetValue(cellId, out InteractiveObject interactive))
                return interactive;

            return null;
        }

        #region Map Decompression
        public void DecompressMap(string mapData)
        {
            Cells = new Cell[mapData.Length / 10];
            string cellValues;

            for (int i = 0; i < mapData.Length; i += 10)
            {
                cellValues = mapData.Substring(i, 10);
                Cells[i / 10] = DecompressCell(cellValues, Convert.ToInt16(i / 10));
            }
        }

        public Cell DecompressCell(string cellData, short cellId)
        {
            byte[] cellInformation = new byte[cellData.Length];

            for (int i = 0; i < cellData.Length; i++)
                cellInformation[i] = Convert.ToByte(Hash.GetHash(cellData[i]));

            CellType type = (CellType)((cellInformation[2] & 56) >> 3);
            bool isActive = (cellInformation[0] & 32) >> 5 != 0;
            bool isLineOfSight = (cellInformation[0] & 1) != 1;
            bool hasInteractiveObject = ((cellInformation[7] & 2) >> 1) != 0;
            short layerObject2Num = Convert.ToInt16(((cellInformation[0] & 2) << 12) + ((cellInformation[7] & 1) << 12) + (cellInformation[8] << 6) + cellInformation[9]);
            short layerObject1Num = Convert.ToInt16(((cellInformation[0] & 4) << 11) + ((cellInformation[4] & 1) << 12) + (cellInformation[5] << 6) + cellInformation[6]);
            byte level = Convert.ToByte(cellInformation[1] & 15);
            byte slope = Convert.ToByte((cellInformation[4] & 60) >> 2);

            return new Cell(cellId, isActive, type, isLineOfSight, level, slope, hasInteractiveObject ? layerObject2Num : Convert.ToInt16(-1), layerObject1Num, layerObject2Num, this);
        }

        public class MapJson
        {
            [JsonPropertyName("id")]
            public short Id { get; set; }

            [JsonPropertyName("width")]
            public byte Width { get; set; }

            [JsonPropertyName("heigth")]
            public byte Height { get; set; }

            [JsonPropertyName("mapData")]
            public string MapData { get; set; } = string.Empty;

            [JsonPropertyName("X")]
            public sbyte X { get; set; }

            [JsonPropertyName("Y")]
            public sbyte Y { get; set; }
        }
        #endregion

        #region Dispose Region
        public void Dispose() => Dispose(true);
        ~Map() => Dispose(false);

        public void Clear()
        {
            Id = 0;
            X = 0;
            Y = 0;
            Entities.Clear();
            Interactives.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            Entities.Clear();
            Interactives.Clear();

            Cells = null;
            Entities = null;
            Interactives = null;
            disposed = true;
        }

        internal IEnumerable<object> GetNeighbours(string cellID)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

}
