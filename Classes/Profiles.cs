using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Handheld_Control_Panel.Classes
{
    public class Profiles: List<Profile_Main>
    {





    }



    public class Profile_Main
    {
        //profile name is also the folder name
        public string ProfileName { get; set; }
        public Profile_Info profile_Info { get; set; }
        public Profile_Exe profile_Exe { get; set; }
        public Profile_Parameters profile_Parameters { get; set; }
        public Profile_ControllerMapping profile_ControllerMapping { get; set; }

    }
    public class Profile_Info
    {
        
        public string ProfileExe { get; set; }
        public bool DefaultProfile
        {
            get { return defaultProfile; }
            set
            {
                defaultProfile = value;
                if (value == true) { VisibilityDefaultProfile = Visibility.Visible; } else { VisibilityDefaultProfile = Visibility.Collapsed; }
            }
        }
        public bool defaultProfile { get; set; }
        public bool activeProfile { get; set; }
        public bool ActiveProfile
        {
            get { return activeProfile; }
            set
            {
                activeProfile = value;
                if (value == true) { VisibilityActiveProfile = Visibility.Visible; } else { VisibilityActiveProfile = Visibility.Collapsed; }
            }
        }
        public Visibility VisibilityActiveProfile { get; set; } = Visibility.Collapsed;
        public Visibility VisibilityDefaultProfile { get; set; } = Visibility.Collapsed;

    }
    public class Profile_Exe
    {
        public string Exe_Path { get; set; }
        public string Exe_Type { get; set; }
        public string Exe_ID { get; set; }
        public string Exe_Image_Path { get; set; }
        public int LastLaunched { get; set; } = 0;
        public int NumberLaunches { get; set; } = 0;

    }
    public class Profile_Parameters
    {
        public string Offline_TDP1 { get; set; } = "";
        public string Offline_TDP2 { get; set; } = "";
        public string Offline_ActiveCores { get; set; } = "";
        public string Offline_MAXCPU { get; set; } = "";
        public string Offline_FPSLimit { get; set; } = "";
        public string Offline_EPP { get; set; } = "";
        public string Offline_GPUCLK { get; set; } = "";
        public string Online_TDP1 { get; set; } = "";
        public string Online_TDP2 { get; set; } = "";
        public string Online_ActiveCores { get; set; } = "";
        public string Online_MAXCPU { get; set; } = "";
        public string Online_FPSLimit { get; set; } = "";
        public string Online_EPP { get; set; } = "";
        public string Online_GPUCLK { get; set; } = "";
        public int AutoTDP_CPU_Offset { get; set; } = 0;
        public int AutoTDP_GPU_Offset { get; set; } = 0;
    }
    public class Profile_ControllerMapping
    {

    }
}
