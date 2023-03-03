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

namespace Handheld_Control_Panel.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class QAMHomePage : Page
    {
        private string windowpage;
        private List<UserControl> userControls = new List<UserControl>();
        private int selectedUserControl = -1;
        private int highlightedUserControl = -1;
        private List<string> listUserControls;
        public QAMHomePage()
        {
            InitializeComponent();
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.SystemTheme + "." + Properties.Settings.Default.systemAccent);

            listUserControls = Properties.Settings.Default.qamUserControls.Split(';').ToList();
            addUserControls();
          
        }

       

        private void addUserControls()
        {
            if (listUserControls.Count > 0)
            {
                foreach (string item in listUserControls)
                {
                    object control;
                    switch (item)
                    {
                        case "Brightness_Slider":
                            stackPanel.Children.Add(new Brightness_Slider());
                            break;
                        case "CoreParking_Slider":
                            stackPanel.Children.Add(new CoreParking_Slider());
                            break;
                        case "CPUFrequency_Slider":
                            stackPanel.Children.Add(new CPUFrequency_Slider());
                            break;
                        case "EPP_Slider":
                            stackPanel.Children.Add(new EPP_Slider());
                            break;
                        case "FPSLimit_Slider":
                            stackPanel.Children.Add(new FPSLimit_Slider());
                            break;
                        case "GPUCLK_Slider":
                            stackPanel.Children.Add(new GPUCLK_Slider());
                            break;
                        case "QuickAction_Wrappanel":
                            stackPanel.Children.Add(new QuickAction_Wrappanel());
                            break;
                        case "Refresh_Listbox":
                            stackPanel.Children.Add(new Refresh_Listbox());
                            break;


                        case "Resolution_Listbox":
                            stackPanel.Children.Add(new Resolution_Listbox());
                            break;
                        case "Scaling_Listbox":
                            stackPanel.Children.Add(new Scaling_Listbox());
                            break;
                        case "TDP_Slider":
                            stackPanel.Children.Add(new TDP_Slider());
                            break;
                        case "TDP2_Slider":
                            stackPanel.Children.Add(new TDP2_Slider());
                            break;
                        case "Volume_Slider":
                            stackPanel.Children.Add(new Volume_Slider());
                            break;
                        default: break;
                    }

                }

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
                    userControls.Add((UserControl)child);
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
                int intReturn = WindowPageUserControl_Management.globalHandlePageControllerInput(windowpage, action, userControls, highlightedUserControl, selectedUserControl, stackPanel)[0];
              
                highlightedUserControl = intReturn;
             
            }

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.pageControllerInput -= handleControllerInputs;
        }
    }
}
