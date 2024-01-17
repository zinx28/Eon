using Eon.Services;
using Eon.Services.Logs;
using Eon.Services.Yes;
using System;
using System.Diagnostics;
using System.IO;
using Wpf.Ui.Common.Interfaces;

namespace UiDesktopApp5.Views.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : INavigableView<ViewModels.SettingsViewModel>
    {
        public ViewModels.SettingsViewModel ViewModel
        {
            get;
        }

        public SettingsPage(ViewModels.SettingsViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
            YEAHPASSWORD.Password = SavedData.Token;
            //YeahPathBBG.Content = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Eon");
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Loggers.Log($"Going To Tebex -> Please Pay");
            Process.Start(new ProcessStartInfo("https://eonfn.tebex.io") { UseShellExecute = true });
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            Loggers.Log($"Go to EonFN!");
            Process.Start(new ProcessStartInfo("https://discord.gg/eonfn") { UseShellExecute = true });
        }

        private void Button_Click_2(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Eon"));
        }
    }
}