using Eon.Services.Launching;
using Eon.Services.Logs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace UiDesktopApp5
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : UiWindow
    {
        // ADDED TO LAUNCHER FOR NOTES - THE HEADLESS DOES NOT WORK THIS WAS A TEMP THING THEN WE USED A C++ HEADLESS EXE

#if DEVTESTING || HEADLESS
        string url = "http://localhost:2451";
        string version = "0.0.3";
        string updaterexe = "EronUpdater.exe";
        string updatersupported = "0.0.1"; // DEV TESTING
        string infoapi = "/launcher/info/dever";

#else
        string url = "http://api.eonfn.com:2451";
        string version = "0.1.92";
        string updaterexe = "EonUpdater.exe";
        string updatersupported = "0.0.6"; // normally the same (changed to fix a issue)
        string infoapi = "/launcher/info"; 
        
#endif
        public Window2()
        {
            InitializeComponent();
            Loggers.Log("Checking Services!");
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
               // await Task.Delay(600);
                string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string DataFolder = System.IO.Path.Combine(BaseFolder, "Eon");
                Directory.CreateDirectory(DataFolder);
                WebClient webClient = new WebClient();
                dynamic Da = null;
                RPC.Init();
                await Task.Run(async () =>
                {
                    string Info = webClient.DownloadString($"{url}{infoapi}");
                    Da = JsonConvert.DeserializeObject<dynamic>(Info);
                });

                // System.Windows.MessageBox.Show(Info);


                // System.Windows.MessageBox.Show(Da.launcherversion.ToString());
                // if(Da.updaterversion == updatersupported)
                // {

                Loggers.Log($"[LAUNCHER] {Da.launcherversion.ToString()}");
                Loggers.Log($"[UPDATER] {Da.updaterversion.ToString()}");

                if (Da.launcherversion.ToString() == version)
                {
                    Loggers.Log("Eon Version Is Correct!");
                    Hide();
                    var login = new Window1();
                    login.Show();
                }
                else
                { 
                    Directory.CreateDirectory(Path.Combine(DataFolder, "Updater"));
                    await webClient.DownloadFileTaskAsync(new Uri(Da.updater.ToString()), Path.Combine(DataFolder, "Updater", updaterexe));

                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = Path.Combine(DataFolder, "Updater", updaterexe),
                        Verb = "runas",
                        UseShellExecute = true
                    };
                    Process.Start(startInfo);
                    Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                Loggers.Log($"[CHECKER] {ex.Message}");
                System.Windows.MessageBox.Show("ERROR PLEASE CHECK LOGS");
                Status.Content = "Eon Services Issue";
                Status.FontSize = 20;
            }
  
        }

        private void UiWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.mutex.ReleaseMutex();
            Application.Current.Shutdown();
        }
    }
}
