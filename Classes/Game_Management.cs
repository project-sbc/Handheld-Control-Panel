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
                    Task.Run(() => System.Diagnostics.Process.Start(appLocation));
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

                        default: break;
                    }
                }
            
            }
          

        }

        private static void RunGame(string command)
        {
            try
            {
                Task.Run(() => System.Diagnostics.Process.Start(command));

                //Process.Start(new ProcessStartInfo()
                //{
                    //UseShellExecute = true,
                    //FileName = game.Executable,
                    //WorkingDirectory = game.WorkingDir
                //});
            }
            catch { /* ignore */ }
        }

      
        public static void RunLaunchString(string command)
        {
           
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = command
                });
            }
            catch { /* ignore */ }
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
                   
                    default:
                        foreach (var game in launcher.Games)
                        {
                            GameLauncherItem launcherItem = new GameLauncherItem();
                            launcherItem.gameName = game.Name;
                            launcherItem.gameID = game.Id;
                            launcherItem.launchCommand = game.LaunchString;
                            launcherItem.path = game.WorkingDir + "\\" + game.Executable;
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
    }
}
