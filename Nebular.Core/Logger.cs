using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Core
{
    public enum Channels
    {
        Info = 1,
        Warning = 2,
        Critical = 4,
        Log = 8,
    }
    public static class Logger
    {
        public static void info(object message, Channels channels = Channels.Info)
        {
            Console.WriteLine(message);
        }

        public static void Write(object message, Channels channels = Channels.Info)
        {
            Console.WriteLine(message);
        }

        public static void WriteColor1(Object message) 
        {
            Write(message, Channels.Info);
        }

    }
}
