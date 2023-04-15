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
using static Vanara.Interop.KnownShellItemPropertyKeys;

namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class Action_Dropdown : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private HotKeyAction selectedObject;
        public Action_Dropdown()
        {
            InitializeComponent();
            setListboxItemsource();
          
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
            toggleMouseMode.DisplayHotkeyAction = Application.Current.Resources["Hotkeys_Action_Toggle_MouseMode"].ToString();
            toggleMouseMode.HotkeyAction = "Toggle_MouseMode";

            hotkeyAction.Add(toggleMouseMode);


            HotKeyAction gotoDesktop = new HotKeyAction();
            gotoDesktop.DisplayHotkeyAction = Application.Current.Resources["Hotkeys_Action_Desktop"].ToString();
            gotoDesktop.HotkeyAction = "Desktop";

            hotkeyAction.Add(gotoDesktop);

            if (Global_Variables.cpuType == "AMD" && Global_Variables.processorName.Contains("6800U"))
            {
                HotKeyAction autoTDP = new HotKeyAction();
                autoTDP.DisplayHotkeyAction = Application.Current.Resources["Hotkeys_Action_Toggle_AutoTDP"].ToString();
                autoTDP.HotkeyAction = "Toggle_AutoTDP";

                hotkeyAction.Add(autoTDP);
            }
           

            HotKeyAction toggleWinOSK = new HotKeyAction();
            toggleWinOSK.DisplayHotkeyAction = Application.Current.Resources["Hotkeys_Action_Toggle_Windows_OSK"].ToString();
            toggleWinOSK.HotkeyAction = "Toggle_Windows_OSK";

            hotkeyAction.Add(toggleWinOSK);

            HotKeyAction toggleHCPOSK = new HotKeyAction();
            toggleHCPOSK.DisplayHotkeyAction = Application.Current.Resources["Hotkeys_Action_Toggle_HCP_OSK"].ToString();
            toggleHCPOSK.HotkeyAction = "Toggle_HCP_OSK";

            hotkeyAction.Add(toggleHCPOSK);

            
            controlList.ItemsSource = hotkeyAction;

            foreach (HotKeyAction hka in hotkeyAction)
            {
                if (Global_Variables.hotKeys.editingHotkey.Action == hka.HotkeyAction)
                {
                    controlList.SelectedItem = hka;
                    actionLabel.Content = hka.DisplayHotkeyAction;
                }
            }


        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");
            controlList.Visibility = Visibility.Collapsed;
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
                            Global_Variables.mainWindow.changeUserInstruction("HotKeyEditPage_Instruction");
                        }
                        else
                        {
                            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            Global_Variables.mainWindow.changeUserInstruction("SelectedListBox_Instruction");
                    
                        }

                        break;
                    case "B":
                        Global_Variables.mainWindow.changeUserInstruction("HotKeyEditPage_Instruction");
                        if (controlList.Visibility == Visibility.Visible)
                        {
                            

                            if (selectedObject != null)
                            {
                                
                                controlList.SelectedItem = selectedObject;
                                actionLabel.Content = selectedObject.DisplayHotkeyAction;
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
                    HotKeyAction hka = (HotKeyAction)controlList.SelectedItem;
                
                    selectedObject = (HotKeyAction)controlList.SelectedItem;
                    if (Global_Variables.hotKeys.editingHotkey.Action != hka.HotkeyAction)
                    {
                        Global_Variables.hotKeys.editingHotkey.Action = hka.HotkeyAction;
                        actionLabel.Content = hka.DisplayHotkeyAction;
                        Global_Variables.hotKeys.raiseHotkeyActionChangedEvent();
                    }
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
            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (controlList.Visibility == Visibility.Visible)
            {
                controlList.Visibility = Visibility.Collapsed;
                PackIconUnicons icon = (PackIconUnicons)button.Content;
                icon.RotationAngle = 0;
            }
            else
            {
                controlList.Visibility = Visibility.Visible;
                PackIconUnicons icon = (PackIconUnicons)button.Content;
                icon.RotationAngle = 90;
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
    public class HotKeyAction
    {
        public string DisplayHotkeyAction { get; set; }
        public string HotkeyAction { get; set; }
    }
}
