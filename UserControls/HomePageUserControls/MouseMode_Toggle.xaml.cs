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
    public partial class MouseMode_Toggle : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        public MouseMode_Toggle()
        {
            InitializeComponent();
            setControlValue();
          
        }

        private  void setControlValue()
        {

            control.IsOn = Global_Variables.mousemodes.status_MouseMode();

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");
            Global_Variables.valueChanged += Global_Variables_valueChanged;
        }

        private void Global_Variables_valueChanged(object? sender, valueChangedEventArgs e)
        {
            valueChangedEventArgs valueChangedEventArgs = (valueChangedEventArgs)e;
            if (valueChangedEventArgs.Parameter == "MouseModeEnabled" && control.IsLoaded)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    if (Global_Variables.MouseModeEnabled != control.IsOn && border.Tag == "")
                    {
                        control.IsOn = Global_Variables.MouseModeEnabled;

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
                if (control.IsOn != Global_Variables.mousemodes.status_MouseMode())
                {
                    if (control.IsOn)
                    { Global_Variables.mousemodes.start_MouseMode(); }
                    else
                    {
                        Global_Variables.mousemodes.end_MouseMode();
                    }
                }
                
                
            }
          
        }
    
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
        }
    }
}
