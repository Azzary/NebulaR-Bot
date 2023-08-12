using Nebular.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Scripting.API.GeneralAPI
{
    public class ClientAPI
    {

        private WindowManager WindowManager { get; set; }
        public ClientAPI(WindowManager windowMananger)
        {
            WindowManager = windowMananger;
        }

        public void Wait(int timeout)
        {
            Task.Delay(timeout).Wait();
        }

        public void PostMessageClick (int X, int Y) => WindowManager.PostMessageClick(X, Y);
        public void Move(int X, int Y) => WindowManager.Click(X, Y);
        public void Click(int X, int Y) => WindowManager.Click(X, Y);
        public void DoubleClick(int X, int Y) => WindowManager.DoubleClick(X, Y);
        public void RightClick(int X, int Y) => WindowManager.RightClick(X, Y);
        public void Press(string key)
        {
            if (Enum.TryParse(key, out VirtualKeys virtualKey))
                WindowManager.Press(virtualKey);
        }
        public void PressEnter() => WindowManager.PressEnter();


    }
}
