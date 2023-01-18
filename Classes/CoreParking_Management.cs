using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.Run_CLI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Handheld_Control_Panel.Classes.CoreParking_Management
{
    public static class CoreParking_Management
    {
      
        public static void readActiveCores()
        {

            Debug.WriteLine("core park start");
            string Power = SystemParameters.PowerLineStatus.ToString();
            string result = Run_CLI.Run_CLI.RunCommand(" -Q SCHEME_CURRENT sub_processor CPMAXCORES", true, "C:\\windows\\system32\\powercfg.exe", 1000).Trim();
            string maxCores = "";
            int intMaxCores = 0;
            string[] resultArray = result.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);


            if (resultArray.Length > 2)
            {
                if (Power == "Online")
                {
                    maxCores = resultArray[resultArray.Length - 2];
                    maxCores = maxCores.Replace("\r", "");
                    maxCores = maxCores.Replace(" ", "");
                    maxCores = maxCores.Substring(maxCores.IndexOf(":") + 1, 10).Trim();
                    intMaxCores = Convert.ToInt32(maxCores, 16);
                }
                else
                {
                    maxCores = resultArray[resultArray.Length - 1];
                    maxCores = maxCores.Replace("\r", "");
                    maxCores = maxCores.Replace(" ", "");
                    maxCores = maxCores.Substring(maxCores.IndexOf(":") + 1, 10).Trim();
                    intMaxCores = Convert.ToInt32(maxCores, 16);

                }
                double calculateCores = (intMaxCores * Global_Variables.Global_Variables.maxCpuCores) / 100;
                calculateCores = Math.Round(calculateCores, 0);

                Global_Variables.Global_Variables.cpuActiveCores = (int)calculateCores;
            }


        }
       
        public static void changeActiveCores(double coreCount)
        {
            double CorePercentage = Math.Round(coreCount / Global_Variables.Global_Variables.maxCpuCores, 2) * 100;
            string Power = SystemParameters.PowerLineStatus.ToString();
            if (Power == "Online")
            {
                Run_CLI.Run_CLI.RunCommand(" /setacvalueindex scheme_current sub_processor CPMAXCORES " + Convert.ToString(CorePercentage), false, "C:\\windows\\system32\\powercfg.exe", 1000);
                Run_CLI.Run_CLI.RunCommand(" /setacvalueindex scheme_current sub_processor CPMINCORES " + Convert.ToString(CorePercentage), false, "C:\\windows\\system32\\powercfg.exe", 1000);
            }
            else
            {
                Run_CLI.Run_CLI.RunCommand(" /setdcvalueindex scheme_current sub_processor CPMAXCORES " + Convert.ToString(CorePercentage), false, "C:\\windows\\system32\\powercfg.exe", 1000);
                Run_CLI.Run_CLI.RunCommand(" /setdcvalueindex scheme_current sub_processor CPMINCORES " + Convert.ToString(CorePercentage), false, "C:\\windows\\system32\\powercfg.exe", 1000);
            }
            Run_CLI.Run_CLI.RunCommand(" /S scheme_current", false, "C:\\windows\\system32\\powercfg.exe", 1000);
            readActiveCores();
      
        }

        public static void unhidePowercfgCoreParking()
        {
            Run_CLI.Run_CLI.RunCommand(" -attributes SUB_PROCESSOR CPMAXCORES -ATTRIB_HIDE", false, "C:\\windows\\system32\\powercfg.exe", 1000);
            Run_CLI.Run_CLI.RunCommand(" -attributes SUB_PROCESSOR CPMINCORES -ATTRIB_HIDE", false, "C:\\windows\\system32\\powercfg.exe", 1000);
            Run_CLI.Run_CLI.RunCommand(" -attributes SUB_PROCESSOR PROCFREQMAX -ATTRIB_HIDE", false, "C:\\windows\\system32\\powercfg.exe", 1000);

        }

    }
}
