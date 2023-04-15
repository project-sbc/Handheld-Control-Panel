using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handheld_Control_Panel.Classes
{
    public static class Powercfg
    {
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
    }
}
