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
    public partial class MainWindow : Window
    {
        private System.Collections.IList menuItems;
        public MainWindow()
        {
            Controller_Management.start_Controller_Management();
            InitializeComponent();
            //subscribe to controller events
            Controller_Management.buttonEvents.controllerInput += handleControllerInputs;
            //set menu item list
            menuItems = new List<NavigationViewItem>();
            menuItems = mainWindowNavigationView.MenuItems;
        }

        private void handleControllerInputs(object sender, EventArgs e)
        {
            //get action from custom event args for controller
            Handheld_Control_Panel.Classes.Controller_Management.controllerInputEventArgs args = (Handheld_Control_Panel.Classes.Controller_Management.controllerInputEventArgs)e;
            string action = args.Action;

            if (MainWindowNavigation.windowNavigation)
            {
                navigateNavigationView(action);
            }
            else
            {
                raiseControllerInput(action);
            }

        }


        public event EventHandler<controllerInputEventArgs> controllerInput;

        public void raiseControllerInput(string action)
        {
            controllerInput?.Invoke(this, new controllerInputEventArgs(action));
        }

        private void navigateNavigationView(string action)
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
            }
        }

        private void NavigationView_ItemInvoked(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewItemInvokedEventArgs args)
        {
            NavigationView navView = sender;
            Debug.WriteLine(navView.SelectedItem);
        }
    }
    public class controllerInputEventArgsMainWindow : EventArgs
    {
        public string Action { get; set; }
        public controllerInputEventArgsMainWindow(string action)
        {
            this.Action = action;
        }
    }
}
