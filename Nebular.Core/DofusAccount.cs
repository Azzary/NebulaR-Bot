using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Core
{

    [Obfuscation(Exclude = true)]
    public class DofusAccount
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Speudo { get; set; }
        public string Serveur { get; set; }
        public string Key { get; set; }
    }

}
