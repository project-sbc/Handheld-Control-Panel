using Nefarius.Drivers.HidHide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.VisualStyles;

namespace Handheld_Control_Panel.Classes.Global_Variables
{
    public static class Global_Variables
    {
        public static QuickAccessMenu qam;

        public static string xmlFile = AppDomain.CurrentDomain.BaseDirectory + "Resources\\Profiles.xml";

        //Processor global
        public static string cpuType = "";
        public static string MCHBAR = "";
        public static string processorName = "";

        //TDP global
        public static double readPL1 = 0;
        public static double readPL2 = 0;
        public static double setPL1 = 0;
        public static double setPL2 = 0;

        //AMD GPU CLOCK
        public static string gpuclk = "Default";

        //Shut down boolean to stop threads
        public static bool useRoutineThread = true;


        //brightness and volume setting
        public static int brightness = -1;
        public static int volume = 0;
        public static bool muteVolume = false;

        //hidhide
        public static HidHideControlService hidHide;

        //mouse mode
        public static bool mouseMode = false;

        //cpu settings
        public static int cpuMaxFrequency = 0;
        public static int cpuActiveCores = 0;
        public static int maxCpuCores = 1;
        public static int baseCPUSpeed = 1100;

        public static int EPP;
        //RTSS fps limit
        public static int FPSLimit = 0;

        //Profile 
        public static bool profileAutoApplied = false;
      
        public static Profiles_Management profiles;
        public static HotKey_Management hotKeys;

        //Power
        public static string powerStatus = "";
        public static int batteryLevel = -1;
        //network
        public static string networkStatus = "";
        //time
        public static string time = "";
        //controller status
        public static bool controllerConnected = false;

        //controller keyboard shortcuts
        public static Dictionary<ushort,ActionParameter> controllerHotKeyDictionary= new Dictionary<ushort, ActionParameter>();
        public static Dictionary<string, ActionParameter> KBHotKeyDictionary = new Dictionary<string, ActionParameter>();
        
        //kill controller loop thread
        public static bool killControllerThread = false;

        //display settings
        public static string resolution = "";
        public static string refreshRate = "";
        public static string scaling = "Default";

        public static List<string> resolutions = new List<string>();
        public static Dictionary<string, List<string>> resolution_refreshrates = new Dictionary<string, List<string>>();

        public static List<string> scalings = new List<string>();
        public static List<string> FPSLimits = new List<string>();
        public static List<string> FanModes = new List<string>();

   

        //amd power slide
        public static int AMDPowerSlide;

        //fan controls
        public static string fanDevice = "";
        public static bool fanControlDevice = false;
        public static bool fanControlEnable = false;
        public static string fanControlMode = "Hardware";
        public static int fanSpeed = 0;

        //cpu values
        public static double cpuTemp = 0;

        //language pack
        public static ResourceDictionary languageDict = new ResourceDictionary();

    }

    public struct ActionParameter
    {
        public string Action;
        public string Parameter;
    }
}
