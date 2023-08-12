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

    public class ConnectArgs : ZaapMessage
    {
        public enum TFieldId
        {
            GAMENAME = 1,
            RELEASENAME = 2,
            INSTANCEID = 3,
            HASH = 4,
        }

        private string GameName;

        private string ReleaseName;

        private int InstanceId;

        private string Hash;

        public ConnectArgs()
        {

        }

        public override void Serialize(TProtocol.TProtocol protocol, BigEndianWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(TProtocol.TProtocol protocol, BigEndianReader reader)
        {
            while (true)
            {
                var field = protocol.ReadFieldBegin(reader);

                if (field.Type == TType.STOP)
                {
                    break;
                }
                switch ((TFieldId)field.Id)
                {
                    case TFieldId.GAMENAME:
                        this.GameName = reader.ReadUTF7BitLength();
                        break;
                    case TFieldId.RELEASENAME:
                        this.ReleaseName = reader.ReadUTF7BitLength();
                        break;
                    case TFieldId.INSTANCEID:
                        this.InstanceId = reader.ReadInt();
                        break;
                    case TFieldId.HASH:
                        this.Hash = reader.ReadUTF7BitLength();
                        break;
                    default:
                        break;
                }
            }
        }
        public override string ToString()
        {
            return GameName + "," + ReleaseName;
        }

    }
}
