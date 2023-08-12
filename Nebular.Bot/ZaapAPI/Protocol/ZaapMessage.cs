using Nebular.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Nebular.Zaap.ZaapAPI.TProtocol;

namespace Nebular.Zaap.Network
{
    [Obfuscation(Exclude = true)]

    public abstract class ZaapMessage
    {
        public abstract void Serialize(TProtocol protocol, BigEndianWriter writer);
        public abstract void Deserialize(TProtocol protocol, BigEndianReader reader);

    }
}
