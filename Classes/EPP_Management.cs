using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Handheld_Control_Panel.Classes.Run_CLI;
using Handheld_Control_Panel.Classes.Global_Variables;

namespace Handheld_Control_Panel.Classes.EPP_Management
{
    public static class EPP_Management
    {

        public static void unhidePowercfgEPP()
        {
            Run_CLI.Run_CLI.RunCommand(" -attributes SUB_PROCESSOR PERFEPP -ATTRIB_HIDE", false, "C:\\windows\\system32\\powercfg.exe", 1000);
            Run_CLI.Run_CLI.RunCommand(" -attributes SUB_PROCESSOR PERFEPP1 -ATTRIB_HIDE", false, "C:\\windows\\system32\\powercfg.exe", 1000);
        }
        public static void readEPP()
        {


            string Power = SystemParameters.PowerLineStatus.ToString();
            string result = Run_CLI.Run_CLI.RunCommand(" -Q SCHEME_CURRENT SUB_PROCESSOR PERFEPP", true, "C:\\windows\\system32\\powercfg.exe", 1000).Trim();
            string EPP = "";
            int intEPP = 0;
            string[] resultArray = result.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);


            if (resultArray.Length > 2)
            {
                if (Power == "Online")
                {
                    EPP = resultArray[resultArray.Length - 2];
                    EPP = EPP.Replace("\r", "");
                    EPP = EPP.Replace(" ", "");
                    EPP = EPP.Substring(EPP.IndexOf(":")+1,10).Trim();
                    intEPP = Convert.ToInt32(EPP, 16);
                }
                else
                {
                    EPP = resultArray[resultArray.Length - 1];
                    EPP = EPP.Replace("\r", "");
                    EPP = EPP.Replace(" ", "");
                    EPP = EPP.Substring(EPP.IndexOf(":") + 1, 10).Trim();
                    intEPP = Convert.ToInt32(EPP, 16);

                }
                if(Global_Variables.Global_Variables.EPP != intEPP)
                {
                    Global_Variables.Global_Variables.EPP = intEPP;
                }
                
            }

       
        }

        public static void changeEPP(int EPP)
        {
            
            string Power = SystemParameters.PowerLineStatus.ToString();
            if (Power == "Online")
            {
                Run_CLI.Run_CLI.RunCommand(" /setacvalueindex scheme_current SUB_PROCESSOR PERFEPP " + Convert.ToString(EPP), false, "C:\\windows\\system32\\powercfg.exe", 1000);
                Run_CLI.Run_CLI.RunCommand(" /setacvalueindex scheme_current SUB_PROCESSOR PERFEPP1 " + Convert.ToString(EPP), false, "C:\\windows\\system32\\powercfg.exe", 1000);
             
            }
            else
            {
                Run_CLI.Run_CLI.RunCommand(" /setdcvalueindex scheme_current SUB_PROCESSOR PERFEPP " + Convert.ToString(EPP), false, "C:\\windows\\system32\\powercfg.exe", 1000);
                Run_CLI.Run_CLI.RunCommand(" /setdcvalueindex scheme_current SUB_PROCESSOR PERFEPP1 " + Convert.ToString(EPP), false, "C:\\windows\\system32\\powercfg.exe", 1000);
            
            }
            Run_CLI.Run_CLI.RunCommand(" /S scheme_current", false, "C:\\windows\\system32\\powercfg.exe", 1000);

            readEPP();
        

        }

     
    }
}
