using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.TaskSchedulerWin32;
using Handheld_Control_Panel.Classes.UserControl_Management;
using Handheld_Control_Panel.Classes.Volume_Management;
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
using Windows.Devices.Radios;

namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class VolumeMute_Toggle : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        public VolumeMute_Toggle()
        {
            InitializeComponent();
            setControlValue();
          
        }

        private void setControlValue()
        {
            
            control.IsOn = Global_Variables.Mute;

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");
            Global_Variables.valueChanged += Global_Variables_volumeMuteChanged;
        }

        private void Global_Variables_volumeMuteChanged(object? sender, EventArgs e)
        {
            valueChangedEventArgs valueChangedEventArgs = (valueChangedEventArgs)e;
            if (valueChangedEventArgs.Parameter == "VolumeMute")
            {
                this.Dispatcher.BeginInvoke(() => {
                    if (Global_Variables.Mute != control.IsOn)
                    {
                        control.IsOn = Global_Variables.Mute;
                    }

                });
            }
           
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
                AudioManager.SetMasterVolumeMute(control.IsOn);
               
            }
          
        }
      
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
            Global_Variables.valueChanged -= Global_Variables_volumeMuteChanged;
        }
    }
}
