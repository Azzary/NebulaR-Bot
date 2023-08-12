using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Zaap.ZaapAPI.TProtocol
{
    [Obfuscation(Exclude = true)]

    public enum TType
    {
        STOP = 0,
        VOID = 1,
        BOOL = 2,
        BYTE = 3,
        DOUBLE = 4,
        I16 = 6,
        I32 = 8,
        I64 = 10,
        STRING = 11,
        STRUCT = 12,
        MAP = 13,
        SET = 14,
        LIST = 15,
    }
    public class TField
    {
        public string Name;
        public TType Type;

        public int Id;

        public TField(string name, TType type, int id)
        {
            this.Name = name;
            this.Type = type;
            this.Id = id;
        }
    }
}
