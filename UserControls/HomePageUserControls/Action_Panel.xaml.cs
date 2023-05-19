using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.TaskSchedulerWin32;
using Handheld_Control_Panel.Classes.UserControl_Management;
using Handheld_Control_Panel.Styles;
using MahApps.Metro.Controls;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class Action_Panel : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private bool dragStarted = false;
        private List<Action_Panel_Items> apiList = new List<Action_Panel_Items>();

        public Action_Panel()
        {
            InitializeComponent();
            //setControlValue();
            UserControl_Management.setupControl(null);


            loadActionPanelList();


        }

        private void loadActionPanelList()
        {
            foreach (HotkeyItem hki in Global_Variables.hotKeys)
            {
                if (hki.AddHomePage)
                {
                    Action_Panel_Items api = new Action_Panel_Items();
                    api.hki = hki;
                    switch(hki.Action)
                    {
                        case "Open_Program":
                            
                            break;
                        default:
                            api.data = Application.Current.Resources["Path_Data_" + hki.Action].ToString();
                            api.imageVisibility = Visibility.Collapsed;
                            api.canvasVisibility = Visibility.Visible;
                            break;
                    }
                    if (hki.Action != "")
                    {
                        apiList.Add(api);
                    }
                }

            }

            controlList.ItemsSource = apiList;


        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //UserControl_Management.setThumbSize(control);

            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.", "");

        }




        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args = (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl == usercontrol)
            {
                Classes.UserControl_Management.UserControl_Management.handleUserControl(border, null, args.Action);

            }
        }




        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;

        }




    }
    public class Action_Panel_Items
    {
        public HotkeyItem hki { get; set; }
        public string data { get; set; }
        public Visibility canvasVisibility { get; set; }
        public Visibility imageVisibility { get; set; }
        public Image image { get; set; }

    }
}
