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

namespace Handheld_Control_Panel.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class CustomizeHomePage : Page
    {
        private string windowpage;
        private List<UserControl> userControls = new List<UserControl>();

        private int highlightedUserControl = -1;
        public CustomizeHomePage()
        {
            InitializeComponent();
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.SystemTheme + "." + Properties.Settings.Default.systemAccent);

           
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //get unique window page combo from getwindow to string
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            //subscribe to controller input events
            Controller_Window_Page_UserControl_Events.pageControllerInput += handleControllerInputs;
          

            controlList.ItemsSource = Global_Variables.homePageItems;


        }

        private void handleControllerInputs(object sender, EventArgs e)
        {
            //get action from custom event args for controller
            controllerPageInputEventArgs args = (controllerPageInputEventArgs)e;
            string action = args.Action;

            if (args.WindowPage == windowpage)
            {
                //global method handles the event tracking and returns what the index of the highlighted and selected usercontrolshould be
                int intReturn = 0;//WindowPageUserControl_Management.globalHandlePageControllerInput(windowpage, action, userControls, highlightedUserControl, 0, stackPanel);
              
                highlightedUserControl = intReturn;
   
            }

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.pageControllerInput -= handleControllerInputs;
            Global_Variables.homePageItems.saveList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (controlList.SelectedItem != null)
            {
                HomePageItem hpi = controlList.SelectedItem as HomePageItem;
                if (controlList.SelectedIndex > 0)
                {
                    int index = controlList.SelectedIndex;
                    Global_Variables.homePageItems.Remove(hpi);
                    Global_Variables.homePageItems.Insert(index -1,hpi);
                    controlList.Items.Refresh();

                }
                
            }
        }
    }
}
