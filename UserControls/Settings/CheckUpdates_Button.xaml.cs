using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.Update_Software;
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
using System.Windows.Controls.Primitives;
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
    /// 
    
    public partial class CheckUpdates_Button : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";

        public string Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public CheckUpdates_Button()
        {
            InitializeComponent();
            UserControl_Management.setupControl(control);
            version.Content = Version;
        }

        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.", "");
            //if (control is Slider) { UserControl_Management.setThumbSize((Slider)control); }
           
            if (Window.GetWindow(this).ActualWidth < 650) { subText.Visibility = Visibility.Collapsed; }


        }
        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {
                if (args.Action == "A")
                {
                    control.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

                }
                else
                {
                    
                    Classes.UserControl_Management.UserControl_Management.handleUserControl(border, control, args.Action);
                }
                
            }
        }

        private void control_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(control.IsLoaded)
            {
                UserControl_Management.Slider_ValueChanged(sender, e);
            }
       
        }

   

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Update_Software.checkForUpdates(false);
        }
    }
}
