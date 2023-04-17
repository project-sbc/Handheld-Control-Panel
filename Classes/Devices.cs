using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Handheld_Control_Panel.Classes
{
    public abstract class HandheldDevice
    {
        public string Manufacturer;
        public string Motherboard;

        public string CpuType;

        public bool FanCapable = false;
        public ushort FanToggleAddress;
        public ushort FanChangeAddress;
        public int MaxFanSpeed;
        public int MinFanSpeed;
        public int MinFanSpeedPercentage;

        public int MaxGPUClock;
        public int MinGPUClock;
        public int MaxCPUClock;
        public int MinCPUClock;


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
            this.Manufacturer = "GPD";
            this.Motherboard = "1619-04";

            this.FanCapable = true;
            this.FanToggleAddress = 0x275;
            this.FanChangeAddress = 0x1809;
            this.MaxFanSpeed = 184;
            this.MinFanSpeed = 20;
            this.MinFanSpeedPercentage = 20;

            this.MaxCPUClock = 4600;
            this.MinCPUClock = 1100;
            this.MinGPUClock = 400;
            this.MaxGPUClock = 2200;
        }
    }
    public class GPDWin4 : HandheldDevice
    {
        public GPDWin4()
        {
            this.Manufacturer = "GPD";
            this.Motherboard = "1618-04";

            this.FanCapable = false;
            this.FanToggleAddress = 0;
            this.FanChangeAddress = 0;
            this.MaxFanSpeed = 184;
            this.MinFanSpeed = 20;
            this.MinFanSpeedPercentage = 20;

            this.MaxCPUClock = 4600;
            this.MinCPUClock = 1100;
            this.MinGPUClock = 400;
            this.MaxGPUClock = 2200;
        }
    }
    public class GenericDevice : HandheldDevice
    {
        public GenericDevice()
        {
            this.Manufacturer = "";
            this.Motherboard = "";

            this.FanCapable = false;
          
        }




    }
}
