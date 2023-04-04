using Handheld_Control_Panel.Classes.Run_CLI;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Handheld_Control_Panel.Classes
{
    public static class Steam_Management
    {


        public static Dictionary<string, string> syncSteam_Library()
        {
            Dictionary<string, string> result = new Dictionary<string,  string>();

            if (Directory.Exists(Properties.Settings.Default.directorySteam))
            {
                string[] lines = System.IO.File.ReadAllLines(Properties.Settings.Default.directorySteam + "\\steamapps\\libraryfolders.vdf");
                List<string> paths = new List<string>();
                bool foundApps = false;



                foreach (string line in lines)
                {
                    if (line.Contains("path")) 
                    { 
                        paths.Add(@line.Replace("path", "").Replace("\"","").Replace("\t",""));
                      
                    }
                    if (foundApps && line.Contains("}")) { foundApps = false; }
                    if (foundApps && !line.Contains("{"))
                    {
                        int firstQuote = line.IndexOf('"');
                        int secondQuote = line.IndexOf('"', firstQuote + 1);
                        string gameName = line.Substring(firstQuote + 1, secondQuote - firstQuote - 1);
                        if (gameName != "228980")
                        {
                            result.Add(gameName, "");
                        }
                      
                        Debug.WriteLine(line.Substring(firstQuote + 1, secondQuote - firstQuote - 1));
                    }
                    if (line.Contains("apps")) { foundApps = true; }


                
                }


                foreach (string path in paths)
                {
                    foreach (KeyValuePair<string,string> entry in result)
                    {
                        string gameName = getGameName(entry.Key, path.Replace("\\\\","\\"));
                        if (gameName != "") { result[entry.Key] = gameName; }
                    }
                }
            }

            return result;

        }

        public static string NormalizePath(string path)
        {
            return System.IO.Path.GetFullPath(new Uri(path).LocalPath)
                       .TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant().Trim();
        }
        public static string getGameName(string gameID, string path)
        {
            string result = "";
            string directory = path + "\\steamapps";
         

            if (File.Exists(directory + "\\appmanifest_" + gameID + ".acf"))
            {
                string[] lines = System.IO.File.ReadAllLines(directory + "\\appmanifest_" + gameID + ".acf");

                foreach (string line in lines)
                {
                    if (line.Contains("name"))
                    {
                        result = line.Replace("\"", "").Replace("name", "").Replace("\t", "");
                        Debug.WriteLine(line.Replace("\"", "").Replace("name", "").Replace("\t", ""));
                        break;
                    }
                }
            }
   


            return result;

        }
 
        public static bool steamRunning()
        {
            Process[] pname = Process.GetProcessesByName("Steam");
            if (pname.Length != 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static void openSteamGame(string gameID)
        {
            if (Properties.Settings.Default.directorySteam != "")
            {
                //string steamLaunch = "steam://rungameid/" + gameID;
                //System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Steam\steam.exe", steamLaunch);
                Classes.Task_Scheduler.Task_Scheduler.runTask(() => Run_CLI.Run_CLI.RunCommand(" \"steam://rungameid/" + gameID,false, Properties.Settings.Default.directorySteam + "\\Steam.exe", 6000, true));
            }



        }

        public static void openSteamBigPicture()
        {
            if (Properties.Settings.Default.directorySteam != "")
            {
                if (steamRunning())
                {
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => Run_CLI.Run_CLI.RunCommand(" \"steam://open/bigpicture\"", false, Properties.Settings.Default.directorySteam + "\\Steam.exe", 6000, false));

                }
                else
                {
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => Run_CLI.Run_CLI.RunCommand(" -bigpicture", false, Properties.Settings.Default.directorySteam + "\\Steam.exe", 6000, false));
                }
            }



        }
        public static void setSteamDirectory()
        {
            if (Properties.Settings.Default.directorySteam == "")
            {
                //32 and 64 bit install locations in registry
                string steam32 = "SOFTWARE\\VALVE\\STEAM";
                string steam64 = "SOFTWARE\\Wow6432Node\\Valve\\STEAM";
                string installPath = "";
                RegistryKey keyReg = Registry.LocalMachine.OpenSubKey(steam64);
                if (keyReg != null )
                {
                    if (keyReg.GetValue("InstallPath") != null)
                    {
                        Properties.Settings.Default.directorySteam = keyReg.GetValue("InstallPath").ToString();
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        //if null might be 32 bit
                        keyReg = Registry.LocalMachine.OpenSubKey(steam32);
                        if (keyReg.GetValue("InstallPath") != null)
                        {
                            Properties.Settings.Default.directorySteam = keyReg.GetValue("InstallPath").ToString();
                            Properties.Settings.Default.Save();
                        }

                    }
                }

               
                           
            }

        }
    }
}
