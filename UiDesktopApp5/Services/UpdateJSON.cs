using IniParser.Model;
using IniParser;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eon.Services.Logs;
using System.Windows.Forms;

namespace Eon.Services
{
    public class UpdateJSON
    {

        public static async Task WriteToConfig(string buildName, string buildPath, string VersionID, string buildID, string FileName)
        {
            try
            {
                string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string DataFolder = Path.Combine(BaseFolder, "Eon");
                Directory.CreateDirectory(DataFolder);
                string FilePath = Path.Combine(DataFolder, FileName);

                JArray jsonArray;
                if (File.Exists(FilePath))
                {
                    string jsonData = await File.ReadAllTextAsync(FilePath);
                    jsonArray = JArray.Parse(jsonData);
                }
                else
                {
                    jsonArray = new JArray();
                }

                JObject existingEntry = jsonArray.FirstOrDefault(item => item["buildID"].ToString() == buildID) as JObject;

                if (existingEntry != null)
                {
                    existingEntry["buildName"] = buildName;
                    existingEntry["buildPath"] = buildPath;
                    existingEntry["played"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    Loggers.Log($"[buildName]: {buildName}");
                    Loggers.Log($"[buildPath]: {buildPath}");
                }
                else
                {
                    Loggers.Log($"[buildName]: {buildName}");
                    Loggers.Log($"[VersionID]: {VersionID}");
                    Loggers.Log($"[buildPath]: {buildPath}");
                    Loggers.Log($"[buildID]: {buildID}");
                    JObject newEntry = new JObject();
                    newEntry["buildName"] = buildName;
                    newEntry["VersionID"] = VersionID;
                    newEntry["buildPath"] = buildPath;
                    newEntry["buildID"] = buildID;
                    newEntry["played"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                    jsonArray.Add(newEntry);
                }

                await File.WriteAllTextAsync(FilePath, jsonArray.ToString());
            }
            catch(Exception ex)
            {
                
                Loggers.Log($"Json Write Config: {ex.Message}");
                MessageBox.Show("Please Tell A Dev To Fix This error :(");
            }
            
        }

        public async static Task<string> ReadValue(string buildID, string SEARCh, string FileName)
        {
            try
            {
                string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string DataFolder = Path.Combine(BaseFolder, "Eon");
                string FilePath = Path.Combine(DataFolder, FileName);
                Directory.CreateDirectory(DataFolder);
                JArray jsonArray;

                if (File.Exists(FilePath))
                {
                    string jsonData = await File.ReadAllTextAsync(FilePath);
                    jsonArray = JArray.Parse(jsonData);

                    JObject existingEntry = jsonArray.FirstOrDefault(item => item["buildID"].ToString() == buildID) as JObject;

                    if (existingEntry != null)
                    {
                        return existingEntry[SEARCh].ToString();
                    }
                    else
                    {
                        return "NONE";
                    }
                }
                else
                {
                    return "NONE";
                }

            }
            catch (Exception ex)
            {
                Loggers.Log($"[ya]: {ex.Message}");
                return "NONE";
            }
        }
    }
}
