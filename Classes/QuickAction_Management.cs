using Handheld_Control_Panel.Classes.Global_Variables;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Vanara.Interop.KnownShellItemPropertyKeys;
using System.Windows.Input;

using System.Windows;
using Notification.Wpf;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using WindowsInput;
using System.Data.Common;
using YamlDotNet.Core.Tokens;
using System.Windows.Threading;
using System.Threading;

namespace Handheld_Control_Panel.Classes
{
    public static class QuickAction_Management
    {
        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr handle);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }
        public static void runHotKeyAction(ActionParameter actionParameter)
        {
            Log_Writer.writeLog("Starting quick action " + actionParameter.Action);

            switch (actionParameter.Action)
            {
                case "Open_Program":
                    Global_Variables.Global_Variables.profiles.openProgram(actionParameter.Parameter);


                    break;


                case "Toggle_HCP_OSK":
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Global_Variables.Global_Variables.mainWindow.toggleOSK()));
                                       

                    break;
                case "Toggle_AutoTDP":
                    if (Global_Variables.Global_Variables.autoTDP)
                    {
                        Notification_Management.Show("Stop AutoTDP");
                        AutoTDP_Management_OLD.endAutoTDP();
                    }
                    else
                    {
                        Notification_Management.Show("Start AutoTDP");
                        AutoTDP_Management_OLD.startAutoTDPThread();
                    }
                    break;

                case "Toggle_AMD_RSR":
                    if (Global_Variables.Global_Variables.cpuType == "AMD")
                    {
                        int rsrState = ADLX_Management.GetRSRState();
                        if (rsrState == 1)
                        {
                            ADLX_Management.SetRSR(false);
                        }
                        else
                        {
                            ADLX_Management.SetRSR(true);
                        }
                    }

                    break;

                case "Toggle_Windows_OSK":
                    Process[] pname = Process.GetProcessesByName("TabTip");
    
                    if (pname.Length == 0)
                    {
                        ProcessStartInfo psi = new ProcessStartInfo()
                        {
                            UseShellExecute = true,
                            FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "microsoft shared\\ink\\TabTip.exe")
                        };
                        System.Diagnostics.Process.Start(psi);
                     
                    }
                    Thread.Sleep(300);

                    pname = Process.GetProcessesByName("TabTip");
                    if (pname.Length != 0)
                    {
                        OSKTablet oskt = new OSKTablet();
                        oskt.Main();
                    }
                    



                    break;


                case "Show_Hide_HCP":
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Global_Variables.Global_Variables.mainWindow.toggleWindow()));


                    
                   
                    break;

                case "Show_Hide_HCP_ProcessSuspend":
                    if (Global_Variables.Global_Variables.mainWindow.Visibility != Visibility.Visible)
                    {
                        FullScreen_Management.checkSuspendProcess();
                    }

                    System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Global_Variables.Global_Variables.mainWindow.toggleWindow()));

                    break;
                case "Change_FanSpeed":
                    if (Global_Variables.Global_Variables.Device.FanCapable)
                    {
                        int paramFS;

                        if (Int32.TryParse(actionParameter.Parameter, out paramFS))
                        {
                            paramFS = (int)(paramFS + Global_Variables.Global_Variables.fanSpeed);
                            //error trap fan speed, if speed is down and less than min go to 0. if speed is less than min and going up go to min speed. if > 100 go to 100
                            if (paramFS < Global_Variables.Global_Variables.Device.MinFanSpeedPercentage && actionParameter.Parameter.Contains("-")) { paramFS = 0; }
                            if (paramFS < Global_Variables.Global_Variables.Device.MinFanSpeedPercentage && !actionParameter.Parameter.Contains("-")) { paramFS = Global_Variables.Global_Variables.Device.MinFanSpeedPercentage; }
                            if (paramFS >100 ) { paramFS = 100; }


                            Notification_Management.Show(Application.Current.Resources["Hotkeys_Action_" + actionParameter.Action].ToString() + " " + actionParameter.Parameter + " %, TDP: " + paramFS.ToString() + " %");
                            if (!Global_Variables.Global_Variables.fanControlEnabled) { Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Fan_Management.Fan_Management.setFanControlManual()); }
                            Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Fan_Management.Fan_Management.setFanSpeed(paramFS));
                      
                        }

                    }
                    
                    break;
                case "Change_FanSpeed_Mode":

                    if (actionParameter.Parameter != null )
                    {
                        if (!Global_Variables.Global_Variables.softwareAutoFanControlEnabled)
                        {
                            int fsParameter;
                            string[] fsValues = actionParameter.Parameter.Split(";");
                            bool applyNextValue = false;
                           
                            if (!Global_Variables.Global_Variables.fanControlEnabled)
                            {
                                
                                if (Int32.TryParse(fsValues[0], out fsParameter))
                                {
                                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Fan_Management.Fan_Management.setFanControlManual());
                                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Fan_Management.Fan_Management.setFanSpeed(fsParameter));
                                    return;
                                }
                            }
                            else
                            {
                                foreach (string fsValue in fsValues)
                                {
                                    if (fsValue != "")
                                    {
                                        if (Int32.TryParse(fsValue, out fsParameter))
                                        {
                                            if (applyNextValue)
                                            {
                                                if (!Global_Variables.Global_Variables.fanControlEnabled) {  }
                                                Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Fan_Management.Fan_Management.setFanSpeed(fsParameter));

                                                return;
                                            }
                                            if (fsValue == Global_Variables.Global_Variables.FanSpeed.ToString())
                                            {
                                                applyNextValue = true;
                                            }
                                        }

                                    }
                                }
                                if (Int32.TryParse(fsValues[0], out fsParameter))
                                {
                                   
                                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Fan_Management.Fan_Management.setFanSpeed(fsParameter));
                                    return;
                                }
                            }
                            
                        }
                        else
                        {
                            //display message about auto fan enabled, disable it first
                        }

                    }


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

                case "Change_TDP_Mode":

                    if (actionParameter.Parameter != null)
                    {
                        int tdpParameter;
                        string[] tdpValues = actionParameter.Parameter.Split(";");
                        bool applyNextValue = false;
                        foreach(string tdpValue in tdpValues)
                        {
                            if (tdpValue != "") 
                            {
                                if (Int32.TryParse(tdpValue, out tdpParameter))
                                {
                                    if (applyNextValue)
                                    {
                                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.TDP_Management.TDP_Management.changeTDP(tdpParameter, tdpParameter));
                                        return;
                                    }
                                    if (tdpValue == Global_Variables.Global_Variables.ReadPL1.ToString())
                                    {
                                        applyNextValue = true;
                                    }
                                }
                                
                            }
                        }
                        if (Int32.TryParse(tdpValues[0], out tdpParameter))
                        {
                            Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.TDP_Management.TDP_Management.changeTDP(tdpParameter, tdpParameter));
                            return;
                        }

                    }

                 
                    break;
                case "Open_Steam_BigPicture":
                    Notification_Management.Show(Application.Current.Resources["Hotkeys_Action_" + actionParameter.Action].ToString());
                    Steam_Management.openSteamBigPicture();
                 
                    break;
                case "Open_Playnite":
                    Playnite_Management.playniteToggle();
                    break;
                case "Change_Brightness":

                    
                    int paramBrightness;

                    if (Int32.TryParse(actionParameter.Parameter, out paramBrightness))
                    {
                        param = (int)(paramBrightness + Global_Variables.Global_Variables.Brightness);
                        if (param > 100) { param = 100; }
                        if (param < 0) { param = 0; }
                        Notification_Management.Show(Application.Current.Resources["Hotkeys_Action_" + actionParameter.Action].ToString() + " " + actionParameter.Parameter + " %");
                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Brightness_Management.WindowsSettingsBrightnessController.setBrightness(param));
                    }
                    break;
                case "Change_Brightness_Mode":

                    if (actionParameter.Parameter != null)
                    {
                        int Parameter;
                        string[] Values = actionParameter.Parameter.Split(";");
                        bool applyNextValue = false;
                        foreach (string Value in Values)
                        {
                            if (Value != "")
                            {
                                if (Int32.TryParse(Value, out Parameter))
                                {
                                    if (applyNextValue)
                                    {
                                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Brightness_Management.WindowsSettingsBrightnessController.setBrightness(Parameter));
                                      
                                        return;
                                    }
                                    if (Value == Global_Variables.Global_Variables.Brightness.ToString())
                                    {
                                        applyNextValue = true;
                                    }
                                }

                            }
                        }
                        if (Int32.TryParse(Values[0], out Parameter))
                        {
                            Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Brightness_Management.WindowsSettingsBrightnessController.setBrightness(Parameter));
                            return;
                        }
                    }


                    break;
                case "Change_Volume_Mode":

                    if (actionParameter.Parameter != null)
                    {
                        int Parameter;
                        string[] Values = actionParameter.Parameter.Split(";");
                        bool applyNextValue = false;
                        foreach (string Value in Values)
                        {
                            if (Value != "")
                            {
                                if (Int32.TryParse(Value, out Parameter))
                                {
                                    if (applyNextValue)
                                    {
                                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Volume_Management.AudioManager.SetMasterVolume(Parameter));

                                        return;
                                    }
                                    if (Value == Global_Variables.Global_Variables.Volume.ToString())
                                    {
                                        applyNextValue = true;
                                    }
                                }

                            }
                        }
                        if (Int32.TryParse(Values[0], out Parameter))
                        {
                            Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Volume_Management.AudioManager.SetMasterVolume(Parameter));
                            return;
                        }
                    }


                    break;
                case "Change_Refresh_Mode":
                    if (actionParameter.Parameter != null)
                    {

                        string[] Values = actionParameter.Parameter.Split(";");
                        bool applyNextValue = false;
                        foreach (string Value in Values)
                        {
                            if (Value != "")
                            {
                                if (applyNextValue)
                                {
                                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Display_Management.Display_Management.SetDisplayRefreshRate(Value));

                                    return;
                                }
                                if (Value == Global_Variables.Global_Variables.RefreshRate)
                                {
                                    applyNextValue = true;
                                }

                            }
                        }
                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Display_Management.Display_Management.SetDisplayRefreshRate(Values[0]));
                        return;
                    }


                    break;
                case "Change_Resolution_Mode":
                    if (actionParameter.Parameter != null)
                    {
                       
                        string[] Values = actionParameter.Parameter.Split(";");
                        bool applyNextValue = false;
                        foreach (string Value in Values)
                        {
                            if (Value != "")
                            {
                                if (applyNextValue)
                                {
                                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Display_Management.Display_Management.SetDisplayResolution(Value));

                                    return;
                                }
                                if (Value == Global_Variables.Global_Variables.Resolution)
                                {
                                    applyNextValue = true;
                                }

                            }
                        }
                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Display_Management.Display_Management.SetDisplayResolution(Values[0]));
                        return;
                    }


                    break;
                case "Change_Volume":
                    int paramVol;
                    if (Int32.TryParse(actionParameter.Parameter, out paramVol))
                    {
                        param = (int)(paramVol + Global_Variables.Global_Variables.Volume);
                        if (param > 100) { param = 100; }
                        if (param < 0) { param = 0; }
                        Notification_Management.Show(Application.Current.Resources["Hotkeys_Action_" + actionParameter.Action].ToString() + " " + actionParameter.Parameter + " %");
                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Volume_Management.AudioManager.SetMasterVolume(param));
                    }
                 
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
                    InputSimulator inputSimulator = new InputSimulator();
                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.LWIN);
                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_D);
                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_D);
                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.LWIN);
                    inputSimulator = null;
              
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
