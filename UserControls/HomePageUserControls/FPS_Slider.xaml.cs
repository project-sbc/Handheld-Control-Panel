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
    public partial class FPS_Slider : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private bool dragStarted= false;
        public FPS_Slider()
        {
            InitializeComponent();
            //setControlValue();
            UserControl_Management.setupControl(control);
         
        }

       
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           //UserControl_Management.setThumbSize(control);

            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            Global_Variables.valueChanged += Global_Variables_valueChanged;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");
            if (RTSS.directoryRTSSExists())
            {
                if (Global_Variables.FPSLimit == 0)
                {
                    controlToggle.IsOn = false;
                    secondLabel.Visibility = Visibility.Collapsed;
                    control.Visibility = Visibility.Collapsed;
                }
                else
                {
                    controlToggle.IsOn = true;
                    control.Value = Global_Variables.FPSLimit;
                }
            }
            else
            {
                controlToggle.IsOn = false;
                secondLabel.Visibility = Visibility.Collapsed;
                control.Visibility = Visibility.Collapsed;
            }
        }

        private void Global_Variables_valueChanged(object? sender, valueChangedEventArgs e)
        {
            valueChangedEventArgs valueChangedEventArgs = (valueChangedEventArgs)e;
            if (valueChangedEventArgs.Parameter == "FPSLimit" && !dragStarted )
            {
                this.Dispatcher.BeginInvoke(() => {
                    if (Global_Variables.FPSLimit != control.Value && border.Tag == "" && control.IsLoaded)
                    {
                        control.Value = Global_Variables.FPSLimit;
                    }

                });
            }

        }

      

        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {
                if (args.Action == "A")
                {
                    controlToggle.IsOn = !controlToggle.IsOn;
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
                sliderValueChanged();
            }
        }

        private void controlToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (controlToggle.IsLoaded)
            {
                if (RTSS.RTSSRunning())
                {
                    if (controlToggle.IsOn)
                    {
                        double value = control.Value;
                        control.Visibility = Visibility.Visible;
                        secondLabel.Visibility = Visibility.Visible;
                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.RTSS.setRTSSFPSLimit((int)value));
                    }
                    else
                    {
                        control.Visibility = Visibility.Collapsed;
                        secondLabel.Visibility = Visibility.Collapsed;
                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.RTSS.setRTSSFPSLimit(0));
                    }
                }
                else
                { 
                    if (controlToggle.IsOn)
                    {
                        if (RTSS.directoryRTSSExists())
                        {
                            RTSS.startRTSS();
                        }
                        else
                        {
                            controlToggle.IsOn = false;
                            MessageBox.Show("RTSS is not installed. Install RTSS before enabling this feature.");
                        }
                    }
                 
                }
            }
        }
    }
}
