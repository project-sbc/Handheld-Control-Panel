using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Task_Scheduler;
using Linearstar.Windows.RawInput;
using ModernWpf;


namespace Handheld_Control_Panel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    
    public partial class App : Application
    {
        public App()
        {
            //start task scheduler
            Task_Scheduler.startScheduler();
        

    
        }
     
    }
}
