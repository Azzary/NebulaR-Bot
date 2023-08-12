
using Nebular.Core.Network.Messages;

namespace Nebular.Core.NebularProtocol.Messages
{
    public class DofusPacketMessage : Message
    {
        public DofusPacketMessage() { }
        public DofusPacketMessage(string packet, string botToken, string uidClient) 
        {
            Packet = packet;
            BotToken = botToken;
            UidClient = uidClient;
        }
        public override int MessageId => Id;

        public static int Id => 1;

        public string Packet { get; set; } = "";
        public string BotToken { get; set; } = "";
        public string UidClient { get; set; } = "";

    }
}
