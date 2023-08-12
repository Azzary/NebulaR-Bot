using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Core.NebularProtocol.IO
{
    public class Singleton<T> where T : new()
    {
        private static T _instance;
        private static readonly object _lock = new object();

        public static T Get
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                    return _instance;
                }
            }
        }
    }

}
