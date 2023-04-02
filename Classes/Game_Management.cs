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

namespace Handheld_Control_Panel.Classes
{
   

    public static class Game_Management
    {
 
        public static void LaunchApp(string gameID, string appType, string launcherID, string appLocation)
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

                    Guid LauncherID = new Guid(launcherID);
                    var game = Global_Variables.Global_Variables.gameLauncher.GetAllGames().First(l => l.LauncherId == LauncherID && l.Id == gameID);
                    if (game != null)
                    {
                        switch (appType)
                        {
                            case "Epic Games":
                                RunLaunchString(game);
                                break;
                            case "Steam":
                                RunLaunchString(game);
                                break;

                            default: break;
                        }
                    }
                  

                }
            
            }
          

        }

        private static void RunGame(IGame? game)
        {
            if (game is null)
            {
                return;
            }

            if (game.IsRunning)
            {
               
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = game.Executable,
                    WorkingDirectory = game.WorkingDir
                });
            }
            catch { /* ignore */ }
        }

      
        public static void RunLaunchString(IGame? game)
        {
            if (game is null)
            {
                return;
            }

            if (game.IsRunning)
            {
               
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = game.LaunchString
                });
            }
            catch { /* ignore */ }
        }






        public static List<GameLauncherItem> syncBattleNet_Library()
        {
            List<GameLauncherItem> list = new List<GameLauncherItem>(); 

            
            foreach (var launcher in Global_Variables.Global_Variables.gameLauncher.GetLaunchers())
            {
                switch(launcher.Name)
                {
                   
                    default:
                        foreach (var game in launcher.Games)
                        {
                            GameLauncherItem launcherItem = new GameLauncherItem();
                            launcherItem.gameName = game.Name;
                            launcherItem.gameID = game.Id;
                            launcherItem.launcherID = game.LauncherId.ToString();
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
        public string launcherID { get; set; }
    }
}
