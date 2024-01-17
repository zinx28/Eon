using Eon.Services.Logs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Eon.Services.Launching
{
    public class FakeAC
    {
        public static Process _FNLauncherProcess;
        public static Process _FNAntiCheatProcess;

        // Improved a bit from v1! ik shocking
        public static void Start(string Path69, string FileName, string args = "", string t = "r")
        {
            try
            {
                if (File.Exists(Path.Combine(Path69, "FortniteGame\\Binaries\\Win64\\", FileName)))
                {
                    ProcessStartInfo ProcessIG = new ProcessStartInfo()
                    {
                        FileName = Path.Combine(Path69, "FortniteGame\\Binaries\\Win64\\", FileName),
                        Arguments = args,
                        CreateNoWindow = true,
                    };

                    Process YYEAH = Process.Start(ProcessIG);
                    if (YYEAH.Id == 0)
                    {
                        MessageBox.Show("FAILED STARTING!?!?!");
                    }
                    YYEAH.Freeze();

                    if (t == "r")
                    {
                        Loggers.Log("Started FNANTICHEATPROCESS");
                        _FNAntiCheatProcess = YYEAH;
                    }
                    else
                    {
                        Loggers.Log("Started FNLAUNCHERPROCESS");
                        _FNLauncherProcess = YYEAH;
                    }
                }
            }catch (Exception ex)
            {
                MessageBox.Show("PLEASE REPORT THIS " + ex.Message);
            }
        }
    }
}
