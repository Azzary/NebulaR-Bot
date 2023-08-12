using Nebular.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Nebular.Zaap.Network;
using Nebular.Zaap.ZaapAPI.TProtocol;

namespace Nebular.Zaap.ZaapAPI.Protocol
{
    [Obfuscation(Exclude = true)]

    public class ConnectResult : ZaapMessage
    {
        public ConnectResult()
        {

        }

        public override void Deserialize(TProtocol.TProtocol protocol, BigEndianReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(TProtocol.TProtocol protocol, BigEndianWriter writer)
        {
            protocol.WriteFieldBegin(new TField("success", TType.STRING, 0), writer);

            string toSend = "success";

            writer.WriteInt(toSend.Length);
            writer.WriteUTFBytes(toSend);

            protocol.WriteFieldStop(writer);
        }

    }
}
