using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Diagnostics;
using System.Threading;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.Run_CLI;
using System.Drawing.Text;

namespace Handheld_Control_Panel.Classes.Fan_Management
{
    public static class Fan_Management
    {
        public static void determineFanDevice()
        {
            string manufacturer = Motherboard_Info.Motherboard_Info.Manufacturer.ToUpper();
            string product = Motherboard_Info.Motherboard_Info.Product.ToUpper();
            string system = Motherboard_Info.Motherboard_Info.SystemName.ToUpper();
            //if one x player enable fan device
            if (manufacturer.Contains("ONE") && manufacturer.Contains("NETBOOK") && product.Contains("ONE") && product.Contains("X") && product.Contains("PLAYER"))
            {
                Global_Variables.Global_Variables.fanControlDevice = true;
                if (Global_Variables.Global_Variables.cpuType == "Intel") { Global_Variables.Global_Variables.fanDevice = "OXP_Intel"; }
                if (Global_Variables.Global_Variables.cpuType == "AMD") { Global_Variables.Global_Variables.fanDevice = "OXP_AMD"; }
            }

            if (manufacturer == "GPD" && system == "WM26800U")
            {
                Global_Variables.Global_Variables.fanControlDevice = true;
                Global_Variables.Global_Variables.fanDevice = "WM2_AMD";
            }
        }


        public static string processEC = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\EC\\EC.exe";

        private static Dictionary<string, string> ecEnableLookUp = new Dictionary<string, string>()
        {
           {"OXP_AMD", "0x4A"},
           {"OXP_Intel", "0x4A"},
           {"WM2_AMD", "0x275" },

        };

        private static Dictionary<string, string> ecReadWriteLookUp = new Dictionary<string, string>()
        {
           {"OXP_AMD", "0x4B"},
           {"OXP_Intel", "0x4B"},
           {"WM2_AMD", "0x1809" },

        };
        private static Dictionary<string, double> ecFanRange = new Dictionary<string, double>()
        {
           {"OXP_AMD", 100},
           {"OXP_Intel", 255},
           {"WM2_AMD", 184 },
        };
        public static void readSoftwareFanControl()
        {
            string argument = " -winring0 -r " + ecEnableLookUp[Global_Variables.Global_Variables.fanDevice];
            string result = "";

            result = Run_CLI.Run_CLI.RunCommand(argument, true, processEC, 2000);

            if (result != null)
            {
                if (result.Contains("0x00"))
                {
                    Global_Variables.Global_Variables.fanControlEnable = false;
                    Global_Variables.Global_Variables.fanControlMode = "Hardware";
                }
                else 
                {
                    Global_Variables.Global_Variables.fanControlEnable = true;
                    Global_Variables.Global_Variables.fanControlMode = "Manual";
                }
            }
        }
        public static void readFanSpeed()
        {
            try
            {
                string argument = " -winring0 -r " + ecReadWriteLookUp[Global_Variables.Global_Variables.fanDevice];
                string result = "";

                result = Run_CLI.Run_CLI.RunCommand(argument, true, processEC, 2000);

                if (result != null)
                {
                    result = result.Replace("\r", "").Trim();
                    result = result.Replace("\n", "").ToUpper();
                    int decValue = Convert.ToInt32(result, 16);

                    double fanPercentage = Math.Round(100 * ((double)decValue / ecFanRange[Global_Variables.Global_Variables.fanDevice]), 0);
                    Global_Variables.Global_Variables.fanSpeed = (int)fanPercentage;
                }
            }
            catch { 
            
            }

        }
        public static void generateFanControlModeList()
        {
            Global_Variables.Global_Variables.FanModes.Add("Hardware");
            Global_Variables.Global_Variables.FanModes.Add("Manual");
        }
        public static void enableSoftwareFanControl()
        {
            string argument = " -winring0 -w " + ecEnableLookUp[Global_Variables.Global_Variables.fanDevice] + " 0x01";

            string result = "";

            result = Run_CLI.Run_CLI.RunCommand(argument, false, processEC, 2000);
            Task.Delay(400);
            readSoftwareFanControl();
        }
        public static void disableSoftwareFanControl()
        {
            string processEC = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\EC\\EC.exe";
            string argument = " -winring0 -w "+ ecEnableLookUp[Global_Variables.Global_Variables.fanDevice] + " 0x00";

            string result = "";

            result = Run_CLI.Run_CLI.RunCommand(argument, false, processEC, 2000);
            Task.Delay(400);
            readSoftwareFanControl();
        }
        public static void setFanSpeed(int fanSpeed)
        {
            if (Global_Variables.Global_Variables.fanControlEnable)
            {
                string hexValue = "";
                if (ecFanRange[Global_Variables.Global_Variables.fanDevice] == 100)
                {
                    hexValue = fanSpeed.ToString("X");
                }
                else
                {
                    double normalizedFanSpeed = Math.Round(((double)fanSpeed / 100) * 255,0);
                    int fanspeedInt = (int)normalizedFanSpeed;
                    hexValue = fanspeedInt.ToString("X");
                }
                
                string argument = " -winring0 -w " + ecReadWriteLookUp[Global_Variables.Global_Variables.fanDevice] + " " + hexValue;

                string result = "";

                result = Run_CLI.Run_CLI.RunCommand(argument, false, processEC, 2000);
                readFanSpeed();
            }

        }

      
    }

  



}
