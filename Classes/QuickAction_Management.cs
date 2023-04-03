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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Handheld_Control_Panel.Classes
{
    public static class QuickAction_Management
    {
            
        public static void runHotKeyAction(ActionParameter actionParameter)
        {
            

            switch (actionParameter.Action)
            {
                case "Toggle_HCP_OSK":
                    Global_Variables.Global_Variables.mainWindow.toggleOSK();

                    break;

                case "Toggle_Windows_OSK":
                    Process[] pname = Process.GetProcessesByName("tabtip");
                    OSKTablet oskt = new OSKTablet();
                    if (pname.Length == 0)
                    {
                        ProcessStartInfo psi = new ProcessStartInfo()
                        {
                            UseShellExecute = true,
                            FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "microsoft shared\\ink\\TabTip.exe")
                        };
                        System.Diagnostics.Process.Start(psi);
                        oskt.Main();
                    }
                    else
                    {
                        oskt.Main();
                    }


                    break;
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
                case "Desktop":
             
                    Shell32.Shell shellObject = new Shell32.Shell();
                    shellObject.ToggleDesktop();
                    
                    break;
                default: break;
            }
        }

  

      
    }
    public class OSKTablet
    {
        public void Main()
        {
            var uiHostNoLaunch = new UIHostNoLaunch();
            var tipInvocation = (ITipInvocation)uiHostNoLaunch;
            tipInvocation.Toggle(GetDesktopWindow());
            Marshal.ReleaseComObject(uiHostNoLaunch);
        }

        [ComImport, Guid("4ce576fa-83dc-4F88-951c-9d0782b4e376")]
        class UIHostNoLaunch
        {
        }

        [ComImport, Guid("37c994e7-432b-4834-a2f7-dce1f13b834b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface ITipInvocation
        {
            void Toggle(IntPtr hwnd);
        }

        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();
    }
}
