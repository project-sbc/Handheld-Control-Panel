using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using GameLib;
using GameLib.Core;
using Handheld_Control_Panel.Classes.Global_Variables;
using System.Windows;
using System.Threading;
using System.Xml.Linq;

namespace Handheld_Control_Panel.Classes
{
   

    public static class Game_Management
    {
 
        public static void LaunchApp(string gameID, string appType, string launchcommand, string appLocation)
        {
            if (appType =="App")
            {
                if (File.Exists(appLocation))
                {
                    RunGame(appLocation);
                }

            }
            else
            {
                if (gameID != "")
                {
                    switch (appType)
                    {
                        case "Epic Games":
                            RunLaunchString(launchcommand);
                            break;
                        case "Steam":
                            RunLaunchString(launchcommand);
                            break;
                        case "Battle.net":
                            string battlenetfile = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "Battle.net\\Battle.net.exe");
                            if (BattleNetRunning())
                            {
                                Run_CLI.Run_CLI.RunCommand(" --exec=\"launch " + gameID.ToUpper() + "\"", false, battlenetfile, 3000, true);
                            }
                            else
                            {

                                RunGame(battlenetfile);
                                Thread.Sleep(15000);
                                Run_CLI.Run_CLI.RunCommand(" --exec=\"launch " + gameID.ToUpper() + "\"", false, battlenetfile, 3000, true);

                            }

                            break;
                        case "GOG Galaxy":
                            Run_CLI.Run_CLI.RunCommand(" /command=runGame /gameId=" + gameID, false, Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "GOG Galaxy", "GalaxyClient.exe"));

                            break;
                        default: break;
                    }
                }
            
            }
          

        }

        private static void RunGame(string command)
        {
            try
            {
                if (File.Exists(command))
                {
                    
                    Process.Start(new ProcessStartInfo()
                    {
                        UseShellExecute = true,
                        FileName = Path.GetFileName(command),
                        WorkingDirectory = Path.GetDirectoryName(command)
                    });
                }

              
            }
            catch { /* ignore */ }
        }

      
        public static void RunLaunchString(string command)
        {
           
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = command
                };
                System.Diagnostics.Process.Start(psi);
              
            }
            catch { /* ignore */ }
        }


        public static bool BattleNetRunning()
        {
            Process[] pname = Process.GetProcessesByName("Battle.net.exe");
            if (pname.Length != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static List<GameLauncherItem> syncGame_Library()
        {
            List<GameLauncherItem> list = new List<GameLauncherItem>();
             //gamelauncher
            LauncherManager gameLauncher = new LauncherManager(new LauncherOptions() { QueryOnlineData = false });
            
            foreach (var launcher in gameLauncher.GetLaunchers())
            {
                switch(launcher.Name)
                {
                    case "Steam":
                        foreach (var game in launcher.Games)
                        {
                            if (game.Id != "228980")
                            {
                                GameLauncherItem launcherItem = new GameLauncherItem();
                                launcherItem.gameName = game.Name;
                                launcherItem.gameID = game.Id;
                                launcherItem.launchCommand = game.LaunchString;

                                if (game.Executables.Count() == 1)
                                {
                                    launcherItem.path = game.Executables.First();
                                    launcherItem.exe = Path.GetFileNameWithoutExtension(game.Executables.First());
                                }
                                else
                                {
                                    string[] array = launcherItem.gameName.Split(' ');
                                    foreach (string exe in game.Executables)
                                    {
                                        string exeName = Path.GetFileNameWithoutExtension(exe);
                                        foreach (string arr in array)
                                        {
                                            if (exeName.Contains(arr))
                                            {
                                                launcherItem.path = exe;
                                                launcherItem.exe = exeName;
                                                break;
                                            }
                                        }
                                        if (launcherItem.path != "") { break; }
                                    }
                                }
                                
                                launcherItem.appType = launcher.Name;
                                list.Add(launcherItem);
                            }
                            
                        }
                        break;
                    case "Battle.net":
                        foreach (var game in launcher.Games)
                        {
                            GameLauncherItem launcherItem = new GameLauncherItem();
                            launcherItem.gameName = game.Name;
                            launcherItem.gameID = game.Id;
                            launcherItem.launchCommand = game.LaunchString;
                            switch(game.Name)
                            {
                                case "Call of Duty Black Ops Cold War":
                                    launcherItem.path = game.WorkingDir + "\\BlackOpsColdWar.exe";
                                    launcherItem.exe = "BlackOpsColdWar";
                                    break;

                                default:
                                    launcherItem.path = game.Executables.First();
                                    launcherItem.exe = Path.GetFileNameWithoutExtension(launcherItem.path);
                                    break;
                            }
                      
                            launcherItem.appType = launcher.Name;
                            list.Add(launcherItem);

                          

                        }
                        break;
                    case "Epic Games":
                        foreach (var game in launcher.Games)
                        {
                            GameLauncherItem launcherItem = new GameLauncherItem();
                            launcherItem.gameName = game.Name;
                            launcherItem.gameID = game.Id;
                            launcherItem.launchCommand = game.LaunchString;
                            launcherItem.path = game.Executable.Replace("/","\\");
                            launcherItem.exe = Path.GetFileNameWithoutExtension(launcherItem.path);
                            launcherItem.appType = launcher.Name;
                            list.Add(launcherItem);
  
                        }
                        break;
                   
                    default:
                        foreach (var game in launcher.Games)
                        {
                            GameLauncherItem launcherItem = new GameLauncherItem();
                            launcherItem.gameName = game.Name;
                            launcherItem.gameID = game.Id;
                            launcherItem.launchCommand = game.LaunchString;
                            launcherItem.path = game.Executable;
                            launcherItem.exe = Path.GetFileNameWithoutExtension(launcherItem.path);
                            launcherItem.appType = launcher.Name;
                            list.Add(launcherItem);
                        }
                        break;

                }

            }
            return list;

        }
     
       

    }

    public class GameLauncherItem
    {
        public string gameID { get; set; }
        public string gameName { get; set; }
        public string appType { get; set; }
        public string launchCommand { get; set; }
        public string path { get; set; }
        public string exe { get; set; }
    }
}
