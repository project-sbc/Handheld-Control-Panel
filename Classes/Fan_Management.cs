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
using ControlzEx.Standard;

namespace Handheld_Control_Panel.Classes.Fan_Management
{
    public static class Fan_Management
    {
      

       


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
                    case "GPDWin4":
                        GPDWin4 w4 = (GPDWin4)Global_Variables.Global_Variables.Device;
                        Global_Variables.Global_Variables.fanControlEnabled = w4.fanIsEnabled();
                        w4 = null;
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
                    case "GPDWin4":
                        GPDWin4 win4 = (GPDWin4)Global_Variables.Global_Variables.Device;
                        win4.readFanSpeed();
                        win4 = null;
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
                    case "GPDWin4":
                        GPDWin4 win4 = (GPDWin4)Global_Variables.Global_Variables.Device;
                        win4.setFanSpeed(speedPercentage);
                        win4 = null;
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

   
   

}
