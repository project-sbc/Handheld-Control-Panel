using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
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
    public partial class FPSLimit_Slider : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        public FPSLimit_Slider()
        {
            if (RTSS.directoryRTSSExists())
            {
                InitializeComponent();
                UserControl_Management.setupControl(control);
                if (RTSS.RTSSRunning())
                {
                    handleMaxFPSLimit();
                    startRTSS.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //hide slider during load so that the thumb size can still be adjusted (wont work when collapsed)
                    //control.Visibility= Visibility.Collapsed;
                    toggleSwitch.Visibility = Visibility.Collapsed;
                    unitLabel.Visibility = Visibility.Collapsed;
                    startRTSS.Visibility = Visibility.Visible;
                }
            }
            else
            {
                this.Visibility= Visibility.Collapsed;
            }

          
        }

        private void handleMaxFPSLimit()
        {
            toggleSwitch.Visibility= Visibility.Visible;
            if (Global_Variables.FPSLimit == 0)
            {
                toggleSwitch.IsOn = true;
                unitLabel.Visibility = Visibility.Hidden;
                control.Visibility = Visibility.Collapsed;
            }
            else
            {
                toggleSwitch.IsOn = false;
                unitLabel.Visibility = Visibility.Visible;
                control.Visibility = Visibility.Visible;
            }
          

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.", "");
            if (control is Slider) { UserControl_Management.setThumbSize((Slider)control); }
            if (RTSS.RTSSRunning() == false) { control.Visibility=Visibility.Collapsed; }
            if (Window.GetWindow(this).ActualWidth < 650) { subText.Visibility = Visibility.Collapsed; }


        }
        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {
                if (args.Action == "A")
                {
                    if (toggleSwitch.Visibility== Visibility.Visible)
                    {
                        toggleSwitch.IsOn = !toggleSwitch.IsOn;
                    }
                    else
                    {
                        startRTSS.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }
                   
                    
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

        private void toggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            bool toggle = toggleSwitch.IsOn;
           
            if (toggleSwitch.IsOn)
            {
                control.Visibility = Visibility.Collapsed;
                unitLabel.Visibility = Visibility.Hidden;
                Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.RTSS.setRTSSFPSLimit(0));
            } 
            else 
            {
                unitLabel.Visibility = Visibility.Visible;
                control.Visibility = Visibility.Visible;
                control.Value = control.Maximum;
                double value = control.Value;
                Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.RTSS.setRTSSFPSLimit((int)value));
            }
        }

        private void startRTSS_Click(object sender, RoutedEventArgs e)
        {
            RTSS.startRTSS();
            handleMaxFPSLimit();
            startRTSS.Visibility = Visibility.Collapsed;
        }
    }
}
