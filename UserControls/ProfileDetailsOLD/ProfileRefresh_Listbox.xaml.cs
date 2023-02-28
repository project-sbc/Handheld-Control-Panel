using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Display_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.UserControl_Management;
using Handheld_Control_Panel.Styles;
using MahApps.Metro.Controls;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class ProfileRefresh_Listbox : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private object selectedObject;
        

        public ProfileRefresh_Listbox()
        {
            InitializeComponent();
            //UserControl_Management.setupControl(control);

            if (Global_Variables.profiles.editingProfile.RefreshRate == "") { toggleSwitch.IsOn = false; control.Visibility = Visibility.Collapsed; } else { toggleSwitch.IsOn = true; }

            setListboxItemsource();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            Display_Management.resolutionProfileChangedEvent += handleResolutionChanged;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");
           
            if (Window.GetWindow(this).ActualWidth < 650) { subText.Visibility = Visibility.Collapsed; }
        }
        private void handleResolutionChanged(object sender, EventArgs e)
        {
            setListboxItemsource();
        }
        private void setListboxItemsource()
        {
            
            if (Global_Variables.profiles.editingProfile.Resolution != "" && control.Visibility == Visibility.Visible)
            {
                control.ItemsSource = Global_Variables.resolution_refreshrates[Global_Variables.profiles.editingProfile.Resolution];

                if (Global_Variables.profiles.editingProfile.RefreshRate != "")
                {
                    control.SelectedItem = Global_Variables.profiles.editingProfile.RefreshRate;
                    toggleSwitch.IsOn = true;
                }
            }
            else
            {
                toggleSwitch.IsOn = false;
                control.Visibility = Visibility.Collapsed;
                unitLabel.Visibility = Visibility.Hidden;
            }
        }


        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {
                switch (args.Action)
                {
                    case "X":
                        toggleSwitch.IsOn = !toggleSwitch.IsOn;
                        break;
                    case "A":
                        handleListboxChange();
                        break;

                    default:
                        Classes.UserControl_Management.UserControl_Management.handleUserControl(border, control, args.Action);
                        if (args.Action == "Highlight" && control.SelectedItem != selectedObject && selectedObject != null) { control.SelectedItem = selectedObject; }

                        break;


                }
               
            }
        }

        private void handleListboxChange()
        {
            if (control.IsLoaded && control.Visibility==Visibility.Visible)
            {
                if (control.SelectedItem != null)
                {
                    string refresh = control.SelectedItem.ToString();
                    Global_Variables.profiles.editingProfile.RefreshRate = refresh;
                    selectedObject = control.SelectedItem;
                }

            }

        }

        private void control_TouchUp(object sender, TouchEventArgs e)
        {
            handleListboxChange();
        }

        private void control_MouseUp(object sender, MouseButtonEventArgs e)
        {
            handleListboxChange();
        }
        private void toggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (control.IsLoaded)
            {
                bool toggle = toggleSwitch.IsOn;

                if (!toggleSwitch.IsOn)
                {
                    control.Visibility = Visibility.Collapsed;
                    unitLabel.Visibility = Visibility.Hidden;
                    Global_Variables.profiles.editingProfile.RefreshRate = "";
                }
                else
                {
                    unitLabel.Visibility = Visibility.Visible;
                    control.Visibility = Visibility.Visible;
                    setListboxItemsource();
                    //Global_Variables.profiles.editingProfile.Resolution = control.SelectedItem.ToString();
                }
            }

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
        }
    }

  
}
