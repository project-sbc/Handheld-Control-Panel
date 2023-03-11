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
    public partial class HotKeyParameter_Listbox : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private object selectedObject;
        

        public HotKeyParameter_Listbox()
        {
            InitializeComponent();
            //UserControl_Management.setupControl(control);
            setListboxItemsource();
            Global_Variables.hotKeys.hotkeyActionChangedEvent += HotKeys_hotkeyActionChangedEvent;
        }

        private void HotKeys_hotkeyActionChangedEvent(object? sender, EventArgs e)
        {
            //event triggered when hotkey is changed from controller to keyboard or vice versa
            value.Content = "";
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
            this.Visibility = Visibility.Visible;
            List<Parameter> hotkeyParameter = new List<Parameter>();
            if (control.ItemsSource!= null) { control.ItemsSource = null; }
            //set as default visible and switch to collapsed in switch statement if its not a parameter type of action (like opening closing hcp)
          
            switch (Global_Variables.hotKeys.editingHotkey.Action)
            {

                case "Change_TDP":
                    for (int i = -5; i < 6; i++)
                    {
                        if (i != 0)
                        {
                            Parameter parameter = new Parameter();
                            if (i >0) { parameter.DisplayParameter = "+" + i.ToString(); } else { parameter.DisplayParameter = i.ToString(); }
                            parameter.ParameterValue = i.ToString();
                            hotkeyParameter.Add(parameter);
                        }
                        

                    }

                    break;
                case "Change_GPUCLK":
                    for (int i = -5; i < 6; i++)
                    {
                        if (i != 0)
                        {
                            Parameter parameter = new Parameter();
                            int j = i * 50; //multiply by 50 to give a larger value without making the loop complicated
                            if (i > 0) { parameter.DisplayParameter = "+" + j.ToString() + " MHz"; } else { parameter.DisplayParameter =j.ToString() + " MHz"; }
                            parameter.ParameterValue = j.ToString();
                            hotkeyParameter.Add(parameter);
                        }


                    }
                    break;
                case "Change_Brightness":
                    for (int i = -3; i < 4; i++)
                    {
                        if (i != 0)
                        {
                            Parameter parameter = new Parameter();
                            int j = i * 5; //multiply by 5 to give a larger value without making the loop complicated
                            if (i > 0) { parameter.DisplayParameter = "+" + j.ToString() + "%"; } else { parameter.DisplayParameter = j.ToString() +"%"; }
                            parameter.ParameterValue = j.ToString();
                            hotkeyParameter.Add(parameter);
                        }

                    }
                    break;

                default:
                    this.Visibility = Visibility.Collapsed;
                    break;
            }
            if (this.Visibility== Visibility.Visible)
            {
                if (hotkeyParameter.Count > 0)
                {

                    control.ItemsSource = hotkeyParameter;
                    foreach (Parameter param in hotkeyParameter)
                    {
                        if (Global_Variables.hotKeys.editingHotkey.Parameter == param.ParameterValue)
                        {
                            control.SelectedItem = param;
                            value.Content = param.DisplayParameter;
                        }
                    }
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
                    Parameter selected = (Parameter)control.SelectedItem;
                    if (Global_Variables.hotKeys.editingHotkey.Parameter != selected.ParameterValue)
                    {
                        Global_Variables.hotKeys.editingHotkey.Parameter = selected.ParameterValue;
                        value.Content = selected.DisplayParameter;
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
    public class ParameterOLD
    {
        public string DisplayParameter { get; set; }
        public string ParameterValue { get; set; }
    }
   
  
}
