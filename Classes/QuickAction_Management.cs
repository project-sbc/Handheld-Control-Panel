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
                    //MessageBox.Show("make the quick access menu!!!");
                    break;
                case "Change_TDP":
                    int param;

                    if (Int32.TryParse(actionParameter.Parameter, out param))
                    {
                        param = (int)(param + Global_Variables.Global_Variables.readPL1);
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
                    //MessageBox.Show("make the quick access menu!!!");
                    break;



                default: break;
            }
        }

        public static async Task<bool> GetBluetoothIsEnabledAsync()
        {
            var radios = await Radio.GetRadiosAsync();
            var bluetoothRadio = radios.FirstOrDefault(radio => radio.Kind == RadioKind.Bluetooth);
            return bluetoothRadio != null && bluetoothRadio.State == RadioState.On;
        }
        public static async Task<bool> GetWifiIsEnabledAsync()
        {
            var radios = await Radio.GetRadiosAsync();
            var wifiRadio = radios.FirstOrDefault(radio => radio.Kind == RadioKind.WiFi);
            return wifiRadio != null && wifiRadio.State == RadioState.On;
        }
        public static async System.Threading.Tasks.Task ToggleWifi()
        {
            var radios = await Radio.GetRadiosAsync();
            var wifiRadio = radios.FirstOrDefault(radio => radio.Kind == RadioKind.WiFi);
            if (wifiRadio !=null)
            {
                if (wifiRadio.State != RadioState.Off) { wifiRadio.SetStateAsync(RadioState.Off); } else { wifiRadio.SetStateAsync(RadioState.On); }
            }
        }
        public static async System.Threading.Tasks.Task ToggleBT()
        {
            var radios = await Radio.GetRadiosAsync();
            var bluetoothRadio = radios.FirstOrDefault(radio => radio.Kind == RadioKind.Bluetooth);
            if (bluetoothRadio != null)
            {
                if (bluetoothRadio.State == RadioState.Off) { bluetoothRadio.SetStateAsync(RadioState.Off); } else { bluetoothRadio.SetStateAsync(RadioState.On); }
            }
        }
    }
}
