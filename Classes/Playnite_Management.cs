using Handheld_Control_Panel.Classes.Run_CLI;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handheld_Control_Panel.Classes
{
    public static class Playnite_Management
    {
        public static void playniteToggle()
        {
            string command;
            if (Global_Variables.Global_Variables.settings.directoryPlaynite != "")
            {
                if (playniteRunning())
                {
                    command = Global_Variables.Global_Variables.settings.directoryPlaynite + "\\Playnite.FullscreenApp.exe --shutdown";
                   
                }
                else
                {
                    //run game management one because why not
                    command = Global_Variables.Global_Variables.settings.directoryPlaynite + "\\Playnite.FullscreenApp.exe";
                }
                try
                {
                                       
                    RunNotAsAdmin_Shell.SystemUtility.ExecuteProcessUnElevated(command,"");

                }
                catch { /* ignore */ }

            }


        }
        public static void setPlayniteDirectory()
        {
            if (Global_Variables.Global_Variables.settings.directoryPlaynite == "")
            {
                if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Playnite") + "\\Playnite.FullscreenApp.exe"))
                {
                    Global_Variables.Global_Variables.settings.directoryPlaynite = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Playnite");
                }
            }

        }
        private static bool playniteRunning()
        {


            Process[] pname = Process.GetProcessesByName("Playnite.FullscreenApp");
            if (pname.Length != 0)
            {
                return true;

            }
            else
            {
                return false;
            }

        }

    }
}
