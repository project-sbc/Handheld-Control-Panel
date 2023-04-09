using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Handheld_Control_Panel.Classes
{
    public static class AutoProfile_Management
    {

        public static void checkAutoProfileApplicator()
        {
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
                        if (Global_Variables.Global_Variables.powerStatus != "" && Global_Variables.Global_Variables.powerStatus != Power)
                        {
                            Global_Variables.Global_Variables.profiles.activeProfile.applyProfile();
                        }
                        //we exit this routine if a process is found, active profile with running exe has top priority
                        return;
                    }

                }
            }

            //continue forward if active profile doesn't have running exe


            foreach (Profile profile in Global_Variables.Global_Variables.profiles)
            {
                if (profile.Exe != "")
                {
                    //check if that exe is still running
                    Process[] pList = Process.GetProcessesByName(profile.Exe);

                    //if pLlist is greater than 0 than process detected running
                    if (pList.Length > 0)
                    {
                        profile.applyProfile();
                        //we exit this routine if a process is found, this is the new active profile with running exe
                        return;
                    }

                }
            }

            //lastly, we check if the active profile needs a 



        }

    }
}
