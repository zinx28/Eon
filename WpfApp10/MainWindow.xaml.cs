using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Controls;
using System.Runtime.InteropServices;

namespace WpfApp10
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UiWindow
    {
        string url = "http://127.0.0.1:2451";
        string version = "0.0.7";
        public MainWindow()
        {
            InitializeComponent();
            THINGG();
        }

   
        public async Task THINGG()
        {
            try
            {
                await Task.Run(async () => await CloseAllInfectedProcesses("Eon"));
                Status.Content = "Checking Smth Ig";
                var yeepie = WindowsIdentity.GetCurrent();
                var ya = new WindowsPrincipal(yeepie);
                if (ya.IsInRole(WindowsBuiltInRole.Administrator))
                {
                  //  await Task.Run(async () => await CloseAllInfectedProcesses("UiDesktopApp5"));
                     // idfk
                    Status.Content = "Checking Files";
                    string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string DataFolder = System.IO.Path.Combine(BaseFolder, "Eon");
                    Directory.CreateDirectory(DataFolder);

                    Task.Delay(500);

                    WebClient webClient = new WebClient();
                    string Info = webClient.DownloadString($"{url}/launcher/info");
                    dynamic Da = JsonConvert.DeserializeObject<dynamic>(Info);
                    string LauncherInfo = System.IO.Path.Combine(DataFolder, "Data.json");
                    /*
                     {

                     }

                     */
                    if (Da.updaterversion == version)
                    {
                        //if (File.Exists(LauncherInfo))
                       // {
                         //   Status.Content = "You Basically Got Latest Launcher, Man idk what im do ing";
                        //}
                       // else
                        //{
                            Status.Content = "Checking Files";
                            string[] yaNO = new string[]
                            {
                                "/Q"
                            };
                        
                            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DevDiv\VC\Servicing\14.0\RuntimeMinimum", false))
                            {
                                
                                if (key != null)
                                {
                                    var PleaseHelpy = key.GetValue("Version");
                                    //System.Windows.MessageBox.Show($"{((string)PleaseHelpy).StartsWith("14")}");
                                    if (!((string)PleaseHelpy).StartsWith("14"))
                                    {
                                        await Task.Run(async () => await DownloadAndRun("https://aka.ms/vs/17/release/vc_redist.x64.exe", Path.Combine(DataFolder, "vc_redist.x64.exe"), "VC Redist", yaNO));
                                        await Task.Run(async () => File.Delete(Path.Combine(DataFolder, "vc_redist.x64.exe")));
                                    }
                                }else
                                {
                                 
                                    await Task.Run(async () => await DownloadAndRun("https://aka.ms/vs/17/release/vc_redist.x64.exe", Path.Combine(DataFolder, "vc_redist.x64.exe"), "VC Redist", yaNO));
                                    await Task.Run(async () => File.Delete(Path.Combine(DataFolder, "vc_redist.x64.exe")));
                                }
                            }

                            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DirectX", false))
                            {
                                if (key == null)
                                {
                                    await Task.Run(async () => await DownloadAndRun($"{url}/files/dxwebsetup.exe", Path.Combine(DataFolder, "dxwebsetup.exe"), "DirectX", yaNO));
                                    await Task.Run(async () => File.Delete(Path.Combine(DataFolder, "dxwebsetup.exe")));
                                }
                            }
                      
                            Status.Content = "Downloading Launcher";

                        try
                        {
                            await Task.Run(async () => await DownloadAndRun($"{Da.launcher}", Path.Combine(DataFolder, "Eon.zip"), "Eon Launcher", yaNO, false));
                        }catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show("ERROR DOWNLOADING LAUNCHER " + ex.Message);
                            System.Windows.MessageBox.Show("Your Antiviruses might effect the launcher\nThats why we and other server recommend you to use Windows Defender");
                        }
                            
                          //  System.Windows.MessageBox.Show("Ysa");
                            await Task.Run(async () =>
                                {
                                    string runtimesDirectory = Path.Combine(DataFolder, "runtimes");
                                  //  Directory.CreateDirectory(runtimesDirectory);
                                    using (ZipArchive archive = ZipFile.OpenRead(Path.Combine(DataFolder, "Eon.zip")))
                                    {
                                        foreach(ZipArchiveEntry entry in archive.Entries)
                                        {
                                            string filePath = Path.Combine(DataFolder, entry.FullName);
                                            filePath = Path.GetFullPath(filePath);

                                            // Ensure the directory for the file exists
                                            string directoryPath = Path.GetDirectoryName(filePath);
                                            Directory.CreateDirectory(directoryPath);

                                           // System.Windows.MessageBox.Show(entry.FullName);

                                            if (!entry.FullName.EndsWith("/")) 
                                            {
                                          
                                                if (File.Exists(filePath))
                                                {
                                                    File.Delete(filePath);
                                                }

                                                entry.ExtractToFile(filePath);
                                            }
                                            else
                                            {
                                                Directory.CreateDirectory(filePath);
                                            }
                                        }
                                    }

                            });
                        
                            Status.Content = "Cleaning Up...";

                  
                         
                            await Task.Run(async () => File.Delete(Path.Combine(DataFolder, "Eon.zip")));
 
                            Status.Content = "Please Wait....";
                            await Task.Run(() =>
                            {
                                string DesktopFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                                string userProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                                string StatupProgams = Path.Combine(userProfileFolder, "AppData", "Roaming", "Microsoft", "Windows", "Start Menu", "Programs");

                                try
                                {
                                    string StatupProgams1 = Path.Combine(StatupProgams, "Eon", "Eon" + ".lnk");
                                    Directory.CreateDirectory(Path.Combine(StatupProgams, "Eon"));
                                    string DesktopFolderPath1 = Path.Combine(DesktopFolderPath, "Eon" + ".lnk");

                                    if (!File.Exists(StatupProgams1))
                                    {
                                        var shellType = Type.GetTypeFromProgID("WScript.Shell");
                                        dynamic shell = Activator.CreateInstance(shellType);
                                        string escapedFolderPath = Uri.EscapeDataString(DesktopFolderPath);


                                        var shortcut = shell.CreateShortcut(StatupProgams1);
                                        shortcut.TargetPath = Path.Combine(DataFolder, "Eon.exe");
                                        shortcut.WorkingDirectory = escapedFolderPath;
                                        shortcut.Save();
                                    }


                                    if (!File.Exists(DesktopFolderPath1))
                                    {
                                        var shellType = Type.GetTypeFromProgID("WScript.Shell");
                                        dynamic shell = Activator.CreateInstance(shellType);
                                        string escapedFolderPath = Uri.EscapeDataString(DesktopFolderPath1);


                                        var shortcut = shell.CreateShortcut(DesktopFolderPath1);
                                        shortcut.TargetPath = Path.Combine(DataFolder, "Eon.exe");
                                        shortcut.WorkingDirectory = escapedFolderPath;
                                        shortcut.Save();
                                    }

                                   
                                    try
                                    {
                                        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall", true))
                                        {
                                            if (key != null)
                                            {
                                                string PATHGOGMMGO = Path.Combine(DataFolder, "Eon.exe");
                                                using (RegistryKey appKey = key.CreateSubKey("Eon"))
                                                {
                                                    appKey.SetValue("DisplayName", "Eon");
                                                    appKey.SetValue("DisplayIcon", PATHGOGMMGO);
                                                    appKey.SetValue("DisplayVersion", Da.launcherversion);
                                                    appKey.SetValue("Publisher", "Eon");
                                                    appKey.SetValue("InstallLocation", PATHGOGMMGO);
                                                    appKey.SetValue("UninstallString", PATHGOGMMGO); 
                                                }
                                            }
                                        }
                                    }catch (Exception ex) {

                                    }
                                    
                                }
                                catch (Exception ex)
                                {
                                    System.Windows.MessageBox.Show("ERROR SAVING SHORTCUTS! " + ex.Message);
                                }
                              //  string shortcutPath = Path.Combine(startMenuFolderPath, "Programs", "Eon" + ".lnk");
                               
                            });


                            Process process = new Process();
                            process.StartInfo.FileName = Path.Combine(DataFolder, "Eon.exe");
                            process.Start();
                            Application.Current.Shutdown();
                            //https://aka.ms/vs/17/release/vc_redist.x64.exe
                            //dxwebsetup.exe - custom api
                            //download launcher - basically bc people would use this to launch ig
                       // }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Updater Out Of Date.. Please Download The Latest Download On Our Discord");
                        Application.Current.Shutdown();
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Please Run Launcher As Administrator");
                    Application.Current.Shutdown();
                }

            }
            catch (WebException ex) 
            {
                System.Windows.MessageBox.Show(ex.Status.ToString());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                //  Status.Content = ex.Message;
                Status.Content = "Unable To Connect To Eon Services";
                Status.FontSize = 20;

            }
        }

        private static string GetRegistryValue(string keyName, string valueName)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName))
            {
                if (key != null)
                {
                    return key.GetValue(valueName)?.ToString();
                }
            }
            return null;
        }

        public async Task CloseAllInfectedProcesses(string ProcessName)
        {
            Process[] process = Process.GetProcessesByName(ProcessName);

            if(process.Length> 0)
            {
                foreach(Process process1 in process)
                {
                    if (!process1.CloseMainWindow()){
                        process1.Kill(); 
                    }else
                    {
                      //  System.Windows.MessageBox.Show("b");
                    }
                }
            }else
            {
               // System.Windows.MessageBox.Show("EO");
            }
        }

        public async Task DownloadAndRun(string FileDownload, string WhereToDownload, string FileName1, string[] args, bool RunFile = true)//string[] args
        {
            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(5);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Status.Content = $"Downloading... {FileName1}";
                });
                using (client)
                {
                    string FileName = Path.GetFileName(new Uri(FileDownload).LocalPath);

                    HttpResponseMessage response = await client.GetAsync(FileDownload);

                    if (response.IsSuccessStatusCode)
                    {
                        using (HttpContent content = response.Content)
                        {
                            byte[] fileByte = await content.ReadAsByteArrayAsync();

                            File.WriteAllBytes(WhereToDownload, fileByte);

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Status.Content = $"Downloaded {FileName1}!";
                            });
                        }
                        if (RunFile)
                        {
                            Process Yafds = new Process();
                            Yafds.StartInfo.Arguments = string.Join(" ", args);
                            Yafds.StartInfo.FileName = WhereToDownload;
                            Yafds.StartInfo.UseShellExecute = true;
                            Yafds.Start();
                            Yafds.WaitForExit();
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Status.Content = $"{FileName1} Installed";
                            });
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Status.Content = "Man TURN YOUR FLIPING INTERNET BACK ON";
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Your Antiviruses might effect the launcher\nThats why we and other server recommend you to use Windows Defender");
            }
        }
    }
}
