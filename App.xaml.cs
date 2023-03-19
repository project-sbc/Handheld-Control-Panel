using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Task_Scheduler;
using Linearstar.Windows.RawInput;
using ModernWpf;
using RTSSSharedMemoryNET;

namespace Handheld_Control_Panel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Error);
            Log_Writer.writeLog(e.Exception.Message);
            e.Handled = true;
        }
  
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //set global error handler
          
         

            bool quietStart = false;
            //if start is from system32 (task scheduled start) then set quietStart to true, means auto start
            if (String.Equals("C:\\Windows\\System32", Directory.GetCurrentDirectory(), StringComparison.OrdinalIgnoreCase))
            {
                quietStart = true;
            }
            var splashScreen = new SplashScreenStartUp();

            if (!quietStart)
            {
                //if not quiet start then show splashscreen
                this.MainWindow = splashScreen;
                splashScreen.Show();
            }

            //you can do additional work here, call start routine
            Classes.Start_Up.Start_Routine();
            MainWindow mainWindow = new MainWindow();
            Task.Factory.StartNew(() =>
            {

                //since we're not on the UI thread
                //once we're done we need to use the Dispatcher
                //to create and show the main window
                this.Dispatcher.Invoke(() =>
                {
                    //initialize the main window, set it as the application main window
                    //and close the splash screen

                    this.MainWindow = mainWindow;
                    mainWindow.Show();
                    if (!quietStart)
                    {
                        //close splashscreen if open
                        splashScreen.Close();
                    }

                });
            });




        }
    }
}
