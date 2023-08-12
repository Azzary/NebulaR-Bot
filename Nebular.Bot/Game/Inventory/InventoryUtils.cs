using Nebular.Bot.Game.Inventory.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Inventory
{
    public static class InventoryUtils
    {
        private static Dictionary<int, List<InventoryPositions>> possiblePositions = new Dictionary<int, List<InventoryPositions>>()
        {
            { 1,  new List<InventoryPositions>() { InventoryPositions.AMULET } },
            { 2,  new List<InventoryPositions>() { InventoryPositions.WEAPON } },
            { 3,  new List<InventoryPositions>() { InventoryPositions.WEAPON } },
            { 4,  new List<InventoryPositions>() { InventoryPositions.WEAPON } },
            { 5,  new List<InventoryPositions>() { InventoryPositions.WEAPON } },
            { 6,  new List<InventoryPositions>() { InventoryPositions.WEAPON } },
            { 7,  new List<InventoryPositions>() { InventoryPositions.WEAPON } },
            { 8,  new List<InventoryPositions>() { InventoryPositions.WEAPON } },
            { 9,  new List<InventoryPositions>() { InventoryPositions.LEFT_RING, InventoryPositions.RIGHT_RING } },
            { 10, new List<InventoryPositions>() { InventoryPositions.BELT } },
            { 11, new List<InventoryPositions>() { InventoryPositions.BOOTS } },
            { 16, new List<InventoryPositions>() { InventoryPositions.HAT } },
            { 17, new List<InventoryPositions>() { InventoryPositions.CAPE } },
            { 18, new List<InventoryPositions>() { InventoryPositions.PET } },
            { 19, new List<InventoryPositions>() { InventoryPositions.WEAPON } }, //axe
            { 20, new List<InventoryPositions>() { InventoryPositions.WEAPON } }, //tool
            { 21, new List<InventoryPositions>() { InventoryPositions.WEAPON } }, //pickaxe
            { 22, new List<InventoryPositions>() { InventoryPositions.WEAPON } }, //scythe
            { 23, new List<InventoryPositions>() { InventoryPositions.DOFUS1, InventoryPositions.DOFUS2, InventoryPositions.DOFUS3, InventoryPositions.DOFUS4, InventoryPositions.DOFUS5, InventoryPositions.DOFUS6 } },
            { 82, new List<InventoryPositions>() { InventoryPositions.SHIELD } },
            { 83, new List<InventoryPositions>() { InventoryPositions.WEAPON } }, //soul stone
        };

        public static List<InventoryPositions> GetPossiblePositions(int objectType) => possiblePositions.ContainsKey(objectType) ? possiblePositions[objectType] : null;

        public static InventoryItemTypes GetInventoryItemsType(byte type)
        {
            switch (type)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 83:
                    return InventoryItemTypes.EQUIPMENT;

                case 12:
                case 13:
                case 85:
                case 86:
                    return InventoryItemTypes.DIVERSE;

                case 15:
                case 33:
                case 34:
                case 35:
                case 36:
                case 38:
                case 41:
                case 46:
                case 47:
                case 48:
                case 50:
                case 51:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                case 58:
                case 59:
                case 60:
                case 65:
                case 68:
                case 84:
                case 96:
                case 98:
                case 100:
                case 103:
                case 104:
                case 105:
                case 106:
                case 107:
                case 108:
                case 109:
                case 111:
                    return InventoryItemTypes.RESOURCES;

                case 24:
                    return InventoryItemTypes.USABLE;

                default:
                    return InventoryItemTypes.UNKNOWN;
            }
        }
    }

}
