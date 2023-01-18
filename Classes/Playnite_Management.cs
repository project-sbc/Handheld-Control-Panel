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
            if (Properties.Settings.Default.directoryPlaynite != "")
            {
                if (playniteRunning())
                {
                    Run_CLI.Run_CLI.RunCommand(" --shutdown", false, Properties.Settings.Default.directoryPlaynite + "\\Playnite.FullscreenApp.exe", 6000, false);
                }
                else
                {
                    Run_CLI.Run_CLI.RunCommand(" --startfullscreen", false, Properties.Settings.Default.directoryPlaynite + "\\Playnite.FullscreenApp.exe", 6000, false);
                }
            }


        }
        public static void setPlayniteDirectory()
        {
            if (Properties.Settings.Default.directoryPlaynite == "")
            {
                if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Playnite") + "\\Playnite.FullscreenApp.exe"))
                {
                    Properties.Settings.Default.directoryPlaynite = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Playnite");
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
