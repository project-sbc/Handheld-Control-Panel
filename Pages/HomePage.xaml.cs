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
using Handheld_Control_Panel.UserControls;

namespace Handheld_Control_Panel.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private string windowpage;
        private List<UserControl> userControls = new List<UserControl>();

        private int highlightedUserControl = -1;
        private int selectedUserControl = -1;
        public HomePage()
        {
            InitializeComponent();
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.SystemTheme + "." + Properties.Settings.Default.systemAccent);
            addUserControlsToPage();

            MainWindow wnd = (MainWindow)Application.Current.MainWindow;
            wnd.changeUserInstruction("HomePage_Instruction");
            wnd = null;
        }
        private void addUserControlsToPage()
        {

            try
            {
                List<string> list = Properties.Settings.Default.qamUserControls.Split(';').ToList();
                foreach (HomePageItem item in Global_Variables.homePageItems)
                {

                    if (item.UserControl != "")
                    {
                        if (item.Enabled)
                        {
                            Debug.WriteLine(item.UserControl);
                            switch (item.UserControl)
                            {
                                
                                case "UserControl_FanControl":
                                    stackPanel.Children.Add(new Fan_Slider());
                                    break;
                                case "Usercontrol_Wifi":
                                    stackPanel.Children.Add(new Wifi_Toggle());
                                    break;
                                case "Usercontrol_Bluetooth":
                                    stackPanel.Children.Add(new Bluetooth_Toggle());
                                    break;
                                case "Usercontrol_Brightness":
                                    stackPanel.Children.Add(new Brightness_Slider());
                                    break;
                                case "Usercontrol_VolumeMute":
                                    stackPanel.Children.Add(new VolumeMute_Toggle());
                                    break;
                                case "Usercontrol_Volume":
                                    stackPanel.Children.Add(new Volume_Slider());
                                    break;
                                case "Usercontrol_Resolution":
                                    stackPanel.Children.Add(new Resolution_Dropdown());
                                    break;
                                case "Usercontrol_RefreshRate":
                                    stackPanel.Children.Add(new RefreshRate_Dropdown());
                                    break;
                                case "Usercontrol_TDP":
                                    stackPanel.Children.Add(new TDP_Slider());
                                    break;
                                case "Usercontrol_TDP2":
                                    if (!Properties.Settings.Default.combineTDP)
                                    {
                                        stackPanel.Children.Add(new TDP2_Slider());
                                    }

                                    break;
                                case "Usercontrol_EPP":
                                    stackPanel.Children.Add(new EPP_Slider());
                                    break;
                                case "Usercontrol_FPSLimit":
                                    stackPanel.Children.Add(new FPS_Slider());
                                    break;
                                case "Usercontrol_ActiveCores":
                                    stackPanel.Children.Add(new ActiveCores_Slider());
                                    break;
                                case "Usercontrol_MaxCPUFrequency":
                                    stackPanel.Children.Add(new MaxCPU_Slider());
                                    break;
                                case "Usercontrol_GPUCLK":
                                    stackPanel.Children.Add(new GPUCLK_Slider());
                                    break;
                                case "Divider":
                                    stackPanel.Children.Add(new Divider());
                                    break;
                                case "Usercontrol_Scaling":
                                    stackPanel.Children.Add(new Scaling_Dropdown());
                                    break;
                                case "Usercontrol_MouseMode":
                                    stackPanel.Children.Add(new MouseMode_Toggle());
                                    break;
                                case "Usercontrol_Controller":
                                    stackPanel.Children.Add(new Controller_Toggle());
                                    break;
                                default: break;
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " during add controls");
            }
            
        

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //get unique window page combo from getwindow to string
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            //subscribe to controller input events
            Controller_Window_Page_UserControl_Events.pageControllerInput += handleControllerInputs;
            getUserControlsOnPage();

        }

        private void getUserControlsOnPage()
        {
            foreach (object child in stackPanel.Children)
            {
                if (child is UserControl)
                {
                    if (!child.ToString().Contains(".Divider"))
                    {
                        userControls.Add((UserControl)child);
                    }
                   
                }

            }
        }
        //
        private void handleControllerInputs(object sender, EventArgs e)
        {
            //get action from custom event args for controller
            controllerPageInputEventArgs args = (controllerPageInputEventArgs)e;
            string action = args.Action;

            if (args.WindowPage == windowpage)
            {
                //global method handles the event tracking and returns what the index of the highlighted and selected usercontrolshould be
                int[] intReturn = WindowPageUserControl_Management.globalHandlePageControllerInput(windowpage, action, userControls, highlightedUserControl, selectedUserControl, stackPanel);
              
                highlightedUserControl = intReturn[0];
                selectedUserControl = intReturn[1];
   
            }

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.pageControllerInput -= handleControllerInputs;
        }
    }
}
