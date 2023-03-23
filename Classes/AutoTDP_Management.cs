using RTSSSharedMemoryNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Handheld_Control_Panel.Classes
{
    internal class Backend
    {
       
    }

    public static class AutoTDP_Management
    {
        public const string CppFunctionsDLL = @"Resources\AMD\ADLX\ADLX_PerformanceMetrics.dll";
        public const string CppFunctionsDLL2 = @"Resources\AMD\ADLX\ADLX_AutoTuning.dll";
        public const string CppFunctionsDLL3 = @"Resources\AMD\ADLX\ADLX_3DSettings.dll";

        [DllImport(CppFunctionsDLL, CallingConvention = CallingConvention.Cdecl)] public static extern int GetFPSData();

        [DllImport(CppFunctionsDLL, CallingConvention = CallingConvention.Cdecl)] public static extern int GetGPUMetrics(int GPU, int Sensor);

        [DllImport(CppFunctionsDLL2, CallingConvention = CallingConvention.Cdecl)] public static extern int SetAutoTuning(int GPU, int num);

        [DllImport(CppFunctionsDLL2, CallingConvention = CallingConvention.Cdecl)] public static extern int GetAutoTuning(int GPU);

        [DllImport(CppFunctionsDLL2, CallingConvention = CallingConvention.Cdecl)] public static extern int GetFactoryStatus(int GPU);

        [DllImport(CppFunctionsDLL3, CallingConvention = CallingConvention.Cdecl)] public static extern int SetFPSLimit(int GPU, bool isEnabled, int FPS);

        public static void writeGPUInfo()
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


                Thread.Sleep(2000);
            }


        }


    }
}
