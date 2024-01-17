using Eon.Services.Logs;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Eon.Services { 
    public class VersionSorter
    {
        public class BuildInfo
        {
            public string BuildImage { get; set; } = "";
            public string BuildVersion { get; set; } = "";
            public bool BuildSupported { get; set; } = false;
        }

        public static BuildInfo SortOutMyVersion(string decodemystring)
        {
            try
            {
                string MyVersionBBG = Encoding.UTF8.GetString(Convert.FromBase64String(decodemystring));
                var IGSO = new BuildInfo();
                if (MyVersionBBG.Contains("-CL"))
                {
                    int lastIndex = MyVersionBBG.LastIndexOf("-CL");
                    if (lastIndex >= 0)
                    {
                        MyVersionBBG = MyVersionBBG.Substring(0, lastIndex);
                    }
                }
                if (MyVersionBBG.Length >= 19)
                {
                    MyVersionBBG = MyVersionBBG.Remove(0, 19);
                }

                if (MyVersionBBG == "Cert")
                {
                    IGSO.BuildVersion = "Cert";
                    IGSO.BuildImage = "Season1.jpg";
                    IGSO.BuildSupported = false;
                } else if (MyVersionBBG == "Live")
                {
                    IGSO.BuildVersion = "Live";
                    IGSO.BuildImage = "Alpha.png";
                    IGSO.BuildSupported = false;
                }
                else if (MyVersionBBG == "Next") // serason 2
                {
                    IGSO.BuildVersion = "Next";
                    IGSO.BuildImage = "Season1.jpg";
                    IGSO.BuildSupported = false;
                }
                else
                {
                    try
                    {
                        Loggers.Log($"Showing Some Special Stuff to help zinxy");
                        Loggers.Log($"MyBVersionTryingToLookAt!: {MyVersionBBG}");
                        string cleanedString = new string(MyVersionBBG.Where(c => char.IsDigit(c) || c == '.').ToArray());
                        Loggers.Log($"CleanedString: {cleanedString}");
                        string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
                        cleanedString = cleanedString.Replace(",", decimalSeparator);

                        if (float.TryParse(cleanedString, NumberStyles.Float, CultureInfo.InvariantCulture, out float versionFloat))
                        {
                            Loggers.Log($"Yay: {versionFloat}");
                            
                        //    Loggers.Log($"Yay: {versionFloat}");
                            IGSO.BuildImage = $"Season{(int)Math.Floor(float.Parse(MyVersionBBG))}.jpg";
                            IGSO.BuildVersion = versionFloat.ToString();
                            if (versionFloat < 11.00)
                            {
                                Loggers.Log($"OLD SEASON");
                            }
                            else if (versionFloat < 19.00)
                            {
                                //MessageBox.Show(versionFloat.ToString());
                                if (versionFloat >= 12.4 == versionFloat < 12.5)
                                {
                                    Loggers.Log($"Season 12");
                                    IGSO.BuildSupported = true;
                                }else
                                {
                                    if ((int)Math.Floor(versionFloat) == 12) 
                                    {
                                        Loggers.Log($"bro has that wrong version :Skull:");
                                    }
                                    else
                                    {
                                        Loggers.Log($"{versionFloat}");
                                    }
                                      
                                }
                            }
                            else if (versionFloat < 23.00)
                            {
                                Loggers.Log($"{versionFloat}");
                            }
                            else if (versionFloat > 23.00)
                            {
                                Loggers.Log($"{versionFloat}");
                            }
                        }
                        else
                        {
                            Loggers.Log($"Error 1: {versionFloat}");
                            Loggers.Log($"Error 2: {decodemystring}");
                            Loggers.Log($"Error 3: {MyVersionBBG}");
                            MessageBox.Show("Please Check Your Launcher Logs!");
                        }
                    }catch (Exception ex)
                    {
                        Loggers.Log($"Error showing! {ex.Message}");
                        MessageBox.Show("(SHOW THIS TO A HELPER -> THEY WILL SEND THE LOGS FILE TO ZINX!!!!!)");
                    }
                    

                    //  MessageBox.Show(MyVersionBBG);
                   
                }

                return IGSO;
            }
            catch (Exception ex)
            {
                Loggers.Log($"Nof error! {ex.Message}");
                MessageBox.Show("NOf " + ex.Message);
            }

            return null;
        }
    }
}
