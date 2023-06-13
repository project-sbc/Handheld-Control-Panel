using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage;

namespace Handheld_Control_Panel.Classes
{
    public static class AutoProfile_Management
    {
        private static DateTime lastProfileCheck = DateTime.Now;
        public static void checkAutoProfileApplicator()
        {
            

            //this is last profile checker to see if device went to sleep, if the datetime since last checked is more than a minute the device probably went to sleep so reapply
           
            if (DateTime.Now.Subtract(lastProfileCheck).Seconds > 40) 
            {
                if (Global_Variables.Global_Variables.profiles.activeProfile != null)
                {
                    lastProfileCheck = DateTime.Now;
                    Global_Variables.Global_Variables.profiles.activeProfile.applyProfile(true, false);
                    return;
                }
            }
            lastProfileCheck = DateTime.Now;
            //this has the workflow for auto applying profiles when power status changes OR an exe is detected running with a profile
            string Power = SystemParameters.PowerLineStatus.ToString();
            //first thing... we check if there is an active profile and if its tied to an exe

            //check if active profile
            if (Global_Variables.Global_Variables.profiles.activeProfile != null)
            {
                //check if it has an exe
                if (Global_Variables.Global_Variables.profiles.activeProfile.Exe != "")
                {
                    //check if that exe is still running
                    Process[] pList = Process.GetProcessesByName(Global_Variables.Global_Variables.profiles.activeProfile.Exe);

                    //if pLlist is greater than 0 than process detected running
                    if (pList.Length > 0)
                    {
                        //reapply profile is power status changed from charger to battery or vice versa. Global variables has the last power status, we will update that later in this routine
                        if (Global_Variables.Global_Variables.powerStatus != "" && Global_Variables.Global_Variables.powerStatus != Power && Global_Variables.Global_Variables.profiles.activeProfile.SeparateChargerBattery == true)
                        {
                     
                            Global_Variables.Global_Variables.profiles.activeProfile.applyProfile(true,false);
                            Global_Variables.Global_Variables.powerStatus = Power;
                        }
                       
                        //we exit this routine if a process is found, active profile with running exe has top priority
                        return;

                    }
                    else
                    {
                       

                        //check if the profile currently applied is auto applied
                        if (Global_Variables.Global_Variables.profileAutoApplied == true)
                        {
                            
                            if (Global_Variables.Global_Variables.profiles.defaultProfile != null)
                            {
                                Global_Variables.Global_Variables.profiles.defaultProfile.applyProfile(false,false);
                    
                                return;
                            }
                         
                        }
                       
                    }

                }
              
            }

            //continue forward if active profile doesn't have running exe
            foreach (Profile_Main profile in Global_Variables.Global_Variables.profiles)
            {
                if (profile.profile_Info.ProfileExe != "")
                {
                    //check if that exe is still running
                    Process[] pList = Process.GetProcessesByName(profile.profile_Info.ProfileExe);

                    //if pLlist is greater than 0 than process detected running
                    if (pList.Length > 0)
                    {
                        profile.applyProfile(true,false);
                        //we exit this routine if a process is found, this is the new active profile with running exe
                        return;
                    }

                }
            }

            //lastly, we check if the active profile needs a power change. we need to do this last because it has least precedent over the other changes  above
            if (Global_Variables.Global_Variables.profiles.activeProfile != null)
            {
                //reapply profile is power status changed from charger to battery or vice versa. Global variables has the last power status, we will update that later in this routine
                if (Global_Variables.Global_Variables.powerStatus != "" && Global_Variables.Global_Variables.powerStatus != Power && Global_Variables.Global_Variables.profiles.activeProfile.profile_Parameters.SeparateChargerBattery == true)
                {
                    Global_Variables.Global_Variables.profiles.activeProfile.applyProfile(true, false);
                    Global_Variables.Global_Variables.powerStatus = Power;
                    return;
                }
            }

            //if none of the above, we will just check to see if global powerstatus needs update and be done
            if (Global_Variables.Global_Variables.powerStatus != Power && Global_Variables.Global_Variables.profiles.activeProfile.profile_Parameters.SeparateChargerBattery == true)
            {
                Global_Variables.Global_Variables.powerStatus = Power;
            }

        }


        public static void checkAutoProfileApplicator_StartUp()
        {

            //continue forward if active profile doesn't have running exe
            foreach (Profile_Main profile in Global_Variables.Global_Variables.profiles)
            {
                if (profile.profile_Info.ProfileExe != "")
                {
                    //check if that exe is still running
                    Process[] pList = Process.GetProcessesByName(profile.profile_Info.ProfileExe);

                    //if pLlist is greater than 0 than process detected running
                    if (pList.Length > 0)
                    {
                        Global_Variables.Global_Variables.profiles.activeProfile = profile;
                        //we exit this routine if a process is found, this is the new active profile with running exe
                        return;
                    }

                }
            }


        }

    }
}
