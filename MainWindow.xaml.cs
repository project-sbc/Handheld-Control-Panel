using Handheld_Control_Panel.Classes.Controller_Management;
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
using System.Reflection;
using System.Windows.Interop;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;
using Handheld_Control_Panel.Classes.Fan_Management;
using Notification.Wpf;
using Notification.Wpf.Classes;
using System.Threading;
using System.Printing;
using System.Windows.Forms;

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
        public bool disable_B_ToClose = false;
        public OSK osk;

        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        public MainWindow()
        {
          
            InitializeComponent();

            
            Global_Variables.mainWindow = this;
                      

            //start mouse mode, required because controller management makes call to mousemode
            Global_Variables.mousemodes = new MouseMode_Management();

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

          

            updateStatusBar();

            //run timer to update time, wifi and battery status  and other stuff
            startTimer();


            //notifyicon stuff
            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.BaseDirectory + "\\Handheld Control Panel.exe");
            m_notifyIcon.Click += M_notifyIcon_Click;
            m_notifyIcon.MouseDoubleClick += M_notifyIcon_DoubleClick;


            //hide if autostart
            if (String.Equals("C:\\Windows\\System32", Directory.GetCurrentDirectory(), StringComparison.OrdinalIgnoreCase))
            {
               // this.Hide();
            }
           
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr SetForegroundWindow(IntPtr hwnd);

        
        private void M_notifyIcon_Click(object? sender, EventArgs e)
        {
            var contextMenu = new ContextMenu();
            var menuItem = new MenuItem();
            menuItem.Header = "Close";
            var menuItemOpen = new MenuItem();
            menuItemOpen.Header = "Open";
            menuItem.Click += MenuItem_Click;
            menuItemOpen.Click += MenuItemOpen_Click;
            contextMenu.Items.Add(menuItemOpen);
            contextMenu.Items.Add(menuItem);
     
            contextMenu.IsOpen = true;

            // Get context menu handle and bring it to the foreground
            if (PresentationSource.FromVisual(contextMenu) is HwndSource hwndSource)
            {
                _ = SetForegroundWindow(hwndSource.Handle);
            }
        }
        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            toggleWindow();
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void toggleOSK()
        {
            if (osk == null)
            {
                osk = new OSK();
                osk.Show();
            }
            else
            {
                osk.Close();
                osk = null;
            }

        }

        private void M_notifyIcon_DoubleClick(object? sender, EventArgs e)
        {
            toggleWindow();
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
            if (this.Visibility != Visibility.Visible)
            {
                updateStatusBar();
               
            }
            ParallelTaskUpdate_Management.UpdateTaskAlternate();
        }
        #endregion
        #region update status bar
        private async void updateStatusBar()
        {
            await Task.Run(() =>
            {
                checkNetworkInterface();
                checkPowerStatus();
       
            });
            Time.Text = DateTime.Now.ToString("h:mm tt");
        }
        private void checkNetworkInterface()
        {
            this.Dispatcher.BeginInvoke(() => {
                InternetStatus.Text = "\uF384";
                //Gets internet status to display on overlay
                NetworkInterface[] networkCards = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                bool connectedDevice = false;
                foreach (NetworkInterface networkCard in networkCards)
                {
                    if (networkCard.OperationalStatus == OperationalStatus.Up)
                    {

                        if (networkCard.NetworkInterfaceType == NetworkInterfaceType.Ethernet) { InternetStatus.Text = "\uE839"; }
                        if (networkCard.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) { InternetStatus.Text = "\uE701"; }
                    }


                }

            });
          
           
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

            this.Dispatcher.BeginInvoke(() => {
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
            });
          
           
        }

        #endregion
        private void handleControllerInputs(object sender, EventArgs e)
        {
            //get action from custom event args for controller
            Handheld_Control_Panel.Classes.Controller_Management.controllerInputEventArgs args = (Handheld_Control_Panel.Classes.Controller_Management.controllerInputEventArgs)e;
            var mainWindowHandle = new WindowInteropHelper(this).Handle;
            if (this.Visibility == Visibility.Visible)
            {
                switch (args.Action)
                {
                   
                    case "LT":
                        navigateListBox(true);
                        break;
                    case "RT":
                        navigateListBox(false);
                        break;
                    case "B":
                        if (disable_B_ToClose)
                        {
                            Controller_Window_Page_UserControl_Events.raisePageControllerInputEvent(args.Action, window + page);
                        }
                        else
                        {
                            switch (page)
                            {
                                case "CustomizeHomePage":
                                    navigateFrame("HomePage");
                                    break;

                                case "AutoFanPage":
                                    navigateFrame("SettingsPage");
                                    break;
                                case "HotKeyEditPage":
                                    Global_Variables.hotKeys.editingHotkey.LoadProfile(Global_Variables.hotKeys.editingHotkey.ID);
                                    navigateFrame("HotKeyPage");
                                    break;
                                case "MouseModeEditPage":
                                    Global_Variables.mousemodes.editingMouseMode.LoadProfile(Global_Variables.mousemodes.editingMouseMode.ID);
                                    navigateFrame("MouseModePage");
                                    break;
                                case "ProfileEditPage":
                                    Global_Variables.profiles.editingProfile.LoadProfile(Global_Variables.profiles.editingProfile.ID);
                                    navigateFrame("ProfilesPage");
                                    break;
                                default:
                                    toggleWindow();
                                    break;
                            }

                        }

                        break;
                   
                    default:
                        Controller_Window_Page_UserControl_Events.raisePageControllerInputEvent(args.Action, window + page);
                        break;

                }
            }
       
          
        }
     
        public void toggleWindow()
        {
           
            if (this.Visibility == Visibility.Hidden || this.WindowState == WindowState.Minimized) 
            {
                //call check for suspend process   stop this online games are ruined by this
                //FullScreen_Management.checkSuspendProcess();

                this.WindowState = WindowState.Normal;
                if (navigation.SelectedIndex != -1)
                {

                    //ListBoxItem lbi = navigation.SelectedItem as ListBoxItem;
                   // frame.Navigate(new Uri("Pages\\" + lbi.Tag.ToString() + "Page.xaml", UriKind.RelativeOrAbsolute));


                }
       
                this.Show();
              
                m_notifyIcon.Visible = false;
            }
            else
            {
                //check resume process
                //FullScreen_Management.checkResumeProcess();


                this.Hide();
                
                m_notifyIcon.Visible = true;
            }

        }

        #region navigation
        private void navigateListBox(bool left)
        {
            if (left)
            {
                if (navigation.SelectedIndex > 0) { navigation.SelectedIndex = navigation.SelectedIndex -1; } else { navigation.SelectedIndex = navigation.Items.Count-1; }
            }
            else
            {
                if (navigation.SelectedIndex < (navigation.Items.Count - 1)) { navigation.SelectedIndex=navigation.SelectedIndex + 1; } else { navigation.SelectedIndex = 0; }
            }
        }
        private void navigation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (navigation.SelectedItem != null )
            {
               
                ListBoxItem lbi = navigation.SelectedItem as ListBoxItem;
                frame.Navigate(new Uri("Pages\\" + lbi.Tag.ToString() + "Page.xaml", UriKind.RelativeOrAbsolute));
                
                page = lbi.Tag.ToString() + "Page";
            }
        }
        public void navigateFrame(string pageName)
        {

            //logic is if launching from game launcher or 
            if (pageName == "ProfilesPage" && navigation.SelectedItem !=null)
            {
                ListBoxItem lbi = navigation.SelectedItem as ListBoxItem;
                if (lbi.Tag.ToString() == "AppLauncher")
                {
                    frame.Navigate(new Uri("Pages\\AppLauncherPage.xaml", UriKind.RelativeOrAbsolute));
                    page = "AppLauncherPage";
                }
                else
                {
                    frame.Navigate(new Uri("Pages\\" + pageName + ".xaml", UriKind.RelativeOrAbsolute));
                    page = pageName;
                }
            }
            else
            {
                frame.Navigate(new Uri("Pages\\" + pageName + ".xaml", UriKind.RelativeOrAbsolute));
                page = pageName;
            }
  
        }

        public void reinitializeProfiles()
        {
            this.BeginInvoke(new Action(() =>
            {
                Global_Variables.profiles = new Profiles_Management();
            }));
        }

        public void ShowNotificationInWindow(string title, NotificationType notificationType)
        {
            this.BeginInvoke(new Action(() =>
            {
                var notificationManager = new NotificationManager();

                var content = new NotificationContent
                {
                    Title = title,

                    Type = notificationType,

                    TrimType = NotificationTextTrimType.NoTrim, // will show attach button on message
                    RowsCount = 3, //Will show 3 rows and trim after

                    CloseOnClick = true, // Set true if u want close message when left mouse button click on message (base = true)

                    Background = new SolidColorBrush(Colors.DarkGray),
                    Foreground = new SolidColorBrush(Colors.White)

                };


                notificationManager.Show(content, "WindowArea");


            }));
           

           
        }

        private void frame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //page = frame.Source.ToString().Replace("Pages/","").Replace(".xaml","");
            //if (!page.Contains("Profile")) { Global_Variables.profiles.editingProfile = null; }

        }

        #endregion

        #region windows events

        public void setWindowSizePosition(bool forceRun = false)
        {
            if (this.IsLoaded || forceRun)
            {
                getDPIScaling();
                //this relies on getting dpi to scale correctly to the display. This NEEDS to be done after loaded or during loading right after getting DPI. Otherwise
                //scaling might not be correct. Added if check for loaded or forceRun

                //this is used to set the side which the control panel sits on and can be used to fix position after resolution changes
                //icon needs to be rotated for which side it is on
                WindowInteropHelper _windowInteropHelper = new WindowInteropHelper(this);
                Screen screen = Screen.FromHandle(_windowInteropHelper.Handle);

                double scaling;
                if (Double.TryParse(Global_Variables.Scaling, out scaling))
                {
                    scaling = scaling / 100;
                    this.Top = Math.Round(screen.Bounds.Height / scaling * 0.03, 0);

                    this.Height = Math.Round(screen.Bounds.Height / scaling * 0.91, 0);
                    if (Properties.Settings.Default.dockWindowRight)
                    {
                        //if dockWindowRight is true, move to right side of screen
                        this.Left = Math.Round((screen.Bounds.Width / scaling) - this.Width, 0);
                        //packIconFontAwesome.RotationAngle = 0;
                        borderCorner1.CornerRadius = new System.Windows.CornerRadius(11, 0, 0, 11);
                        borderCorner2.CornerRadius = new System.Windows.CornerRadius(11, 0, 0, 11);
                        borderCorner3.CornerRadius = new System.Windows.CornerRadius(0, 0, 0, 11);
                    }
                    if (!Properties.Settings.Default.dockWindowRight)
                    {
                        borderCorner1.CornerRadius = new System.Windows.CornerRadius(0, 11, 11, 0);
                        borderCorner2.CornerRadius = new System.Windows.CornerRadius(0, 11, 11, 0);
                        borderCorner3.CornerRadius = new System.Windows.CornerRadius(0, 0, 11, 0);
                        this.Left = 0;
                        //packIconFontAwesome.RotationAngle = 180;
                    }
                }
                else
                {
                    this.Top = Math.Round(screen.Bounds.Height * 0.03, 0);

                    this.Height = Math.Round(screen.Bounds.Height * 0.91, 0);
                    if (Properties.Settings.Default.dockWindowRight)
                    {
                        //if dockWindowRight is true, move to right side of screen
                        this.Left = screen.Bounds.Width - this.Width;
                        //packIconFontAwesome.RotationAngle = 0;
                        borderCorner1.CornerRadius = new System.Windows.CornerRadius(11, 0, 0, 11);
                        borderCorner2.CornerRadius = new System.Windows.CornerRadius(11, 0, 0, 11);
                        borderCorner3.CornerRadius = new System.Windows.CornerRadius(0, 0, 0, 11);
                    }
                    if (!Properties.Settings.Default.dockWindowRight)
                    {
                        borderCorner1.CornerRadius = new System.Windows.CornerRadius(0, 11, 11, 0);
                        borderCorner2.CornerRadius = new System.Windows.CornerRadius(0, 11, 11, 0);
                        borderCorner3.CornerRadius = new System.Windows.CornerRadius(0, 0, 11, 0);
                        this.Left = 0;
                        //packIconFontAwesome.RotationAngle = 180;
                    }
                }

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

            //disable software fan control if on an enabled device
            Fan_Management.setFanControlHardware();

            //set auto tdp to false to make sure the autoTDP thread closes properly
            Global_Variables.autoTDP = false;

           
            if(Global_Variables.Device.FanCapable)
            {
                Global_Variables.softwareAutoFanControlEnabled = false;
                Fan_Management.setFanControlHardware();
                WinRingEC_Management.OlsFree();
            }
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

                    
                    case "AutoFanPage_Instruction":
                        instructionStackPanel.Children.Add(new AutoFanPage_Instruction());
                        break;
                    case "AppLauncherPage_Instruction":
                        instructionStackPanel.Children.Add(new AppLauncherPage_Instruction());
                        break;
                    case "HomePage_Instruction":
                        instructionStackPanel.Children.Add(new HomePage_Instruction());
                        break;
                    case "CustomizeHomePage_Instruction":
                        instructionStackPanel.Children.Add(new CustomizeHomePage_Instruction());
                        break;
                    case "HotKeyPage_Instruction":
                        instructionStackPanel.Children.Add(new HotKeyPage_Instruction());
                        break;
                    case "ProfilePage_Instruction":
                        instructionStackPanel.Children.Add(new ProfilePage_Instruction());
                        break; 
                    case "ProfileEditPage_Instruction":
          
                        instructionStackPanel.Children.Add(new ProfileEditPage_Instruction());
                        break; 
                    case "HotKeyEditPage_Instruction":
       
                        instructionStackPanel.Children.Add(new HotKeyEditPage_Instruction());
                        break;
                    case "SelectedListBox_Instruction":
                        disable_B_ToClose= true;
                        instructionStackPanel.Children.Add(new SelectedListBox_Instruction());
                        break;
                    case "MouseModePage_Instruction":
                        instructionStackPanel.Children.Add(new MouseModePage_Instruction());
                        break;
                    case "MouseModeEditPage_Instruction":
                        disable_B_ToClose = true;
                        instructionStackPanel.Children.Add(new MouseModeEditPage_Instruction());
                        break;
                        


                    default: break;
                }

            });

           
        }

        private void MetroWindow_StateChanged(object sender, EventArgs e)
        {
            if (false)
            {
                if (this.WindowState == WindowState.Minimized)
                {
                    this.ShowInTaskbar = false;

                    //navigation.SelectedIndex = 0;
                    //frame.Source = null;
                    //change interval to 15 seconds
                    updateTimer.Interval = new TimeSpan(0, 0, 15);
                    //change controller timer interval to 100 ms to hot key recognition when not open
                    Controller_Management.timerController.Interval = TimeSpan.FromMilliseconds(Controller_Management.passiveTimerTickInterval);
                }
                if (this.WindowState == WindowState.Normal)
                {
                    this.ShowInTaskbar = true;
                    //navigation.SelectedIndex = 0;
                    updateTimer.Interval = new TimeSpan(0, 0, 3);
                    //change controller timer interval to 20 ms for active use
                    Controller_Management.timerController.Interval = TimeSpan.FromMilliseconds(Controller_Management.activeTimerTickInterval);
                    setWindowSizePosition();
                    if (navigation.SelectedItem != null)
                    {

                        //ListBoxItem lbi = navigation.SelectedItem as ListBoxItem;
                        //frame.Navigate(new Uri("Pages\\" + lbi.Tag.ToString() + "Page.xaml", UriKind.RelativeOrAbsolute));


                    }
                }
            }
          
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
          
           
            //added a DPI call to the setWindowSizePosition routine, set parameter to true to force run it
            setWindowSizePosition(true);

            //check if multiple instances are running, if yes message and close the program to prevent errors from two instances running
            if (Start_Up.checkMultipleProgramsRunning()) { System.Windows.MessageBox.Show("More than one instance of this program running. Closing this to prevent errors."); this.Close(); }


        }
        private void getDPIScaling()
        {
            //used to get absolute resolution (not scaled resolution). IT NEEDS TO RUN ON MAINWINDOW to use the visual tree helper
            Global_Variables.Scaling = (VisualTreeHelper.GetDpi(this).DpiScaleX * 100).ToString();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            toggleWindow();
        }

        
        private void MetroWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility == Visibility.Hidden)
            {
                this.ShowInTaskbar = false;

                //navigation.SelectedIndex = 0;
                //frame.Source = null;
                //change interval to 15 seconds
                updateTimer.Interval = new TimeSpan(0, 0, 15);
                //change controller timer interval to 100 ms to hot key recognition when not open
                Controller_Management.timerController.Interval = TimeSpan.FromMilliseconds(Controller_Management.passiveTimerTickInterval);
            }
            if (this.Visibility == Visibility.Visible)
            {
                this.ShowInTaskbar = true;
                //navigation.SelectedIndex = 0;
                updateTimer.Interval = new TimeSpan(0, 0, 3);
                //change controller timer interval to 20 ms for active use
                Controller_Management.timerController.Interval = TimeSpan.FromMilliseconds(Controller_Management.activeTimerTickInterval);
                setWindowSizePosition();
               
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
