using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Notification;
using System.Windows.Media;

namespace Handheld_Control_Panel.Classes
{
    public static class Notification_Management
    {
        private static readonly NotificationManager __NotificationManager = new();


        public static void Show(string message, bool forceEnable=false)
        {
            if (Properties.Settings.Default.enableNotifications || forceEnable)
            {
                __NotificationManager.Show("Handheld Control Panel", message);
            }
      
        }

        public static void ShowInWindow(string title, NotificationType notificationType)
        {
            Global_Variables.Global_Variables.mainWindow.ShowNotificationInWindow(title,  notificationType);


        }

        public static void ShowYesNoPrompt(string title, NotificationType notificationType, string action)
        {
            Global_Variables.Global_Variables.mainWindow.ShowNotificationInWindowYESNO(title, notificationType, action);


        }


    }
}
