using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.UserControls;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using MahApps.Metro;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlzEx.Theming;
using System.Windows.Controls.Primitives;
using Handheld_Control_Panel.Classes.Global_Variables;
using System.Windows.Interop;
using MahApps.Metro.IconPacks;
using System.Runtime.InteropServices;


namespace Handheld_Control_Panel.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    /// 


    public partial class PowerPage : Page
    {
        private string windowpage;
        private List<powerpageitem> powerpageitems = new List<powerpageitem>();
        public PowerPage()
        {
            InitializeComponent();
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.SystemTheme + "." + Properties.Settings.Default.systemAccent);

            MainWindow wnd = (MainWindow)Application.Current.MainWindow;
            wnd.changeUserInstruction("HomePage_Instruction");
            wnd = null;



            List<Process> listProcesses = new List<Process>();

            Process[] pList = Process.GetProcesses();
            foreach (Process p in pList)
            {
                if (p.MainWindowHandle != IntPtr.Zero)
                {
                   
               
                    if (!listProcesses.Contains(p) && IsForegroundFullScreen(new HandleRef(null, p.MainWindowHandle), null) && !ExcludeFullScreeProcessList.Contains(p.ProcessName))
                    {
                        listProcesses.Add(p);
                    }
                }
            }

           

            if (listProcesses.Count >0)
            {
                foreach (Process p in listProcesses)
                {
                    powerpageitem gameppi = new powerpageitem();
                    gameppi.displayitem = Application.Current.Resources["UserControl_CloseGame"].ToString() + " " + p.ProcessName;
                    gameppi.item = "CloseGame";
                    gameppi.processID = p.Id;
                    gameppi.Kind = PackIconMaterialKind.MicrosoftXboxController;
                    powerpageitems.Add(gameppi);
                }

              
            }

            powerpageitem hideppi = new powerpageitem();
            hideppi.displayitem = Application.Current.Resources["UserControl_HideHCP"].ToString();
            hideppi.item = "HideHCP";
            hideppi.Kind = PackIconMaterialKind.DockRight;
            powerpageitems.Add(hideppi);

            powerpageitem closeppi = new powerpageitem();
            closeppi.displayitem = Application.Current.Resources["UserControl_CloseHCP"].ToString();
            closeppi.item = "CloseHCP";
            closeppi.Kind = PackIconMaterialKind.WindowClose;
            powerpageitems.Add(closeppi);

            powerpageitem restartppi = new powerpageitem();
            restartppi.displayitem = Application.Current.Resources["UserControl_RestartPC"].ToString();
            restartppi.item = "RestartPC";
            restartppi.Kind = PackIconMaterialKind.Restart;
            powerpageitems.Add(restartppi);

            powerpageitem shutdownppi = new powerpageitem();
            shutdownppi.displayitem = Application.Current.Resources["UserControl_ShutdownPC"].ToString();
            shutdownppi.item = "ShutdownPC";
            shutdownppi.Kind = PackIconMaterialKind.Power;
            powerpageitems.Add(shutdownppi);


            controlList.ItemsSource = powerpageitems;
        }

        private List<string> ExcludeFullScreeProcessList = new List<string>()
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

     

        public bool IsForegroundFullScreen(HandleRef hWnd, System.Windows.Forms.Screen screen)
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //get unique window page combo from getwindow to string
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            //subscribe to controller input events
            Controller_Window_Page_UserControl_Events.pageControllerInput += handleControllerInputs;


            

        }

        private void handleControllerInputs(object sender, EventArgs e)
        {
            //get action from custom event args for controller
            controllerPageInputEventArgs args = (controllerPageInputEventArgs)e;
            string action = args.Action;

            if (args.WindowPage == windowpage)
            {
                //global method handles the event tracking and returns what the index of the highlighted and selected usercontrolshould be
                if (controlList.SelectedItem != null)
                {
                    powerpageitem ppi = controlList.SelectedItem as powerpageitem;
                    int index = controlList.SelectedIndex;
                    switch (action)
                    {
                        case "A":
                            handleListChange();
                            break;
                                                  
                       
                        case "Up":
                            if (index > 0) { controlList.SelectedIndex = index - 1; controlList.ScrollIntoView(controlList.SelectedItem); }
                            break;
                        case "Down":
                            if (index < controlList.Items.Count - 1) { controlList.SelectedIndex = index + 1; controlList.ScrollIntoView(controlList.SelectedItem); }
                            break;
                            default: break;

                    }

                }
                else
                {
                    
                    if (action == "Up" || action == "Down")
                    {
                        if (controlList.Items.Count > 0) { controlList.SelectedIndex = 0; controlList.ScrollIntoView(controlList.SelectedItem); }
                    
                    }
                }


            }

        }


        private void handleListChange()
        {
            if (controlList.SelectedItem != null)
            {
                powerpageitem ppi = controlList.SelectedItem as powerpageitem;
                int index = controlList.SelectedIndex;

                MainWindow wnd = (MainWindow)Application.Current.MainWindow;
                var mainWindowHandle = new WindowInteropHelper(wnd).Handle;
                switch (ppi.item)
                {
                    case "CloseGame":
                        int processID = ppi.processID;
                        if (processID != 0)
                        {
                            System.Diagnostics.Process procs = null;

                            try
                            {
                                procs = Process.GetProcessById(processID);



                                if (!procs.HasExited)
                                {
                                    procs.CloseMainWindow();
                                }
                            }
                            finally
                            {
                                if (procs != null)
                                {
                                    procs.Dispose();
                                }
                                powerpageitems.Remove(ppi);
                                controlList.Items.Refresh();
                            }
                        }
                        break;


                    case "HideHCP":
                        wnd.toggleWindow();
                        break;
                    case "CloseHCP":
                        wnd.Close();
                        wnd = null;
                        break;
                    case "RestartPC":
                        if (CheckForegroundWindowQAM.IsActive(mainWindowHandle))
                        {
                            var psi = new ProcessStartInfo("shutdown", "/r /t 0");
                            psi.CreateNoWindow = true;
                            psi.UseShellExecute = false;
                            Process.Start(psi);
                        }
                        break;
                    case "ShutdownPC":
                        if (CheckForegroundWindowQAM.IsActive(mainWindowHandle))
                        {
                            var psi = new ProcessStartInfo("shutdown", "/s /t 0");
                            psi.CreateNoWindow = true;
                            psi.UseShellExecute = false;
                            Process.Start(psi);
                        }
                        break;
                    default: break;

                }

            }
          
           

        }
     

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.pageControllerInput -= handleControllerInputs;
        }

        private void controlList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            handleListChange();
        }
    }
    public class powerpageitem
    {
        public string displayitem { get; set; }
        public string item { get; set; }
        public PackIconMaterialKind Kind { get; set; }
        public int processID { get; set; } = 0;
    }
}
