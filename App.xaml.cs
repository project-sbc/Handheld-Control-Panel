using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Fan_Management;
using Handheld_Control_Panel.Classes.Global_Variables;


namespace Handheld_Control_Panel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string message = "An unhandled exception just occurred: " + e.Exception.Message + ". Stack Trace: " + e.Exception.StackTrace + ". Source: " + e.Exception.Source + ". Inner Exception: " + e.Exception.InnerException;
            MessageBox.Show(message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Error);
            Log_Writer.writeLog(message);
            Environment.Exit(0);
           
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //LOAD SETTINGS FIRST!!!!
            Global_Variables.settings = Load_Settings.loadSettings(AppDomain.CurrentDomain.BaseDirectory + "\\Settings\\Settings.xml");
            

            bool quietStart = false;
            //if start is from system32 (task scheduled start) then set quietStart to true, means auto start
            if (String.Equals("C:\\Windows\\System32", Directory.GetCurrentDirectory(), StringComparison.OrdinalIgnoreCase))
            {
                quietStart = true;
            }

            var splashScreen = new SplashScreenStartUp();

            if (!Global_Variables.settings.hideSplashScreen && !quietStart)
            {
                //if not quiet start then show splashscreen
                this.MainWindow = splashScreen;
                splashScreen.Show();
            }



            //you can do additional work here, call start routine
           

            await Task.Run(() => Start_Up.Start_Routine());



            this.MainWindow = new MainWindow();

            if (quietStart)
            {
                MainWindow.Visibility = Visibility.Hidden;
            }
            else
            {
                if (!Global_Variables.settings.hideSplashScreen)
                {
                    splashScreen.Close();
                }
                MainWindow.Show();
            }

           
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //close task scheduler
            // Dispose of thread to allow program to close properly
            Handheld_Control_Panel.Classes.Task_Scheduler.Task_Scheduler.closeScheduler();

            //mouse keyboard input hook
            MouseKeyHook.Unsubscribe();

            //kill controller thread
            Global_Variables.killControllerThread = true;

            //restore original power plan applied before launching the app
            Powercfg.closingAppPowerPlanRestore();

            //set auto tdp to false to make sure the autoTDP thread closes properly
            Global_Variables.autoTDP = false;


            if (Global_Variables.Device.FanCapable)
            {
                Global_Variables.softwareAutoFanControlEnabled = false;
                Global_Variables.fanControlEnabled = false;
                Fan_Management.setFanControlHardware();
                WinRingEC_Management.OlsFree();
            }

            
        }
    }
}
