using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Zaap.ZaapAPI.TProtocol
{
    [Obfuscation(Exclude = true)]

    public enum TMessageType
    {
        CALL = 1,
        REPLY = 2,
        EXCEPTION = 3,
        ONEWAY = 4,
    }
    public class TMessage
    {
        public string Name;

        public int Type;
        public TMessageType TypeEnum => (TMessageType)Type;

        public int SequenceId;
    }
}
