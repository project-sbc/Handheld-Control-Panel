using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Handheld_Control_Panel.Classes
{
    public static class Load_Settings
    {
        public static object lockObjectSettings = new object();
        public static Settings loadSettings(string filename)
        {
            lock(lockObjectSettings)
            {
                if (File.Exists(filename))
                {
                    using (StreamReader sr = new StreamReader(filename))
                    {
                        XmlSerializer xmls = new XmlSerializer(typeof(Settings));
                        return xmls.Deserialize(sr) as Settings;
                    }
                }
            }
           

            
            Settings newSettings = new Settings();
            return newSettings;
        }
    }

    public class Settings
    {
        public string systemAccent { get; set; } = "Teal";
        public string language { get; set; } = "English";
        public string IntelMMIOMSR { get; set; } = "MMIO+MSR";
        public int minTDP { get; set; } = 5;
        public int maxTDP { get; set; } = 35;
        public bool checkUpdatesAtStartUp { get; set; } = true;
        public bool useHIDHideAndVIGEM { get; set; } = false;
        public bool dockWindowRight { get; set; } = true;
        public int maxGPUCLK { get; set; } = 1500;
        public bool combineTDP = true;
        public bool autoStartRTSS = false;
        public string SystemTheme = "Dark";
        public DateTime lastCheckUpdate;
        public string directorySteam = "";
        public string directoryPlaynite = "";
        public string directoryEpicGames = "";
        public int maxRTSSFPSLimit = 60;
        public string GUID="";
        public string instanceID="";
        public string qamUserControls = "Usercontrol_Bluetooth1;Usercontrol_Wifi1;Usercontrol_MouseMode1;Usercontrol_Controller0;Divider1;Usercontrol_Volume1;Usercontrol_VolumeMute1;Usercontrol_Brightness1;Divider1;Usercontrol_Resolution1;Usercontrol_RefreshRate1;Usercontrol_Scaling1;Usercontrol_FPSLimit1;Divider1;Usercontrol_TDP1;Usercontrol_TDP21;Usercontrol_EPP1;Usercontrol_ActiveCores1;Usercontrol_MaxCPUFrequency1;Usercontrol_GPUCLK1;Divider1;UserControl_FanControl1;";
        public bool enableNotifications = true;
        public string appSortMethod = "Sort_Method_ProfileName";
        public double joystickDeadzone = 1200;
        public string fanCurveTemperature = "";
        public string fanCurvePackagePower = "";
        public bool fanAutoModeTemp = true;
        public bool startAutoFan = false;
        public bool startSafeMode = true;
        public bool hideSplashScreen = false;
        public string directoryRTSS = "";
        public bool enableViGEmController = true;
        public void Save()
        {
            //use lock to prevent multiple threads from using stream writer which will cause an error
            lock (Load_Settings.lockObjectSettings)
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Settings"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Settings");
                }

                using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Settings\\Settings.xml"))
                {
                    XmlSerializer xmls = new XmlSerializer(typeof(Settings));
                    xmls.Serialize(sw, this);
                }
            }
           
        }

       
    }

   
}
