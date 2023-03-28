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
using OpenLibSys;

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
                
                //if (Global_Variables.Global_Variables.cpuType == "Intel") { Global_Variables.Global_Variables.fanDevice = "OXP_Intel"; }
                if (Global_Variables.Global_Variables.cpuType == "AMD") { Global_Variables.Global_Variables.fanDevice = "OXP_AMD"; Global_Variables.Global_Variables.fanControlDevice = true; }
            }

            if (manufacturer == "GPD" && product.Contains("G1619") && Global_Variables.Global_Variables.cpuType == "AMD")
            {
                Global_Variables.Global_Variables.fanControlDevice = true;
                Global_Variables.Global_Variables.fanDevice = "WM2_AMD";
            }
        }


        public static string processEC = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\EC\\EC.exe";

        private static Dictionary<string, string> ecEnableLookUp = new Dictionary<string, string>()
        {
           {"OXP_AMD", "0x4A"},
           //{"OXP_Intel", "0x4A"},
           {"WM2_AMD", "0x275" },

        };

        private static Dictionary<string, string> ecReadWriteLookUp = new Dictionary<string, string>()
        {
           {"OXP_AMD", "0x4B"},
           //{"OXP_Intel", "0x4B"},
           {"WM2_AMD", "0x1809" },

        };
        private static Dictionary<string, double> ecFanRange = new Dictionary<string, double>()
        {
           {"OXP_AMD", 100},
           //{"OXP_Intel", 255},
           {"WM2_AMD", 184 },
        };
        public static void readSoftwareFanControl()
        {
            switch(Global_Variables.Global_Variables.fanDevice)
            {
                case "OXP_AMD":
                    readSoftwareFanControlOXPAya();

                    break;

                case "WM2_AMD":
                    readSoftwareFanControlWM2();

                    break;

                default: break;
            }

          
        }

        public static void readSoftwareFanControlOXPAya()
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
        public static void readSoftwareFanControlWM2()
        {
            byte returnvalue = WinRingEC_Management.ECRamRead(0x275);
            if (returnvalue == 0) 
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

    public static class WinRingEC_Management
    {
        static ushort reg_addr = 0x4E;
        static ushort reg_data = 0x4F;
        static Ols ols;
        public static void ECRamWrite(ushort address, byte data)
        {
            if (ols == null)
                OlsInit();
            if (ols == null)
                return;
            byte high_byte = (byte)((address >> 8) & 0xFF);
            byte low_byte = (byte)(address & 0xFF);
            try
            {
                ols.WriteIoPortByte(reg_addr, 0x2E);
                ols.WriteIoPortByte(reg_data, 0x11);
                ols.WriteIoPortByte(reg_addr, 0x2F);
                ols.WriteIoPortByte(reg_data, high_byte);

                ols.WriteIoPortByte(reg_addr, 0x2E);
                ols.WriteIoPortByte(reg_data, 0x10);
                ols.WriteIoPortByte(reg_addr, 0x2F);
                ols.WriteIoPortByte(reg_data, low_byte);

                ols.WriteIoPortByte(reg_addr, 0x2E);
                ols.WriteIoPortByte(reg_data, 0x12);
                ols.WriteIoPortByte(reg_addr, 0x2F);
                ols.WriteIoPortByte(reg_data, data);

                if (ols != null)
                    ols.DeinitializeOls();
            }
            catch
            {
                ols = null;
                return;
            }
            ols = null;
        }

        public static byte ECRamRead(ushort address)
        {
            if (ols == null)
                OlsInit();
            if (ols == null)
                return 0;
            byte data = 0;
            byte high_byte = (byte)((address >> 8) & 0xFF);
            byte low_byte = (byte)(address & 0xFF);
            try
            {
                ols.WriteIoPortByte(reg_addr, 0x2E);
                ols.WriteIoPortByte(reg_data, 0x11);
                ols.WriteIoPortByte(reg_addr, 0x2F);
                ols.WriteIoPortByte(reg_data, high_byte);

                ols.WriteIoPortByte(reg_addr, 0x2E);
                ols.WriteIoPortByte(reg_data, 0x10);
                ols.WriteIoPortByte(reg_addr, 0x2F);
                ols.WriteIoPortByte(reg_data, low_byte);

                ols.WriteIoPortByte(reg_addr, 0x2E);
                ols.WriteIoPortByte(reg_data, 0x12);
                ols.WriteIoPortByte(reg_addr, 0x2F);
                data = ols.ReadIoPortByte(reg_data);
                if (ols != null)
                    ols.DeinitializeOls();
            }
            catch
            {
                ols = null;
                return 0;
            }
            ols = null;
            return data;
        }

        unsafe public static void OlsInit()
        {
            //-----------------------------------------------------------------------------
            // Initialize
            //-----------------------------------------------------------------------------
            ols = new OpenLibSys.Ols();
            
            // Check support library sutatus
            switch (ols.GetStatus())
            {
                case (uint)Ols.Status.NO_ERROR:
                    break;
                case (uint)Ols.Status.DLL_NOT_FOUND:
                    ols = null;
                    // MessageBox.Show("WingRing0 Status Error!! DLL_NOT_FOUND");
                    break;
                case (uint)Ols.Status.DLL_INCORRECT_VERSION:
                    ols = null;
                    //  MessageBox.Show("WingRing0 Status Error!! DLL_INCORRECT_VERSION");
                    break;
                case (uint)Ols.Status.DLL_INITIALIZE_ERROR:
                    ols = null;
                    //  MessageBox.Show("WingRing0 Status Error!! DLL_INITIALIZE_ERROR");
                    break;
            }
            if (ols == null)
            {
                RaiseOlsInitFailedEvent();
                return;
            }

            // Check WinRing0 status
            switch (ols.GetDllStatus())
            {
                case (uint)Ols.OlsDllStatus.OLS_DLL_NO_ERROR:
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_NOT_LOADED:
                    //  MessageBox.Show("WingRing0 DLL Status Error!! OLS_DRIVER_NOT_LOADED");
                    ols = null;
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_UNSUPPORTED_PLATFORM:
                    // MessageBox.Show("WingRing0 DLL Status Error!! OLS_UNSUPPORTED_PLATFORM");
                    ols = null;
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_NOT_FOUND:
                    //  MessageBox.Show("WingRing0 DLL Status Error!! OLS_DLL_DRIVER_NOT_FOUND");
                    ols = null;
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_UNLOADED:
                    // MessageBox.Show("WingRing0 DLL Status Error!! OLS_DLL_DRIVER_UNLOADED");
                    ols = null;
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_NOT_LOADED_ON_NETWORK:
                    //  MessageBox.Show("WingRing0 DLL Status Error!! DRIVER_NOT_LOADED_ON_NETWORK");
                    ols = null;
                    break;
                case (uint)Ols.OlsDllStatus.OLS_DLL_UNKNOWN_ERROR:
                    //  MessageBox.Show("WingRing0 DLL Status Error!! OLS_DLL_UNKNOWN_ERROR");
                    ols = null;
                    break;
            }
            if (ols == null)
            {
                RaiseOlsInitFailedEvent();
                return;
            }
        }

        private static void RaiseOlsInitFailedEvent()
        {
            throw new NotImplementedException();
        }

        public static void OlsFree()
        {
            if (ols != null)
                ols.DeinitializeOls();
        }

    }



}
