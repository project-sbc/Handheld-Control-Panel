using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Handheld_Control_Panel.Classes
{
    public static class FullScreen_Management
    {
        public static List<string> ExcludeFullScreeProcessList = new List<string>()
        {
           {"TextInputHost"},

         };

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(HandleRef hWnd, [In, Out] ref RECT rect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

    

        public static bool IsForegroundFullScreen(HandleRef hWnd, System.Windows.Forms.Screen screen)
        {
            if (screen == null)
            {
                screen = System.Windows.Forms.Screen.PrimaryScreen;
            }
            RECT rect = new RECT();
            GetWindowRect(hWnd, ref rect);


            return new System.Drawing.Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top).Contains(screen.Bounds);
        }
        private static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hwnd, ref placement);
            return placement;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(
            IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public ShowWindowCommands showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        internal enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }


        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);


        public static Process suspendedProcess;
      
      

        public static void checkResumeProcess()
        {
            if (suspendedProcess != null)
            {
                var process = suspendedProcess;
                suspendedProcess = null;
                if (process.ProcessName == string.Empty)
                    return;

                foreach (ProcessThread pT in process.Threads)
                {
                    IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                    if (pOpenThread == IntPtr.Zero)
                    {
                        continue;
                    }

                    var suspendCount = 0;
                    do
                    {
                        suspendCount = ResumeThread(pOpenThread);
                    } while (suspendCount > 0);

                    CloseHandle(pOpenThread);
                }
               
            }
        }
        public static void checkSuspendProcess()
        {
            Process[] pList = Process.GetProcesses();
            foreach (Process process in pList)
            {
                if (process.MainWindowHandle != IntPtr.Zero)
                {


                    if (FullScreen_Management.IsForegroundFullScreen(new HandleRef(null, process.MainWindowHandle), null) && !FullScreen_Management.ExcludeFullScreeProcessList.Contains(process.ProcessName))
                    {
                        suspendedProcess = process;

                        foreach (ProcessThread pT in process.Threads)
                        {
                            IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                            if (pOpenThread == IntPtr.Zero)
                            {
                                continue;
                            }

                            SuspendThread(pOpenThread);

                            CloseHandle(pOpenThread);
                        }
                        return;

                    }
                }
            }


           
        }

      
    }
}
