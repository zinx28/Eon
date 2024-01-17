using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System;
using Wpf.Ui.Common.Interfaces;
using UiDesktopApp5.Views.Windows;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Timers;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;
using System.Reflection;
using Eon.Services.Logs;
using Eon.Services;
using System.Linq;
using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using Eon.Services.Launching;
using Eon.Services.Yes;

namespace UiDesktopApp5.Views.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : INavigableView<ViewModels.DashboardViewModel>
    {
        public ViewModels.DashboardViewModel ViewModel
        {
            get;
        }
        private int currentIndex = 0;
        private Dictionary<string, BitmapImage> imageCache = new Dictionary<string, BitmapImage>();
        private DispatcherTimer timer;

        public DashboardPage(ViewModels.DashboardViewModel viewModel)
        {
            ViewModel = viewModel;
        
            InitializeComponent();
            Loggers.Log($"Adding News To Launcher");
            AddNewsToLauncher();
            AddBuilds();
        }

        public async Task AddBuilds()
        {
            try
            {
                //MessageBox.Show("v");
            
                SavedData.RececdsBuilds = SavedData.RececdsBuilds.OrderByDescending(yes => {
                    if (DateTime.TryParseExact(yes["played"].ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime dateTime)) {
                        return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    }
                    else
                    {
                        MessageBox.Show("FAILED TO PARSE");
                        return DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    }
                }).ToList();

                List<JObject> yay = SavedData.RececdsBuilds.Take(5).ToList();

                if(yay.Count != 0)
                {
                    itemsWrapPanel.Children.Clear();
                }

                foreach (dynamic Yeah in yay)
                {
                    //MessageBox.Show(Yeah.ToString());
                    dynamic Shocked = VersionSorter.SortOutMyVersion(Yeah.buildID.ToString());

                    if (Shocked == null)
                    {
                        Loggers.Log($"No Builds??? No Longer Shocked");
                    }
                    else
                    {
                        Loggers.Log($"Yeah! {Yeah.buildName}");
 
                        CreateItem(Yeah.buildName.ToString(), Shocked.BuildSupported, Shocked.BuildImage, Yeah.buildPath.ToString());
                    }
                   
                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async Task AddNewsToLauncher()
        {
            try
            {
                int nogr = 0;
                foreach (dynamic item in SavedData.Da)
                {
                    nogr += 1;
                    Loggers.Log($"News -> {item.title}");
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new MemoryStream(SavedData.NewsBBG[nogr]);
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    if (bitmapImage != null)
                    {
                        imageCache[item.image.ToString()] = bitmapImage;
                    }
                    Title.Content = item.title;
                    Body.Text = item.body;
                    SolidColorBrush backgroundBrus1h = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF171717"));
                    ChangeImage.ImageSource = bitmapImage;
                    AddNewsItemToWrapPanel(item.title.ToString(), backgroundBrus1h);
                }
                dynamic newsItem = SavedData.Da[0];
                Border border = AddNewsNofii.Children[0] as Border;
                Body.Text = newsItem.body;
                Title.Content = newsItem.title;
                SolidColorBrush backgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00A3FF"));
                border.Background = backgroundBrush;
                border.Width = 60;
                border.Opacity = 1;
                border.CornerRadius = new CornerRadius(4);
                string imageUrl = newsItem.image.ToString();
                if (imageCache.TryGetValue(imageUrl, out BitmapImage bitmapImage1))
                {
                    ChangeImage.ImageSource = bitmapImage1;
                }
                Loggers.Log($"News Timer Is now moving");
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(5);
                timer.Tick += Timer_Tick;
                timer.Start();
            }
            catch (Exception ex)
            {
                Loggers.Log($"NEWS: {ex.Message}");
                MessageBox.Show(ex.Message);
                Title.Content = "Error";
                Body.Text = "Failed To Grab News";
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            currentIndex = (currentIndex + 1) % SavedData.Da.Count;
            for (int i = 0; i < SavedData.Da.Count; i++)
            {   
                dynamic newsItem = SavedData.Da[i];
                Border border = AddNewsNofii.Children[i] as Border;

                if (i == currentIndex)
                {
                    Body.Text = newsItem.body;
                    Title.Content = newsItem.title;
                    SolidColorBrush backgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00A3FF"));
                    border.Background = backgroundBrush;
                    border.Width = 60;
                    border.Opacity = 1;
                    border.CornerRadius = new CornerRadius(4);
                    string imageUrl = newsItem.image.ToString();
                    if (imageCache.TryGetValue(imageUrl, out BitmapImage bitmapImage))
                    {
                        ChangeImage.ImageSource = bitmapImage;
                        Loggers.Log($"News: Updated");
                    }  
                }
                else
                {
                    border.Background = Brushes.Black;
                    border.Height = 11;
                    border.Width = 11;
                    border.Opacity = 0.45;
                }
            }
        }
        private void AddNewsItemToWrapPanel(string title, Brush background)
        {
            Border border = new Border();
            border.Margin = new Thickness(8);
            border.CornerRadius = new CornerRadius(5);
            border.Background = background;
            border.Height = 11;
            border.Width = 11;
            border.Opacity = 0.45;
            TextBlock titleText = new TextBlock();
            titleText.Foreground = Brushes.White;
            border.Tag = title;

            AddNewsNofii.Children.Add(border);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Loggers.Log($"Texbex xxxxxxxx");
           // Process.Start(new ProcessStartInfo("https://eonfn.tebex.io") { UseShellExecute = true });
        }

        private Border CreateItem(string buildName, bool buildSupported, string BuildImage, string PathOfVersion)
        {
            Border itemBorder = new Border
            {
                CornerRadius = new CornerRadius(10),
                Height = 228,
                Background = new SolidColorBrush(Color.FromArgb(28, 217, 217, 217)),
                Margin = new Thickness(5)
            };

            Grid grid = new Grid
            {
                Width = 173
            };

            Border imageBackgroundBorder = new Border
            {
                Margin = new Thickness(10, 10, 10, 77),
                CornerRadius = new CornerRadius(10),
                Height = 141
            };
            try
            {
                imageBackgroundBorder.Background = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/Assets/{BuildImage}")))
                {
                    Stretch = Stretch.UniformToFill
                };
            }
            catch (Exception ex)
            {
                imageBackgroundBorder.Background = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/Assets/unknown.jpg")))
                {
                    Stretch = Stretch.UniformToFill
                };
            }

            if (buildSupported)
            {
                Border labelBackgroundBorder = new Border
                {
                    Margin = new Thickness(10, 10, 92, 114),
                    CornerRadius = new CornerRadius(10),
                    Background = new SolidColorBrush(Color.FromArgb(176, 35, 36, 40))
                };
                Label label = new Label
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 10,
                    Content = buildSupported ? "OFFICIAL" : "UNOFFICIAL",
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 255, 25))
                };
                labelBackgroundBorder.Child = label;

                imageBackgroundBorder.Child = labelBackgroundBorder;
            }

            Label label1 = new Label
            {
                Content = buildName,
                Margin = new Thickness(10, 156, 71, 54),
                FontWeight = FontWeights.Bold
            };

            Label label2 = new Label
            {
                Content = buildSupported ? "supported *" : "unsupported *",
                Margin = new Thickness(10, 171, 94, 38),
                Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 255, 25))
            };

            Button launchButton = new Button
            {
                Content = "Launch",
                Margin = new Thickness(0, 194, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 148,
                Height = 27,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Loggers.Log($"Tryomg Tp Add Buidls");
            launchButton.Click += (sender, e) =>
            {
                Loggers.Log($"Making {buildName} a working button");
                LaunchButton_Click(sender, e, buildName, PathOfVersion);
            };
            grid.Children.Add(imageBackgroundBorder);

            grid.Children.Add(label1);
            grid.Children.Add(label2);
            grid.Children.Add(launchButton);
            itemBorder.Child = grid;
            itemsWrapPanel.Children.Add(itemBorder);
            return itemBorder;
        }

        async void LaunchButton_Click(object sender, RoutedEventArgs e, string buildName, string PathOfVersion)
        {
            try
            {
                Loggers.Log($"Launch Button Detected!");
                if (PSBasics._FortniteProcess == null)
                {
                    if (File.Exists(System.IO.Path.Join(PathOfVersion, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe")))
                    {
                        await Task.Run(async () =>
                        {
                            Loggers.Log($"Launching Eron");
                            PSBasics.Start(PathOfVersion, "-NOSSLPINNING -EpicPortal -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -nobe -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck -steamimportavailable -HTTP=WinINet", SavedData.Email, SavedData.Password);
                            FakeAC.Start(PathOfVersion, "FortniteClient-Win64-Shipping_BE.exe", $"", "r");
                            FakeAC.Start(PathOfVersion, "FortniteLauncher.exe", $"", "dsf");
                            RPC.Update($"Playing {buildName}");
                            PSBasics._FortniteProcess.WaitForInputIdle();
#if DEVTESTING
                            //AppDomain.CurrentDomain.BaseDirectory
                            string BaseFolder = AppDomain.CurrentDomain.BaseDirectory;
                            string DataFolder = Path.Combine(BaseFolder);
#else
                            string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                            string DataFolder = Path.Combine(BaseFolder, "Eon");
#endif

                            Loggers.Log($"Injecting Game");
                            Inject.InjectDll(PSBasics._FortniteProcess.Id, Path.Combine(DataFolder, "Eron.dll"));
                            PSBasics._FortniteProcess.WaitForExit();
                            PSBasics._FortniteProcess = null;
                            RPC.Update($"In Launcher");
                        });
                    }
                    else
                    {
                        MessageBox.Show("Damn This Path Isnt A Thing Anymore :(");
                        Loggers.Log($"Path Not Found (Deleted Or Move)");
                    }
                }
                else
                {
                    MessageBox.Show("Detected Game Open (TODO)");
                    Loggers.Log($"Detected Game Open (TODO)");
                }

            }
            catch (Exception ex)
            {
                
                Loggers.Log($"No! {ex.Message}");
                MessageBox.Show("ERROR TRYING TO LAUNCH! - check logs");
                MessageBox.Show(ex.Message);
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Loggers.Log($"join eon today ");
            Process.Start(new ProcessStartInfo("https://www.discord.com/invite/eonfn") { UseShellExecute = true });
        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Loggers.Log($"Click clap");
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.YEAHBBG();

        }
    }
}