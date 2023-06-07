using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.Run_CLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handheld_Control_Panel.Classes.GPUCLK_Management
{
    public static class GPUCLK_Management
    {
        private static Object objLock = new Object();

        public static void changeAMDGPUClock(int gpuclk)
        {
            string BaseDir = AppDomain.CurrentDomain.BaseDirectory;

            if (gpuclk >= 100 && Global_Variables.Global_Variables.cpuType == "AMD")
            {
                lock (objLock)
                {
                    string processRyzenAdj = "";
                    string result = "";
                    string commandArguments = "";


                    try
                    {
                        processRyzenAdj = BaseDir + "\\Resources\\AMD\\RyzenAdj\\ryzenadj.exe";

                        lock (objLock)
                        {
                            if (gpuclk > Global_Variables.Global_Variables.settings.maxGPUCLK) { gpuclk = Global_Variables.Global_Variables.settings.maxGPUCLK; }
                            commandArguments = " --gfx-clk=" + gpuclk.ToString();
                            //Log_Writer.writeLog("Read TDP AMD processRyzenAj=" + processRyzenAdj + "; commandarugment=" + commandArguments);

                            result = Run_CLI.Run_CLI.RunCommand(commandArguments, false, processRyzenAdj);

                            Global_Variables.Global_Variables.GPUCLK = gpuclk;

                        }



                    }
                    catch { }


                }
            }
        }


    }
}
