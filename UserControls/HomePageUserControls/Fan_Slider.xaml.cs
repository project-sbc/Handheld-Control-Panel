using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Fan_Management;
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


namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class Fan_Slider : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private bool dragStarted= false;
        private DispatcherTimer changeValue = new DispatcherTimer();
        private string fanMode;
        public Fan_Slider()
        {
            if (Global_Variables.Device.FanCapable)
            {
                InitializeComponent();
                //setControlValue();
                UserControl_Management.setupControl(control);

                determineFanMode();


                //set initial fanspeed value
                if (Global_Variables.FanSpeed == 0)
                {
                    control.Value = control.Minimum;
                    labelControl.Content = "0";
                }
                else
                {
                    if (Global_Variables.FanSpeed < Global_Variables.Device.MinFanSpeedPercentage)
                    {
                        control.Value = control.Minimum + 1;
                        labelControl.Content = control.Minimum + 1;
                    }
                    else
                    {
                        control.Value = Global_Variables.FanSpeed;
                        labelControl.Content = Global_Variables.FanSpeed.ToString();
                    }
                }

                //set up timer
                changeValue.Interval = new TimeSpan(0, 0, 1);
                changeValue.Tick += ChangeValue_Tick;
            }
            else { this.Visibility = Visibility.Collapsed; }

        }
        private void handleChangingFanModes()
        {
            if (fanMode == "Software")
            {
                Global_Variables.softwareAutoFanControlEnabled = false;
                fanMode = "Hardware";
                Fan_Management.setFanControlHardware();
            }
            else
            {
                if (fanMode == "Hardware")
                {
                    Global_Variables.softwareAutoFanControlEnabled = false;
                    fanMode = "Manual";
                    Fan_Management.setFanControlManual();
                }
                else
                {
                    fanMode = "Software";
                    Fan_Management.setFanControlManual();
                    AutoFan_Management.startAutoFan();
                }
            }
            determineFanMode();
        }
        private void determineFanMode()
        {
            //check if manual control enabled
            Fan_Management.readSoftwareFanControl();
            if (Global_Variables.fanControlEnabled)
            {
                if (Global_Variables.softwareAutoFanControlEnabled)
                {
                    fanMode = "Software";
                    control.Visibility = Visibility.Collapsed;
                    labelControl.Visibility = Visibility.Visible;
                }
                else
                {
                    fanMode = "Manual";
                    control.Visibility = Visibility.Visible;
                    labelControl.Visibility = Visibility.Visible;
                }
            }
            else
            {
                fanMode = "Hardware";
                control.Visibility = Visibility.Collapsed;
                labelControl.Visibility = Visibility.Collapsed;
            }
            controlButton.Content = Application.Current.Resources["FanMode_" + fanMode];
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

        }

        private void Global_Variables_valueChanged(object? sender, valueChangedEventArgs e)
        {
            valueChangedEventArgs valueChangedEventArgs = (valueChangedEventArgs)e;
            if (valueChangedEventArgs.Parameter == "FanSpeed" && !dragStarted)
            {
                this.Dispatcher.BeginInvoke(() => {
                    if (control.IsLoaded && labelControl.Visibility == Visibility.Visible)
                    {

                        if (Global_Variables.FanSpeed == 0)
                        {
                            labelControl.Content = "0";
                        }
                        else
                        {
                            if (Global_Variables.FanSpeed < Global_Variables.Device.MinFanSpeedPercentage)
                            {
                                labelControl.Content = control.Minimum + 1;
                            }
                            else
                            {
                                labelControl.Content = Global_Variables.FanSpeed.ToString();
                            }
                        }
                    }

                    if (border.Tag == "" && control.IsLoaded && control.Visibility == Visibility.Visible)
                    {
                       
                        if (Global_Variables.FanSpeed == 0)
                        {
                            control.Value = control.Minimum;
                            labelControl.Content = "0";
                        }
                        else
                        {
                            if (Global_Variables.FanSpeed < Global_Variables.Device.MinFanSpeedPercentage)
                            {
                                control.Value = control.Minimum + 1;
                                labelControl.Content = control.Minimum + 1;
                            }
                            else
                            {
                                control.Value = Global_Variables.FanSpeed;
                                labelControl.Content = Global_Variables.FanSpeed.ToString();
                            }
                        }


                        
                    }

                });
            }

        }

      

        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {
                if (args.Action == "A" && this.Visibility == Visibility.Visible)
                {
                    handleChangingFanModes();
                }
                else
                {
                    Classes.UserControl_Management.UserControl_Management.handleUserControl(border, control, args.Action);

                }
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
            if (Global_Variables.fanControlEnabled && !Global_Variables.softwareAutoFanControlEnabled){
                UserControl_Management.Slider_ValueChanged((Slider)control, null);
              
            }
     
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
            if (control.IsLoaded && fanMode== "Manual")
            {
                if (control.Value < Global_Variables.Device.MinFanSpeedPercentage)
                {
                    labelControl.Content = "0";
                }
                else
                {
                    labelControl.Content = control.Value.ToString();
                }
                if (!dragStarted)
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
        
        }
       

        private void controlButton_Click(object sender, RoutedEventArgs e)
        {
            handleChangingFanModes();
        }
    }
}
