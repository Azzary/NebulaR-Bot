using Nebular.Bot.Interface;
using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;

/*
    This file is part of the Bot Dofus Retro project
    Bot Dofus Retro Copyright (C) 2020 - 2023 Alvaro Prendes — All rights reserved.
    Created by Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Nebular.Bot.Network
{
    public static class PacketHandler
    {
        public static readonly List<PacketData> methods = new List<PacketData>();

        public static void Initialize()
        {
            Assembly asm = typeof(Program).GetTypeInfo().Assembly;

            foreach (MethodInfo type in asm.GetTypes().SelectMany(x => x.GetMethods()).Where(m => m.GetCustomAttributes(typeof(PacketAttribute), false).Length > 0))
            {
                PacketAttribute attribute = type.GetCustomAttributes(typeof(PacketAttribute), true)[0] as PacketAttribute;
                Type type_string = Type.GetType(type.DeclaringType.FullName);

                object instance = Activator.CreateInstance(type_string, null);
                methods.Add(new PacketData(instance, attribute.packet, type, attribute.OnlyServer));
            }
        }
        private static float nb = 0f;
        public static void Receive(Dofus client, string packet, string source)
        {
            PacketData method = methods.Find(m => (m.OnlyServer== false || source == "Server") && packet.StartsWith(m.PacketName));
            try
            {
                if (method != null)
                    method?.Information.Invoke(method.Instance, new object[2] { client, packet });
            }
            catch (Exception)
            {
            }
            nb++;
        }

       
    }
}
