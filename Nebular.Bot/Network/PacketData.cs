using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Network
{
    public class PacketData
    {
        public object Instance { get; set; }
        public string PacketName { get; set; }
        public bool OnlyServer { get; set; }
        public MethodInfo Information { get; set; }

        public PacketData(object _instance, string _packetName, MethodInfo _information, bool onlyServer)
        {
            Instance = _instance;
            PacketName = _packetName;
            Information = _information;
            OnlyServer = onlyServer;
        }
    }
}
