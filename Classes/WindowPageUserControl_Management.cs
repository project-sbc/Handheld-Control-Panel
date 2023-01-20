using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handheld_Control_Panel.Classes
{
    public static class WindowPageUserControl_Management
    {
        public static string getWindowPageFromWindowToString(string window)
        {
            
            return window.Replace(" ", "").Replace(":", "").Replace("Handheld_Control_Panel.", "").Replace(".xaml", "").Replace("Pages/", ""); 
        }

        public static void switchToOuterNavigation(string window)
        {
            switch (window)
            {
                case "MainWindow":
                    MainWindowNavigation.windowNavigation = true;
                    break;
                case "QuickAccessMenu":
                    QuickAccessMenuNavigation.windowNavigation = true;
                    break;
                case "GameLauncher":
                    GameLauncherNavigation.windowNavigation = true;
                    break;
                default: break;
            }
        }
    }
}
