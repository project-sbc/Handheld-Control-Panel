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

namespace Handheld_Control_Panel.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
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


            

            string gamerunning = OSD_Management.gameRunning();

            if (gamerunning != "")
            {
                powerpageitem gameppi = new powerpageitem();
                gameppi.displayitem = Application.Current.Resources["UserControl_CloseGame"].ToString() + " " + gamerunning;
                gameppi.item = "CloseGame";
                gameppi.Kind = PackIconMaterialKind.MicrosoftXboxController;
                powerpageitems.Add(gameppi);
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
                        int processID = OSD_Management.closeGame();
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
    }
}
