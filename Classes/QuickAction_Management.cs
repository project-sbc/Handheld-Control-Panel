using Handheld_Control_Panel.Classes.Global_Variables;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Vanara.Interop.KnownShellItemPropertyKeys;
using System.Windows.Input;
using Windows.Devices.Radios;

namespace Handheld_Control_Panel.Classes
{
    public static class QuickAction_Management
    {
            
        public static void runHotKeyAction(ActionParameter actionParameter)
        {
            switch (actionParameter.Action)
            {
                case "Show_Hide_HCP":
                    if (Global_Variables.Global_Variables.mainWindow.WindowState == System.Windows.WindowState.Minimized)
                    {
                        Global_Variables.Global_Variables.mainWindow.WindowState = System.Windows.WindowState.Normal;
                    }
                    else
                    {
                        Global_Variables.Global_Variables.mainWindow.WindowState = System.Windows.WindowState.Minimized;
                    }
                    break;
                case "Change_TDP":
                    int param;

                    if (Int32.TryParse(actionParameter.Parameter, out param))
                    {
                        param = (int)(param + Global_Variables.Global_Variables.ReadPL1);
                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.TDP_Management.TDP_Management.changeTDP(param, param));
                    }
                    break;
                case "Open_Steam_BigPicture":
                    Steam_Management.openSteamBigPicture();
                    break;
                case "Open_Playnite":
                    Playnite_Management.playniteToggle();
                    break;
                case "Change_Brightness":
                    //MessageBox.Show("make the quick access menu!!!");
                    break;
                case "Change_GPUCLK":
                    int paramGPU;

                    if (Int32.TryParse(actionParameter.Parameter, out paramGPU))
                    {
                        if (Global_Variables.Global_Variables.GPUCLK != 0)
                        {
                            param = paramGPU + Global_Variables.Global_Variables.GPUCLK;
                            Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.GPUCLK_Management.GPUCLK_Management.changeAMDGPUClock(paramGPU));
                        }
                        else { Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.GPUCLK_Management.GPUCLK_Management.changeAMDGPUClock(400)); }
                    }
                    break;
                case "Toggle_MouseMode":
                    MouseMode_Management.MouseMode_Management.toggle_MouseMode();
                    break;
                case "Toggle_Controller":
                    Controller_Management.Controller_Management.powerCycleController();
                    break;

                default: break;
            }
        }

  

        public static void toggleQuickAccessMenu()
        {

        }
    }
}
