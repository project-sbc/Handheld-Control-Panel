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
using System.Windows;
using Notification.Wpf;

namespace Handheld_Control_Panel.Classes
{
    public static class QuickAction_Management
    {
            
        public static void runHotKeyAction(ActionParameter actionParameter)
        {
            

            switch (actionParameter.Action)
            {
                case "Show_Hide_HCP":
                    Global_Variables.Global_Variables.mainWindow.toggleWindow();
                    Notification_Management.Show(Application.Current.Resources["Hotkeys_Action_" + actionParameter.Action].ToString());
                    break;
                case "Change_TDP":
                   
                    int param;

                    if (Int32.TryParse(actionParameter.Parameter, out param))
                    {
                        param = (int)(param + Global_Variables.Global_Variables.ReadPL1);
                        Notification_Management.Show(Application.Current.Resources["Hotkeys_Action_" + actionParameter.Action].ToString() + " " + actionParameter.Parameter + " W, TDP: " + param.ToString() + " W");
                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.TDP_Management.TDP_Management.changeTDP(param, param));
                    }
                    break;
                case "Open_Steam_BigPicture":
                    Steam_Management.openSteamBigPicture();
                    Notification_Management.Show(Application.Current.Resources["Hotkeys_Action_" + actionParameter.Action].ToString());
                    break;
                case "Open_Playnite":
                    Playnite_Management.playniteToggle();
                    break;
                case "Change_Brightness":
                    Notification_Management.Show(Application.Current.Resources["Hotkeys_Action_" + actionParameter.Action].ToString() + " " + actionParameter.Parameter + " %");
                    break;
                case "Change_GPUCLK":
                    int paramGPU;
                    Notification_Management.Show(Application.Current.Resources["Hotkeys_Action_" + actionParameter.Action].ToString() + " " + actionParameter.Parameter + " MHz");
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
                    Global_Variables.Global_Variables.mousemodes.toggle_MouseMode();
                    Notification_Management.Show(Application.Current.Resources["Hotkeys_Action_" + actionParameter.Action].ToString());
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
