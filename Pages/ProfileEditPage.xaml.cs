﻿using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.UserControls;
using Microsoft.Win32.TaskScheduler;
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
using MahApps.Metro;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlzEx.Theming;
using System.Windows.Controls.Primitives;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.UserControls;
using System.Windows.Threading;

namespace Handheld_Control_Panel.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class ProfileEditPage : Page
    {
        private string windowpage;
        private List<UserControl> userControls = new List<UserControl>();
        private DispatcherTimer saveDT = new DispatcherTimer();
        private int highlightedUserControl = -1;
        private int selectedUserControl = -1;
        public ProfileEditPage()
        {
            InitializeComponent();
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.SystemTheme + "." + Properties.Settings.Default.systemAccent);
           

            MainWindow wnd = (MainWindow)Application.Current.MainWindow;
            wnd.changeUserInstruction("ProfileEditPage_Instruction");
            wnd = null;
        }
       
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //get unique window page combo from getwindow to string
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            //subscribe to controller input events
            Controller_Window_Page_UserControl_Events.pageControllerInput += handleControllerInputs;
            getUserControlsOnPage();
         

        }

        private void getUserControlsOnPage()
        {
            foreach (object child in stackPanel.Children)
            {
                if (child is UserControl)
                {
                    userControls.Add((UserControl)child);
                }

            }
        }
        //
        private void handleControllerInputs(object sender, EventArgs e)
        {
            //get action from custom event args for controller
            controllerPageInputEventArgs args = (controllerPageInputEventArgs)e;
            string action = args.Action;

            if (args.WindowPage == windowpage)
            {
                switch(args.Action)
                {
                    case "B":
                        Global_Variables.profiles.editingProfile.LoadProfile(Global_Variables.profiles.editingProfile.ID);
                        MainWindow wnd = (MainWindow)Application.Current.MainWindow;
                        wnd.navigateFrame("ProfilesPage");
                        wnd = null;
                        break;
                    case "Start":
                        Global_Variables.profiles.editingProfile.SaveToXML();
                        
                        runSaveMessage();
                        break;


                    default:
                        //global method handles the event tracking and returns what the index of the highlighted and selected usercontrolshould be
                        int[] intReturn = WindowPageUserControl_Management.globalHandlePageControllerInput(windowpage, action, userControls, highlightedUserControl, selectedUserControl, stackPanel);

                        highlightedUserControl = intReturn[0];
                        selectedUserControl = intReturn[1];

                        break;


                }
              
   
            }

        }
        private void runSaveMessage()
        {
           
            saveDT.Interval = new TimeSpan(0, 0, 3);
            saveDT.Tick += SaveDT_Tick;
            saveDT.Start();
            SaveLabel.Visibility = Visibility.Visible;

        }

        private void SaveDT_Tick(object? sender, EventArgs e)
        {
           
            SaveLabel.Visibility = Visibility.Collapsed;
            saveDT.Stop();
        }

      

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.pageControllerInput -= handleControllerInputs;
        }
    }
}
