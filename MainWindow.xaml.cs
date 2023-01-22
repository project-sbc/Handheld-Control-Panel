using Handheld_Control_Panel.Classes.Controller_Management;
using ModernWpf.Controls;
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
using System.Windows.Shapes;
using ControlzEx.Theming;
using MahApps.Metro.Controls;

namespace Handheld_Control_Panel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public static class MainWindowNavigation
    {
        public static bool windowNavigation = true;
    }
    public partial class MainWindow : MetroWindow
    {
        private string window = "MainWindow";
        private string page = "";
        public MainWindow()
        {
            
            InitializeComponent();
            //subscribe to controller events
            Controller_Management.buttonEvents.controllerInput += handleControllerInputs;
            mainWindowNavigationView.SelectedItem = mainWindowNavigationView.MenuItems[0];


            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.SystemTheme + "." + Properties.Settings.Default.systemAccent);
        }

        private void handleControllerInputs(object sender, EventArgs e)
        {
            //get action from custom event args for controller
            Handheld_Control_Panel.Classes.Controller_Management.controllerInputEventArgs args = (Handheld_Control_Panel.Classes.Controller_Management.controllerInputEventArgs)e;
            if (MainWindowNavigation.windowNavigation)
            {
                navigateNavigationView(args.Action);
            }
            else
            {
                Controller_Window_Page_UserControl_Events.raisePageControllerInputEvent(args.Action,window+page);
            }

        }



        private void navigateNavigationView(string action)
        {
            try
            {
                if (action == "Up")
                {
                    mainWindowNavigationView.SelectedItem = mainWindowNavigationView.MenuItems[mainWindowNavigationView.MenuItems.IndexOf(mainWindowNavigationView.SelectedItem) - 1];
                }
                if (action == "Down")
                {
                    mainWindowNavigationView.SelectedItem = mainWindowNavigationView.MenuItems[mainWindowNavigationView.MenuItems.IndexOf(mainWindowNavigationView.SelectedItem) + 1];
                }
                if (action == "Right" || action == "A")
                {
                    MainWindowNavigation.windowNavigation = false;
                    Controller_Window_Page_UserControl_Events.raisePageControllerInputEvent("Right", window + page);
                }
            }
            catch
            {

            }
        }

    
        private void mainWindowNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem navigationViewItem = (NavigationViewItem)mainWindowNavigationView.SelectedItem;
            frame.Navigate(new Uri("Pages\\" + navigationViewItem.Tag.ToString() + ".xaml", UriKind.RelativeOrAbsolute));
            page = navigationViewItem.Tag.ToString();
            
        }

       
    }
    
}
