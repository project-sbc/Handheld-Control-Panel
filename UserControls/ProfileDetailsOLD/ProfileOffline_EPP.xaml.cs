﻿using Handheld_Control_Panel.Classes;
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
    public partial class ProfileOffline_EPP : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        public ProfileOffline_EPP()
        {
            InitializeComponent();
            UserControl_Management.setupControl(control);
            handleToggleSwitch();
          
        }

        private void handleToggleSwitch()
        {
            if (Global_Variables.profiles.editingProfile.Offline_EPP == "")
            {
                toggleSwitch.IsOn = false;
                unitLabel.Visibility = Visibility.Hidden;
                control.Visibility = Visibility.Collapsed;
            }
            else
            {
                toggleSwitch.IsOn = true;
                unitLabel.Visibility = Visibility.Visible;
                control.Visibility = Visibility.Visible;
                control.Value = Int32.Parse(Global_Variables.profiles.editingProfile.Offline_EPP);
            }
          

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");
            if (control is Slider) { UserControl_Management.setThumbSize((Slider)control); }
            

        }
        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {
                if (args.Action == "A")
                {
                    toggleSwitch.IsOn= !toggleSwitch.IsOn;
                }
                else
                {
                    Classes.UserControl_Management.UserControl_Management.handleUserControl(border, control, args.Action);
                }
                
            }
        }

        private void control_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(control.IsLoaded && control.Visibility == Visibility.Visible)
            {
                UserControl_Management.Slider_ValueChanged(sender, e);
            }
       
        }

        private void toggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (control.IsLoaded)
            {
                bool toggle = toggleSwitch.IsOn;

                if (!toggleSwitch.IsOn)
                {
                    control.Visibility = Visibility.Collapsed;
                    unitLabel.Visibility = Visibility.Hidden;
                    Global_Variables.profiles.editingProfile.Offline_EPP = "";
                }
                else
                {
                    unitLabel.Visibility = Visibility.Visible;
                    control.Visibility = Visibility.Visible;
                    Global_Variables.profiles.editingProfile.Offline_EPP = control.Value.ToString();
                }
            }
          
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
        }
    }
}