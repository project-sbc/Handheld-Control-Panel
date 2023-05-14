using RTSSSharedMemoryNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibreHardwareMonitor;
using LibreHardwareMonitor.Hardware;
using System.Windows.Media.Animation;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using PidSharp;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Security.Cryptography;
using Windows.Graphics;
using System.Windows.Controls.Primitives;

namespace Handheld_Control_Panel.Classes
{
    
    public static class AutoTDP_Management
    {
        public static Thread autoTDPThread;
        #region fps get data
        private static List<int> fps = new List<int>();
        private static double fps_Avg;
        private static double fps_Min;
        private static double fps_Max;
        private static void getFPS()
        {

            if (RTSS.RTSSRunning())
            {

                AppFlags appFlag = appFlags[0];
                AppEntry[] appEntries = OSD.GetAppEntries(appFlag);

                foreach (AppFlags af in appFlags)
                {
                    appEntries = OSD.GetAppEntries(af);
                    if (appEntries.Length > 0)
                    {
                        foreach (var app in appEntries)
                        {
                            Process p = Process.GetProcessById(app.ProcessId);
                            if (p != null)
                            {
                                if (p.MainWindowHandle != IntPtr.Zero)
                                {
                                    int currFps = (int)app.InstantaneousFrames;
                                    fps.Add(currFps);
                                    if (fps.Count > 3)
                                    {
                                        fps.RemoveAt(0);
                                        fps_Avg = Math.Round(fps.Average(), 0);
                                        fps_Min = fps.Min();
                                        fps_Max = fps.Max();

                                    }
                                }
                            }
                            

                            return;

                        }
                    }
                        
                   
                }

               


            }
            else
            {
                RTSS.startRTSS();
            }
        }

        #endregion
        private static Computer computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = false,
            IsMotherboardEnabled = false,
            IsControllerEnabled = false,
            IsNetworkEnabled = false,
            IsStorageEnabled = false,
            IsBatteryEnabled = false,
            IsPsuEnabled = false
        };
        public static void endAutoTDP()
        {
            Global_Variables.Global_Variables.autoTDP = false;
            //Powercfg.setBatterySaverModePowercfg();
        }

        public static void startAutoTDP()
        {
            if (Global_Variables.Global_Variables.cpuType == "AMD" && Global_Variables.Global_Variables.processorName.Contains("6800U") && Global_Variables.Global_Variables.Device.AutoTDP != "None")
            {
                TDP_Management.TDP_Management.changeTDP(25, 25);
                Global_Variables.Global_Variables.autoTDP = true;
                Powercfg.setHyaticePowerPlanModePowercfg();
                autoTDPThread = new Thread(() => { mainAutoTDPLoop(); });


                //set amd power slider
                Classes.Task_Scheduler.Task_Scheduler.runTask(() => AMDPowerSlide_Management.AMDPowerSlide_Management.setAMDRyzenAdjPowerPerformance());
                autoTDPThread.IsBackground = true;
                autoTDPThread.Start();
            }

         
        }
        private static double minCPU = Global_Variables.Global_Variables.Device.MinCPUClock;
        private static double maxCPU = Global_Variables.Global_Variables.Device.MaxCPUClock;
        private static double minGPU = Global_Variables.Global_Variables.Device.MinGPUClock;
        private static double maxGPU = Global_Variables.Global_Variables.Device.MaxGPUClock;


        private static PidController cpuPID = new PidController(-4, -1, 0, maxCPU, minCPU)
        {
            TargetValue = 70
        };
        private static PidController gpuPID = new PidController(-4, -1, 0, maxGPU, minGPU)
        {
            TargetValue = 78
        };
        private static PidController tdpPID = new PidController(-2, -1, -1, 25, 5)
        {
            TargetValue = 58,
        };


        private static void mainAutoTDPLoop()
        {
            //computer.Open() starts a new Librehardware monitor instance so we can start getting data. We close it at the end when auto tdp is done to save resources
            computer.Open();
            if (!RTSS.RTSSRunning())
            {
                RTSS.startRTSS();
            }
            OSD osd =null;

            while (Global_Variables.Global_Variables.autoTDP)
            {
                
                //thread sleep just adds a pause (in ms)
                Thread.Sleep(1000);
                getInformationForAutoTDP();

               
                Debug.WriteLine("procUtility: " + procUtility[procUtility.Count - 1].ToString());
                Debug.WriteLine("gpuUsage: " + gpuUsage[gpuUsage.Count - 1].ToString());


                cpuPID.CurrentValue = procUtility_Avg;
                double value1 = Math.Min(Math.Ceiling(cpuPID.ControlOutput / 50.0) * 50.0, maxCPU);
                gpuPID.CurrentValue = gpuUsage_Avg;
                double value2 = Math.Min(Math.Ceiling(gpuPID.ControlOutput / 50.0) * 50.0, maxGPU);
                if (fps_Avg>1)
                {
                    tdpPID.CurrentValue = fps_Avg;
                }
               
                double value3 = Math.Round(tdpPID.ControlOutput, 0);
                Debug.WriteLine("new max cpu: " + value1.ToString());
                Debug.WriteLine("new gpu: " + value2.ToString());
                Debug.WriteLine("package power: " + cpuPower.ToString());
                Debug.WriteLine("FPS avg :" + fps_Avg.ToString());
                Debug.WriteLine("FPS min :" + fps_Min.ToString());
                if (RTSS.RTSSRunning())
                {
                    if (osd == null)
                    {
                        osd = new OSD("autoTDP");
                    }
                    
                    osd.Update("CPU clock " + cpuClock_Avg + "\nFPS: " + fps_Avg + "\ncpu usage avg: " + cpuUsage_Avg + "\ngpuUsage: " + gpuUsage_Avg + "\ncpu value: " + value1.ToString() + "\ngpu value: " + value2.ToString() + "\npower: " + cpuPower + "\ntdp " + Global_Variables.Global_Variables.readPL1);
                }

                if (Global_Variables.Global_Variables.CPUMaxFrequency != value1)
                {
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => MaxProcFreq_Management.MaxProcFreq_Management.changeCPUMaxFrequency((int)Math.Round(value1, 0)));

                }
                if (Global_Variables.Global_Variables.GPUCLK != value2)
                {
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => GPUCLK_Management.GPUCLK_Management.changeAMDGPUClock((int)Math.Round(value2, 0)));
 
                }
                if (Global_Variables.Global_Variables.readPL1 != value3 && fps_Avg >1)
                {
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => TDP_Management.TDP_Management.changeTDP((int)value3,(int)value3));
                    //lastTDPValue = value3;
                }


            }
            if (osd !=null)
            {
                osd.Update("");
                osd.Dispose();
            }

            //when autotdp is set to false the loop will end 

            //close librehardware monitor instance
            computer.Close();
        }
      
      

        #region support routines

      

        private static void getInformationForAutoTDP()
        {
        start:
            //get d3d usage, cpu usage
            getLibreHardwareMonitorInfo();
            //getGPUUsage();
            //get proc utility
            getProcUtility();
            //get cpu
           
            getCPUUsage();
            getFPS();

            if (cpuClock.Count < 3 && Global_Variables.Global_Variables.autoTDP)
            {
                Thread.Sleep(250);
                goto start;
            }

            


        }
  
        #region get gpu usage from AMD driver

      
        #endregion
        public static void getLibreHardwareMonitorInfo()
        {

            computer.Accept(new UpdateVisitor());
            getLibre_packagepower();
            getLibre_GPUD3D();
            libreGetCpuClock();
        }
        #region gpu D3D usage from libre hardware monitor
        private static List<int> gpuUsage = new List<int>();
        private static double gpuUsage_Avg;
        private static int gpuUsage_Min;
        private static int gpuUsage_Max;

        private static void getLibre_GPUD3D()
        {
            IHardware amdRadeon = computer.Hardware.FirstOrDefault(c => c.Name.StartsWith("AMD Radeon"));

            if (amdRadeon != null)
            {
                ISensor D3D = amdRadeon.Sensors.FirstOrDefault(c => c.Name == "D3D 3D");
                if (D3D != null)
                {
                    gpuUsage.Add((int)D3D.Value);
                    if (gpuUsage.Count > 1)
                    {
                        if (gpuUsage.Count > 3) { gpuUsage.RemoveAt(0); }
                        gpuUsage_Avg = Math.Round(gpuUsage.Average(),0);
                        gpuUsage_Min = gpuUsage.Min();
                        gpuUsage_Max = gpuUsage.Max();
                        
                    }
                }
            }
        }
        #endregion
        #region processor useage
        private static List<double> cpuUsage = new List<double>();
        private static double cpuUsage_Avg;
        private static double cpuUsage_Min;
        private static double cpuUsage_Max;
        
        private static void getCPUUsage()
        {

            cpuUsage.Add((Math.Max(1, procUtility[procUtility.Count - 1] * minCPU) / cpuClock[cpuClock.Count - 1]));
            if (cpuUsage.Count > 3)
            {
                cpuUsage.RemoveAt(0);
                cpuUsage_Avg = Math.Round(cpuUsage.Average(), 0);
                cpuUsage_Min = cpuUsage.Min();
                cpuUsage_Max = cpuUsage.Max();

            }
        }
        #endregion
        #region processor utility
        private static List<float> procUtility = new List<float>();
        private static double procUtility_Avg;
        private static float procUtility_Min;
        private static float procUtility_Max;
        private static PerformanceCounter theCPUCounter = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
        private static void getProcUtility()
        {
            var newcpuutil = theCPUCounter.NextValue();
            Debug.WriteLine("new cpu util " + newcpuutil);
            procUtility.Add(newcpuutil);
            if (procUtility.Count > 3)
            {
                procUtility.RemoveAt(0);
                procUtility_Avg = Math.Round(procUtility.Average(),0);
                procUtility_Min = procUtility.Min();
                procUtility_Max = procUtility.Max();
             
            }
        }
        #endregion
        #region processor clock
        private static List<float> cpuClock = new List<float>();
        private static double cpuClock_Avg;
        private static float cpuClock_Min;
        private static float cpuClock_Max;
   

        private static PerformanceCounter theCPUClockCounter = new PerformanceCounter("Processor Information", "Processor Frequency", "_Total");
        public static void libreGetCpuClock()
        {

            IHardware cpu = computer.Hardware.FirstOrDefault(c => c.Name.StartsWith("AMD Ryzen"));

            if (cpu != null)
            {
                ISensor clock = cpu.Sensors.FirstOrDefault(c => c.Name == "Core #1");
                if (clock != null)
                {
                    cpuClock.Add((int)clock.Value);
                    if (cpuClock.Count > 1)
                    {
                        if (cpuUsage.Count > 3) { cpuClock.RemoveAt(0); }
                        cpuClock_Avg = Math.Round(cpuClock.Average(), 0);
                        cpuClock_Min = cpuClock.Min();
                        cpuClock_Max = cpuClock.Max();

                    }
                }
            }
        }
        #endregion
        #region gpu D3D usage from libre hardware monitor

        private static float? cpuPower;
        private static void getLibre_packagepower()
        {
            IHardware cpu = computer.Hardware.FirstOrDefault(c => c.Name.StartsWith("AMD"));

            if (cpu != null)
            {
                ISensor package = cpu.Sensors.FirstOrDefault(c => c.Name == "Package");
                if (package != null)
                {
                    cpuPower = package.Value;
                   
                }
            }
        }
        #endregion


        #region constants and dll references
        public const string CppFunctionsDLL = @"Resources\AMD\ADLX\ADLX_PerformanceMetrics.dll";
        public const string CppFunctionsDLL2 = @"Resources\AMD\ADLX\ADLX_AutoTuning.dll";
        public const string CppFunctionsDLL3 = @"Resources\AMD\ADLX\ADLX_3DSettings.dll";

        [DllImport(CppFunctionsDLL, CallingConvention = CallingConvention.Cdecl)] public static extern int GetFPSData();

        [DllImport(CppFunctionsDLL, CallingConvention = CallingConvention.Cdecl)] public static extern int GetGPUMetrics(int GPU, int Sensor);

        [DllImport(CppFunctionsDLL2, CallingConvention = CallingConvention.Cdecl)] public static extern int SetAutoTuning(int GPU, int num);

        [DllImport(CppFunctionsDLL2, CallingConvention = CallingConvention.Cdecl)] public static extern int GetAutoTuning(int GPU);

        [DllImport(CppFunctionsDLL2, CallingConvention = CallingConvention.Cdecl)] public static extern int GetFactoryStatus(int GPU);

        [DllImport(CppFunctionsDLL3, CallingConvention = CallingConvention.Cdecl)] public static extern int SetFPSLimit(int GPU, bool isEnabled, int FPS);
        [DllImport(CppFunctionsDLL3, CallingConvention = CallingConvention.Cdecl)] public static extern int SetRSR(bool isEnabled);
        #endregion
       
        private static List<AppFlags> appFlags = new List<AppFlags>()
        {
            {AppFlags.OpenGL },
            {AppFlags.Direct3D9Ex },
            {AppFlags.Direct3D9 },
            {AppFlags.Direct3D10 },
            {AppFlags.Direct3D11 },
            {AppFlags.Direct3D12 },
            {AppFlags.Direct3D12AFR },
            {AppFlags.OpenGL },
            {AppFlags.Vulkan }
        };
        #endregion
    }
}
