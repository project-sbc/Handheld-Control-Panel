using Handheld_Control_Panel.Classes.Global_Variables;
using RTSSSharedMemoryNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Threading;

using MessageBox = System.Windows.MessageBox;
using System.Windows.Media.Animation;
using Handheld_Control_Panel.Classes.Task_Scheduler;
using AutoUpdaterDotNET;

namespace Handheld_Control_Panel.Classes
{
    public static class Start_Up
    {

        public static void Start_Routine()
        {
                                  

            //run hyatice powerplan
            Powercfg.setupPowerPlan();

            //run all routines to get device ready

            //    librehardwaremonitor.Monitor();

            //test code here
            //Display_Management.Display_Management.testGettingResolutionFromNewNugetPackage();
            //test code
                       

            //Load XML profile file
            XML_Management.Load_XML_File.load_XML_File();


            //check steam/playnite installed for added features
            Steam_Management.setSteamDirectory();
            Playnite_Management.setPlayniteDirectory();
            EpicGames_Management.setEpicGamesDirectory();




            //get cpu information
            TDP_Management.TDP_Management.determineCPU();


            //check fan control device capability
            Global_Variables.Global_Variables.Device = Device_Management.GetCurrentDevice();

            Fan_Management.Fan_Management.readSoftwareFanControl();
                    

            //check to make sure driver isn't blocked for intel (checks for intel in routine)
            //TDP_Management.TDP_Management.checkDriverBlockRegistry();

            //Make sure powercfg profile has coreparking and maxprocfreq unhidden or otherwise those wont work
            MaxProcFreq_Management.MaxProcFreq_Management.unhidePowercfgMaxProcFreq();
            CoreParking_Management.CoreParking_Management.unhidePowercfgCoreParking();
            EPP_Management.EPP_Management.unhidePowercfgEPP();

            //get max core count
            Global_Variables.Global_Variables.maxCpuCores = new ManagementObjectSearcher("Select * from Win32_Processor").Get().Cast<ManagementBaseObject>().Sum(item => int.Parse(item["NumberOfCores"].ToString()));

            //load lists (resolutions, refresh rates, scalings)
            Display_Management.Display_Management.generateDisplayResolutionAndRateList();
          
            
            //load language resource
            loadLanguage();

            //start task scheduler
            Task_Scheduler.Task_Scheduler.startScheduler();

            //update values
            ParallelTaskUpdate_Management.UpdateTask();
                   

            //check if RTSS should be started at startup
            RTSS.checkAutoStartRTSS();

            //apply rtss fps limit of 0 by default if cant find limit between 5-60
            RTSS.getRTSSFPSLimit();

            //apply default profile, 0 is default profile ID and will only apply if 0 exists
            //XML_Management.Manage_XML_Profiles.applyProfile("0", false);


            //wrap in dispatcher because calling profiles from UI thread later will give an error. REMINDER: this is running on separate thread for spinner
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => Global_Variables.Global_Variables.profiles = new Profiles_Management()));


           
            

            AutoProfile_Management.checkAutoProfileApplicator_StartUp();
            if (Global_Variables.Global_Variables.profiles.activeProfile != null)
            {
                Global_Variables.Global_Variables.profiles.activeProfile.applyProfile(true, false);
            }

            Global_Variables.Global_Variables.hotKeys = new Action_Management();
            Global_Variables.Global_Variables.homePageItems = new CustomizeHome_Management();

            Global_Variables.Global_Variables.hotKeys.generateGlobalControllerHotKeyList();
            Global_Variables.Global_Variables.hotKeys.generateGlobalKeyboardHotKeyList();


            //variable startSafeMode is a way to make the fan go back to hardware control operation in the case the app crashes and doesn't properly put the fan back into hardware control.
            //It works by turning false if the app closes properly. It will normally be true so if something does happen then it won't properly close and turn it false.

            if (Global_Variables.Global_Variables.Device.FanCapable)
            {
                if (!Properties.Settings.Default.startSafeMode)
                {
                    if (Properties.Settings.Default.startAutoFan)
                    {
                        AutoFan_Management.startAutoFan();
                    }
                }
                else
                {
                    Fan_Management.Fan_Management.setFanControlHardware();
                  
                }
            }
            Properties.Settings.Default.startSafeMode = true;
            Properties.Settings.Default.Save();

            //bind autostart event but DONT start it yet
            Update_Software.Update_Software.bindUpdateEvent();

        }

        private static void AutoUpdaterOnCheckForUpdateEvent(object startUp, UpdateInfoEventArgs args)
        {
            throw new NotImplementedException();
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

                case "简体中文":
                    System.Windows.Application.Current.Resources.MergedDictionaries.Remove(Global_Variables.Global_Variables.languageDict);
                    Global_Variables.Global_Variables.languageDict.Source = new Uri("StringResources/StringResources.zh-Hans.xaml", UriKind.RelativeOrAbsolute);
                    System.Windows.Application.Current.Resources.MergedDictionaries.Add(Global_Variables.Global_Variables.languageDict);
                    break;
                case "Pусский":
                    System.Windows.Application.Current.Resources.MergedDictionaries.Remove(Global_Variables.Global_Variables.languageDict);
                    Global_Variables.Global_Variables.languageDict.Source = new Uri("StringResources/StringResources.ru.xaml", UriKind.RelativeOrAbsolute);
                    System.Windows.Application.Current.Resources.MergedDictionaries.Add(Global_Variables.Global_Variables.languageDict);
                    break;
                case "日本語":
                    System.Windows.Application.Current.Resources.MergedDictionaries.Remove(Global_Variables.Global_Variables.languageDict);
                    Global_Variables.Global_Variables.languageDict.Source = new Uri("StringResources/StringResources.jp.xaml", UriKind.RelativeOrAbsolute);
                    System.Windows.Application.Current.Resources.MergedDictionaries.Add(Global_Variables.Global_Variables.languageDict);
                    break;
                case "Português (Brasil)":
                    System.Windows.Application.Current.Resources.MergedDictionaries.Remove(Global_Variables.Global_Variables.languageDict);
                    Global_Variables.Global_Variables.languageDict.Source = new Uri("StringResources/StringResources.pt-br.xaml", UriKind.RelativeOrAbsolute);
                    System.Windows.Application.Current.Resources.MergedDictionaries.Add(Global_Variables.Global_Variables.languageDict);
                    break;
                case "한국어":
                    System.Windows.Application.Current.Resources.MergedDictionaries.Remove(Global_Variables.Global_Variables.languageDict);
                    Global_Variables.Global_Variables.languageDict.Source = new Uri("StringResources/StringResources.kr.xaml", UriKind.RelativeOrAbsolute);
                    System.Windows.Application.Current.Resources.MergedDictionaries.Add(Global_Variables.Global_Variables.languageDict);
                    break;
                case "Español":
                    System.Windows.Application.Current.Resources.MergedDictionaries.Remove(Global_Variables.Global_Variables.languageDict);
                    Global_Variables.Global_Variables.languageDict.Source = new Uri("StringResources/StringResources.es-ES.xaml", UriKind.RelativeOrAbsolute);
                    System.Windows.Application.Current.Resources.MergedDictionaries.Add(Global_Variables.Global_Variables.languageDict);
                    break;
            }
            
        }

        public static bool checkMultipleProgramsRunning()
        {
            Process[] processes = Process.GetProcessesByName("Handheld Control Panel");
            if (processes.Length > 1)
            { return true; }  else { return false; }
        }
        
    }
}
