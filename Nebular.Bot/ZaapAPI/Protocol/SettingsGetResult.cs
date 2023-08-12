using Nebular.IO;
using Newtonsoft.Json;
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

    public class SettingsGetResult : ZaapMessage
    {
        public string Value
        {
            get;
            set;
        }
        public SettingsGetResult(string value)
        {
            this.Value = value;
        }

        public override void Deserialize(TProtocol.TProtocol protocol, BigEndianReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(TProtocol.TProtocol protocol, BigEndianWriter writer)
        {
            string[] values = new string[] { Value };
            string toSend = JsonConvert.SerializeObject(values);
            protocol.WriteFieldBegin(new TField(toSend, TType.STRING, 0), writer);
            writer.WriteInt(toSend.Length);
            writer.WriteUTFBytes(toSend);
            protocol.WriteFieldStop(writer);
        }
    }
}
