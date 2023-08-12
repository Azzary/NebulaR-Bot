﻿using Nebular.IO;
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

    public class UserInfoGet : ZaapMessage
    {
        public enum TFieldId
        {
            GAMESESSION = 1,
        }
        public string GameSession
        {
            get;
            set;
        }
        public UserInfoGet()
        {
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
                    case TFieldId.GAMESESSION:
                        this.GameSession = reader.ReadUTF7BitLength();
                        break;
                    default:
                        break;
                }
            }
        }

        public override void Serialize(TProtocol.TProtocol protocol, BigEndianWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
