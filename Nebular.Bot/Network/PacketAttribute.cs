using System;

namespace Nebular.Bot.Network
{
    class PacketAttribute : Attribute
    {
        public string packet;
        public bool OnlyServer = false;

        public PacketAttribute(string _packet) => packet = _packet;
        public PacketAttribute(string _packet, bool onlyServer)
        {
            packet = _packet;
            OnlyServer = onlyServer;
        }
    }
}
