using Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Networking.NetworkOperators;

namespace Handheld_Control_Panel.Classes
{
    public class Profiles: List<Profile_Main>
    {
        private string profileDirectory = AppDomain.CurrentDomain.BaseDirectory + "Profiles\\";
 
        public Profile_Main activeProfile = null;
        public Profile_Main defaultProfile = null;
        public Profile_Main editingProfile = null;
        public Profiles()
        {
            //populates list

            if (!Directory.Exists(profileDirectory))
            {
                Directory.CreateDirectory(profileDirectory);
            }

            string[] folders = Directory.GetDirectories(profileDirectory,"", SearchOption.TopDirectoryOnly);

            foreach (string folder in folders)
            {
                loadProfile(Path.GetDirectoryName(folder), true);
            }
        }


        //Routines below
        public void loadProfile(string profileName, bool firstStart)
        {
            //this is an all encompassing load routine, for both first start, during run (like when you dont save changes to a profile you reload the xml to make it back to the way it was)
            //or imports from outside
            string folder = AppDomain.CurrentDomain.BaseDirectory + profileName;

            Profile_Main profile;
            if (!firstStart)
            {
                if (File.Exists(folder + "Profile_Main.xml"))
                {
                    profile = (Profile_Main)Profiles_XML_SaveLoad.Load_XML(folder + "Profile_Main.xml", "Profile_Main");

                }
                else
                {
                    profile = new Profile_Main();
                }
            }
            else
            {
                profile = Global_Variables.Global_Variables.profiles.First(o => o.ProfileName == profileName);
                //if cant find profile then return doing nothing
                if (profile == null) { return; }
            }

            profile.ProfileName = profileName;

            bool saveXML = false; //this is a bool to determine if an import was done (the individual info/exe/parameter/controllermapping xml) and to save the overall xml with the imported xml

            if (File.Exists(folder + "Profile_Info.xml"))
            {
                Profile_Info profile_Info = (Profile_Info)Profiles_XML_SaveLoad.Load_XML(folder + "Profile_Info.xml", "Profile_Info");
                profile.profile_Info = profile_Info;
                File.Delete(folder + "Profile_Info.xml");
                saveXML = true;
            }
            if (File.Exists(folder + "Profile_Exe.xml"))
            {
                Profile_Exe profile_Exe = (Profile_Exe)Profiles_XML_SaveLoad.Load_XML(folder + "Profile_Exe.xml", "Profile_Exe");
                profile.profile_Exe = profile_Exe;
                File.Delete(folder + "Profile_Exe.xml");
                saveXML = true;
            }
            if (File.Exists(folder + "Profile_Parameters.xml"))
            {
                Profile_Parameters profile_Parameters = (Profile_Parameters)Profiles_XML_SaveLoad.Load_XML(folder + "Profile_Parameters.xml", "Profile_Parameters");
                profile.profile_Parameters = profile_Parameters;
                File.Delete(folder + "Profile_Info.xml");
                saveXML = true;
            }
            if (File.Exists(folder + "Profile_ControllerMapping.xml"))
            {
                Profile_ControllerMapping profile_ControllerMapping = (Profile_ControllerMapping)Profiles_XML_SaveLoad.Load_XML(folder + "Profile_ControllerMapping.xml", "Profile_ControllerMapping");
                profile.profile_ControllerMapping = profile_ControllerMapping;
                File.Delete(folder + "Profile_ControllerMapping.xml");
                saveXML = true;
            }
            if (saveXML)
            {
                Profiles_XML_SaveLoad.Save_XML(Path.GetDirectoryName(folder), "Profile_Main", profile);
            }
            //only add to the list when it is starting up, DO NOT ADD PROFILES TO LIST WHEN LOADING AFTER INITIALIZED OR IT WILL MAKE DUPLICATES
            if (firstStart) { this.Add(profile); }

        }
        public void createProfile(GameLauncherItem gli = null)
        {
            //check if game launcher item GLI has a profile with the game name already first, because if it does then we can return immediately
            if (gli != null)
            {
                if (Global_Variables.Global_Variables.profiles.First(o=>o.ProfileName == gli.gameName) != null) { return; }
            }

            Profile_Main profile = new Profile_Main();


            //establish new profile name, for non game launcher items get the next available 'new profile' name else use GLI game name
             
            if (gli == null)
            {
                int x = 1;
                string newProfileName = "New Profile " + x.ToString();
                while (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Profiles\\" + newProfileName))
                {
                    x++;
                    newProfileName = "New Profile " + x.ToString();
                }
            }
            else 
            { 
                profile.ProfileName = gli.gameName;
                profile.profile_Info.ProfileExe = gli.exe;
                profile.profile_Exe.Exe_ID = gli.gameID;
                profile.profile_Exe.Exe_Type = gli.appType;
                profile.profile_Exe.Exe_LaunchCommand = gli.launchCommand;
                profile.profile_Exe.Exe_Path = gli.path;
                profile.profile_Exe.Exe_Image_Path = gli.imageLocation;
            }
            //create the folder for the xml files since this is a new profile
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Profiles\\" + profile.ProfileName))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Profiles\\" + profile.ProfileName);
            }

            //save the xml
            Profiles_XML_SaveLoad.Save_XML(profile.ProfileName, "Profile_Main", profile);

            this.Add(profile);

        }
        public void createProfilesForGames()
        {
            //routine to auto add games to game launcher
            Notification_Management.ShowInWindow(Application.Current.Resources["Notification_GameSyncing"].ToString(), Notification.Wpf.NotificationType.Information);


            List<GameLauncherItem> result = Game_Management.syncGame_Library();

            if (result.Count > 0)
            {

                foreach (GameLauncherItem item in result)
                {
                    //below makes the new profile
                    Profile_Main gameIDProfile = this.Find(o => o.profile_Exe.Exe_ID == item.gameID);
                    if (gameIDProfile == null)
                    {
                        createProfile(item);
                    }
                }
            }

            Notification_Management.ShowInWindow(Application.Current.Resources["Notification_GameSyncDone"].ToString(), Notification.Wpf.NotificationType.Information);

        }
        public void openGame(string name)
        {
            Profile_Main profile = Global_Variables.Global_Variables.profiles.First(p => p.ProfileName == name);

            if (profile != null)
            {
                profile.applyProfile(true, true);

                Classes.Task_Scheduler.Task_Scheduler.runTask(() => Game_Management.LaunchApp(profile.profile_Exe.Exe_ID, profile.profile_Exe.Exe_Type, profile.profile_Exe.Exe_LaunchCommand, profile.profile_Exe.Exe_Path));

                profile.profile_Exe.NumberLaunches = profile.profile_Exe.NumberLaunches + 1;

                profile.profile_Exe.LastLaunched = DaysBetween(DateTime.ParseExact("2023-01-01", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture), DateTime.Today);
                Profiles_XML_SaveLoad.Save_XML(name,"Profile_Main",profile);

            }
        }
        private int DaysBetween(DateTime d1, DateTime d2)
        {
            TimeSpan span = d2.Subtract(d1);
            return (int)span.TotalDays;
        }
    }


    public class Profile_Main
    {
        //profile name is also the folder name
        public string ProfileName { get; set; }
        public Profile_Info profile_Info { get; set; }
        public Profile_Exe profile_Exe { get; set; }
        public Profile_Parameters profile_Parameters { get; set; }
        public Profile_ControllerMapping profile_ControllerMapping { get; set; }

        public void applyProfile(bool autoApplied, bool changeDisplay)
        {
            if (autoApplied) { Global_Variables.Global_Variables.profileAutoApplied = true; } else { Global_Variables.Global_Variables.profileAutoApplied = false; }

            //remove active profile flag for current
            if (Global_Variables.Global_Variables.profiles.activeProfile != null)
            {
                Global_Variables.Global_Variables.profiles.activeProfile.profile_Info.ActiveProfile = false;

            }
            //apply active profile flag
            Global_Variables.Global_Variables.profiles.activeProfile = this;
           
            string powerStatus = SystemParameters.PowerLineStatus.ToString();

            if (changeDisplay)
            {
                if (profile_Exe.Resolution != "")
                {
                    Display_Management.Display_Management.SetDisplayResolution(profile_Exe.Resolution);
                }
                if (profile_Exe.RefreshRate != "")
                {
                    Display_Management.Display_Management.SetDisplayRefreshRate(profile_Exe.RefreshRate);
                }
            }


            //if setting is false for separating charger and battery, then it defaults to offline values 
            if (!profile_Parameters.SeparateChargerBattery)
            {
                powerStatus = "Offline";
            }

            switch (powerStatus)
            {
                case "Online":
                    if (profile_Parameters.Online_ActiveCores != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => CoreParking_Management.CoreParking_Management.changeActiveCores(Convert.ToInt32(profile_Parameters.Online_ActiveCores))); }
                    if (profile_Parameters.Online_EPP != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => EPP_Management.EPP_Management.changeEPP(Convert.ToInt32(profile_Parameters.Online_EPP))); }
                    if (profile_Parameters.Online_FPSLimit != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => RTSS.setRTSSFPSLimit(Convert.ToInt32(profile_Parameters.Online_FPSLimit))); }
                    if (profile_Parameters.Online_GPUCLK != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => GPUCLK_Management.GPUCLK_Management.changeAMDGPUClock(Convert.ToInt32(profile_Parameters.Online_GPUCLK))); }
                    if (profile_Parameters.Online_MAXCPU != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => MaxProcFreq_Management.MaxProcFreq_Management.changeCPUMaxFrequency(Convert.ToInt32(profile_Parameters.Online_MAXCPU))); }
                    if (profile_Parameters.Online_TDP1 != "" && profile_Parameters.Online_TDP2 == "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => TDP_Management.TDP_Management.changeTDP(Convert.ToInt32(profile_Parameters.Online_TDP1), Convert.ToInt32(profile_Parameters.Online_TDP1))); }
                    if (profile_Parameters.Online_TDP1 != "" && profile_Parameters.Online_TDP2 != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => TDP_Management.TDP_Management.changeTDP(Convert.ToInt32(profile_Parameters.Online_TDP1), Convert.ToInt32(profile_Parameters.Online_TDP2))); }

                    break;
                case "Offline":
                    if (profile_Parameters.Offline_ActiveCores != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => CoreParking_Management.CoreParking_Management.changeActiveCores(Convert.ToInt32(profile_Parameters.Offline_ActiveCores))); }
                    if (profile_Parameters.Offline_EPP != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => EPP_Management.EPP_Management.changeEPP(Convert.ToInt32(profile_Parameters.Offline_EPP))); }
                    if (profile_Parameters.Offline_FPSLimit != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => RTSS.setRTSSFPSLimit(Convert.ToInt32(profile_Parameters.Offline_FPSLimit))); }
                    if (profile_Parameters.Offline_GPUCLK != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => GPUCLK_Management.GPUCLK_Management.changeAMDGPUClock(Convert.ToInt32(profile_Parameters.Offline_GPUCLK))); }
                    if (profile_Parameters.Offline_MAXCPU != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => MaxProcFreq_Management.MaxProcFreq_Management.changeCPUMaxFrequency(Convert.ToInt32(profile_Parameters.Offline_MAXCPU))); }
                    if (profile_Parameters.Offline_TDP1 != "" && profile_Parameters.Offline_TDP2 == "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => TDP_Management.TDP_Management.changeTDP(Convert.ToInt32(profile_Parameters.Offline_TDP1), Convert.ToInt32(profile_Parameters.Offline_TDP1))); }
                    if (profile_Parameters.Offline_TDP1 != "" && profile_Parameters.Offline_TDP2 != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => TDP_Management.TDP_Management.changeTDP(Convert.ToInt32(profile_Parameters.Offline_TDP1), Convert.ToInt32(profile_Parameters.Offline_TDP2))); }

                    break;

                default: break;
            }


            Notification_Management.Show(Application.Current.Resources["Notification_ProfileApplied"] + ": " + ProfileName, false);

        }
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
        public bool ActiveProfile
        {
            get { return activeProfile; }
            set
            {
                activeProfile = value;
                if (value == true) { VisibilityActiveProfile = Visibility.Visible; } else { VisibilityActiveProfile = Visibility.Collapsed; }
            }
        }
        public bool defaultProfile { get; set; } = false;
        public bool activeProfile { get; set; } = false;
       
        public Visibility VisibilityActiveProfile { get; set; } = Visibility.Collapsed;
        public Visibility VisibilityDefaultProfile { get; set; } = Visibility.Collapsed;

    }
    public class Profile_Exe
    {
        public string Exe_Path { get; set; }
        public string Exe_Type { get; set; }
        public string Exe_ID { get; set; }
        public string Exe_LaunchCommand { get; set; }
        public string Exe_Image_Path { get; set; }
        public int LastLaunched { get; set; } = 0;
        public int NumberLaunches { get; set; } = 0;
        public bool Favorite
        {
            get
            {
                if (favoriteIconVisibility == Visibility.Visible) { return true; } else { return false; }
            }
            set
            {

                if (value == true) { favoriteIconVisibility = Visibility.Visible; } else { favoriteIconVisibility = Visibility.Collapsed; }
            }
        }
        public Visibility favoriteIconVisibility { get; set; } = Visibility.Collapsed;
        public string Resolution { get; set; } = "";
        public string RefreshRate { get; set; } = "";

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
        public bool SeparateChargerBattery { get; set; }
        public int AutoTDP_CPU_Offset { get; set; } = 0;
        public int AutoTDP_GPU_Offset { get; set; } = 0;
    }
    public class Profile_ControllerMapping
    {

    }
}
