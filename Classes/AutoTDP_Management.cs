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
                    if (appEntries.Length > 0) { appFlag = af; break; }
                }

                foreach (var app in appEntries)
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

                    break;

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
        public static void endAutoTDPTimer()
        {
            Global_Variables.Global_Variables.autoTDP = false;
        
        }

        private static double originalEPP = Global_Variables.Global_Variables.EPP;
        public static void startAutoTDP()
        {
            Global_Variables.Global_Variables.autoTDP = true;
            autoTDPThread = new Thread(() => { mainAutoTDPLoop(); });
            autoTDPThread.Start();
        
      

            //set amd power slider
            Classes.Task_Scheduler.Task_Scheduler.runTask(() => AMDPowerSlide_Management.AMDPowerSlide_Management.setAMDRyzenAdjPowerPerformance());

            //change power plan EPP to 0% - Be sure to save the original EPP to go back to after AutoTDP stops
            originalEPP = Global_Variables.Global_Variables.EPP;
                       
            Classes.Task_Scheduler.Task_Scheduler.runTask(() => EPP_Management.EPP_Management.changeEPP(0));

    

        }
        private static double minCPU = 1100;
        private static double maxCPU = 4700;
        private static double minGPU = 500;
        private static double maxGPU = 2200;
        private static double lastGPUValue = 500;
        private static double lastCPUValue = 2500;

        private static PidController cpuPID = new PidController(-2, -1, 0, maxCPU, minCPU)
        {
            TargetValue = 70
        };
        private static PidController gpuPID = new PidController(-2, -1, 0, maxGPU, minGPU)
        {
            TargetValue = 70
        };

       
     
        private static void mainAutoTDPLoop()
        {
            //computer.Open() starts a new Librehardware monitor instance so we can start getting data. We close it at the end when auto tdp is done to save resources
            computer.Open();
            OSD osd = new OSD("autoTDP");

            while (Global_Variables.Global_Variables.autoTDP)
            {
                
                //thread sleep just adds a pause (in ms)
                Thread.Sleep(1000);
                getInformationForAutoTDP();

               
                Debug.WriteLine("procUtility: " + procUtility[procUtility.Count - 1].ToString());
                Debug.WriteLine("gpuUsage: " + gpuUsage[gpuUsage.Count - 1].ToString());


                cpuPID.CurrentValue = cpuUsage_Avg;
                double value1 = Math.Min(Math.Ceiling(cpuPID.ControlOutput / 50.0) * 50.0, maxGPU);
                gpuPID.CurrentValue = gpuUsage_Avg;
                double value2 = Math.Min(Math.Ceiling(gpuPID.ControlOutput / 50.0) * 50.0, maxCPU);
                Debug.WriteLine("new max cpu: " + value1.ToString());
                Debug.WriteLine("new gpu: " + value2.ToString());
                Debug.WriteLine("package power: " + cpuPower.ToString());
                Debug.WriteLine("FPS avg :" + fps_Avg.ToString());
                Debug.WriteLine("FPS min :" + fps_Min.ToString());
                if (RTSS.RTSSRunning())
                {
                    osd.Update("FPS: " + fps_Avg + " cpu usage avg: " + cpuUsage_Avg + " gpuUsage: " + gpuUsage_Avg + " new cpu value: " + value1.ToString() + " new gpu value: " + value2.ToString() + " package power: " + cpuPower);
                }

                if (Global_Variables.Global_Variables.CPUMaxFrequency != value1)
                {
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => MaxProcFreq_Management.MaxProcFreq_Management.changeCPUMaxFrequency((int)Math.Round(value1, 0)));
                    lastCPUValue = value1;
                }
                if (Global_Variables.Global_Variables.GPUCLK != value2)
                {
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => GPUCLK_Management.GPUCLK_Management.changeAMDGPUClock((int)Math.Round(value2, 0)));
                    lastGPUValue = value2;
                }
               


            }
            osd.Update("");
            osd.Dispose();
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
            getCPUClock();
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

            procUtility.Add(theCPUCounter.NextValue());
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
        public static void getCPUClock()
        {


            cpuClock.Add(theCPUClockCounter.NextValue());
    
            if (cpuClock.Count > 3)
            {
                cpuClock.RemoveAt(0);
                cpuClock_Avg = cpuClock.Average();
                cpuClock_Min = cpuClock.Min();
                cpuClock_Max = cpuClock.Max();
             
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
