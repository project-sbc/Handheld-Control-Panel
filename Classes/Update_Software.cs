using Handheld_Control_Panel.Classes.Global_Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using AutoUpdaterDotNET;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Handheld_Control_Panel.Classes.Update_Software
{
    public static class Update_Software
    {

        public static closeWindowForUpdate closeWindowEvent = new closeWindowForUpdate();
        public static bool startUp = false;
        public static void bindUpdateEvent()
        {
            AutoUpdater.CheckForUpdateEvent += (args) => AutoUpdaterOnCheckForUpdateEvent(args);
        }
        public static void checkUpdateSettings()
        {
            //run setting upgrade if needed when version is updated
            if (Properties.Settings.Default.upgradeSettingsRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.upgradeSettingsRequired = false;
                Properties.Settings.Default.Save();

                if (TaskSchedulerWin32.TaskSchedulerWin32.checkAutoStart())
                {
                    TaskSchedulerWin32.TaskSchedulerWin32.changeTaskService(true);
                }
            }
        }
        public static void checkForUpdates(bool startUpRoutine = false)
        {
            //check for updates if this is called at startup and the setting for allow check at startup is on OR if this is not at startup and called from settings
            startUp = startUpRoutine;
            if ((startUp && Properties.Settings.Default.checkUpdatesAtStartUp) || !startUp)
            {
                
                AutoUpdater.Start("https://raw.githubusercontent.com/project-sbc/Handheld-Control-Panel/master/Update.xml");
                
            }


        }
        private static void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                Properties.Settings.Default.lastCheckUpdate = DateTime.Now;

                Properties.Settings.Default.Save();
                if (args.IsUpdateAvailable)
                {
                    DialogResult dialogResult;
                    if (args.Mandatory.Value)
                    {
                        dialogResult =
                            System.Windows.Forms.MessageBox.Show(
                                $@"There is new version {args.CurrentVersion} available. You are using version {args.InstalledVersion}. This is required update. Press Ok to begin updating the application.", @"Update Available",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                    }
                    else
                    {
                        dialogResult =
                            System.Windows.Forms.MessageBox.Show(
                                $@"There is new version {args.CurrentVersion} available. You are using version {args.InstalledVersion}. Do you want to update the application now?", @"Update Available",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information);
                    }

                    // Uncomment the following line if you want to show standard update dialog instead.
                    // AutoUpdater.ShowUpdateForm(args);

                    if (dialogResult.Equals(System.Windows.Forms.DialogResult.Yes) || dialogResult.Equals(System.Windows.Forms.DialogResult.OK))
                    {
                        try
                        {
                            //Throw event to close main window
                          
                            if (AutoUpdater.DownloadUpdate(args))
                            {
                                Properties.Settings.Default.upgradeSettingsRequired = true;
                                Properties.Settings.Default.Save();
                                Thread.Sleep(200);
                                Global_Variables.Global_Variables.mainWindow.Close();
                   
                            }
                        }
                        catch (Exception exception)
                        {
                            System.Windows.Forms.MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    if (!startUp)
                    {
                        System.Windows.Forms.MessageBox.Show(@"There is no update available please try again later.", @"No update available",
          MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
            }
            else
            {
                if (args.Error is WebException)
                {
                    if (!startUp)
                        System.Windows.Forms.MessageBox.Show(
                            @"There is a problem reaching update server. Please check your internet connection and try again later.",
                            @"Update Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(args.Error.Message,
                        args.Error.GetType().ToString(), MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

        }

    }
    public class closeWindowForUpdate
    {

        public event EventHandler closeWindowForUpdateEvent;
        public void raiseCloseWindowForUpdateEvent()
        {
            closeWindowForUpdateEvent?.Invoke(this, EventArgs.Empty);

        }
    }
}
