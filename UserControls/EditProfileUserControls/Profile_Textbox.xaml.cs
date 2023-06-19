﻿using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Display_Management;
using Handheld_Control_Panel.Classes.Global_Variables;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

using static Vanara.Interop.KnownShellItemPropertyKeys;

namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class Profile_Textbox : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";


        public Profile_Textbox()
        {
            InitializeComponent();
        
          
        }

      
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");

            control.Text = Global_Variables.profiles.editingProfile.ProfileName;
           

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
            
        }



        private void control_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded && !Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Profiles\\" + control.Text))
            {
                Global_Variables.profiles.editingProfile.ProfileName = control.Text;
            }
        }
             
            
        private void control_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (this.IsLoaded && Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Profiles\\" + control.Text + e.Text))
            {
                e.Handled = true;
                Notification_Management.ShowInWindow("Profile name already exists, choose another name.", Notification.Wpf.NotificationType.Warning);
            }
        }
    }
   
}
