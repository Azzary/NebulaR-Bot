using Nebular.Core.Network.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Core.NebularProtocol.Messages
{
    public class RemoveClientMessage : Message
    {
        public RemoveClientMessage() { }
        public RemoveClientMessage(string botToken, string uidClient)
        {
            BotToken = botToken;
            UidClient = uidClient;
        }
        public override int MessageId => Id;

        public static int Id => 2;

        public string BotToken { get; set; } = "";
        public string UidClient { get; set; } = "";

    }
}
