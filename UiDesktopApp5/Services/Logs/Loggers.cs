using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Eon.Services.Logs
{
    public class Loggers
    {
        private static string appDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Eon", "launcherlog.txt");

        public static void InitializeLogger()
        {
            InitializeLogFile();
            Log("Initializing");
        }

        public static void Log(string message)
        {
            try
            {

                using (StreamWriter writer = File.AppendText(appDataFolderPath))
                {
                    writer.WriteLine($"Eon->{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        static void InitializeLogFile()
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Eon"));
                if (!File.Exists(appDataFolderPath))
                {
                    File.Create(appDataFolderPath).Close();
                }
                else
                {
                    File.WriteAllText(appDataFolderPath, "");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(":( Launcher Logs File Failed To Initialize!");
            }
        }
    }
}
