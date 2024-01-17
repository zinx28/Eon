using Eon.Services;
using Eon.Services.Logs;
using Eon.Services.Yes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using static System.Windows.Forms.AxHost;

namespace UiDesktopApp5
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : UiWindow
    {
#if DEVTESTING || HEADLESS
        string LoginApi = "http://127.0.0.1:2936/launcher/token/";
        string LoginThingy = "http://127.0.0.1:2451";
#else
        string LoginApi = "http://api.eonfn.com:2936/launcher/token/";
        string LoginThingy = "http://api.eonfn.com:2451";
#endif
        public Window1()
        {
            InitializeComponent();
            InitializeChecker();
        }

      
        private async Task InitializeChecker()
        {
            try
            {
                YoLetsGo.Visibility = Visibility.Collapsed;
                YoLetsGo1.Visibility = Visibility.Visible;
                TitleToLoad.Content = "Logging in...";
                string Token = await Task.Run(async () => UpdateINI.ReadValue("Auth", "Token"));
                if(Token != "NONE")
                {
                    WebClient webClient = new WebClient();
                    string Info = null;
                    try
                    {
                        Info = await Task.Run(async () => webClient.DownloadString($"{LoginApi}{Token}"));
                    }
                    catch (Exception ex)
                    {
                        Loggers.Log($"{ex.Message}");
                        ErrorMessage.Content = "Unable to connect to services"; ;
                        System.Windows.MessageBox.Show("Unable to connect to services");
                        YoLetsGo.Visibility = Visibility.Visible;
                        YoLetsGo1.Visibility = Visibility.Collapsed;
                        RealButton.IsEnabled = true;
                        return;
                    }
                    RealButton.IsEnabled = true;
                   
                    if (Info != null)
                    {
                        dynamic Da = JsonConvert.DeserializeObject<dynamic>(Info);
                        System.Windows.MessageBox.Show(Da.ToString());
                        if (Da.email != "")
                        {
                            var app = Application.Current as App;
                            System.Windows.MessageBox.Show(Da.email.ToString());
                            SavedData.Token = Token;
                            SavedData.Email = Da.email;
                            SavedData.Password = Token;
                            await TestThingyLetsFujckinggobbg();
                           
                            await app.StartMain(this);
                        }
                        else
                        {
                            ErrorMessage.Content = "Token Changed?";
                            Loggers.Log($"Detected Token Been Changed");
                        }
                    }
                }
                YoLetsGo.Visibility = Visibility.Visible;
                YoLetsGo1.Visibility = Visibility.Collapsed;
                RealButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                YoLetsGo.Visibility = Visibility.Visible;
                YoLetsGo1.Visibility = Visibility.Collapsed;
                RealButton.IsEnabled = true;
                Loggers.Log($"{ex.Message}");
#if DEVTESTING
                System.Windows.MessageBox.Show(ex.Message.ToString());
#endif

                ErrorMessage.Content = "There Been An Error Please Check Logs";
            }
        }

        private async Task TestThingyLetsFujckinggobbg()
        {
            WebClient webClient = new WebClient();
            string Info = null;
            TitleToLoad.FontSize = 20;
            TitleToLoad.Content = "Fetching Launcher News!";
            try
            {
                await Task.Run(async () =>
                {
                    Info = webClient.DownloadString(LoginThingy + "/launcher/news");
                });

                SavedData.Da = JsonConvert.DeserializeObject<dynamic>(Info);
                int nogr1 = 0;
                foreach (dynamic item in SavedData.Da)
                {
                    nogr1 += 1;
                }

                int nogr = 0;
                foreach (dynamic item in SavedData.Da)
                {
                    nogr += 1;
                    TitleToLoad.Content = $"Fetching Launcher News! ({nogr}/{nogr1})";
                    byte[] imageData = new byte[0];

                    await Task.Run(async () =>
                    {
                        imageData = await webClient.DownloadDataTaskAsync(item.image.ToString());
                    });

                    SavedData.NewsBBG[nogr] = imageData;
                }

            }catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }


            TitleToLoad.Content = "Checking Builds";

            string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string DataFolder = Path.Combine(BaseFolder, "Eon");
            string FilePath = Path.Combine(DataFolder, "builds.json");

            if (File.Exists(FilePath))
            {
                Loggers.Log($"File Path :shocked: {FilePath}");
                try
                {
                    string jsonData = await File.ReadAllTextAsync(FilePath);
                    JArray jsonArray = JArray.Parse(jsonData);
                    foreach (dynamic entry in jsonArray)
                    {
                        try
                        {
                            if (entry["played"] != null)
                            {
                               SavedData.RececdsBuilds.Add(entry);
                            }
                        }
                        catch (Exception ex) {
                            Loggers.Log(ex.Message);
                            System.Windows.MessageBox.Show(ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Loggers.Log(ex.Message);
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TokenBox.Visibility = Visibility.Visible;
                TokenBox.Focus();
                if (!string.IsNullOrEmpty(TokenBox.Password))
                {
                    WebClient webClient = new WebClient();
                    string Info = null;
                    try
                    {
                        YoLetsGo.Visibility = Visibility.Collapsed;
                        YoLetsGo1.Visibility = Visibility.Visible;
                        TitleToLoad.Content = "Logging in...";
                        RealButton.IsEnabled = false;
                        string tokenbbg = TokenBox.Password;
                        Info = await Task.Run(async () => {
                            return await webClient.DownloadStringTaskAsync($"{LoginApi}{tokenbbg}");
                        });
                    }
                    catch (Exception ex)
                    {
                        Loggers.Log($"{ex.Message}");
                        ErrorMessage.Content = "Unable To Connect To Eon!";
                        System.Windows.MessageBox.Show("Unable To Connect To Eon Service! Please Check Logs Or Our Discord!");
                        RealButton.IsEnabled = true;
                        YoLetsGo.Visibility = Visibility.Visible;
                        YoLetsGo1.Visibility = Visibility.Collapsed;
                        return;
                    }
                    RealButton.IsEnabled = true;
                    if (Info != null)
                    {
                        dynamic Da = JsonConvert.DeserializeObject<dynamic>(Info);
                        if (Da.email != "")
                        {
                            if (CheckBoxIg.IsChecked == true)
                            {
                                await UpdateINI.WriteToConfig("Auth", "Token", TokenBox.Password);
                            }
                            var app = Application.Current as App;
                      
                            await TestThingyLetsFujckinggobbg();
                            SavedData.Token = TokenBox.Password;
                            SavedData.Email = Da.email;
                            SavedData.Password = TokenBox.Password;
                            await app.StartMain(this);
                        }
                    }
                    YoLetsGo.Visibility = Visibility.Visible;
                    YoLetsGo1.Visibility = Visibility.Collapsed;
                    ErrorMessage.Content = "Your Token Is Wrong";
                }
                else
                {
                    ErrorMessage.Content = "Please Enter Your Code/Token";
                }  
            }catch (Exception ex)
            {
                Loggers.Log($"{ex.Message}");
                ErrorMessage.Content = "There Been An Error Please Check Logs";
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) { }

        private void TokenBox_PasswordChanged(object sender, RoutedEventArgs e) { }

        private async void ye_Closed(object sender, EventArgs e)
        {
            try
            {
                App.mutex.ReleaseMutex();
                Thread.Sleep(500);
            }
            catch (Exception ex)
            { Loggers.Log($"{ex.Message}"); }
            Application.Current.Shutdown();
        }
    }
}
