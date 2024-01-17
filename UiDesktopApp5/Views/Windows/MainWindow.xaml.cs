using Eon.Services;
using Eon.Services.Launching;
using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using UiDesktopApp5.ViewModels;
using UiDesktopApp5.Views.Pages;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using WindowsAPICodePack.Dialogs;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using System.Diagnostics;
using Eon.Services.Logs;

namespace UiDesktopApp5.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INavigationWindow
    {

  
        public event EventHandler ShowPopupRequested;

        public event EventHandler GOLIBARYPOPUPYEAHBBG;
     
        internal bool OnShowPopupRequested()
        {
            ShowPopupRequested?.Invoke(this, EventArgs.Empty);
            return true;
        }

        internal void YEAHBBG()
        {
            GOLIBARYPOPUPYEAHBBG?.Invoke(this, EventArgs.Empty);
        }

        public ViewModels.MainWindowViewModel ViewModel { get; }

        public MainWindow(ViewModels.MainWindowViewModel viewModel, IPageService pageService, INavigationService navigationService)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
           // RootMainGrid.Visibility = Visibility.Collapsed;
           // SoBadGr.Visibility = Visibility.Collapsed;
            SetPageService(pageService);

            ShowPopupRequested += OpenBuildPath;
            GOLIBARYPOPUPYEAHBBG += GoToLibary;
            navigationService.SetNavigationControl(RootNavigation);
#if DEVTESTING || HEADLESS
            System.Windows.MessageBox.Show("THIS IS A DEV VERSION");
#endif
#if HEADLESS
            System.Windows.MessageBox.Show("HEADLES VERSION!");
#endif
        }



        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog();
            commonOpenFileDialog.IsFolderPicker = true;
            commonOpenFileDialog.Title = "Select A Fortnite Build";
            commonOpenFileDialog.Multiselect = false;
            CommonFileDialogResult commonFileDialogResult = commonOpenFileDialog.ShowDialog();


            bool flag = commonFileDialogResult == CommonFileDialogResult.Ok;
            if (flag)
            {
                if (File.Exists(System.IO.Path.Join(commonOpenFileDialog.FileName, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe")))
                {
                    PATHBUILS.Text = commonOpenFileDialog.FileName;
                    Loggers.Log($"Adding Build Path -> {PATHBUILS.Text}");
                }
                else
                {
                    MessageBox.Show("Please make sure that your the folder contains FortniteGame and Engine In");

                }
            }
        }
        public void OpenBuildPath(object sender, EventArgs e)
        {
            Loggers.Log($"Opening Path Popup!");
            AddBuildPopup.Opacity = 0;
            AddBuildPopup.Visibility = Visibility.Visible;
            var fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.2)
            };

            AddBuildPopup.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        public void GoToLibary(object sender, EventArgs e)
        {
            Loggers.Log($"Going To Libarary");
            Navigate(typeof(Views.Pages.LibraryPage));
            OpenBuildPath(sender, e);
        }

        private void Button_Click(object sender, RoutedEventArgs e) { }
        #region INavigationWindow methods

        public Frame GetFrame()
            => RootFrame;

        public INavigation GetNavigation()
            => RootNavigation;

        public bool Navigate(Type pageType) 
            => 
            RootNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService)
            => RootNavigation.PageService = pageService;

        public void ShowWindow()
            => Show();

        public void CloseWindow()
            => Close();

        #endregion INavigationWindow methods

        /// <summary>
        /// Raises the closed event.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Loggers.Log($"Closing?");

            // Make sure that closing this window will begin the process of closing the application.
            Application.Current.Shutdown();
        }

        private void RootFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e) { }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Loggers.Log($"BUTTON 1!!?");
            string pathBuildsText = PATHBUILS.Text;
            bool ykye = true;
            await Task.Run(async () => {
                Application.Current.Dispatcher.Invoke(() => {
                    if (!string.IsNullOrWhiteSpace(PATHBUILS.Text) && !string.IsNullOrWhiteSpace(NameOFBUILD.Text))
                    {

                        if (File.Exists(System.IO.Path.Join(PATHBUILS.Text, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe")))
                        {
                            YEAHBUTTON.IsEnabled = false;
                            Shocked.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            ykye = false;
                            MessageBox.Show("Please make sure that your the folder contains FortniteGame and Engine In");
                        }

                    }
                    else { 
                        ykye = false;
                        return;
                    }
                    //  MessageBox.Show("YE");
                });
                if (ykye)
                {
                    string result = await VersionSearcher.GetBuildVersion(Path.Combine(pathBuildsText, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe"));
                    
                    if(result == "ERROR")
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ykye = false;
                            MessageBox.Show("There Been A Error!, Please Report This To The Devs... By Telling Us The Build Your Trying To Use");
                        }); 
                    }
                    
                    if (ykye)
                    {
                        await Application.Current.Dispatcher.Invoke(async () =>
                        {
                            await UpdateJSON.WriteToConfig(NameOFBUILD.Text, pathBuildsText, result, Convert.ToBase64String(Encoding.UTF8.GetBytes(result)), "builds.json");
                            Navigate(typeof(Views.Pages.BlankPage));
                            await Task.Delay(100);
                            Navigate(typeof(Views.Pages.LibraryPage));

                            var fadeOutAnimation = new DoubleAnimation
                            {
                                From = 1,
                                To = 0,
                                Duration = TimeSpan.FromSeconds(0.2)
                            };

                            fadeOutAnimation.Completed += (s, ev) =>
                            {
                                AddBuildPopup.Visibility = Visibility.Collapsed;
                                PATHBUILS.Text = "";
                                NameOFBUILD.Text = "";
                            };

                            AddBuildPopup.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

                            YEAHBUTTON.IsEnabled = true;
                            Shocked.Visibility = Visibility.Collapsed;
                        });
                    }
                }
               
            });

        }

        private void AddBuildPopup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Loggers.Log($"Add Build Popup!");
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.2)
            };

            fadeOutAnimation.Completed += (s, ev) =>
            {
                AddBuildPopup.Visibility = Visibility.Collapsed;
                PATHBUILS.Text = "";
                NameOFBUILD.Text = "";
            };

            AddBuildPopup.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

        }

        private void TitleBar_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Loggers.Log($"Title Bar!");
            Point mousePosition = e.GetPosition(this);
            SystemCommands.ShowSystemMenu(this, PointToScreen(mousePosition));
        }

        private void UiWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Loggers.Log($"FORCING FORTNITE TO CLOSE -> LAUNCHER IS CLOSING!");
            App.mutex.ReleaseMutex();
            PSBasics._FortniteProcess?.Kill();

        }
    }
}