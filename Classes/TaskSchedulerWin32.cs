using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handheld_Control_Panel.Classes.TaskSchedulerWin32
{
    public static class TaskSchedulerWin32
    {
        public static void changeTaskService(bool systemAutoStart)
        {
            Microsoft.Win32.TaskScheduler.TaskService ts = new Microsoft.Win32.TaskScheduler.TaskService();
            Microsoft.Win32.TaskScheduler.Task task = ts.GetTask("Handheld_Control_Panel");
            string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            if (!systemAutoStart && task != null)
            {
                task.RegisterChanges();
                ts.RootFolder.DeleteTask("Handheld_Control_Panel");
            }
            if (systemAutoStart && task == null)
            {
                TaskDefinition td = ts.NewTask();

                td.RegistrationInfo.Description = "Handheld Control Panel";
                td.Triggers.AddNew(TaskTriggerType.Logon);
                td.Principal.RunLevel = TaskRunLevel.Highest;
                td.Settings.DisallowStartIfOnBatteries = false;
                td.Settings.StopIfGoingOnBatteries = false;
                td.Settings.RunOnlyIfIdle = false;

                td.Actions.Add(new ExecAction(BaseDir + "\\Handheld Control Panel.exe"));

                Microsoft.Win32.TaskScheduler.TaskService.Instance.RootFolder.RegisterTaskDefinition("Handheld_Control_Panel", td);
            }
            
        }

        public static bool checkAutoStart()
        {
            bool checkStart = false;

            Microsoft.Win32.TaskScheduler.TaskService ts = new Microsoft.Win32.TaskScheduler.TaskService();
            Microsoft.Win32.TaskScheduler.Task task = ts.GetTask("Handheld_Control_Panel");
            string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (task != null)
            {
                TaskDefinition td = task.Definition;
                if (td != null)
                {
                    foreach (Microsoft.Win32.TaskScheduler.Action action in td.Actions)
                    {

                        
                        if (action.ActionType == TaskActionType.Execute)
                        {
                            if (String.Compare(Path.GetFullPath(action.ToString()), Path.GetFullPath(BaseDir + "\\Handheld Control Panel.exe"), StringComparison.InvariantCultureIgnoreCase)==0)
                            {
                                checkStart = true;
                            }
                            else
                            {
                                task.RegisterChanges();
                                ts.RootFolder.DeleteTask("Handheld_Control_Panel");
                            }
                        }
                    }
                }

            }
            return checkStart;

        }
    }
}
