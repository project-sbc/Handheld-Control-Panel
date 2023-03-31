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
using Windows.Devices.Perception.Provider;

namespace Handheld_Control_Panel.Classes
{
    
    public static class AutoTDP_Management
    {
        

        public static void startAutoTDPThread()
        {
            Global_Variables.Global_Variables.autoTDP = true;
            autoTDPThread = new Thread(() => { mainAutoTDPLoop(); });
            autoTDPThread.Start();
        }


        private static void mainAutoTDPLoop()
        {
            //computer.Open() starts a new Librehardware monitor instance so we can start getting data. We close it at the end when auto tdp is done to save resources
            computer.Open();
            //Check to make sure main window is open. We dont want someone closing the program and this autotdp thread is preventing it from ending the overall process. Also make sure autoTDP is still enabled, as soon as someone says we dont want auto tdp anymore thatwill be the last cycle
            while (Global_Variables.Global_Variables.mainWindow.IsLoaded && Global_Variables.Global_Variables.autoTDP)
            {
                getInformationForAutoTDP();

                // use   gpuD3DUsage[gpuD3DUsage.Count-1]   for latest 3D usage value-------- or use gpuD3DUsage_Avg,gpuD3DUsage_Min, gpuD3DUsage_Max, gpuD3DUsage_Slope
                //same applies for fps,   fps[fps.Count-1]    or fps_Avg, fps_Min, fps_Max, fps_Slope
                //  also     procUtility[procUtility.Count-1] or procUtility_Avg, procUtility_Min, procUtility_Max, procUtility_Slope
                //lastly cpu clock is  cpuClock[cpuClock.Count-1]   and  cpuClock_Avg, cpuClock_Min, cpuClock_Max cpuClock_Slope

                // to change TDP we use  this:      Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.TDP_Management.TDP_Management.changeTDP((int)SustainedTDP, (int)BoostTDP));
                //Just put a number in for SustainedTDP  and BoostTDP. They can be the same.   Its a lot of code to change TDP, but its important to use the that line because I have a
                //dedicated task scheduler that prevents multiple calls to ryzenadj at the same time (if two threads try it at the same time it yells at you big time).
               
                //to  change gpu clock we use something similiar:      Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.GPUCLK_Management.GPUCLK_Management.changeAMDGPUClock((int)GPUCLK));
                //  you can change the GPUCLK value in the line above



                //task delay just adds a 500 ms pause before looping
                Task.Delay(500);
            }

            //when autotdp is set to false the loop will end and its important to close the thread it was running on
            autoTDPThread.Abort();
            //close librehardware monitor instance
            computer.Close();
        }

        public static void endAutoTDP()
        {
            Global_Variables.Global_Variables.autoTDP = false;

        }


        #region support routines

        private static Computer computer = new Computer
        {
            IsCpuEnabled = false,
            IsGpuEnabled = true,
            IsMemoryEnabled = false,
            IsMotherboardEnabled = false,
            IsControllerEnabled = false,
            IsNetworkEnabled = false,
            IsStorageEnabled = false,
            IsBatteryEnabled = false,
            IsPsuEnabled = false
        };

        private static void getInformationForAutoTDP()
        {
            //get fps from rtss statistics
            getFPS();
            //get d3d usage, cpu usage
            getLibreHardwareMonitorInfo();
            //get proc utility
            getProcUtility();


        }
        #region fps get data
        private static List<int> fps = new List<int>();
        private static double fps_Avg;
        private static double fps_Min;
        private static double fps_Max;
        private static double fps_Slope;
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
                    if (fps.Count > 5)
                    {
                        fps.RemoveAt(0);
                        fps_Avg = fps.Average();
                        fps_Min = fps.Min();
                        fps_Max = fps.Max();
                        fps_Slope = (fps_Avg - ((fps[3] + fps[4]) / 2) / 2);
                    }

                    break;

                }


            }
        }
        #endregion
        #region get gpu usage from AMD driver
        private static List<int> gpuUsage = new List<int>();
        private static double gpuUsage_Avg;
        private static int gpuUsage_Min;
        private static int gpuUsage_Max;
        private static double gpuUsage_Slope;
        private static void getGPUUsage()
        {

            gpuUsage.Add(GetGPUMetrics(0, 7));
            if (gpuUsage.Count > 5)
            {
                gpuUsage.RemoveAt(0);
                gpuUsage_Avg = gpuUsage.Average();
                gpuUsage_Min = gpuUsage.Min();
                gpuUsage_Max = gpuUsage.Max();
                gpuUsage_Slope = (gpuUsage_Avg - ((gpuUsage[3] + gpuUsage[4]) / 2) / 2);
            }
        }
        #endregion
        public static void getLibreHardwareMonitorInfo()
        {

            computer.Accept(new UpdateVisitor());
            getLibre_GPUD3D();

        }

        #region processor utility
        private static List<float> procUtility = new List<float>();
        private static double procUtility_Avg;
        private static float procUtility_Min;
        private static float procUtility_Max;
        private static double procUtility_Slope;
        private static PerformanceCounter theCPUCounter = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
        private static void getProcUtility()
        {

            procUtility.Add(theCPUCounter.NextValue());
            if (procUtility.Count > 5)
            {
                procUtility.RemoveAt(0);
                procUtility_Avg = procUtility.Average();
                procUtility_Min = procUtility.Min();
                procUtility_Max = procUtility.Max();
                procUtility_Slope = (procUtility_Avg - ((procUtility[3] + procUtility[4]) / 2) / 2);
            }
        }
        #endregion
        #region processor clock
        private static List<float> cpuClock = new List<float>();
        private static double cpuClock_Avg;
        private static float cpuClock_Min;
        private static float cpuClock_Max;
        private static double cpuClock_Slope;

        private static PerformanceCounter theCPUClockCounter = new PerformanceCounter("Processor Information", "Processor Frequency", "_Total");
        public static void getCPUClock()
        {


            cpuClock.Add(theCPUClockCounter.NextValue());
            if (cpuClock.Count > 5)
            {
                cpuClock.RemoveAt(0);
                cpuClock_Avg = cpuClock.Average();
                cpuClock_Min = cpuClock.Min();
                cpuClock_Max = cpuClock.Max();
                cpuClock_Slope = (cpuClock_Avg - ((cpuClock[3] + cpuClock[4]) / 2) / 2);
            }
        }
        #endregion
        #region gpu D3D usage from libre hardware monitor
        private static List<float> gpuD3DUsage = new List<float>();
        private static double gpuD3DUsage_Avg;
        private static float gpuD3DUsage_Min;
        private static float gpuD3DUsage_Max;
        private static double gpuD3DUsage_Slope;
        private static void getLibre_GPUD3D()
        {
            IHardware amdRadeon = computer.Hardware.FirstOrDefault(c => c.Name.StartsWith("AMD Radeon"));

            if (amdRadeon != null)
            {
                ISensor D3D = amdRadeon.Sensors.FirstOrDefault(c => c.Name == "D3D 3D");
                if (D3D != null)
                {
                    gpuUsage.Add((int)D3D.Value);
                    if (gpuUsage.Count > 5)
                    {
                        gpuUsage.RemoveAt(0);
                        gpuUsage_Avg = gpuUsage.Average();
                        gpuUsage_Min = gpuUsage.Min();
                        gpuUsage_Max = gpuUsage.Max();
                        gpuUsage_Slope = (gpuUsage_Avg - ((gpuUsage[3] + gpuUsage[4]) / 2) / 2);
                    }
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
        public static string[] writeGPUInfoOLD_FOR_REFERENCE()
        {


            while (true)
            {
                //if(i == 0)
                //{
                //    Backend.SetAutoTuning(0,0);
                //    i++;
                //}

                int isFactory = GetFactoryStatus(0);
                int autoTuning = GetAutoTuning(0);

                int fpsLimit = SetFPSLimit(0, true, 256);

                int gpuTotalPower = GetGPUMetrics(0, 5);
                int fps = GetFPSData();
                int gpuHotSpot = GetGPUMetrics(0, 2);
                int gpuTemp = GetGPUMetrics(0, 3);
                int gpuClock = GetGPUMetrics(0, 0);
                int gpuVRAMClock = GetGPUMetrics(0, 1);
                int gpuPower = GetGPUMetrics(0, 4);
                int gpuVRAM = GetGPUMetrics(0, 6);
                int gpuUsage = GetGPUMetrics(0, 7);
                int gpuVolt = GetGPUMetrics(0, 8);
                int gpuFan = GetGPUMetrics(0, 9);



                if (fps == -2) Debug.WriteLine($"FPS: Not Supported (use fullscreen if using windowed!)");
                else Debug.WriteLine($"FPS: {fps}");



                if (isFactory == -2 || isFactory == -1) Debug.WriteLine($"Factory Status: Not Supported");
                else Debug.WriteLine($"Factory Status: {isFactory}");

                if (autoTuning == -2 || autoTuning == -1) Debug.WriteLine($"Auto Tuning Status: Not Supported");
                else Debug.WriteLine($"Auto Tuning Status: {autoTuning}");



                if (gpuTemp == -2) Debug.WriteLine($"GPU Temp: Not Supported");
                else Debug.WriteLine($"GPU Temp: {gpuTemp} °C");

                if (gpuUsage == -2) Debug.WriteLine($"GPU Usage: Not Supported");
                else Debug.WriteLine($"GPU Usage: {gpuUsage} %");

                if (gpuClock == -2) Debug.WriteLine($"GPU Clock: Not Supported");
                else Debug.WriteLine($"GPU Clock: {gpuClock} MHz");





                if (gpuPower == -2) Debug.WriteLine($"GPU Power: Not Supported");
                else Debug.WriteLine($"GPU Power: {gpuPower} W");


                Task.Delay(200);
            }


        }

        public static Thread autoTDPThread;
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
