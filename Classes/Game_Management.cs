﻿using Microsoft.Win32;
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
 
        public static void LaunchApp(string gameID, string appType, string appLocation)
        {
            if (appType =="App")
            {


            }
            else
            {
                if (gameID != "")
                {
          
                    var launcher = Global_Variables.Global_Variables.gameLauncher.GetLaunchers().First(l => l.Name == appType);
                    Guid launcherID = launcher.Id;
                    var game = Global_Variables.Global_Variables.gameLauncher.GetAllGames().First(l => l.LauncherId == launcherID && l.Id == gameID);
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



        public static void syncBattleNet_Library()
        {
            List<GameLauncherItem> list = new List<GameLauncherItem>(); 

            

            foreach (var launcher in Global_Variables.Global_Variables.gameLauncher.GetLaunchers())
            {
                switch(launcher.Name)
                {
                    case "Battle.net":
                        foreach (var game in launcher.Games)
                        {
                            GameLauncherItem launcherItem = new GameLauncherItem();
                            launcherItem.gameName = game.Name;
                            launcherItem.gameID = game.Id;
                            launcherItem.installPath = "";
                            launcherItem.appType = launcher.Name;
                            list.Add(launcherItem);
                        }

                        break;
                    case "Steam":
                        foreach (var game in launcher.Games)
                        {
                            GameLauncherItem launcherItem = new GameLauncherItem();
                            launcherItem.gameName = game.Name;
                            launcherItem.gameID = game.Id;
                            launcherItem.installPath = "";
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
                            launcherItem.installPath = "";
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
                            launcherItem.installPath = "";
                            launcherItem.appType = launcher.Name;
                            list.Add(launcherItem);
                        }
                        break;


                }




                Console.WriteLine($"Launcher name: {launcher.Name}");
                Console.WriteLine("Games:");

               
            }


        }
     
       

    }

    public class GameLauncherItem
    {
        public string gameID { get; set; }
        public string gameName { get; set; }
        public string installPath { get; set; }
        public string appType { get; set; }
    }
}