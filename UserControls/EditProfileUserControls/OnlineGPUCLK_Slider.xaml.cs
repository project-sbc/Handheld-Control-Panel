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
    public partial class OnlineGPUCLK_Slider : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private bool dragStarted= false;
        public OnlineGPUCLK_Slider()
        {
            InitializeComponent();
            //setControlValue();
            UserControl_Management.setupControl(control);


            if (Global_Variables.profiles.editingProfile.Online_GPUCLK != "")
            {
                controlToggle.IsOn = true;
                control.Value = Convert.ToDouble(Global_Variables.profiles.editingProfile.Online_GPUCLK);
            }
            else
            {
                controlToggle.IsOn = false;
                secondLabel.Visibility = Visibility.Collapsed;
                control.Visibility = Visibility.Collapsed;
            }
        }

       
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           //UserControl_Management.setThumbSize(control);

            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
        
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");

           

          
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
            if (controlToggle.IsLoaded && controlToggle.Visibility == Visibility.Visible)
            {


                if (controlToggle.IsOn)
                {
                  
                    control.Visibility = Visibility.Visible;
                    secondLabel.Visibility = Visibility.Visible;
                    Global_Variables.profiles.editingProfile.Online_GPUCLK = control.Value.ToString();
                }
                else
                {
                    control.Visibility = Visibility.Collapsed;
                    secondLabel.Visibility = Visibility.Collapsed;
                    Global_Variables.profiles.editingProfile.Online_GPUCLK = "";
                }
            }
        }
    }
}
