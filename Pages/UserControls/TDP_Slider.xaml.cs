using Handheld_Control_Panel.Classes.Controller_Management;
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

namespace Handheld_Control_Panel.Pages.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class TDP_Slider : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "TDP_Slider";
        public TDP_Slider()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            subscribeControllerInputEvents();
            getWindowPageString();
        }
        private void getWindowPageString()
        {
            string[] pagewindow = Window.GetWindow(this).ToString().Replace("}", "").Replace("{", "").Replace("Handheld_Control_Panel.", "").Replace(".xaml", "").Replace("Pages/", "").Split(":");
            windowpage = pagewindow[0].Trim() + pagewindow[1].Trim();
        }
        private void subscribeControllerInputEvents()
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
        }

        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {
                //Debug.WriteLine("YESSSS");
            }

        }
    }
}
