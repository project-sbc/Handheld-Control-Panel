﻿using Handheld_Control_Panel.Classes.Controller_Management;
using ModernWpf.Controls;
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
using System.Windows.Shapes;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MahApps.Metro.IconPacks;

namespace Handheld_Control_Panel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public static class QAMNavigation
    {
        public static bool windowQAMNavigation = true;
    }
    public partial class MainWindow : MetroWindow
    {
        private string window = "QAM";
        private string page = "";
        public MainWindow()
        {
            
            InitializeComponent();


            //start controller management, do this when the window opens to prevent accidental hotkey presses
            Controller_Management.start_Controller_Management();

            //check controller usb device info GUID instance ID
            Controller_Management.getDefaultControllerDeviceInformation();

            MouseKeyHook.Subscribe();

            //subscribe to controller events
            Controller_Management.buttonEvents.controllerInput += handleControllerInputs;

            //set selected item of hamburger nav menu
            navigation.SelectedIndex = 0;

            //set theme
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.SystemTheme + "." + Properties.Settings.Default.systemAccent);


            setWindowSizePosition();
        }

        private void handleControllerInputs(object sender, EventArgs e)
        {
            //get action from custom event args for controller
            Handheld_Control_Panel.Classes.Controller_Management.controllerInputEventArgs args = (Handheld_Control_Panel.Classes.Controller_Management.controllerInputEventArgs)e;
            if (args.Action == "LB")
            {
                navigateListBox(true);
            }
            else
            {
                if (args.Action == "RB")
                {
                    navigateListBox(false);
                }
                else
                {
                    Controller_Window_Page_UserControl_Events.raisePageControllerInputEvent(args.Action, window + page);
                }
            }


        }

        private void navigateListBox(bool left)
        {
            if (left)
            {
                if (navigation.SelectedIndex > 0) { navigation.SelectedIndex = navigation.SelectedIndex -1; }
            }
            else
            {
                if (navigation.SelectedIndex < (navigation.Items.Count - 1)) { navigation.SelectedIndex=navigation.SelectedIndex + 1; }
            }
        }
      
    
        private void setWindowSizePosition()
        {

            //this is used to set the side which the control panel sits on and can be used to fix position after resolution changes
            //icon needs to be rotated for which side it is on
            //PackIconFontAwesome packIconFontAwesome = (PackIconFontAwesome)Close.Content;

            this.Top = 0;

            this.Height =Math.Round( System.Windows.SystemParameters.PrimaryScreenHeight*0.96,0);
            if (Properties.Settings.Default.dockWindowRight && this.Left != System.Windows.SystemParameters.PrimaryScreenWidth - this.Width)
            {
                //if dockWindowRight is true, move to right side of screen
                this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - this.Width;
                //packIconFontAwesome.RotationAngle = 0;
            }
            if (!Properties.Settings.Default.dockWindowRight && this.Left != 0)
            {
                this.Left = 0;
                //packIconFontAwesome.RotationAngle = 180;
            }

        }
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //close task scheduler
            // Dispose of thread to allow program to close properly
            Handheld_Control_Panel.Classes.Task_Scheduler.Task_Scheduler.closeScheduler();


            //mouse keyboard input hook
            MouseKeyHook.Unsubscribe();

            //kill controller thread
            Global_Variables.killControllerThread = true;
        }

        private void frame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //page = frame.Source.ToString().Replace("Pages/","").Replace(".xaml","");
            //if (!page.Contains("Profile")) { Global_Variables.profiles.editingProfile = null; }

        }


        private void MetroWindow_LocationChanged(object sender, EventArgs e)
        {
            setWindowSizePosition();
        }

        private void navigation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (navigation.SelectedItem != null)
            {
                ListBoxItem lbi = navigation.SelectedItem as ListBoxItem;
                //frame.Navigate(new Uri("Pages\\" + lbi.Tag.ToString() + "Page.xaml", UriKind.RelativeOrAbsolute));
            }
        }
    }
    public static class CheckForegroundWindowQAM
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static bool IsActive(IntPtr handle)
        {
            IntPtr activeHandle = GetForegroundWindow();
            return (activeHandle == handle);
        }
    }
}
