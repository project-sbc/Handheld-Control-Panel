using Handheld_Control_Panel.Classes.Global_Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Handheld_Control_Panel.Classes
{
    public static class Start_Up
    {
   

        public static void Start_Routine()
        {
            //error catch


            //run all routines to get device ready

            //test code here

            //test code

            //check for updates first
            //Update_Software.Update_Software.checkForUpdates(true);

            //Load XML profile file
            //XML_Management.Load_XML_File.load_XML_File();

            //check steam/playnite installed for added features
            //Steam_Management.setSteamDirectory();
            //Playnite_Management.setPlayniteDirectory();
            //EpicGames_Management.setEpicGamesDirectory();


            //XML_Management.Manage_XML_Profiles.syncEpicGameToProfile();
            //XML_Management.Manage_XML_Profiles.syncSteamGameToProfile();
            
            //load controller hotkey dictionary
            //XML_Management.Manage_XML_ControllerHotKeys.loadGlobalDictionaryForControllerHotKeys();
            //XML_Management.Manage_XML_ControllerHotKeys.loadGlobalDictionaryForKBHotKeys();

            //start controller timer to read controller states
            //Controller_Management.Controller_Management.start_Controller_Management();

            //get cpu information
            //TDP_Management.TDP_Management.determineCPU();

            //check to make sure driver isn't blocked for intel (checks for intel in routine)
            //TDP_Management.TDP_Management.checkDriverBlockRegistry();

            //Make sure powercfg profile has coreparking and maxprocfreq unhidden or otherwise those wont work
           // MaxProcFreq_Management.MaxProcFreq_Management.unhidePowercfgMaxProcFreq();
           // CoreParking_Management.CoreParking_Management.unhidePowercfgCoreParking();
           // EPP_Management.EPP_Management.unhidePowercfgEPP();

            //get max core count
           // Global_Variables.Global_Variables.maxCpuCores = new ManagementObjectSearcher("Select * from Win32_Processor").Get().Cast<ManagementBaseObject>().Sum(item => int.Parse(item["NumberOfCores"].ToString()));
            
            //load lists (resolutions, refresh rates, scalings)
           // Display_Management.Display_Management.generateDisplayResolutionAndRateList();
            
          //  XML_Management.Manage_XML_Profiles.generateGlobalVariableProfileToExeList();

            //load language resource
           // loadLanguage();

            //start task scheduler
          //  Task_Scheduler.Task_Scheduler.startScheduler();

            //get motherboard info
          //  Fan_Management.Fan_Management.determineFanDevice();

            //check if RTSS should be started at startup
          //  RTSS.checkStartRTSS();

            //apply rtss fps limit of 0 by default if cant find limit between 5-60
           // RTSS.getRTSSFPSLimit();

            //apply default profile, 0 is default profile ID and will only apply if 0 exists
           // XML_Management.Manage_XML_Profiles.applyProfile("0", false);



        }


        public static void loadLanguage()
        {
            Global_Variables.Global_Variables.languageDict.Source = new Uri("StringResources/StringResources.xaml", UriKind.RelativeOrAbsolute);
            switch (Properties.Settings.Default.language)
            {
                default: break;
                case "English":
                    //do nothing, it is default added
                    break;

                case "中文":
                    System.Windows.Application.Current.Resources.MergedDictionaries.Remove(Global_Variables.Global_Variables.languageDict);
                    Global_Variables.Global_Variables.languageDict.Source = new Uri("StringResources/StringResources.zh-Hans.xaml", UriKind.RelativeOrAbsolute);
                    System.Windows.Application.Current.Resources.MergedDictionaries.Add(Global_Variables.Global_Variables.languageDict);
                    break;
                case "Pусский":
                    System.Windows.Application.Current.Resources.MergedDictionaries.Remove(Global_Variables.Global_Variables.languageDict);
                    Global_Variables.Global_Variables.languageDict.Source = new Uri("StringResources/StringResources.ru.xaml", UriKind.RelativeOrAbsolute);
                    System.Windows.Application.Current.Resources.MergedDictionaries.Add(Global_Variables.Global_Variables.languageDict);
                    break;
            }
            
        }
    }
}
