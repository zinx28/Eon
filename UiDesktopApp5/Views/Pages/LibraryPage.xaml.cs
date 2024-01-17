using Eon.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using UiDesktopApp5.Views.Windows;
using Wpf.Ui.Common.Interfaces;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using Eon.Services.Launching;
using Eon.Services.Logs;
using Eon.Services.Yes;

namespace UiDesktopApp5.Views.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class LibraryPage : INavigableView<ViewModels.LibaryViewPageModel>
    {
        public ViewModels.LibaryViewPageModel ViewModel
        {
            get;
        }
        dynamic Da;
        public LibraryPage(ViewModels.LibaryViewPageModel viewModel)
        {
            ViewModel = viewModel;
            
            InitializeComponent();
           // InitializeBuilds();

            Loaded += LibraryPage_Loaded;
         //   MainWindow.LegitAddLatestPath += HandleLegitAddLatestPathEvent;
        }
        private void LibraryPage_Loaded(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("sd");
            Loggers.Log($"Library was just loaded");
            itemsWrapPanel.Children.Clear();
            Border newBorder = new Border
            {
                Margin = new Thickness(10),
                CornerRadius = new CornerRadius(10),
                Height = 228
            };
            newBorder.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/RandomBorder.png")),
                Stretch = Stretch.UniformToFill
            };
            Grid grid = new Grid
            {
                Width = 173
            };
            newBorder.Child = grid;
            newBorder.MouseDown += (sender, e) =>
            {
                Border_MouseDown(sender, e);
            };
            itemsWrapPanel.Children.Add(newBorder);
            Loggers.Log($"Builds Are Now Loading");
            InitializeBuilds();
        }

        private async void InitializeBuilds()
        {
            string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string DataFolder = Path.Combine(BaseFolder, "Eon");
            string FilePath = Path.Combine(DataFolder, "builds.json");
            
         //   MessageBox.Show(FilePath);
            if (File.Exists(FilePath))
            {
                Loggers.Log($"File Path :shocked: {FilePath}");
                try
                {
                    string jsonData = await File.ReadAllTextAsync(FilePath);
                    JArray jsonArray = JArray.Parse(jsonData);
                    foreach (JObject entry in jsonArray)
                    {
                        try
                        {
                            dynamic Shocked = VersionSorter.SortOutMyVersion(entry["buildID"].ToString());
                         
                            if(Shocked == null)
                            {
                                Loggers.Log($"No Builds??? No Longer Shocked");
                            }
                            else
                            {
                                Loggers.Log($"Yeah! {entry["buildName"]}");
                                AddItemsToWrapPanel(entry["buildName"].ToString(), Shocked.BuildSupported, Shocked.BuildImage, entry["buildPath"].ToString());
                            }
                        }catch (Exception ex)
                        {
                            MessageBox.Show("fd "  + ex.Message);
                        }
                        //MessageBox.Show(entry["buildName"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    Loggers.Log($"BUILD ERROR: {ex.Message}");
                    MessageBox.Show("GS "+ ex.Message);
                }

            }else
            {
                Loggers.Log($"Failed To Find File");
            }
        }


        public void AddItemsToWrapPanel(string BuildName, bool Supported, string BuildImage, string PathOfVersion)
        {
            try
            {
                Border item = CreateItem(BuildName, Supported, BuildImage, PathOfVersion);
                if (item != null)
                {
                    itemsWrapPanel.Children.Add(item);
                }
                else
                {
                    Loggers.Log($"Couldn't Create Item (Check builds.json in appdata)");
                }
               
            }catch (Exception ex)
            {
                Loggers.Log($"ADDING: {ex.Message}");
                MessageBox.Show("Error2 " + ex.Message);
            }
        }

        private Border CreateItem(string BuildName, bool Supported, string BuildImage, string PathOfVersion)
        {

            try
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
                catch (Exception ex) {
                    imageBackgroundBorder.Background = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/Assets/unknown.jpg")))
                    {
                        Stretch = Stretch.UniformToFill
                    };
                }
             
                //new Thickness(Supported ? 10 : 15);
                if (Supported)
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
                        Content = Supported ? "OFFICIAL" : "UNOFFICIAL",
                        Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 255, 25))
                    };
                    labelBackgroundBorder.Child = label;

                    imageBackgroundBorder.Child = labelBackgroundBorder;
                }


                Label label1 = new Label
                {
                    Content = BuildName,
                    Margin = new Thickness(10, 156, 35, 54),
                    FontWeight = FontWeights.Bold
                };

                Label label2 = new Label
                {
                    Content = Supported ? "supported *" : "unsupported *",
                    Margin = new Thickness(10, 171, 85, 38),
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
                launchButton.Click += (sender, e) =>
                {
                    Loggers.Log($"Making {BuildName} a working button");
                    LaunchButton_Click(sender, e, BuildName, PathOfVersion);
                };


                Loggers.Log($"Adding All The Children!");
                grid.Children.Add(imageBackgroundBorder);

                grid.Children.Add(label1);
                grid.Children.Add(label2);
                grid.Children.Add(launchButton);
                itemBorder.Child = grid;
                return itemBorder;
            }catch (Exception ex)
            {
                Loggers.Log($"No!! {ex.Message}");
                MessageBox.Show("ER" + ex.Message);
            }

            return null;
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
#if !HEADLESS
                            PSBasics.Start(PathOfVersion, "-EpicPortal -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -nobe -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck -steamimportavailable -HTTP=WinINet", SavedData.Email, SavedData.Password);
#else
                            PSBasics.Start(PathOfVersion, "-EpicPortal -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -nobe -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck -steamimportavailable -nosplash -nosound -nullrhi -HTTP=WinINet", SavedData.Email, SavedData.Password);
#endif
                            FakeAC.Start(PathOfVersion, "FortniteClient-Win64-Shipping_BE.exe", $"", "r");
                            FakeAC.Start(PathOfVersion, "FortniteLauncher.exe", $"", "dsf");
                            RPC.Update($"Playing {buildName}");
                            PSBasics._FortniteProcess.WaitForInputIdle();
#if DEVTESTING || HEADLESS
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
                MessageBox.Show("ERROR TRYING TO LAUNCH!");
                Loggers.Log($"No! {ex.Message}");
                MessageBox.Show(ex.Message);
            }

        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           // MessageBox.Show("YEEPIE I DID IT");

            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.OnShowPopupRequested();
        }
    }
}