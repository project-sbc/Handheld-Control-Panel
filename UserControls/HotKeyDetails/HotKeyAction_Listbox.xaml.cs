using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Display_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.UserControl_Management;
using Handheld_Control_Panel.Styles;
using MahApps.Metro.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

using System.Resources;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Vanara.Interop.KnownShellItemPropertyKeys;

namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class HotKeyAction_Listbox : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private object selectedObject;
        

        public HotKeyAction_Listbox()
        {
            InitializeComponent();
            //UserControl_Management.setupControl(control);
            setListboxItemsource();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");
           
            if (Window.GetWindow(this).ActualWidth < 650) { subText.Visibility = Visibility.Collapsed; }
        }

        private void setListboxItemsource()
        {
            List<HotKeyAction> hotkeyAction = new List<HotKeyAction>();

        
            HotKeyAction showhideHCP = new HotKeyAction();
            showhideHCP.DisplayHotkeyAction = Application.Current.Resources["Hotkeys_Action_Show_Hide_HCP"].ToString();
            showhideHCP.HotkeyAction = "Show_Hide_HCP";

            hotkeyAction.Add(showhideHCP);

            HotKeyAction openProgram = new HotKeyAction();
            openProgram.DisplayHotkeyAction = Application.Current.Resources["Hotkeys_Action_Open_Program"].ToString();
            openProgram.HotkeyAction = "Open_Program";

            hotkeyAction.Add(openProgram);

            HotKeyAction changeTDP = new HotKeyAction();
            changeTDP.DisplayHotkeyAction = Application.Current.Resources["Hotkeys_Action_Change_TDP"].ToString();
            changeTDP.HotkeyAction = "Change_TDP";

            hotkeyAction.Add(changeTDP);

            HotKeyAction openSteamBigPicture = new HotKeyAction();
            openSteamBigPicture.DisplayHotkeyAction = Application.Current.Resources["Hotkeys_Action_Open_Steam_BigPicture"].ToString();
            openSteamBigPicture.HotkeyAction = "Open_Steam_BigPicture";

            hotkeyAction.Add(openSteamBigPicture);

            HotKeyAction changeGPUCLK = new HotKeyAction();
            changeGPUCLK.DisplayHotkeyAction = Application.Current.Resources["Hotkeys_Action_Change_GPUCLK"].ToString();
            changeGPUCLK.HotkeyAction = "Change_GPUCLK";

            hotkeyAction.Add(changeGPUCLK);

            HotKeyAction openPlaynite = new HotKeyAction();
            openPlaynite.DisplayHotkeyAction = Application.Current.Resources["Hotkeys_Action_Open_Playnite"].ToString();
            openPlaynite.HotkeyAction = "Open_Playnite";

            hotkeyAction.Add(openPlaynite);

            HotKeyAction changeBrightness = new HotKeyAction();
            changeBrightness.DisplayHotkeyAction = Application.Current.Resources["Hotkeys_Action_Change_Brightness"].ToString();
            changeBrightness.HotkeyAction = "Change_Brightness";

            hotkeyAction.Add(changeBrightness);

            HotKeyAction toggleMouseMode = new HotKeyAction();
            toggleMouseMode.DisplayHotkeyAction = Application.Current.Resources["Hotkeys_Action_Change_Toggle_MouseMode"].ToString();
            toggleMouseMode.HotkeyAction = "Change_Toggle_MouseMode";

            hotkeyAction.Add(toggleMouseMode);
            

            control.ItemsSource = hotkeyAction;

            foreach (HotKeyAction hka in hotkeyAction)
            {
                if (Global_Variables.hotKeys.editingHotkey.Action == hka.HotkeyAction)
                {
                    control.SelectedItem = hka;
                    value.Content = hka.DisplayHotkeyAction;
                }
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
            if (control.IsLoaded && control.Visibility == Visibility.Visible)
            {
                if (control.SelectedItem != null)
                {
                    HotKeyAction selected = (HotKeyAction)control.SelectedItem;
                    if (Global_Variables.hotKeys.editingHotkey.Action != selected.HotkeyAction)
                    {
                        Global_Variables.hotKeys.editingHotkey.Action = selected.HotkeyAction;
                        value.Content = selected.DisplayHotkeyAction; 
                        Global_Variables.hotKeys.raiseHotkeyActionChangedEvent();
                    }

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

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
        }
    }

    public class HotKeyAction
    {
        public string DisplayHotkeyAction { get; set; }
        public string HotkeyAction { get; set; }
    }
  
}
