using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handheld_Control_Panel.Classes
{
    public static class Powercfg
    {
        public static string BaseDir = AppDomain.CurrentDomain.BaseDirectory;

        public static void setPerformanceModePowercfg()
        {
            Run_CLI.Run_CLI.RunCommand(" /s scheme_min", false, "C:\\windows\\system32\\powercfg.exe", 1000);
        }
        public static void setBatterySaverModePowercfg()
        {
            Run_CLI.Run_CLI.RunCommand(" /s scheme_max", false, "C:\\windows\\system32\\powercfg.exe", 1000);
        }
        public static void setBalancedModePowercfg()
        {
            Run_CLI.Run_CLI.RunCommand(" /s 381b4222-f694-41f0-9685-ff5bb260df2e", false, "C:\\windows\\system32\\powercfg.exe", 1000);
        }

        private static string getGUID(string input)
        {
            if (input.Contains(":"))
            {
                return input.Substring(input.IndexOf(":")+1,38).Trim();
            }
            else { return ""; }
        }
        public static void setHyaticePowerPlanModePowercfg()
        {

            importHyaticePowerPlan();
            
            string result = Run_CLI.Run_CLI.RunCommand(" /l", true, "C:\\windows\\system32\\powercfg.exe", 1000);

            string[] array = result.Split('\n');

            foreach (string str in array)
            {
                if (str.Contains("Optimized Power Saver"))
                {
                    
                    Run_CLI.Run_CLI.RunCommand(" /s " + getGUID(str), false, "C:\\windows\\system32\\powercfg.exe", 1000);
                }
                

            }

            
        }
        public static bool HyaticePowerPlanInstalled()
        {
            string result = Run_CLI.Run_CLI.RunCommand(" /l",true, "C:\\windows\\system32\\powercfg.exe", 1000);

            if (result.Contains("Optimized Power Saver"))
            {
                return true;
            }
            else { return false; }
        }

        public static void importHyaticePowerPlan()
        {
            if (!HyaticePowerPlanInstalled()) 
            {
                string directory = @Path.Combine(BaseDir + "Resources\\HyaticePowerPlan\\HyaticePowerPlan.pow");
                                
                Debug.WriteLine(Run_CLI.Run_CLI.RunCommand(" -import " + directory, true, "C:\\windows\\system32\\powercfg.exe", 2000,true));
            }

        }
    }
}
