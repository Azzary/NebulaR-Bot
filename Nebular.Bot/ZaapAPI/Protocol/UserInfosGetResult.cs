using Nebular.IO;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Reflection;
using Nebular.Zaap.Network;
using Nebular.Zaap.ZaapAPI.TProtocol;

namespace Nebular.Zaap.ZaapAPI.Protocol
{
    public class UserInfosGetResult : ZaapMessage
    {
        [Obfuscation(Exclude = true)]

        public string Login
        {
            get;
            private set;
        }
        public UserInfosGetResult(string login)
        {
            this.Login = login;
        }

        public override void Deserialize(TProtocol.TProtocol protocol, BigEndianReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(TProtocol.TProtocol protocol, BigEndianWriter writer)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("login", Login);

            string toSend = JsonConvert.SerializeObject(values);
            protocol.WriteFieldBegin(new TField(toSend, TType.STRING, 0), writer);

            writer.WriteInt(toSend.Length);
            writer.WriteUTFBytes(toSend);

            protocol.WriteFieldStop(writer);
        }
    }
}
