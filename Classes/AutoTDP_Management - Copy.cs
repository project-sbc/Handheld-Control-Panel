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

namespace Handheld_Control_Panel.Classes
{
    
    public static class AutoTDP_Management_OLD
    {
        

        public static void startAutoTDPThread()
        {
            Global_Variables.Global_Variables.autoTDP = true;
            autoTDPThread = new Thread(() => { mainAutoTDPLoop(); });
            autoTDPThread.Start();
        }


        private static double CPU_Last;
        //proposedCPU is user defined or default of highest cpu clock (set to 9999 because cpu wont reach that clock)
        private static int proposedCPU;
        private static int proposedGPU;
        private static double originalEPP;
        private static int cpuLast;
        private static int gpuLast;
        private static int minCPU = 1100;
        private static int maxCPU= 4700;
        private static int minGPU=533;
        private static int maxGPU=2200;
        private static int offSetCPU=0;
        private static int offSetGPU=0;
        private static int autoOffsetMaxGPU = 125;
        private static int autoMaxGPU;
        private static int tarCPU;
        private static int tarGPU;

        private static List<float> useCPU = new List<float>();

        private static void mainAutoTDPLoopHyatice()
        {
            //computer.Open() starts a new Librehardware monitor instance so we can start getting data. We close it at the end when auto tdp is done to save resources
            computer.Open();
            //Check to make sure main window is open. We dont want someone closing the program and this autotdp thread is preventing it from ending the overall process. Also make sure autoTDP is still enabled, as soon as someone says we dont want auto tdp anymore thatwill be the last cycle


            //HYATICEHYATICEHYATICEHYATICEHYATICEHYATICE
            //HYATICEHYATICEHYATICEHYATICEHYATICEHYATICE
            //HYATICEHYATICEHYATICEHYATICEHYATICEHYATICE


            if (Global_Variables.Global_Variables.autoTDP)
            {
                //First time when autoTDP enabled only
                //HYATICE - Define a CPU_Last Variable; can be temp to the AutoTDP thread as a whole but must survive between loops.
                //set proposedCPU=CPU_Max - as defined by the user in the game/global profile; default to max CPU clock/9999?
                proposedCPU = 9999;
                //set proposedGPU=GPU_Min - as defined by the user in the game/global profile; default to 533mhz
                proposedGPU = 533;
                //ryzenadj.exe --max-performance - This will need to be re-ran if autoTDP is running and power is unplugged, or on wake from sleep.
                Classes.Task_Scheduler.Task_Scheduler.runTask(() => AMDPowerSlide_Management.AMDPowerSlide_Management.setAMDRyzenAdjPowerPerformance());

                //change power plan EPP to 0% - Be sure to save the original EPP to go back to after AutoTDP stops
                originalEPP = Global_Variables.Global_Variables.EPP;

                //  powercfg -setacvalueindex sub_processor perfepp 0
                //  powercfg -setacvalueindex sub_processor perfepp1 0
                //  powercfg -setdcvalueindex sub_processor perfepp 0
                //  powercfg -setdcvalueindex sub_processor perfepp1 0
                //  powercfg / s scheme_current
                Classes.Task_Scheduler.Task_Scheduler.runTask(() => EPP_Management.EPP_Management.changeEPP(0));

                // Will need to apply the same CPU/GPU clocks when exiting AutoTDP
                // Will need to apply ryzenadj --power-saving when exiting AutoTDP if on battery
                // Will need to restore EPP to what it was before AutoTDP ran/probably what's set in the game/global profile
                // Possibly prompt to reset GPU clocks to defaults by restarting the GPU driver?
            }


            //HYATICEHYATICEHYATICEHYATICEHYATICEHYATICE
            //HYATICEHYATICEHYATICEHYATICEHYATICEHYATICE
            //HYATICEHYATICEHYATICEHYATICEHYATICEHYATICE


            while (Global_Variables.Global_Variables.autoTDP)
            {
                //HYATICE - If proposedCPU different than lastCPU Apply CPU changes and update lastCPU
                if (proposedCPU != cpuLast)
                {
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => MaxProcFreq_Management.MaxProcFreq_Management.changeCPUMaxFrequency(proposedCPU));
                }
                cpuLast = proposedCPU;
                if (proposedGPU != gpuLast)
                {
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => GPUCLK_Management.GPUCLK_Management.changeAMDGPUClock(proposedGPU));
                }
                gpuLast = proposedGPU;
                //HYATICE - If proposedGPU different than lastGPU Apply GPU changes and update lastGPU

                //thread sleep just adds a pause (in ms)
                Thread.Sleep(1000);


                getInformationForAutoTDP();


                //HYATICEHYATICEHYATICEHYATICEHYATICEHYATICE
                //HYATICEHYATICEHYATICEHYATICEHYATICEHYATICE
                //HYATICEHYATICEHYATICEHYATICEHYATICEHYATICE
                //
                //
                //      Need the following variables from global/per-game profiles
                //      minCPU (default 1100 for 6800U)
                //      maxCPU (default 4700 for 6800U)
                //      offsetCPU (default 0)
                //      minGPU (default 533 for 6800U; behavior gets wonky below 533)
                //      maxGPU (default 2200 for 6800U)
                //      offsetGPU (default 0)
                        minCPU = 1100;
                        maxCPU = 4700;
                        minGPU = 533;
                        maxGPU = 2200;
                        offSetCPU = 0;
                        offSetGPU = 0;
                autoOffsetMaxGPU = 125;
                //      autoOffsetMaxGPU (set in profiles, default 125)
                //      autoMaxGPU (calculate based on current TDP using the below formula)
                //          
                autoMaxGPU = (int)Math.Round(Math.Min(Math.Max(minGPU, -8708.3367 + 1045.3643 * Math.Log(Global_Variables.Global_Variables.ReadPL1*1000) - autoOffsetMaxGPU), maxGPU),0);
        //
        //      Ensure that measured CPU Actual Clock is at least 1Mhz
        //
        //      Ensure that measured CPU Utility is at least 1%
        //      utilCPU should be measured as a float with at least 1 point of decimal precision
                if (procUtility[procUtility.Count-1] > 1 && cpuClock[cpuClock.Count-1] > 1)
                {
                    useCPU.Add(Math.Max(1, procUtility[procUtility.Count - 1]* minCPU)/ cpuClock[cpuClock.Count - 1]);
                    if (useCPU.Count > 3)
                    {
                        useCPU.RemoveAt(0);
                    }
                    tarCPU = (int)Math.Round(Math.Min(96, useCPU.Average() + .5), 0);
                    proposedCPU = (int)Math.Round(Math.Min(Math.Max(minCPU, cpuClock[cpuClock.Count-1] * useCPU[useCPU.Count-1] / tarCPU + offSetCPU), maxCPU),0);
                }
        //      useCPU = Math.Max(1, utilCPU * baseCPU)/actCPU)
        //      useCPU should be measured as a float with at least 1 point of decimal precision
        //      Need average of useCPU over previous 2 cycles + current cycle
        //
        //      tarCPU = Math.Min(96, average(useCPU)+.5)
        //          The .5 (as a percent) is small enough that it almost never causes FPS dips during actual gameplay, but prevents runaway.
        //
        //      The min(96, x) makes it so that the max tarCPU value is 96%. This means that an average of 99% will still have some headroom to push clocks higher when necessary.
        //
        //      proposedCPU = Math.Min(Math.Max(minCPU, actCPU*useCPU/tarCPU+offsetCPU), maxCPU)
        //
        //      Ensure that measured GPU Usage is at least 1%
        //      useGPU should be measured as a float with at least 1 point of decimal precision
        //      Need average of useGPU over previous 2 cycles + current cycle
        //      
        //      tarGPU = Math.Min(96, average(useGPU)+.5) - See above re: CPU
        //      proposedGPU = Math.Min(Math.Max(minGPU, actGPU*useGPU/tarGPU+offsetGPU), autoMaxGPU)
                if (gpuUsage[gpuUsage.Count-1] > 1)
                {
                    tarGPU = (int)Math.Round(Math.Min(96, gpuUsage_Avg + .5));
                    proposedGPU = (int)Math.Round((double)Math.Min(Math.Max(minGPU, Global_Variables.Global_Variables.GPUCLK * gpuUsage[gpuUsage.Count - 1] / tarGPU + offSetGPU), autoMaxGPU), 0);
                }
                //HYATICEHYATICEHYATICEHYATICEHYATICEHYATICE
                //HYATICEHYATICEHYATICEHYATICEHYATICEHYATICE
                //HYATICEHYATICEHYATICEHYATICEHYATICEHYATICE
            }

            //when autotdp is set to false the loop will end 

            //close librehardware monitor instance
            computer.Close();
        }
        private static void mainAutoTDPLoop()
        {
            //computer.Open() starts a new Librehardware monitor instance so we can start getting data. We close it at the end when auto tdp is done to save resources
            computer.Open();
            //Check to make sure main window is open. We dont want someone closing the program and this autotdp thread is preventing it from ending the overall process. Also make sure autoTDP is still enabled, as soon as someone says we dont want auto tdp anymore thatwill be the last cycle


            double LASTCPU = 3500;
            double LASTGPU = 500;

            if (Global_Variables.Global_Variables.autoTDP)
            {
                //First time when autoTDP enabled only
                //HYATICE - Define a CPU_Last Variable; can be temp to the AutoTDP thread as a whole but must survive between loops.
                //set proposedCPU=CPU_Max - as defined by the user in the game/global profile; default to max CPU clock/9999?
                
                //set proposedGPU=GPU_Min - as defined by the user in the game/global profile; default to 533mhz

                //ryzenadj.exe --max-performance - This will need to be re-ran if autoTDP is running and power is unplugged, or on wake from sleep.
                Classes.Task_Scheduler.Task_Scheduler.runTask(() => AMDPowerSlide_Management.AMDPowerSlide_Management.setAMDRyzenAdjPowerPerformance());

                //change power plan EPP to 0% - Be sure to save the original EPP to go back to after AutoTDP stops
                originalEPP = Global_Variables.Global_Variables.EPP;

                //  powercfg -setacvalueindex sub_processor perfepp 0
                //  powercfg -setacvalueindex sub_processor perfepp1 0
                //  powercfg -setdcvalueindex sub_processor perfepp 0
                //  powercfg -setdcvalueindex sub_processor perfepp1 0
                //  powercfg / s scheme_current
                Classes.Task_Scheduler.Task_Scheduler.runTask(() => EPP_Management.EPP_Management.changeEPP(0));

                // Will need to apply the same CPU/GPU clocks when exiting AutoTDP
                // Will need to apply ryzenadj --power-saving when exiting AutoTDP if on battery
                // Will need to restore EPP to what it was before AutoTDP ran/probably what's set in the game/global profile
                // Possibly prompt to reset GPU clocks to defaults by restarting the GPU driver?
            }


            //HYATICEHYATICEHYATICEHYATICEHYATICEHYATICE
            //HYATICEHYATICEHYATICEHYATICEHYATICEHYATICE
            //HYATICEHYATICEHYATICEHYATICEHYATICEHYATICE
            var cpuPID = new PidController(-2,-1,0,4600,1800)
            {
                TargetValue = 70
            };
            var gpuPID = new PidController(-2, -1, 0, 2200, 600)
            {
                TargetValue = 75
            };

            while (Global_Variables.Global_Variables.autoTDP)
            {
                //HYATICE - If proposedCPU different than lastCPU Apply CPU changes and update lastCPU




                //thread sleep just adds a pause (in ms)
                Thread.Sleep(1000);


                getInformationForAutoTDP();
                Debug.WriteLine("procUtility: " + procUtility[procUtility.Count - 1].ToString());
                Debug.WriteLine("gpuUsage: " + gpuUsage[gpuUsage.Count - 1].ToString());
             

                cpuPID.CurrentValue =procUtility_Avg;
                double value1 = Math.Floor(cpuPID.ControlOutput / 50.0) * 50.0;
                gpuPID.CurrentValue = gpuUsage_Avg;
                double value2 = Math.Floor(gpuPID.ControlOutput / 50.0) * 50.0;
                Debug.WriteLine("new max cpu: " + value1.ToString());
                Debug.WriteLine("new gpu: " + value2.ToString());
                Debug.WriteLine("package power: " + cpuPower.ToString());

                if (LASTCPU != value1)
                {
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => MaxProcFreq_Management.MaxProcFreq_Management.changeCPUMaxFrequency((int)Math.Round(value1, 0)));
                    LASTCPU = value1;
                }
                if (LASTGPU!= value2)
                {
                    //Classes.Task_Scheduler.Task_Scheduler.runTask(() => MaxProcFreq_Management.MaxProcFreq_Management.changeCPUMaxFrequency(2500));
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => GPUCLK_Management.GPUCLK_Management.changeAMDGPUClock((int)Math.Round(value2, 0)));
                    LASTGPU = value2;
                }
                


             


            }

            //when autotdp is set to false the loop will end 

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

        private static void getInformationForAutoTDP()
        {
        start:
            //get d3d usage, cpu usage
            getLibreHardwareMonitorInfo();
            getGPUUsage();
            //get proc utility
            getProcUtility();
            //get cpu
            getCPUClock();

            if (cpuClock.Count < 3 && Global_Variables.Global_Variables.autoTDP)
            {
                Thread.Sleep(250);
                goto start;
            }

            


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
            else
            {
                RTSS.startRTSS();
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
            if (gpuUsage.Count > 3)
            {
                gpuUsage.RemoveAt(0);
                gpuUsage_Avg = gpuUsage.Average();
                gpuUsage_Min = gpuUsage.Min();
                gpuUsage_Max = gpuUsage.Max();
                //gpuUsage_Slope = (gpuUsage_Avg - ((gpuUsage[3] + gpuUsage[4]) / 2) / 2);
            }
        }
        #endregion
        public static void getLibreHardwareMonitorInfo()
        {

            computer.Accept(new UpdateVisitor());
            getLibre_packagepower();

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
            if (procUtility.Count > 3)
            {
                procUtility.RemoveAt(0);
                procUtility_Avg = procUtility.Average();
                procUtility_Min = procUtility.Min();
                procUtility_Max = procUtility.Max();
                //procUtility_Slope = (procUtility_Avg - ((procUtility[3] + procUtility[4]) / 2) / 2);
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
                //cpuClock_Slope = (cpuClock_Avg - ((cpuClock[3] + cpuClock[4]) / 2) / 2);
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
