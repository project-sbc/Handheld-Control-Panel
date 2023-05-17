using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handheld_Control_Panel.Classes
{
    public static class ParallelTaskUpdate_Management
    {
        public static void UpdateTask()
        {
            List<Action> updateTasks = new List<Action>();
            updateTasks.Add(() => Classes.Brightness_Management.WindowsSettingsBrightnessController.getBrightness());
            updateTasks.Add(() => Classes.Volume_Management.AudioManager.GetMasterVolume());
            updateTasks.Add(() => Classes.Volume_Management.AudioManager.GetMasterVolumeMute());
            updateTasks.Add(() => Classes.TDP_Management.TDP_Management.readTDP());
            updateTasks.Add(() => Classes.Display_Management.Display_Management.getCurrentDisplaySettings());
            updateTasks.Add(() => Classes.MaxProcFreq_Management.MaxProcFreq_Management.readCPUMaxFrequency());
            updateTasks.Add(() => Classes.CoreParking_Management.CoreParking_Management.readActiveCores());
            updateTasks.Add(() => Classes.EPP_Management.EPP_Management.readEPP());
            updateTasks.Add(() => Classes.RTSS.getRTSSFPSLimit());
            if (Global_Variables.Global_Variables.Device.FanCapable && Global_Variables.Global_Variables.fanControlEnabled)
            {
                //updateTasks.Add(() => Classes.Fan_Management.Fan_Management.readFanSpeed());
            }
            if (Global_Variables.Global_Variables.mainWindow == null)
            {
                foreach (Action action in updateTasks)
                {
                    action.Invoke();
                }
            }
            else
            {
                Parallel.Invoke(updateTasks.ToArray());
            }
            

        }
        public static void UpdateTaskAlternate()
        {
            Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Brightness_Management.WindowsSettingsBrightnessController.getBrightness());
            Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Volume_Management.AudioManager.GetMasterVolume());
            Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Volume_Management.AudioManager.GetMasterVolumeMute());
            Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.TDP_Management.TDP_Management.readTDP());
            Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Display_Management.Display_Management.getCurrentDisplaySettings());
            Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.MaxProcFreq_Management.MaxProcFreq_Management.readCPUMaxFrequency());
            Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.CoreParking_Management.CoreParking_Management.readActiveCores());
            Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.EPP_Management.EPP_Management.readEPP());
            Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.RTSS.getRTSSFPSLimit());
            if (Global_Variables.Global_Variables.Device.FanCapable && Global_Variables.Global_Variables.softwareAutoFanControlEnabled)
            {
                //checks if thread is alive, if not starts it
                AutoFan_Management.checkFanThreadRunning();
               
            }
       

        }


    }
}
