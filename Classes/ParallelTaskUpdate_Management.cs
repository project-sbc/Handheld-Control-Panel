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

            Parallel.Invoke(updateTasks.ToArray());

        }


    }
}
