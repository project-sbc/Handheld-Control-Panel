using ControlzEx.Standard;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using SharpDX;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using Windows.Networking.NetworkOperators;
using YamlDotNet.Core.Tokens;
using Path = System.IO.Path;

namespace Handheld_Control_Panel.Classes
{
    public class Profiles_Management: List<Profile>
    {
        private string profileDirectory = AppDomain.CurrentDomain.BaseDirectory + "Profiles";
        public Profile activeProfile=null;
        public Profile editingProfile = null;
        public Profile defaultProfile = null;
        public Profiles_Management()
        {
            //populates list
            if (!Directory.Exists(profileDirectory))
            {
                Directory.CreateDirectory(profileDirectory);
            }

            string[] files = Directory.GetFiles(profileDirectory, "*.xml", SearchOption.TopDirectoryOnly);

            foreach (string file in files)
            {
                StreamReader sr = new StreamReader(file);
                XmlSerializer xmls = new XmlSerializer(typeof(Profile));
                this.Add((Profile)xmls.Deserialize(sr));
                sr.Dispose();
                xmls = null;
    
            }

        }
        private string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public void loadProfile(string profilename)
        {
            Profile loadingProfile = this.Find(o => o.ProfileName == profilename);
            if (loadingProfile != null)
            {
                if (File.Exists(profileDirectory + "\\" + profilename + ".xml"))
                {
                    using (StreamReader sw = new StreamReader(profileDirectory + "\\" + profilename + ".xml"))
                    {
                        XmlSerializer xmls = new XmlSerializer(typeof(Profile));
                        loadingProfile = (Profile)xmls.Deserialize(sw);
                    }
                }
              

            }
        }
       
        public void createProfileForGame(string profileName, string path, string gameID, string launchcommand, string apptype, string exe, string imageLocation)
        {
            Profile newProfile = new Profile();
            profileName = RemoveSpecialCharacters(profileName);
            int x = 1;
            if (File.Exists(profileDirectory + profileName + ".xml"))
            {
                //END THE ROUTINE IF THE GAME ALREADY HAS A PROFILE
                newProfile = null;
                return;
            }
    
            newProfile.ProfileName = profileName;
            newProfile.GameID = gameID;
            newProfile.Path = path;
            newProfile.LaunchCommand = launchcommand;
            newProfile.AppType = apptype;

            
            if (exe != null)
            {
                if (File.Exists(exe))
                {
                    exe = Path.GetFileNameWithoutExtension(exe);
                }
                if (exe.Length > 4)
                {
                    if (exe.Contains(".exe"))
                    {
                        exe = exe.Substring(0, exe.Length - 4);
                    }
                }

                newProfile.Exe = exe;
            }
            if (imageLocation != null)
            {
                newProfile.ImageLocation = imageLocation;
            }

           

            this.Add(newProfile);
            Global_Variables.Global_Variables.profiles.SaveToXML(newProfile);




        }
                

        public async Task syncGamesToProfile()
        {
            //gets list of steam games from library.vdf file, then makes profiles for those without one
            Notification_Management.ShowInWindow(Application.Current.Resources["Notification_GameSyncing"].ToString(), Notification.Wpf.NotificationType.Information);
            

            List<GameLauncherItem> result = Game_Management.syncGame_Library();

            if (result.Count > 0)
            {
      
                foreach (GameLauncherItem item in result)
                {
                    Profile gameIDProfile = this.Find(o => o.GameID == item.gameID);
                    if (gameIDProfile == null)
                    {
                        Global_Variables.Global_Variables.profiles.createProfileForGame(item.gameName, item.path,item.gameID,item.launchCommand,item.appType, item.exe, item.imageLocation);
                    }
                }
               
            }

            Notification_Management.ShowInWindow(Application.Current.Resources["Notification_GameSyncDone"].ToString(), Notification.Wpf.NotificationType.Information);

        }


        private int DaysBetween(DateTime d1, DateTime d2)
        {
            TimeSpan span = d2.Subtract(d1);
            return (int)span.TotalDays;
        }
        public void changeProfileFavorite(string ID)
        {
            Profile profile = Global_Variables.Global_Variables.profiles.First(p => p.ID == ID);

            if (profile != null)
            {
                profile.Favorite = !profile.Favorite;
          
                Global_Variables.Global_Variables.profiles.SaveToXML(profile);
            }


        }

        public void openProgram(string ID)
        {
            Profile profile = Global_Variables.Global_Variables.profiles.First(p => p.ID== ID);

            if (profile != null)
            {

                profile.applyProfile(true, true);

                Classes.Task_Scheduler.Task_Scheduler.runTask(() => Game_Management.LaunchApp(profile.GameID, profile.appType, profile.LaunchCommand, profile.Path));

                profile.NumberLaunches = profile.NumberLaunches + 1;


                profile.LastLaunched = DaysBetween(DateTime.ParseExact("2023-01-01", "yyyy-MM-dd",
                                   System.Globalization.CultureInfo.InvariantCulture), DateTime.Today);
                Global_Variables.Global_Variables.profiles.SaveToXML(profile);
        
            }


        }
        public void setCurrentDefaultProfileToFalse(string ID)
        {
            //changes 
            foreach (Profile profile in this)
            {
                if (profile.ID == ID)
                {
                    profile.DefaultProfile = false;
                }
            }

        }
      
        public void deleteProfile(Profile profile)
        {
            if (profile != null)
            {
                string profileName = profile.ProfileName;

                if (Global_Variables.Global_Variables.profiles.activeProfile != null)
                {
                    if (Global_Variables.Global_Variables.profiles.activeProfile == profile)
                    {
                        Global_Variables.Global_Variables.profiles.activeProfile = null;
                    }
                }
                if (Global_Variables.Global_Variables.profiles.defaultProfile != null)
                {
                    if (Global_Variables.Global_Variables.profiles.defaultProfile == profile)
                    {
                        Global_Variables.Global_Variables.profiles.defaultProfile = null;
                    }
                }
                
                if (File.Exists(profileDirectory + profileName + ".xml"))
                {
                    File.Delete(profileDirectory + profileName + ".xml");
                }

                this.Remove(profile);
            }
          

        }
        public void SaveToXML(Profile profile)
        {
            //Profile profile = this.Find(o => o.ProfileName == profileName);
            if (profile != null)
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Profiles\\" + profile.ProfileName + ".xml");
                    XmlSerializer xmls = new XmlSerializer(typeof(Profile));
                    xmls.Serialize(sw, profile);
                    sw.Dispose();
                    xmls = null;
                }


                    );
                
            }

          

        }
        public void addNewProfile(Profile copyProfile)
        {
            
          
            string newProfileName = "NewProfile";
            


            Profile profile = new Profile();

            if (copyProfile != null)
            {
                profile = copyProfile;
            }

            int x = 1;
            if (File.Exists(profileDirectory + newProfileName + ".xml"))
            {
                while (File.Exists(profileDirectory + newProfileName + x.ToString() + ".xml"))
                {
                    x++;
                }
                newProfileName = newProfileName + x.ToString();
            }

            profile.ProfileName = newProfileName;

            this.Add(profile);
            Global_Variables.Global_Variables.profiles.SaveToXML(profile);

        }

      
    }

    public class Profile
    {
        public string ID { get; set; }
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
        public string ProfileName { get; set; } = "";
        public string Exe { get; set; } = "";
        public string Resolution { get; set; } = "";
        public string RefreshRate { get; set; } = "";
        public string path { get; set; } = "";
        public bool SeparateChargerBattery { get; set; }
        public string Path 
        {
            get
            {
                return path;
            }
            set
            {
                if (value != "")
                {
                    if(File.Exists(value))
                    {
                        Exe = System.IO.Path.GetFileNameWithoutExtension(value);
                    }
                    else
                    {
                        Exe = "";
                    }
                }
                else
                {
                    Exe = "";
                }

                path = value;
            }
        }

        public string appType { get; set; } = "";
        public string LaunchCommand { get; set; } = "";

        public string imageLocation { get; set; } = "";
        public string ImageLocation
        {
            get
            {
                return imageLocation;
            }
            set
            {
                if (value != "")
                {
                    if (File.Exists(value.Replace("%20", " ")))
                    {
                        if (appType == "Microsoft Store")
                        {
                            //imageIcon = new BitmapImage(new Uri(value.Replace("%20", " ")));
                        }
                        else
                        {
                            //imageApp = new BitmapImage(new Uri(value.Replace("%20", " ")));
                        }
                        
                    }
                   


                }
                else
                {
                    if (Path != "" && appType != "Steam")
                    {

                        if (File.Exists(Path))
                        {

                            using (Icon ico = Icon.ExtractAssociatedIcon(Path))
                            {
                                //imageIcon = Imaging.CreateBitmapSourceFromHIcon(ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            }

                        }
                    }

                }
                imageLocation = value;
                
            }
        }


        public bool Favorite
        {
            get {
                if (favoriteIconVisibility == Visibility.Visible) { return true; } else { return false; }
            }
            set
            {
             
                if (value == true) { favoriteIconVisibility = Visibility.Visible; } else { favoriteIconVisibility = Visibility.Collapsed; }
            }
        }
        public Visibility favoriteIconVisibility { get; set; } = Visibility.Collapsed;
        //public ImageSource imageIcon { get; set; } = null;
        //public ImageSource imageApp { get; set; } = null;
        public int LastLaunched { get; set; } = 0;
        public int NumberLaunches { get; set; } = 0;
        public string AppType
        {
            get { return appType; }
            set
            {
                appType = value;
                switch (value)
                {
                    case "Steam":
                        icon = PackIconSimpleIconsKind.Steam;
                        iconMaterial = PackIconMaterialKind.None;
                        iconVisibility = Visibility.Visible;
                        if (ImageLocation == "")
                        {
                            string imageDirectory = Properties.Settings.Default.directorySteam + "\\appcache\\librarycache\\" + GameID + "_header";
                            if (File.Exists(imageDirectory + ".jpg"))
                            {
                                //imageApp = new BitmapImage(new Uri(imageDirectory + ".jpg", UriKind.RelativeOrAbsolute));
                            }
                            else
                            {
                                if (File.Exists(imageDirectory + ".png"))
                                {
                                    //imageApp = new BitmapImage(new Uri(imageDirectory + ".png", UriKind.RelativeOrAbsolute));
                                }

                            }
                        }
                        break;
                    case "Battle.net":
                        icon = PackIconSimpleIconsKind.Battledotnet;
                        iconMaterial = PackIconMaterialKind.None;
                        iconVisibility = Visibility.Visible;
                        break;
                    case "Epic Games":
                        icon = PackIconSimpleIconsKind.EpicGames;
                        iconMaterial = PackIconMaterialKind.None;
                        iconVisibility = Visibility.Visible;
                        break;
                    case "GOG Galaxy":
                        icon = PackIconSimpleIconsKind.GoGdotcom;
                        iconMaterial = PackIconMaterialKind.None;
                        iconVisibility = Visibility.Visible;
                        break;
                    case "Exe":
                        icon = PackIconSimpleIconsKind.None;
                        iconMaterial = PackIconMaterialKind.ApplicationCogOutline;
                        iconMaterialVisibility = Visibility.Visible;
                        break;
                    case "Microsoft Store":
                        icon = PackIconSimpleIconsKind.Microsoft;
                        iconMaterial = PackIconMaterialKind.None;
                        iconVisibility = Visibility.Visible;
                        break;

                    default:
                        icon = PackIconSimpleIconsKind.None;
                        iconMaterial = PackIconMaterialKind.None;
                  
                        break;
                }

            }
        }

        public string GameID { get; set; } = "";
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

        public PackIconSimpleIconsKind icon {get;set;}
        public PackIconMaterialKind iconMaterial {get;set;}

        public Visibility iconVisibility { get; set; } = Visibility.Collapsed;
        public Visibility iconMaterialVisibility { get; set; } = Visibility.Collapsed;
       





        

       

        public void applyProfile(bool autoApplied, bool changeDisplay)
        {
            if (autoApplied) { Global_Variables.Global_Variables.profileAutoApplied = true; } else { Global_Variables.Global_Variables.profileAutoApplied = false; }

            //remove active profile flag for current
            if (Global_Variables.Global_Variables.profiles.activeProfile != null) 
            { 
                Global_Variables.Global_Variables.profiles.activeProfile.ActiveProfile = false;

            }
            //apply active profile flag
            Global_Variables.Global_Variables.profiles.activeProfile = this;
            ActiveProfile = true;
            string powerStatus = SystemParameters.PowerLineStatus.ToString();

            if (changeDisplay)
            {
                if (Resolution != "")
                {
                    Display_Management.Display_Management.SetDisplayResolution(Resolution);
                }
                if (RefreshRate != "")
                {
                    Display_Management.Display_Management.SetDisplayRefreshRate(RefreshRate);
                }
            }


           //if setting is false for separating charger and battery, then it defaults to offline values 
            if (!SeparateChargerBattery)
            {
                powerStatus = "Offline";
            }

            switch (powerStatus)
            {
                case "Online":
                    if (Online_ActiveCores != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => CoreParking_Management.CoreParking_Management.changeActiveCores(Convert.ToInt32(Online_ActiveCores))); }
                    if (Online_EPP != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => EPP_Management.EPP_Management.changeEPP(Convert.ToInt32(Online_EPP))); }
                    if (Online_FPSLimit != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => RTSS.setRTSSFPSLimit(Convert.ToInt32(Online_FPSLimit))); }
                    if (Online_GPUCLK != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => GPUCLK_Management.GPUCLK_Management.changeAMDGPUClock(Convert.ToInt32(Online_GPUCLK))); }
                    if (Online_MAXCPU != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => MaxProcFreq_Management.MaxProcFreq_Management.changeCPUMaxFrequency(Convert.ToInt32(Online_MAXCPU))); }
                    if (Online_TDP1 != "" && Online_TDP2 == "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => TDP_Management.TDP_Management.changeTDP(Convert.ToInt32(Online_TDP1), Convert.ToInt32(Online_TDP1))); }
                    if (Online_TDP1 != "" && Online_TDP2 != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => TDP_Management.TDP_Management.changeTDP(Convert.ToInt32(Online_TDP1), Convert.ToInt32(Online_TDP2))); }
                
                    break;
                case "Offline":
                    if (Offline_ActiveCores != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => CoreParking_Management.CoreParking_Management.changeActiveCores(Convert.ToInt32(Offline_ActiveCores))); }
                    if (Offline_EPP != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => EPP_Management.EPP_Management.changeEPP(Convert.ToInt32(Offline_EPP))); }
                    if (Offline_FPSLimit != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => RTSS.setRTSSFPSLimit(Convert.ToInt32(Offline_FPSLimit))); }
                    if (Offline_GPUCLK != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => GPUCLK_Management.GPUCLK_Management.changeAMDGPUClock(Convert.ToInt32(Offline_GPUCLK))); }
                    if (Offline_MAXCPU != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => MaxProcFreq_Management.MaxProcFreq_Management.changeCPUMaxFrequency(Convert.ToInt32(Offline_MAXCPU))); }
                    if (Offline_TDP1 != "" && Offline_TDP2 == "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => TDP_Management.TDP_Management.changeTDP(Convert.ToInt32(Offline_TDP1), Convert.ToInt32(Offline_TDP1))); }
                    if (Offline_TDP1 != "" && Offline_TDP2 != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => TDP_Management.TDP_Management.changeTDP(Convert.ToInt32(Offline_TDP1), Convert.ToInt32(Offline_TDP2))); }

                    break;

                default: break;
            }


            Notification_Management.Show(Application.Current.Resources["Notification_ProfileApplied"] + ": " + ProfileName,false);

        }
    }

    public static class Profile_Events
    {
        public static event EventHandler separateChargerBatteryEvent;
        public static void raiseSeparateChargerBatteryEvent()
        {
            separateChargerBatteryEvent?.Invoke(null, EventArgs.Empty);
        }
    }

}
