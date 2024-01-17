using Eon.Services.Logs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Eon.Services.Launching
{
    public static class PSBasics
    {
        public static Process _FortniteProcess;
        public static void Start(string PATH, string args, string Email, string Password)
        {
            if (Email == null || Password == null)
            {
                MessageBox.Show("Your Token Was Detected Wrong Try Restarting The Launcher!");
                return;
            }
            if (File.Exists(Path.Combine(PATH, "FortniteGame\\Binaries\\Win64\\", "FortniteClient-Win64-Shipping.exe")))
            {
                PSBasics._FortniteProcess = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        Arguments = args + $"-AUTH_LOGIN={Email} -AUTH_PASSWORD={Password} -AUTH_TYPE=epic ",
                        FileName = Path.Combine(PATH, "FortniteGame\\Binaries\\Win64\\", "FortniteClient-Win64-Shipping.exe")
                    },
                    EnableRaisingEvents = true
                };
                PSBasics._FortniteProcess.Exited += new EventHandler(PSBasics.OnFortniteExit);
                PSBasics._FortniteProcess.Start();
                Loggers.Log("Started FORTNITE!!!!");

            }else
            {
                MessageBox.Show("Please :( no pathy redownload or make sure your harddrive is in!");
            }

        }

        public static void OnFortniteExit(object sender, EventArgs e)
        {
            Process fortniteProcess = PSBasics._FortniteProcess;
            if (fortniteProcess != null && fortniteProcess.HasExited)
            {
                PSBasics._FortniteProcess = (Process)null;
            }
            FakeAC._FNLauncherProcess?.Kill();
            FakeAC._FNAntiCheatProcess?.Kill();
        }
    }
}
