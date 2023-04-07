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
    public partial class Type_Dropdown : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private HotKeyTypes selectedObject;
        public Type_Dropdown()
        {
            InitializeComponent();
            setListboxItemsource();
          
        }

        private void setListboxItemsource()
        {
            List<HotKeyTypes> hotkeyTypes = new List<HotKeyTypes>();

            HotKeyTypes controller = new HotKeyTypes();
            controller.DisplayHotKeyType = Application.Current.Resources["Hotkeys_Type_Controller"].ToString();
            controller.HotKeyType = "Controller";
            HotKeyTypes keyboard = new HotKeyTypes();
            keyboard.DisplayHotKeyType = Application.Current.Resources["Hotkeys_Type_Keyboard"].ToString();
            keyboard.HotKeyType = "Keyboard";
            hotkeyTypes.Add(controller);
            hotkeyTypes.Add(keyboard);
            controlList.ItemsSource = hotkeyTypes;
            if (Global_Variables.hotKeys.editingHotkey.Type == "Controller") { controlList.SelectedIndex = 0; actionLabel.Content = controller.DisplayHotKeyType; } else { controlList.SelectedIndex = 1; actionLabel.Content = keyboard.DisplayHotKeyType; }
           

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
                                actionLabel.Content = selectedObject.DisplayHotKeyType;
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
                    HotKeyTypes hka = (HotKeyTypes)controlList.SelectedItem;
       
                    selectedObject = (HotKeyTypes)controlList.SelectedItem;
                    if (hka.HotKeyType != Global_Variables.hotKeys.editingHotkey.Type)
                    {
                        Global_Variables.hotKeys.editingHotkey.Type = hka.HotKeyType;
                        actionLabel.Content = selectedObject.DisplayHotKeyType;
                        Global_Variables.hotKeys.raiseHotkeyClearedEvent(); 
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
    public class HotKeyTypes
    {
        public string DisplayHotKeyType { get; set; }
        public string HotKeyType { get; set; }
    }
}
