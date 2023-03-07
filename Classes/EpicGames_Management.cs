using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Handheld_Control_Panel.Classes
{
    public static class EpicGames_Management
    {


        public static List<EpicGamesLauncherItem> syncEpic_Library()
        {
            //directory of dat file for game info

            //IMPORTANT!!!!!
            //C:\ProgramData\Epic\UnrealEngineLauncher\LauncherInstalled.dat
            //Computer\HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Epic Games\EpicGamesLauncher
            //key AppDataPath

            //steam stuff copy is below

            List<EpicGamesLauncherItem> result = new List<EpicGamesLauncherItem>();
            if (Properties.Settings.Default.directoryEpicGames != "")
            {
                if (File.Exists(Properties.Settings.Default.directoryEpicGames + "\\LauncherInstalled.dat"))
                {

                    string[] lines = System.IO.File.ReadAllLines(Properties.Settings.Default.directoryEpicGames + "\\LauncherInstalled.dat");

                    bool startNewApp = false;
                    string installLocation = "";
                    string namespaceID = "";
                    string itemID = "";
                    string appID = "";
                    EpicGamesLauncherItem epicGamesLauncherItem = new EpicGamesLauncherItem();

                    foreach (string line in lines)
                    {
                      
                        if (line.Contains("InstallLocation"))
                        {
                            startNewApp = true;
                            installLocation = line.Replace("\t", "").Replace("\"InstallLocation\": ", "").Replace(",", "").Replace("\\", "").Replace("\"", "").Replace("/", "\\");
                            epicGamesLauncherItem = new EpicGamesLauncherItem();
                            epicGamesLauncherItem.gameName = installLocation.Split('\\').Last();
                            string gameExe = findGameExeInInstallPath(installLocation, epicGamesLauncherItem.gameName);
                            if (gameExe != "")
                            {
                                epicGamesLauncherItem.installPath = gameExe;
                            }
                            else
                            {
                                epicGamesLauncherItem.installPath = installLocation;
                            }
                        }

                        if (startNewApp && line.Contains("NamespaceId"))
                        {
                            namespaceID = line.Replace("\t", "").Replace("\"NamespaceId\": ", "").Replace(",", "").Replace("\"", "").Replace("\\", "");
                        }
                        if (startNewApp && line.Contains("ItemId"))
                        {
                            itemID = line.Replace("\t", "").Replace("\"ItemId\": ", "").Replace(",", "").Replace("\"", "").Replace("\\", "");
                        }
                        if (startNewApp && line.Contains("AppName"))
                        {
                            appID = line.Replace("\t", "").Replace("\"AppName\": ", "").Replace(",", "").Replace("\"", "").Replace("\\", "");
                            epicGamesLauncherItem.gameID = namespaceID + "%3A" + itemID + "%3A" + appID;
                            result.Add(epicGamesLauncherItem);
                        }
                        if (startNewApp && line.Contains("}"))
                        {
                            startNewApp = false;
                            installLocation = "";
                            namespaceID = "";
                            itemID = "";
                            appID = "";
                            epicGamesLauncherItem = null;
                        }
                    }





                }
            }

            return result;

        }
        public static string findGameExeInInstallPath(string installLocation, string gameName)
        {
            string gameExe = "";

            string[] files = Directory.GetFiles(installLocation, "*.exe");

            if (files.Length > 1)
            {
                foreach (string file in files)
                {
                    if (!file.Contains("UnityCrashHandler") && !file.Contains("CrashDumper"))
                    {
                        if (file.Contains(gameName))
                        {
                            gameExe = file;
                            return gameExe;
                        }
                        gameExe = file;
                        
                    }
                }
            }
            else
            {
                gameExe = files[0];
            }

            return gameExe;
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

        public static void openEpicGame(string gameID)
        {
            if (Properties.Settings.Default.directorySteam != "")
            {
                //string steamLaunch = "steam://rungameid/" + gameID;
                //System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Steam\steam.exe", steamLaunch);
                Classes.Task_Scheduler.Task_Scheduler.runTask(() => Run_CLI.Run_CLI.RunCommand(" \"steam://rungameid/" + gameID, false, Properties.Settings.Default.directorySteam + "\\Steam.exe", 6000, false));
            }



        }

      
        public static void setEpicGamesDirectory()
        {
            if (Properties.Settings.Default.directoryEpicGames == "")
            {
            
                string epicGamesReg = "SOFTWARE\\WOW6432Node\\Epic Games\\EpicGamesLauncher";
                string installPath = "";
                RegistryKey keyReg = Registry.LocalMachine.OpenSubKey(epicGamesReg);

                if (keyReg != null)
                {
                    if (keyReg.GetValue("AppDataPath") != null)
                    {
                        installPath = keyReg.GetValue("AppDataPath").ToString();

                        //need to go up 3 directory levels
                        installPath = Directory.GetParent(@installPath).ToString();
                        installPath = Directory.GetParent(@installPath).ToString();
                        installPath = Directory.GetParent(@installPath).ToString();
                        Properties.Settings.Default.directoryEpicGames = installPath + "\\UnrealEngineLauncher";

                        //\UnrealEngineLauncher\\LauncherInstalled.dat
                        Properties.Settings.Default.Save();
                    }
                }
                


            }

        }
    }

    public class EpicGamesLauncherItem
    {
        public string gameID { get; set; }
        public string gameName { get; set; }
        public string installPath { get; set; }
    }
}
