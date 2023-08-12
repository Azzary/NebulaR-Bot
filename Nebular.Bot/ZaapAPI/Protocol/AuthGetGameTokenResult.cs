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

    public class AuthGetGameTokenResult : ZaapMessage
    {
        /// <summary>
        /// Should be ticket but...
        /// </summary>
        public string Password
        {
            get;
            private set;
        }
        public AuthGetGameTokenResult(string password)
        {
            this.Password = password;
        }

        public override void Deserialize(TProtocol.TProtocol protocol, BigEndianReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(TProtocol.TProtocol protocol, BigEndianWriter writer)
        {
            protocol.WriteFieldBegin(new TField(Password, TType.STRING, 0), writer);

            writer.WriteInt(Password.Length);
            writer.WriteUTFBytes(Password);

            protocol.WriteFieldStop(writer);
        }
    }
}
