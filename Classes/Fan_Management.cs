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
using SharpDX;
using Linearstar.Windows.RawInput;

namespace Handheld_Control_Panel.Classes.Fan_Management
{
    public static class Fan_Management
    {
      

        private static void  GPDWIN4_EC_Enable()
        {
            byte EC_Chip_ID1 = WinRingEC_Management.ECRamRead(0x2000);

            if (EC_Chip_ID1 == 0x55)
            {
                byte EC_Chip_Ver = WinRingEC_Management.ECRamRead(0x1060);
                EC_Chip_Ver = (Byte)(EC_Chip_Ver | 0x80);
                WinRingEC_Management.ECRamWrite(0x1060, EC_Chip_Ver);
            }
        }


        public static string processEC = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\EC\\EC.exe";

      
        public static void readSoftwareFanControl()
        {
            //error FM01
            try
            {
                switch (Global_Variables.Global_Variables.Device.ClassType)
                {
                    case "GPDWinMax2_AMD":
                        GPDWinMax2_AMD wm2amd = (GPDWinMax2_AMD)Global_Variables.Global_Variables.Device;
                        Global_Variables.Global_Variables.fanControlEnabled = wm2amd.fanIsEnabled();
                        wm2amd = null;
                        break;
                    case "OneXPlayer2":
                        OneXPlayer2 oxp2 = (OneXPlayer2)Global_Variables.Global_Variables.Device;
                        Global_Variables.Global_Variables.fanControlEnabled = oxp2.fanIsEnabled();
                        oxp2 = null;
                        break;
                    default: break;

                }
            }
            catch (Exception ex)
            {
                Log_Writer.writeLog(ex.Message, "FM01");

            }


        }

        public static void readFanSpeed()
        {
            //error FM02
            try
            {
                switch (Global_Variables.Global_Variables.Device.ClassType)
                {
                    case "GPDWinMax2_AMD":
                        GPDWinMax2_AMD wm2amd = (GPDWinMax2_AMD)Global_Variables.Global_Variables.Device;
                        wm2amd.readFanSpeed();
                        wm2amd = null;
                        break;
                    case "OneXPlayer2":
                        OneXPlayer2 oxp2 = (OneXPlayer2)Global_Variables.Global_Variables.Device;
                        oxp2.readFanSpeed();
                        oxp2 = null;
                        break;
                    default:break;

                }
            }
            catch (Exception ex)
            {
                Log_Writer.writeLog(ex.Message, "FM02");

            }

        }

        public static void setFanSpeed(int speedPercentage)
        {
            //error FM05
            try
            {
                switch(Global_Variables.Global_Variables.Device.ClassType)
                {
                    case "GPDWinMax2_AMD":
                        GPDWinMax2_AMD wm2amd = (GPDWinMax2_AMD)Global_Variables.Global_Variables.Device;
                        wm2amd.setFanSpeed(speedPercentage);
                        wm2amd = null;
                        break;

                    default: break;
                }

            }
            catch (Exception ex)
            {
                Log_Writer.writeLog(ex.Message, "FM05");

            }

        }

        public static void setFanControlManual()
        {
            //error FM03
            try
            {
                switch (Global_Variables.Global_Variables.Device.ClassType)
                {
                    case "GPDWinMax2_AMD":
                        GPDWinMax2_AMD wm2amd = (GPDWinMax2_AMD)Global_Variables.Global_Variables.Device;
                        wm2amd.enableFanControl();
                        wm2amd = null;
                        break;

                    default: break;
                }
                Global_Variables.Global_Variables.fanControlEnabled = true;
            }
            catch (Exception ex)
            {
                Log_Writer.writeLog(ex.Message, "FM03");

            }

        }
        public static void setFanControlHardware()
        {
            //error FM04
            try
            {
                switch (Global_Variables.Global_Variables.Device.ClassType)
                {
                    case "GPDWinMax2_AMD":
                        GPDWinMax2_AMD wm2amd = (GPDWinMax2_AMD)Global_Variables.Global_Variables.Device;
                        wm2amd.disableFanControl();
                        wm2amd = null;
                        break;

                    default: break;
                }
                Global_Variables.Global_Variables.fanControlEnabled = false;
            }
            catch (Exception ex)
            {
                Log_Writer.writeLog(ex.Message, "FM04");

            }

        }


        //SEPARATE FROM GOOD STUFF
       

      
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
