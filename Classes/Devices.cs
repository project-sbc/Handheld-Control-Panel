using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Handheld_Control_Panel.Classes
{
    public abstract class HandheldDevice
    {
        public string ClassType;
        public string Manufacturer;
        public string Motherboard;

        public string CpuType;

        public bool FanCapable = false;
        public ushort FanToggleAddress;
        public ushort FanChangeAddress;
        public int MaxFanSpeed;
        public int MinFanSpeed;
        public int MinFanSpeedPercentage;
        public string fanCurveTemperature;
        public string fanCurvePackagePower;

        public int MaxGPUClock;
        public int MinGPUClock;
        public int MaxCPUClock;
        public int MinCPUClock;

        public string AutoTDP;
        

    }

    public static class Device_Management
    {
        public static HandheldDevice GetCurrentDevice()
        {
            string manufacturer = Motherboard_Info.Motherboard_Info.Manufacturer.ToUpper();
            string product = Motherboard_Info.Motherboard_Info.Product.ToUpper();

            HandheldDevice handheldDevice = new GenericDevice();

            
            switch(manufacturer)
            {
                case "GPD":
                    switch (product)
                    {
                        case "G1618-04":
                            handheldDevice = new GPDWin4();
                            break;
                        case "G1619-04":
                            handheldDevice = new GPDWinMax2_AMD();
                            break;
                        case "G1619-03":
                            
                            break;
                    }


                    break;
                case "ONE-NETBOOK TECHNOLOGY CO., LTD.":
                case "ONE-NETBOOK":
                    switch (product)
                    {
                        case "ONEXPLAYER 2 ARP23":
                            handheldDevice = new OneXPlayer2();
                            break;
                        case "V01":
                            //handheldDevice = new OneXPlayerMiniAMD();
                            break;
                        case "1002-C":
                            //handheldDevice = new OneXPlayerMiniIntel();
                            break;
                        case "V03":
                           //handheldDevice = new OneXPlayerMiniPro();
                            break;
                    }


                    break;
                case "AYANEO":
                    switch (product)
                    {
                        case "AIR":
                          
                            break;
                        case "AIR Pro":
                          
                            break;
                        case "AIR Lite":
                           
                            break;
                        case "AYA NEO FOUNDER":
                        case "AYANEO 2021":
                            
                            break;
                        case "AYANEO 2021 Pro":
                        case "AYANEO 2021 Pro Retro Power":
                            
                            break;
                        case "NEXT Pro":
                        case "NEXT Advance":
                        case "NEXT":
                            
                            break;
                        case "AYANEO 2":
                        case "GEEK":
                            handheldDevice = new AyaNeo2();
                            break;
                    }
                    break;
                case "AOKZOE":
                    switch (product)
                    {
                        case "AOKZOE A1 AR07":
                            handheldDevice = new AOKZOEA1();
                            break;
                        default: break;

                    }
                    break;
                default:

                    break;
            }
            
            return handheldDevice;
        }

       



    }



    public class GPDWinMax2_AMD : HandheldDevice
    {
        public GPDWinMax2_AMD()
        {
            this.ClassType = "GPDWinMax2_AMD";
            this.Manufacturer = "GPD";
            this.Motherboard = "1619-04";
            this.AutoTDP = "GPUClock";
            this.FanCapable = true;
            this.FanToggleAddress = 0x275;
            this.FanChangeAddress = 0x1809;
            this.MaxFanSpeed = 184;
            this.MinFanSpeed = 0;
            this.MinFanSpeedPercentage = 20;
            this.fanCurveTemperature = "0,0,0,0,0,0,0,0,0,0,30,30,30,30,40,40,50,50,70,70,100";
            this.fanCurvePackagePower = "0,0,0,30,30,40,40,50,60,60,80,90";

            this.MaxCPUClock = 4600;
            this.MinCPUClock = 1100;
            this.MinGPUClock = 400;
            this.MaxGPUClock = 2200;
        }
        public void enableFanControl()
        {
            WinRingEC_Management.ECRamWrite(FanToggleAddress, 0x01);
            Global_Variables.Global_Variables.fanControlEnabled = true;
        }
        public bool fanIsEnabled()
        {
            byte returnvalue = WinRingEC_Management.ECRamRead(FanToggleAddress);
            if (returnvalue == 0) { return false; } else { return true; }
        }
        public void disableFanControl()
        {
            WinRingEC_Management.ECRamWrite(FanToggleAddress,0x00);
            Global_Variables.Global_Variables.fanControlEnabled = false;
        }
        public void readFanSpeed()
        {
            int fanSpeed = 0;

            byte returnvalue = WinRingEC_Management.ECRamRead(FanChangeAddress);

            double fanPercentage = Math.Round(100 * (Convert.ToDouble(returnvalue) / Global_Variables.Global_Variables.Device.MaxFanSpeed), 0);
            Global_Variables.Global_Variables.FanSpeed = fanPercentage;
        }
        public void setFanSpeed(int speedPercentage)
        {
            if (speedPercentage < MinFanSpeedPercentage && speedPercentage > 0)
            {
                speedPercentage = MinFanSpeedPercentage;
            }

            byte setValue = (byte)Math.Round(((double)speedPercentage / 100) * MaxFanSpeed, 0);
            WinRingEC_Management.ECRamWrite(FanChangeAddress, setValue);

            Global_Variables.Global_Variables.FanSpeed = speedPercentage;
        }

    }
    public class GPDWin4 : HandheldDevice
    {
        public GPDWin4()
        {
            this.ClassType = "GPDWin4";
            this.Manufacturer = "GPD";
            this.Motherboard = "1618-04";
            this.AutoTDP = "GPUClock";
            this.FanCapable = true;
            this.FanToggleAddress = 0xC311;
            this.FanChangeAddress = 0xC311;
            this.MaxFanSpeed = 127;
            this.MinFanSpeed = 1;
            this.MinFanSpeedPercentage = 25;
            this.fanCurveTemperature = "0,0,0,0,0,0,0,0,0,0,30,30,30,30,40,40,50,50,70,70,100";
            this.fanCurvePackagePower = "0,0,0,30,30,40,40,50,60,60,80,90";

            this.MaxCPUClock = 4600;
            this.MinCPUClock = 1100;
            this.MinGPUClock = 400;
            this.MaxGPUClock = 2200;

            WinRingEC_Management.InitECWin4();
        }

        public void enableFanControl()
        {
            WinRingEC_Management.ECRamWriteWin4(FanToggleAddress, 0x01);
            Global_Variables.Global_Variables.fanControlEnabled = true;
        }
        public bool fanIsEnabled()
        {
            byte returnvalue = WinRingEC_Management.ECRamReadWin4(FanToggleAddress);
            if (returnvalue == 0) { return false; } else { return true; }
        }
        public void disableFanControl()
        {
            WinRingEC_Management.ECRamWriteWin4(FanToggleAddress, 0x00);
            Global_Variables.Global_Variables.fanControlEnabled = true;
        }
        public void readFanSpeed()
        {
            byte returnvalue = WinRingEC_Management.ECRamReadWin4(FanChangeAddress);

            double fanPercentage = Math.Round(100 * ((Convert.ToDouble(returnvalue)-Convert.ToDouble(MinFanSpeed))  / (MaxFanSpeed - MinFanSpeed)), 0);
            Global_Variables.Global_Variables.FanSpeed = fanPercentage;
        }
        public void setFanSpeed(int speedPercentage)
        {
            if (speedPercentage < MinFanSpeedPercentage && speedPercentage > 0)
            {
                speedPercentage = MinFanSpeedPercentage;
            }

            byte setValue = (byte)(Math.Round(((double)speedPercentage / 100) * (MaxFanSpeed-MinFanSpeed), 0)+ MinFanSpeed);
            
            WinRingEC_Management.ECRamWriteWin4(FanChangeAddress, setValue);

            Global_Variables.Global_Variables.FanSpeed = speedPercentage;
        }
    }
    public class AyaNeo2 : HandheldDevice
    {
        public AyaNeo2()
        {
            this.ClassType = "AyaNeo2";
            this.Manufacturer = "Aya";
            this.Motherboard = "AYANEO 2/GEEK";
            this.AutoTDP = "GPUClock";
            this.FanCapable = true;
            this.FanToggleAddress = 0x44A;
            this.FanChangeAddress = 0x44B;
            this.MaxFanSpeed = 100;
            this.MinFanSpeed = 0;
            this.MinFanSpeedPercentage = 25;
            this.fanCurveTemperature = "0,0,0,0,0,0,0,0,0,0,30,30,30,30,40,40,50,50,70,70,100";
            this.fanCurvePackagePower = "0,0,0,30,30,40,40,50,60,60,80,90";

            this.MaxCPUClock = 4600;
            this.MinCPUClock = 1100;
            this.MinGPUClock = 400;
            this.MaxGPUClock = 2200;
        }
        public void enableFanControl()
        {
            WinRingEC_Management.ECRamWrite(FanToggleAddress, 0x01);
            Global_Variables.Global_Variables.fanControlEnabled = true;
        }
        public bool fanIsEnabled()
        {
            byte returnvalue = WinRingEC_Management.ECRamRead(FanToggleAddress);
            if (returnvalue == 0) { return false; } else { return true; }
        }
        public void disableFanControl()
        {
            WinRingEC_Management.ECRamWrite(FanToggleAddress, 0x00);
            Global_Variables.Global_Variables.fanControlEnabled = false;
        }
        public void readFanSpeed()
        {
            int fanSpeed = 0;

            byte returnvalue = WinRingEC_Management.ECRamRead(FanChangeAddress);

            double fanPercentage = Math.Round(100 * (Convert.ToDouble(returnvalue) / Global_Variables.Global_Variables.Device.MaxFanSpeed), 0);
            Global_Variables.Global_Variables.FanSpeed = fanPercentage;
        }
        public void setFanSpeed(int speedPercentage)
        {
            if (speedPercentage < MinFanSpeedPercentage && speedPercentage > 0)
            {
                speedPercentage = MinFanSpeedPercentage;
            }

            byte setValue = (byte)Math.Round(((double)speedPercentage / 100) * MaxFanSpeed, 0);
            WinRingEC_Management.ECRamWrite(FanChangeAddress, setValue);

            Global_Variables.Global_Variables.FanSpeed = speedPercentage;
        }

    }
    public class OneXPlayer2 : HandheldDevice
    {
        public OneXPlayer2()
        {
            this.ClassType = "OneXPlayer2";
            this.Manufacturer = "One-Netbook";
            this.Motherboard = "ONEXPLAYER 2 ARP23";
            this.AutoTDP = "GPUClock";
            this.FanCapable = true;
            this.FanToggleAddress = 0x44A;
            this.FanChangeAddress = 0x44B;
            this.MaxFanSpeed = 184;
            this.MinFanSpeed = 0;
            this.MinFanSpeedPercentage = 25;
            this.fanCurveTemperature = "0,0,0,0,0,0,0,0,0,0,30,30,30,30,40,40,50,50,70,70,100";
            this.fanCurvePackagePower = "0,0,0,30,30,40,40,50,60,60,80,90";

            this.MaxCPUClock = 4600;
            this.MinCPUClock = 1100;
            this.MinGPUClock = 400;
            this.MaxGPUClock = 2200;
        }
        public void enableFanControl()
        {
            WinRingEC_Management.ECRamWrite(FanToggleAddress, 0x01);
            Global_Variables.Global_Variables.fanControlEnabled = true;
        }
        public bool fanIsEnabled()
        {
       
            byte returnvalue = WinRingEC_Management.ECRamRead(FanToggleAddress);
            if (returnvalue == 0) { return false; } else { return true; }
        }
        public void disableFanControl()
        {
            WinRingEC_Management.ECRamWrite(FanToggleAddress, 0x00);
            Global_Variables.Global_Variables.fanControlEnabled = false;
        }
        public void readFanSpeed()
        {
            int fanSpeed = 0;

            byte returnvalue = WinRingEC_Management.ECRamRead(FanChangeAddress);

            double fanPercentage = Math.Round(100 * (Convert.ToDouble(returnvalue) / Global_Variables.Global_Variables.Device.MaxFanSpeed), 0);
            Global_Variables.Global_Variables.FanSpeed = fanPercentage;
        }
        public void setFanSpeed(int speedPercentage)
        {
            if (speedPercentage < MinFanSpeedPercentage && speedPercentage > 0)
            {
                speedPercentage = MinFanSpeedPercentage;
            }

            byte setValue = (byte)Math.Round(((double)speedPercentage / 100) * MaxFanSpeed, 0);
            WinRingEC_Management.ECRamWrite(FanChangeAddress, setValue);

            Global_Variables.Global_Variables.FanSpeed = speedPercentage;
        }
    }

    public class AOKZOEA1 : HandheldDevice
    {
        public AOKZOEA1()
        {
            this.ClassType = "";
            this.Manufacturer = "";
            this.Motherboard = "";
            this.AutoTDP = "GPUClock";
            this.FanCapable = true;
            this.FanToggleAddress = 0x44A;
            this.FanChangeAddress = 0x44B;
            this.MaxFanSpeed = 255;
            this.MinFanSpeed = 0;
            this.MinFanSpeedPercentage = 25;
            this.fanCurveTemperature = "0,0,0,0,0,0,0,0,0,0,30,30,30,30,40,40,50,50,70,70,100";
            this.fanCurvePackagePower = "0,0,0,30,30,40,40,50,60,60,80,90";

            this.MaxCPUClock = 4600;
            this.MinCPUClock = 1100;
            this.MinGPUClock = 400;
            this.MaxGPUClock = 2200;
        }
        public void enableFanControl()
        {
            WinRingEC_Management.ECRamWrite(FanToggleAddress, 0x01);
            Global_Variables.Global_Variables.fanControlEnabled = true;
        }
        public bool fanIsEnabled()
        {

            byte returnvalue = WinRingEC_Management.ECRamRead(FanToggleAddress);
            if (returnvalue == 0) { return false; } else { return true; }
        }
        public void disableFanControl()
        {
            WinRingEC_Management.ECRamWrite(FanToggleAddress, 0x00);
            Global_Variables.Global_Variables.fanControlEnabled = false;
        }
        public void readFanSpeed()
        {
            int fanSpeed = 0;

            byte returnvalue = WinRingEC_Management.ECRamRead(FanChangeAddress);

            double fanPercentage = Math.Round(100 * (Convert.ToDouble(returnvalue) / Global_Variables.Global_Variables.Device.MaxFanSpeed), 0);
            Global_Variables.Global_Variables.FanSpeed = fanPercentage;
        }
        public void setFanSpeed(int speedPercentage)
        {
            if (speedPercentage < MinFanSpeedPercentage && speedPercentage > 0)
            {
                speedPercentage = MinFanSpeedPercentage;
            }

            byte setValue = (byte)Math.Round(((double)speedPercentage / 100) * MaxFanSpeed, 0);
            WinRingEC_Management.ECRamWrite(FanChangeAddress, setValue);

            Global_Variables.Global_Variables.FanSpeed = speedPercentage;
        }
    }
    public class OneXPlayerMiniPro : HandheldDevice
    {
        public OneXPlayerMiniPro()
        {
            this.ClassType = "OneXPlayerMiniPro";
            this.Manufacturer = "One-Netbook";
            this.Motherboard = "";
            this.AutoTDP = "GPUClock";
            this.FanCapable = true;
            this.FanToggleAddress = 0x44A;
            this.FanChangeAddress = 0x44B;
            this.MaxFanSpeed = 255;
            this.MinFanSpeed = 0;
            this.MinFanSpeedPercentage = 25;
            this.fanCurveTemperature = "0,0,0,0,0,0,0,0,0,0,30,30,30,30,40,40,50,50,70,70,100";
            this.fanCurvePackagePower = "0,0,0,30,30,40,40,50,60,60,80,90";

            this.MaxCPUClock = 4600;
            this.MinCPUClock = 1100;
            this.MinGPUClock = 400;
            this.MaxGPUClock = 2200;
        }
        public void enableFanControl()
        {
            WinRingEC_Management.ECRamWrite(FanToggleAddress, 0x01);
            Global_Variables.Global_Variables.fanControlEnabled = true;
        }
        public bool fanIsEnabled()
        {

            byte returnvalue = WinRingEC_Management.ECRamRead(FanToggleAddress);
            if (returnvalue == 0) { return false; } else { return true; }
        }
        public void disableFanControl()
        {
            WinRingEC_Management.ECRamWrite(FanToggleAddress, 0x00);
            Global_Variables.Global_Variables.fanControlEnabled = false;
        }
        public void readFanSpeed()
        {
            int fanSpeed = 0;

            byte returnvalue = WinRingEC_Management.ECRamRead(FanChangeAddress);

            double fanPercentage = Math.Round(100 * (Convert.ToDouble(returnvalue) / Global_Variables.Global_Variables.Device.MaxFanSpeed), 0);
            Global_Variables.Global_Variables.FanSpeed = fanPercentage;
        }
        public void setFanSpeed(int speedPercentage)
        {
            if (speedPercentage < MinFanSpeedPercentage && speedPercentage > 0)
            {
                speedPercentage = MinFanSpeedPercentage;
            }

            byte setValue = (byte)Math.Round(((double)speedPercentage / 100) * MaxFanSpeed, 0);
            WinRingEC_Management.ECRamWrite(FanChangeAddress, setValue);

            Global_Variables.Global_Variables.FanSpeed = speedPercentage;
        }
    }
    public class GenericDevice : HandheldDevice
    {
        public GenericDevice()
        {
            this.Manufacturer = "Generic";
            this.Motherboard = "Generic";
            this.AutoTDP = "None";
            this.FanCapable = false;
          
        }




    }
}
