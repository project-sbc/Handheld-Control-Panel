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
using Windows.Management.Deployment;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using System.Drawing.Drawing2D;
using Windows.System;
using System.Windows.Documents;
using static Vanara.Interop.KnownShellItemPropertyKeys;
using System.Reflection;

namespace Handheld_Control_Panel.Classes
{
   

    public static class Game_Management
    {
 
        public static void LaunchApp(string gameID, string appType, string launchcommand, string appLocation)
        {
            if (appType =="Exe")
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
                        case "Microsoft Store":
                            PackageManager pm = new PackageManager();
                            pm.FindPackage(gameID).GetAppListEntries().First().LaunchAsync();
                            pm = null;
                            break;
                        default: break;
                    }
                }
            
            }
          
            Global_Variables.Global_Variables.mainWindow.updateTimer.Stop();
            Global_Variables.Global_Variables.mainWindow.updateTimer.Interval = new TimeSpan(0, 0, 15);
            Global_Variables.Global_Variables.mainWindow.updateTimer.Start();

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

        public static void GetMicrosoftStoreApps()
        {
            //this is obsolete, i can keep this for reference

            //microsoft store apps below
            PackageManager packageManager = new PackageManager();
            IEnumerable<Windows.ApplicationModel.Package> packages = packageManager.FindPackages();

            string[] filesInDirectory;
            string xboxGameDirectory = "C:\\XboxGames";
            if (Directory.Exists(xboxGameDirectory))
            {
                filesInDirectory = Directory.GetDirectories(xboxGameDirectory);

                if (filesInDirectory.Length > 0)
                {
                    string[] strings = filesInDirectory.Select(x => Path.GetFileName(x)).ToArray();

                    if (strings.Length > 0)
                    {
                        foreach (Package package in packages)
                        {
                            string install = package.InstalledLocation.Path;
                            string sig = package.SignatureKind.ToString();

                            if (install.Contains("WindowsApps") && sig == "Store" && package.IsFramework == false)
                            {
                                if (strings.Contains(package.DisplayName))
                                {
                                    //get full name   like unique ID
                                    Debug.WriteLine(package.Id.FullName);

                                    //launch game using this
                                    packageManager.FindPackage(package.Id.FullName).GetAppListEntries().FirstOrDefault().LaunchAsync();
                                   
                                }



                            }


                        }


                    }





                }
            }
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
            LauncherManager gameLauncher = new LauncherManager(new GameLib.Core.LauncherOptions() { QueryOnlineData = false });
            
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
                                        if (game.Name.Contains("Call of duty", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (exeName.Contains("cod", StringComparison.OrdinalIgnoreCase))
                                            {
                                                launcherItem.path = exe;
                                                launcherItem.exe = exeName;
                                                break;
                                            }
                                        }
                                        foreach (string arr in array)
                                        {
                                            if (exeName.Contains(arr, StringComparison.OrdinalIgnoreCase))
                                            {
                                                launcherItem.path = exe;
                                                launcherItem.exe = exeName;
                                                break;
                                            }
                                        }
                                        if (launcherItem.path != null) { break; }
                                    }
                                }
                                if (launcherItem.path == "" || launcherItem.exe == "")
                                {
                                    launcherItem.path = game.Executables.Last();
                                    launcherItem.exe = Path.GetFileNameWithoutExtension(game.Executables.Last());
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

            //microsoft store apps below

            PackageManager packageManager = new PackageManager();
            IEnumerable<Windows.ApplicationModel.Package> packages = packageManager.FindPackages();

            string[] filesInDirectory;
            string xboxGameDirectory = "C:\\XboxGames";
            if (Directory.Exists(xboxGameDirectory))
            {
                filesInDirectory = Directory.GetDirectories(xboxGameDirectory);

                if (filesInDirectory.Length > 0)
                {
                    string[] strings = filesInDirectory.Select(x => Path.GetFileName(x)).ToArray();

                    if (strings.Length > 0)
                    {
                        foreach (Package package in packages)
                        {
                            string install = package.InstalledLocation.Path;
                            string sig = package.SignatureKind.ToString();

                            if (install.Contains("WindowsApps") && sig == "Store" && package.IsFramework == false)
                            {
                                if (strings.Contains(package.DisplayName))
                                {
                                    GameLauncherItem launcherItem = new GameLauncherItem();
                                    launcherItem.gameName = package.DisplayName;
                                    launcherItem.gameID = package.Id.FullName;
                                    launcherItem.launchCommand = package.Id.FullName;


                                    //launcherItem.path = game.Executable;
                                    //launcherItem.exe = Path.GetFileNameWithoutExtension(launcherItem.path);
                                    launcherItem.appType = "Microsoft Store";
                                    launcherItem.imageLocation = package.Logo.AbsolutePath;
                                    list.Add(launcherItem);

                                }



                            }


                        }


                    }



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
        public string imageLocation { get; set; } = "";
    }
}
