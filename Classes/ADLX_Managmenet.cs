using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace Handheld_Control_Panel.Classes
{
    public static class ADLX_Managmenet
    {

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
        [DllImport(CppFunctionsDLL3, CallingConvention = CallingConvention.Cdecl)] public static extern int GetRSRState();

        [DllImport(CppFunctionsDLL3, CallingConvention = CallingConvention.Cdecl)] public static extern bool SetRSRSharpness(int sharpness);
        [DllImport(CppFunctionsDLL3, CallingConvention = CallingConvention.Cdecl)] public static extern int GetRSRSharpness();

        #endregion




    }
}
