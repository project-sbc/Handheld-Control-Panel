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
using System.Windows.Threading;
using Windows.Devices.Radios;

namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class TDP2_Slider : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        public TDP2_Slider()
        {
            InitializeComponent();
            //setControlValue();
            UserControl_Management.setupControl(control);
            label.Content = Application.Current.Resources["Usercontrol_TDP"] + " - " + control.Value.ToString() + "W";
            
        }

       
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           //UserControl_Management.setThumbSize(control);

            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            Global_Variables.valueChanged += Global_Variables_valueChanged;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");

        }

        private void Global_Variables_valueChanged(object? sender, EventArgs e)
        {
            valueChangedEventArgs valueChangedEventArgs = (valueChangedEventArgs)e;
            if (valueChangedEventArgs.Parameter == "TDP2")
            {
                this.Dispatcher.BeginInvoke(() => {
                    if (Global_Variables.ReadPL2 != control.Value)
                    {
                        control.Value = Global_Variables.ReadPL2;
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


     
      
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
            Global_Variables.valueChanged -= Global_Variables_valueChanged;
        }

        private void control_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            if (control.IsLoaded && control.Visibility != Visibility.Collapsed)
            {
                label.Content = Application.Current.Resources["Usercontrol_TDP"] + " - " + control.Value.ToString() + "W";
                UserControl_Management.Slider_ValueChanged(sender, e);
            }
        }
    }
}
