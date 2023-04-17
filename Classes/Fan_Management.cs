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
                if (Global_Variables.Global_Variables.Device.FanCapable)
                {
                    byte returnvalue;

                    ushort lookup = Global_Variables.Global_Variables.Device.FanToggleAddress;
                    if (lookup > 0)
                    {
                        returnvalue = WinRingEC_Management.ECRamRead(lookup);
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
                if (Global_Variables.Global_Variables.Device.FanCapable)
                {
                    byte returnvalue;

                    ushort lookup = Global_Variables.Global_Variables.Device.FanChangeAddress;
                    if (lookup > 0)
                    {
                        returnvalue = WinRingEC_Management.ECRamRead(lookup);

                        double dblValue = Convert.ToDouble(returnvalue);

                        double fanPercentage = Math.Round(100 * (dblValue / Global_Variables.Global_Variables.Device.MaxFanSpeed), 0);
                        Global_Variables.Global_Variables.FanSpeed = fanPercentage;
                    }
                }
            }
            catch (Exception ex)
            {
                Log_Writer.writeLog(ex.Message, "FM02");

            }

        }

        public static void setFanSpeed(double speedPercentage)
        {
            //error FM03
            try
            {
                if (Global_Variables.Global_Variables.Device.FanCapable && Global_Variables.Global_Variables.fanControlEnable)
                {
                    if (speedPercentage < 30 && speedPercentage > 0)
                    {
                        speedPercentage = 30;
                    }
                    byte setValue = 0;
                    double fanRange = Global_Variables.Global_Variables.Device.MaxFanSpeed;
                    if (fanRange > 0)
                    {
                        if (Global_Variables.Global_Variables.Device.MaxFanSpeed == 100)
                        {
                            setValue = (byte)speedPercentage;
                        }
                        else
                        {
                            double normalizedFanSpeed = Math.Round(((double)speedPercentage / 100) * fanRange, 0);
                            setValue = (byte)normalizedFanSpeed;
                        }

                        ushort lookup = Global_Variables.Global_Variables.Device.FanChangeAddress;
                        if (lookup > 0)
                        {
                            WinRingEC_Management.ECRamWrite(lookup, setValue);

                            Global_Variables.Global_Variables.FanSpeed = speedPercentage;
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                Log_Writer.writeLog(ex.Message, "FM03");

            }

        }

        public static void setFanControlManual()
        {
            //error FM03
            try
            {
                if (Global_Variables.Global_Variables.Device.FanCapable)
                {
                   
                    ushort lookup = Global_Variables.Global_Variables.Device.FanToggleAddress;
                    if (lookup > 0)
                    {
                        WinRingEC_Management.ECRamWrite(lookup, 0x01);

                        Global_Variables.Global_Variables.fanControlEnable = true;
                        Global_Variables.Global_Variables.fanControlMode = "Manual";
                    }

                }
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
                if (Global_Variables.Global_Variables.Device.FanCapable)
                {

                    ushort lookup = Global_Variables.Global_Variables.Device.FanToggleAddress;
                    if (lookup > 0)
                    {
                        WinRingEC_Management.ECRamWrite(lookup, 0x00);

                        Global_Variables.Global_Variables.fanControlEnable = false;
                        Global_Variables.Global_Variables.fanControlMode = "Hardware";
                    }

                }
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
