using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Display_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.TaskSchedulerWin32;
using Handheld_Control_Panel.Classes.UserControl_Management;
using Handheld_Control_Panel.Styles;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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


namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class ProfileRefreshRate_Dropdown : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private object selectedObject = "";
        private List<string> RefreshRates = new List<string>();
        public ProfileRefreshRate_Dropdown()
        {
            InitializeComponent();
            setControlValue();
          
        }

        private  void setControlValue()
        {
            if (Global_Variables.profiles.editingProfile.profile_Exe.Resolution != "")
            {
                foreach (string refresh in Global_Variables.resolution_refreshrates[Global_Variables.profiles.editingProfile.profile_Exe.Resolution])
                {
                    RefreshRates.Add(refresh);
                }
                RefreshRates.Insert(0, "");
              
                controlList.ItemsSource = RefreshRates;

                controlList.SelectedItem = Global_Variables.profiles.editingProfile.profile_Exe.Resolution;
                selectedObject = Global_Variables.profiles.editingProfile.profile_Exe.Resolution;
            }

            controlList.Visibility = Visibility.Collapsed;
         

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");
            Global_Variables.valueChanged += Global_Variables_valueChanged;
        }

        private void Global_Variables_valueChanged(object? sender, valueChangedEventArgs e)
        {
            valueChangedEventArgs valueChangedEventArgs = e as valueChangedEventArgs;
            if (valueChangedEventArgs.Parameter == "ProfileResolution")
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (Global_Variables.profiles.editingProfile.profile_Exe.Resolution != "")
                    {
                        RefreshRates.Clear();
                        RefreshRates = Global_Variables.resolution_refreshrates[Global_Variables.profiles.editingProfile.profile_Exe.Resolution];
                        RefreshRates.Insert(0, "");
                        controlList.ItemsSource = RefreshRates;
                        if (controlList.Items.Contains(Global_Variables.profiles.editingProfile.profile_Exe.RefreshRate) && controlList.SelectedItem != Global_Variables.profiles.editingProfile.profile_Exe.RefreshRate && controlList.IsLoaded)
                        {
                            controlList.SelectedItem = Global_Variables.profiles.editingProfile.profile_Exe.RefreshRate;

                        }
                    }
                    else
                    {
                        controlList.ItemsSource = null;
                    }


                }));
               
             
            }
           
        }

        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {
                switch(args.Action)
                {
                    case "A":
                        if (controlList.Visibility == Visibility.Visible)
                        {
                            handleListboxChange();
                     
                            Global_Variables.mainWindow.changeUserInstruction("ProfileEditPage_Instruction");
                        }
                        else
                        {
                            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            Global_Variables.mainWindow.changeUserInstruction("SelectedListBox_Instruction");

                        }

                        break;
                    case "B":
                 
                        if (controlList.Visibility == Visibility.Visible)
                        {
                            Global_Variables.mainWindow.changeUserInstruction("ProfileEditPage_Instruction");

                            if (selectedObject != null)
                            {
                                controlList.SelectedItem = selectedObject;
                            }
                            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

                        }
  
                        break;
                    default:
                        Classes.UserControl_Management.UserControl_Management.handleUserControl(border, controlList, args.Action);

                        break;
                }

            }
        }
        private void handleListboxChange()
        {
            if (controlList.IsLoaded)
            {
                if (controlList.SelectedItem != null)
                {
                    string refreshrate = controlList.SelectedItem.ToString();
                    Global_Variables.profiles.editingProfile.profile_Exe.RefreshRate = refreshrate;
                    selectedObject = controlList.SelectedItem;
                 
                    if (controlList.Visibility == Visibility.Visible)
                    {
                        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }
                }

            }

        }


        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
            Global_Variables.valueChanged -= Global_Variables_valueChanged;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (controlList.Visibility == Visibility.Visible)
            {
                controlList.Visibility = Visibility.Collapsed;
                PackIconUnicons icon = (PackIconUnicons)button.Content;
                icon.RotationAngle = 0;
                //
                MainWindow wnd2 = (MainWindow)Application.Current.MainWindow;
                wnd2.changeUserInstruction("ProfileEditPage_Instruction");
                wnd2 = null;
            }
            else
            {
                controlList.Visibility = Visibility.Visible;
                PackIconUnicons icon = (PackIconUnicons)button.Content;
                icon.RotationAngle = 90;
                MainWindow wnd2 = (MainWindow)Application.Current.MainWindow;
                wnd2.changeUserInstruction("SelectedListBox_Instruction");
                wnd2 = null;
            }
        }


        private void controlList_TouchUp(object sender, TouchEventArgs e)
        {
            handleListboxChange();
        }

        private void controlList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            handleListboxChange();
        }
    }
}
