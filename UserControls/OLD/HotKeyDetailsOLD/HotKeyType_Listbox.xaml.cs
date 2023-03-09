using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Display_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.UserControl_Management;
using Handheld_Control_Panel.Styles;
using MahApps.Metro.Controls;
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
using static Vanara.Interop.KnownShellItemPropertyKeys;

namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class HotKeyType_Listbox : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private object selectedObject;
        

        public HotKeyType_Listbox()
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
            List<HotKeyTypes> hotkeyTypes = new List<HotKeyTypes>();

            HotKeyTypes controller = new HotKeyTypes();
            controller.DisplayHotKeyType = Application.Current.Resources["Hotkeys_Type_Controller"].ToString();
            controller.HotKeyType = "Controller";
            HotKeyTypes keyboard = new HotKeyTypes();
            keyboard.DisplayHotKeyType = Application.Current.Resources["Hotkeys_Type_Keyboard"].ToString();
            keyboard.HotKeyType = "Keyboard";
            hotkeyTypes.Add(controller);
            hotkeyTypes.Add(keyboard);
            control.ItemsSource = hotkeyTypes;
            if (Global_Variables.hotKeys.editingHotkey.Type == "Controller") { control.SelectedIndex = 0; icon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.MicrosoftXboxController; } else { control.SelectedIndex = 1; }

         

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
                    HotKeyTypes selected = (HotKeyTypes)control.SelectedItem;
                    if (Global_Variables.hotKeys.editingHotkey.Type != selected.HotKeyType)
                    {
                        Global_Variables.hotKeys.editingHotkey.Type = selected.HotKeyType;
                        Global_Variables.hotKeys.editingHotkey.Hotkey = "";
                        Global_Variables.hotKeys.raiseHotkeyClearedEvent();
                    }
                        
                    if (selected.HotKeyType== "Controller") { icon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.MicrosoftXboxController; } else { icon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Keyboard; }
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

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
        }
    }

    public class HotKeyTypesOLD
    {
        public string DisplayHotKeyType { get; set; }
        public string HotKeyType { get; set; }
    }
  
}
