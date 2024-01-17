using Eon.Services.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using UiDesktopApp5.Models;
using UiDesktopApp5.Services;
using UiDesktopApp5.Views.Windows;
using Windows.Media.Protection.PlayReady;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace UiDesktopApp5
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        const int SW_RESTORE = 9;
#if DEVTESTING || HEADLESS
        public static Mutex mutex = new Mutex(true, Guid.NewGuid().ToString().Replace("-", ""));
#else
       public static Mutex mutex = new Mutex(true, "{EE07F053-22E7-4E7A-9F39-AB8A1B89A223}");
#endif



        private async void OnStartup(object sender, StartupEventArgs e)
        {
            Loggers.InitializeLogger();
            // Create and start the host
           // MessageBox.Show("Hello, Eon Has Shut Down! Clicking OK will take you to the hydro discord server \nWe will be hosting Chapter 2 season 3");
         //   Process.Start(new ProcessStartInfo("https://discord.gg/eQ8YgtNQm5") { UseShellExecute = true });

            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                var login = new Window2();
                login.Show();

            }
            else
            {
                Process currentProcess = Process.GetCurrentProcess();
                Process[] processes = Process.GetProcessesByName(currentProcess.ProcessName);
                foreach (Process process in processes)
                {
                    if (process.Id != currentProcess.Id)
                    {
                        ShowWindow(process.MainWindowHandle, SW_RESTORE);
                        SetForegroundWindow(process.MainWindowHandle);
                        Application.Current.Shutdown();
                        return;
                    }
                }
                Application.Current.Shutdown();
            }
           // mutex.ReleaseMutex();
            // Ensure that the application will shut down gracefully when the main window is closed
            //host.WaitForShutdown();
        }

        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private IHostBuilder CreateHostBuilder(string[] args)
        {

            var defaultBasePath = AppDomain.CurrentDomain.BaseDirectory;

            return Host.CreateDefaultBuilder().ConfigureAppConfiguration(c => {
                var entryAssemblyLocation = Assembly.GetEntryAssembly()?.Location;
                var basePath = !string.IsNullOrEmpty(entryAssemblyLocation)
                    ? Path.GetDirectoryName(entryAssemblyLocation)
                    : defaultBasePath;

                c.SetBasePath(basePath);
            })
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<ApplicationHostService>();

                    // Page resolver service
                    services.AddSingleton<IPageService, PageService>();

                    // Theme manipulation
                    services.AddSingleton<IThemeService, ThemeService>();

                    // TaskBar manipulation
                    services.AddSingleton<ITaskBarService, TaskBarService>();

                    // Service containing navigation, same as INavigationWindow... but without window
                    services.AddSingleton<INavigationService, NavigationService>();

                    // Main window with navigation
                    services.AddScoped<INavigationWindow, Views.Windows.MainWindow>();
                    services.AddScoped<ViewModels.MainWindowViewModel>();

                    // Views and
                    // BANK PAGE
                    services.AddScoped<Views.Pages.BlankPage>();
                    services.AddScoped<Views.Pages.DashboardPage>();
                    services.AddScoped<ViewModels.DashboardViewModel>();
                    services.AddScoped<Views.Pages.LibraryPage>();
                    services.AddScoped<ViewModels.LibaryViewPageModel>();
                    services.AddScoped<Views.Pages.SettingsPage>();
                    services.AddScoped<ViewModels.SettingsViewModel>();

                    // Configuration
                    services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
                });
        }

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>


        public async Task StartMain(Window1 Shcoedk)
        {
            try
            {
                var host = CreateHostBuilder(new string[0]).Build();
                await host.StartAsync();
                Shcoedk.Hide();
             //   MessageBox.Show("SHCOEKD");
                //  await _host.StartAsync();
            }
            catch(Exception ex)
            {
                Loggers.Log($"App -> {ex.Message}");
                MessageBox.Show(ex.Message);
            }
            finally
            {
          
              //  (Application.Current.MainWindow as MainWindow)?.Dispatcher.Invoke(() =>
                //{
                //    (Application.Current.MainWindow as MainWindow)?.Close();
               // });
            }
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
           // await _host.StopAsync();

           // _host.Dispose();
          // _host?.StopAsync().GetAwaiter().GetResult();
           // Application.Current.Shutdown();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }
    }
}