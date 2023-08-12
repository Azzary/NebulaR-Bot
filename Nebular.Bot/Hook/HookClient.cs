using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Nebular.Bot.Hook
{

    class HookClient
    {
        Dofus Client { get; set; }

        public HookClient(Dofus Client) 
        {
            this.Client = Client;
            Task.Run(async () =>
            {
                for (int i = 0; i < 20; i++)
                {
                    await Task.Delay(400);
                    Hook();
                }
            });
        }
        private List<int> HookedPID { get; set; } = new List<int>();
        private void Hook()
        {
            int uid = new Random().Next(9745612);
            foreach (var childProcess in Client.GetAllPid())
            {
                try
                {
                    if (!HookedPID.Contains(childProcess.Id) && childProcess.ProcessName.Contains("Dofus"))
                    {
                        HookedPID.Add(childProcess.Id);
                        Console.WriteLine(childProcess.Id + "is in: " + HookedPID.Contains(childProcess.Id));
                        Thread hookThread = new Thread(() => RunHook(childProcess.Id, uid));
                        hookThread.Start();
                    }
                }
                catch(InvalidOperationException )
                {
                }
            }
        }

        private void RunHook(int childPid, int uid)
        {
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "hook.exe");
                string info = $"";
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = $"{childPid} {(int)Client.ClientType}:{uid} {ProxyHook.port}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = false
                };

                Process pythonProcess = Process.Start(startInfo);
                pythonProcess.WaitForExit();
            }
            catch (Exception)
            {
            }
        }

    }


    public static class ProcessExtensions
    {
        public static IEnumerable<Process> GetChildProcesses(this Process process)
        {
            List<Process> children = new List<Process>();
            ManagementObjectSearcher mos = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", process.Id));

            foreach (ManagementObject mo in mos.Get())
            {
                try
                {
                    children.Add(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));
                }
                catch { }
            }

            return children;
        }
    }

}
