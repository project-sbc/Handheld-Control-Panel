using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Handheld_Control_Panel.Classes.Run_CLI;
using Handheld_Control_Panel.Classes.Global_Variables;
using System.Windows.Documents;

namespace Handheld_Control_Panel.Classes.AMDPowerSlide_Management
{
    public static class AMDPowerSlide_Management
    {

        public static void readAMDPowerSlide()
        {


            string Power = SystemParameters.PowerLineStatus.ToString();
            string result = Run_CLI.Run_CLI.RunCommand(" -Q SCHEME_CURRENT c763b4ec-0e50-4b6b-9bed-2b92a6ee884e 7ec1751b-60ed-4588-afb5-9819d3d77d90", true, "C:\\windows\\system32\\powercfg.exe", 1000).Trim();
            string amdPS = "";
            int intAMDPS = 0;
            string[] resultArray = result.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);


            if (resultArray.Length > 2)
            {
                if (Power == "Online")
                {
                    amdPS = resultArray[resultArray.Length - 2];
                    amdPS = amdPS.Replace("\r", "");
                    amdPS = amdPS.Replace(" ", "");
                    amdPS = amdPS.Substring(amdPS.IndexOf(":")+1,10).Trim();
                    intAMDPS = Convert.ToInt32(amdPS, 16);
                }
                else
                {
                    amdPS = resultArray[resultArray.Length - 1];
                    amdPS = amdPS.Replace("\r", "");
                    amdPS = amdPS.Replace(" ", "");
                    amdPS = amdPS.Substring(amdPS.IndexOf(":") + 1, 10).Trim();
                    intAMDPS = Convert.ToInt32(amdPS, 16);

                }
                Global_Variables.Global_Variables.AMDPowerSlide = intAMDPS;
            }

       
        }

        public static void changeAMDPowerSlide(int amdPS)
        {
            
            string Power = SystemParameters.PowerLineStatus.ToString();
            if (Power == "Online")
            {
                Run_CLI.Run_CLI.RunCommand(" /setacvalueindex c763b4ec-0e50-4b6b-9bed-2b92a6ee884e 7ec1751b-60ed-4588-afb5-9819d3d77d90 " + Convert.ToString(amdPS), false, "C:\\windows\\system32\\powercfg.exe", 1000);
             
            }
            else
            {
                Run_CLI.Run_CLI.RunCommand(" /setdcvalueindex c763b4ec-0e50-4b6b-9bed-2b92a6ee884e 7ec1751b-60ed-4588-afb5-9819d3d77d90 " + Convert.ToString(amdPS), false, "C:\\windows\\system32\\powercfg.exe", 1000);
            
            }
            Run_CLI.Run_CLI.RunCommand(" /S scheme_current", false, "C:\\windows\\system32\\powercfg.exe", 1000);

            readAMDPowerSlide();
        

        }


        public static void setAMDRyzenAdjPowerPerformance()
        {
            string processRyzenAdj = "";
            string result = "";
            string commandArguments = "";
            string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
            try
            {
                processRyzenAdj = BaseDir + "\\Resources\\AMD\\RyzenAdj\\ryzenadj.exe";
       
                commandArguments = " --max-performance";

                result = Run_CLI.Run_CLI.RunCommand(commandArguments, true, processRyzenAdj);
               
               
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  Run AMD TDP Change: " + ex.Message + ", processRyzenAdj is " + processRyzenAdj + ", result is " + result + ", commandargument is " + commandArguments; ;
                Log_Writer.writeLog(errorMsg);
                MessageBox.Show(errorMsg);

            }
        }
     
    }
}
