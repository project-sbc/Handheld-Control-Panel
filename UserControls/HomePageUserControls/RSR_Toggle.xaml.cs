using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Windows.Devices.Radios;

namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class RSR_Toggle : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        public RSR_Toggle()
        {
            if (Global_Variables.cpuType == "AMD")
            {
                int RSRResult = ADLX_Managmenet.GetRSRState();
                if (RSRResult > -1)
                {

                    InitializeComponent();

                    if (RSRResult == 0) { control.IsOn = false; } else { control.IsOn = true; }

                }
                else
                {
                    this.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                this.Visibility = Visibility.Collapsed;
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");

        }
        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {
                Classes.UserControl_Management.UserControl_Management.handleUserControl(border, control, args.Action);

            }
        }


        private void toggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (control.IsLoaded)
            {
                bool controlValue = control.IsOn;
                ToggleRSR(controlValue);
                
            }
          
        }
        
        private void ToggleRSR(bool controlValue)
        {
            ADLX_Managmenet.SetRSR(controlValue);
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
        }
    }
}
