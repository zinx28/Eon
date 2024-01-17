using Eon.Services.Logs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Eon.Services
{
    class VersionSearcher
    {
        private static List<int> Search(byte[] src, byte[] pattern)
        {
            List<int> indices = new List<int>();

            int srcLength = src.Length;
            int patternLength = pattern.Length;
            int maxSearchIndex = srcLength - patternLength;

            for (int i = 0; i <= maxSearchIndex; i++)
            {
                if (src[i] != pattern[0])
                    continue;

                bool found = true;
                for (int j = 1; j < patternLength; j++)
                {
                    if (src[i + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    indices.Add(i);
                }
            }

            return indices;
        }

        public async static Task<string> GetBuildVersion(string exePath)
        {
            try
            {
                Loggers.Log($"Trying To Get Build Version!");
                Application.Current.Dispatcher.Invoke(() => {
                //    MessageBox.Show("YE");
                });
                string result = "";
                int numThreads = Environment.ProcessorCount;
                List<byte>[] binaryDataChunks;

                using (BinaryReader binaryReader = new BinaryReader(new FileStream(exePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    long fileSize = binaryReader.BaseStream.Length;
                    long chunkSize = fileSize / numThreads;

                    binaryDataChunks = new List<byte>[numThreads];
                    Task[] tasks = new Task[numThreads];
                    
                    for (int i = 0; i < numThreads; i++)
                    {
                        int threadIndex = i;
                        long startPosition = i * chunkSize;
                        long endPosition = (i == numThreads - 1) ? fileSize : startPosition + chunkSize;

                        tasks[i] = Task.Run(() =>
                        {
                            using (BinaryReader threadBinaryReader = new BinaryReader(new FileStream(exePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                            {
                                threadBinaryReader.BaseStream.Position = startPosition;

                                List<byte> chunkData = new List<byte>();
                                long remainingBytes = endPosition - startPosition;
                                while (remainingBytes > 0)
                                {
                                    int bytesToRead = (int)Math.Min(remainingBytes, 4096); 
                                    byte[] buffer = threadBinaryReader.ReadBytes(bytesToRead);
                                    chunkData.AddRange(buffer);

                                    remainingBytes -= bytesToRead;
                                }

                                binaryDataChunks[threadIndex] = chunkData;
                            }
                        });
                    }

                    await Task.WhenAll(tasks);
                }

                List<byte> allBinaryData = binaryDataChunks.SelectMany(chunk => chunk).ToList();

                byte[] pattern = Encoding.Unicode.GetBytes("++Fortnite+Release-");
                List<int> list = Search(allBinaryData.ToArray(), pattern);
                string listAsString = string.Join(", ", list);
             //   MessageBox.Show(listAsString);
                if (list.Count != 0)
                {


                    foreach (int num in list)
                    {
                        using (BinaryReader chunkBinaryReader = new BinaryReader(new FileStream(exePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                        {
                            chunkBinaryReader.BaseStream.Position = num;

                            byte[] buffer = new byte[100];
                            int bytesRead = chunkBinaryReader.Read(buffer, 0, buffer.Length);

                            if (bytesRead >= 12)
                            {
                                string chunkText = Encoding.Unicode.GetString(buffer, 0, bytesRead);

                                Match match = Regex.Match(chunkText, "\\+\\+Fortnite\\+Release-((\\d{1,2})\\.(\\d{1,2})|Live|Next|Cert)[-CL]*(\\d*)", RegexOptions.IgnoreCase);
                                if (match.Success)
                                {
                                    string text = string.Format("{0:x}", num);
                                    result = match.Value;
                                   // MessageBox.Show(listAsString);
                                    break; 
                                }
                            }
                        }
                    }

                }

               // MessageBox.Show(result);

                if (result.Contains("-CL"))
                {
                    //result = result.Substring(0, result.LastIndexOf("-CL"));
                }
                //MessageBox.Show(result);
                //result = result.Remove(0, 19);


                return result;
            }
            catch (Exception ex)
            {
                Loggers.Log($"Error 32 - searchy: {ex.Message}");
                MessageBox.Show(ex.Message);
                return "ERROR";
            }

        }
    }
}
