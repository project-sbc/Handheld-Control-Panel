using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Notification;

namespace Handheld_Control_Panel.Classes
{
    public static class Notification_Management
    {
        private static readonly NotificationManager __NotificationManager = new();


        public static void Show(string title, string message)
        {
            if (Properties.Settings.Default.enableNotifications)
            {
                __NotificationManager.Show(title, message);
            }
      
        }

    }
}
