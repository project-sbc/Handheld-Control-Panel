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
    public partial class GPUCLK_Slider : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private bool dragStarted= false;
        private DispatcherTimer changeValue = new DispatcherTimer();
        public GPUCLK_Slider()
        {
            InitializeComponent();
            //setControlValue();
            UserControl_Management.setupControl(control);
         

            //set up timer
            changeValue.Interval = new TimeSpan(0,0,2);
            changeValue.Tick += ChangeValue_Tick;
        }

        private void ChangeValue_Tick(object? sender, EventArgs e)
        {
            sliderValueChanged();
            changeValue.Stop();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           //UserControl_Management.setThumbSize(control);

            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            Global_Variables.valueChanged += Global_Variables_valueChanged;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");

            if (Global_Variables.GPUCLK == 0)
            {
                controlToggle.IsOn = false;
                secondLabel.Visibility = Visibility.Collapsed;
                control.Visibility = Visibility.Collapsed;
            }
            else
            {
                controlToggle.IsOn = true;
                control.Value = Global_Variables.GPUCLK;
            }
        }

        private void Global_Variables_valueChanged(object? sender, valueChangedEventArgs e)
        {
            valueChangedEventArgs valueChangedEventArgs = (valueChangedEventArgs)e;
            if (valueChangedEventArgs.Parameter == "GPUCLK" && !dragStarted && Global_Variables.GPUCLK != 0)
            {
                this.Dispatcher.BeginInvoke(() => {
                 
                    if (Global_Variables.GPUCLK != control.Value && border.Tag == "" && control.IsLoaded)
                    {
                        control.Value = Global_Variables.GPUCLK;
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
            changeValue.Stop();
            changeValue.Tick -= ChangeValue_Tick;
        }
        private void sliderValueChanged()
        {
            UserControl_Management.Slider_ValueChanged((Slider)control, null);
        }
       

        private void control_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStarted = true;
        }

        private void control_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStarted = false;
            sliderValueChanged();
        }

        private void control_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStarted && control.IsLoaded)
            {
                if (border.Tag == "")
                {
                    sliderValueChanged();
                }
                else
                {
                    changeValue.Stop(); 
                    changeValue.Start();
                }
               
            }
        }
        private void controlToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (controlToggle.IsLoaded)
            {
                if (controlToggle.IsOn)
                {
                    double value = control.Value;
                    control.Visibility = Visibility.Visible;
                    secondLabel.Visibility = Visibility.Visible;
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.GPUCLK_Management.GPUCLK_Management.changeAMDGPUClock((int)value));
                }
                else
                {
                    control.Visibility = Visibility.Collapsed;
                    secondLabel.Visibility = Visibility.Collapsed;
                
                }
            }
        }

    }
}
