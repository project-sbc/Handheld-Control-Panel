﻿using System;
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
using Handheld_Control_Panel.Classes.Fan_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.Task_Scheduler;
using Linearstar.Windows.RawInput;
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
            string message = "An unhandled exception just occurred: " + e.Exception.Message + ". Stack Trace: " + e.Exception.StackTrace + ". Source: " + e.Exception.Source + ". Inner Exception: " + e.Exception.InnerException;
            MessageBox.Show(message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Error);
            Log_Writer.writeLog(message);
            e.Handled = true;
           
        }
  
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

           


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
                    
                    if (!quietStart)
                    {
                        //close splashscreen if open
                        splashScreen.Close();
                        mainWindow.Show();
                    }
                    else
                    {
                        mainWindow.Hide();
                    }

                });
            });




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
                Fan_Management.setFanControlHardware();
                WinRingEC_Management.OlsFree();
            }

            
        }
    }
}
