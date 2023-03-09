using Handheld_Control_Panel.Classes.Controller_Management;
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

using System.Runtime.InteropServices;
using MahApps.Metro.IconPacks;
using System.Windows.Threading;
using System.Management;
using System.Net.NetworkInformation;
using Handheld_Control_Panel.UserControls;
using ControlzEx.Standard;

namespace Handheld_Control_Panel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : MetroWindow
    {
        private string window = "MainWindow";
        private string page = "";
        private DispatcherTimer updateTimer = new DispatcherTimer(DispatcherPriority.Background);
        private bool disable_B_ToClose = false;
        public MainWindow()
        {
            
            InitializeComponent();

            Global_Variables.mainWindow = this;

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

            updateStatusBar();

            //run timer to update time, wifi and battery status  and other stuff
            startTimer();
        }

        #region timer
        private void startTimer()
        {

            updateTimer.Interval = new TimeSpan(0, 0, 3);
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();

        }

        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            if (this.WindowState != WindowState.Minimized)
            {
                updateStatusBar();
               
            }
            ParallelTaskUpdate_Management.UpdateTask();
        }
        #endregion
        #region update status bar
        private void updateStatusBar()
        {
            checkNetworkInterface();
            checkPowerStatus();
            Time.Text = DateTime.Now.ToString("h:mm tt");
        }
        private void checkNetworkInterface()
        {
            InternetStatus.Text = "\uF384";
            //Gets internet status to display on overlay
            NetworkInterface[] networkCards = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            bool connectedDevice = false;
            foreach (NetworkInterface networkCard in networkCards)
            {
                if (networkCard.OperationalStatus == OperationalStatus.Up)
                {
                    if (networkCard.NetworkInterfaceType == NetworkInterfaceType.Ethernet) { InternetStatus.Text = "\uE839";  }
                    if (networkCard.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) { InternetStatus.Text = "\uE701";  }
                }


            }
           
        }

        private void checkPowerStatus()
        {
           
            int batterylevel = -1;
            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_Battery");
            string powerStatus = "AC";
            foreach (ManagementObject mo in mos.Get())
            {
                powerStatus = mo["EstimatedChargeRemaining"].ToString();
            }
            if (powerStatus != "AC")
            {
                batterylevel = Int16.Parse(powerStatus);
                System.Windows.PowerLineStatus Power = SystemParameters.PowerLineStatus;
                powerStatus = Power.ToString();

            }
            else { powerStatus = "AC"; }


            switch (powerStatus)
            {
                case "Online":
                    if (batterylevel < 10 && batterylevel >= 0) { BatteryStatus.Text = "\uE85A"; }
                    if (batterylevel < 20 && batterylevel >= 10) { BatteryStatus.Text = "\uE85B"; }
                    if (batterylevel < 30 && batterylevel >= 20) { BatteryStatus.Text = "\uE85C"; }
                    if (batterylevel < 40 && batterylevel >= 30) { BatteryStatus.Text = "\uE85D"; }
                    if (batterylevel < 50 && batterylevel >= 40) { BatteryStatus.Text = "\uE85E"; }
                    if (batterylevel < 60 && batterylevel >= 50) { BatteryStatus.Text = "\uE85F"; }
                    if (batterylevel < 70 && batterylevel >= 60) { BatteryStatus.Text = "\uE860"; }
                    if (batterylevel < 80 && batterylevel >= 70) { BatteryStatus.Text = "\uE861"; }
                    if (batterylevel < 90 && batterylevel >= 80) { BatteryStatus.Text = "\uE862"; }
                    if (batterylevel <= 100 && batterylevel >= 90) { BatteryStatus.Text = "\uE83E"; }
                    BatteryPercentage.Text = batterylevel.ToString() + "%";
                    break;
                case "Offline":
                    if (batterylevel < 10 && batterylevel >= 0) { BatteryStatus.Text = "\uE850"; }
                    if (batterylevel < 20 && batterylevel >= 10) { BatteryStatus.Text = "\uE851"; }
                    if (batterylevel < 30 && batterylevel >= 20) { BatteryStatus.Text = "\uE852"; }
                    if (batterylevel < 40 && batterylevel >= 30) { BatteryStatus.Text = "\uE853"; }
                    if (batterylevel < 50 && batterylevel >= 40) { BatteryStatus.Text = "\uE854"; }
                    if (batterylevel < 60 && batterylevel >= 50) { BatteryStatus.Text = "\uE855"; }
                    if (batterylevel < 70 && batterylevel >= 60) { BatteryStatus.Text = "\uE856"; }
                    if (batterylevel < 80 && batterylevel >= 70) { BatteryStatus.Text = "\uE857"; }
                    if (batterylevel < 90 && batterylevel >= 80) { BatteryStatus.Text = "\uE858"; }
                    if (batterylevel < 100 && batterylevel >= 90) { BatteryStatus.Text = "\uE859"; }
                    BatteryPercentage.Text = batterylevel.ToString() + "%";
                    break;
                default:
                    BatteryPercentage.Text = "";
                    BatteryStatus.Text = "";
                    break;
            }
           
        }

        #endregion
        private void handleControllerInputs(object sender, EventArgs e)
        {
            //get action from custom event args for controller
            Handheld_Control_Panel.Classes.Controller_Management.controllerInputEventArgs args = (Handheld_Control_Panel.Classes.Controller_Management.controllerInputEventArgs)e;
            
            switch(args.Action)
            {
                case "LB":
                    navigateListBox(true);
                    break;
                case "RB":
                    navigateListBox(false);
                    break;
                case "B":
                    if (disable_B_ToClose)
                    {
                        Controller_Window_Page_UserControl_Events.raisePageControllerInputEvent(args.Action, window + page);
                    }
                    else
                    {
                        this.WindowState = WindowState.Minimized;
                    }
                 
                    break;
                default:
                    Controller_Window_Page_UserControl_Events.raisePageControllerInputEvent(args.Action, window + page);
                    break;

            }
          
        }

        #region navigation
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
        private void navigation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (navigation.SelectedItem != null)
            {
               
                ListBoxItem lbi = navigation.SelectedItem as ListBoxItem;
                frame.Navigate(new Uri("Pages\\" + lbi.Tag.ToString() + "Page.xaml", UriKind.RelativeOrAbsolute));
                
                HeaderLabel.Content = Application.Current.Resources["MainWindow_NavigationView_" + lbi.Tag].ToString();
                SubheaderLabel.Content = Application.Current.Resources["MainWindow_NavigationView_Sub_" + lbi.Tag].ToString();
                page = lbi.Tag.ToString() + "Page";
            }
        }
        public void navigateFrame(string pageName)
        {
            frame.Navigate(new Uri("Pages\\" + pageName + ".xaml" , UriKind.RelativeOrAbsolute));
            page = pageName;
        }

        private void frame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //page = frame.Source.ToString().Replace("Pages/","").Replace(".xaml","");
            //if (!page.Contains("Profile")) { Global_Variables.profiles.editingProfile = null; }

        }

        #endregion

        #region windows events
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

            //stop timers
           
            updateTimer.Stop(); 
        }
        
        private void MetroWindow_LocationChanged(object sender, EventArgs e)
        {
            setWindowSizePosition();
        }
        #endregion

        public void changeUserInstruction(string newInstructionUserControl)
        {

            this.Dispatcher.BeginInvoke(() => {
                disable_B_ToClose = false;
                instructionStackPanel.Children.Clear(); 
                switch (newInstructionUserControl)
                {
                    case "HomePage_Instruction":
                        instructionStackPanel.Children.Add(new HomePage_Instruction());
                        break;
                    case "CustomizeHomePage_Instruction":
                        instructionStackPanel.Children.Add(new CustomizeHomePage_Instruction());
                        break;
                    case "HotKeyPage_Instruction":
                        instructionStackPanel.Children.Add(new HotKeyPage_Instruction());
                        break;
                    case "HotKeyEditPage_Instruction":
                        disable_B_ToClose = true;
                        instructionStackPanel.Children.Add(new HotKeyEditPage_Instruction());
                        break;
                    case "SelectedListBox_Instruction":
                        disable_B_ToClose= true;
                        instructionStackPanel.Children.Add(new SelectedListBox_Instruction());
                        break;
                    default: break;
                }

            });

           
        }

        private void MetroWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                frame.Source = null;
                //change interval to 15 seconds
                updateTimer.Interval = new TimeSpan(0,0,15);
                //change controller timer interval to 100 ms to hot key recognition when not open
                Controller_Management.timerController.Interval = TimeSpan.FromMilliseconds(100);
            }
            if (this.WindowState == WindowState.Normal)
            {
                navigation.SelectedIndex = 0;
                updateTimer.Interval = new TimeSpan(0, 0, 3);
                //change controller timer interval to 20 ms for active use
                Controller_Management.timerController.Interval = TimeSpan.FromMilliseconds(20);
                setWindowSizePosition();
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            getDPIScaling();
        }
        private void getDPIScaling()
        {
            //used to get absolute resolution (not scaled resolution). IT NEEDS TO RUN ON MAINWINDOW to use the visual tree helper
            Global_Variables.Scaling = (VisualTreeHelper.GetDpi(this).DpiScaleX * 100).ToString();

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
