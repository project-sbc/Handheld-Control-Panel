using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Handheld_Control_Panel.Classes.Run_CLI;
using Handheld_Control_Panel.Classes.Global_Variables;

namespace Handheld_Control_Panel.Classes.MaxProcFreq_Management
{
    public static class MaxProcFreq_Management
    {

        public static void unhidePowercfgMaxProcFreq()
        {
            Run_CLI.Run_CLI.RunCommand(" -attributes SUB_PROCESSOR PROCFREQMAX -ATTRIB_HIDE", false, "C:\\windows\\system32\\powercfg.exe", 1000);
        }
        public static void readCPUMaxFrequency()
        {


            string Power = SystemParameters.PowerLineStatus.ToString();
            string result = Run_CLI.Run_CLI.RunCommand(" -Q SCHEME_CURRENT sub_processor PROCFREQMAX", true, "C:\\windows\\system32\\powercfg.exe", 1000).Trim();
            string cpuFreq = "";
            int intCpuFreq = 0;
            string[] resultArray = result.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);


            if (resultArray.Length > 2)
            {
                if (Power == "Online")
                {
                    cpuFreq = resultArray[resultArray.Length - 2];
                    cpuFreq = cpuFreq.Replace("\r", "");
                    cpuFreq = cpuFreq.Replace(" ", "");
                    cpuFreq = cpuFreq.Substring(cpuFreq.IndexOf(":")+1,10).Trim();
                    intCpuFreq= Convert.ToInt32(cpuFreq, 16);
                }
                else
                {
                    cpuFreq = resultArray[resultArray.Length - 1];
                    cpuFreq = cpuFreq.Replace("\r", "");
                    cpuFreq = cpuFreq.Replace(" ", "");
                    cpuFreq = cpuFreq.Substring(cpuFreq.IndexOf(":") + 1, 10).Trim();
                    intCpuFreq = Convert.ToInt32(cpuFreq, 16);

                }
                Global_Variables.Global_Variables.cpuMaxFrequency = intCpuFreq;
            }

       
        }

        public static void changeCPUMaxFrequency(int maxCPU)
        {
            
            string Power = SystemParameters.PowerLineStatus.ToString();
            if (Power == "Online")
            {
                Run_CLI.Run_CLI.RunCommand(" /setacvalueindex scheme_current sub_processor PROCFREQMAX " + Convert.ToString(maxCPU), false, "C:\\windows\\system32\\powercfg.exe", 1000);
             
            }
            else
            {
                Run_CLI.Run_CLI.RunCommand(" /setdcvalueindex scheme_current sub_processor PROCFREQMAX " + Convert.ToString(maxCPU), false, "C:\\windows\\system32\\powercfg.exe", 1000);
            
            }
            Run_CLI.Run_CLI.RunCommand(" /S scheme_current", false, "C:\\windows\\system32\\powercfg.exe", 1000);

            readCPUMaxFrequency();
        

        }

     
    }
}
