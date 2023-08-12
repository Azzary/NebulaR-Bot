using Nebular.Bot.Game;
using Nebular.Bot.Hook;
using Nebular.Bot.Interface;
using Nebular.Bot.Scripting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Nebular.Core.ProcessHandler
{
    public class Dofus
    {
        public ScriptCreator ScriptCreator { get; set; }
        public EnumClientType ClientType { get; private set; } = EnumClientType.DofusRetro;
        public WindowManager WindowManager { get; private set; }
        public Process Process { get; private set; }

        public int PID => Process.Id;

        public Account Account { get; internal set; }
        public string PathIaFile { get; internal set; }

        public void Hook()
        {
            Account = new Account(this);
            new HookClient(this);
        }

        public List<Process> GetAllPid()
        {
            var childProcesses = Process.GetChildProcesses().ToList();
            childProcesses.Add(Process);
            return childProcesses;
        }
        public Dofus() 
        {
            string arguments = ClientType == EnumClientType.DofusRetro ? "" : $@"--port={228546} --gameName=dofus --gameRelease=main --instanceId=1 --hash=3c18fe6c-cb8b-413a-a8af-9b8c3a0e44df --canLogin=true";

            if (!File.Exists("pathDofus.txt"))
            {
                string pathTest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ankama\\Retro\\Dofus Retro.exe");
                if (File.Exists(pathTest))
                {
                    File.WriteAllText("PathDofus.txt", pathTest);
                }
                else
                {
                    DofusPathSelector pathSelectorForm = new DofusPathSelector();
                    pathSelectorForm.ShowDialog();
                }
            }

            string path = File.ReadAllText("pathDofus.txt");

            Process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            ScriptCreator = new ScriptCreator(this);
            Process.Start();
            Process.WaitForInputIdle();
            this.Hook();
            Thread.Sleep(4000);
            WindowManager = new WindowManager(PID);
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);


        public void AbsorbApplication(Panel panelContainer)
        {
            IntPtr targetWindowHandle = Process.MainWindowHandle;
            SetParent(targetWindowHandle, panelContainer.Handle);
            MoveWindow(targetWindowHandle, 0, 0, panelContainer.Width, panelContainer.Height, true);
        }

        internal void log(object v1, object v2 = null)
        {
            if(v2 != null)
                Core.Logger.Write(v1 + " " + v2);
            else
                Core.Logger.Write(v1);
        }
    }


}
