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
using Windows.Devices.Radios;
using static Vanara.Interop.KnownShellItemPropertyKeys;

namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class Parameter_Dropdown : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private HotKeyAction selectedObject;
        public Parameter_Dropdown()
        {
            InitializeComponent();
           
        }
        private void HotKeys_hotkeyActionChangedEvent(object? sender, EventArgs e)
        {
            //event triggered when hotkey is changed from controller to keyboard or vice versa
            actionLabel.Content = "";
            Global_Variables.hotKeys.editingHotkey.Parameter = "";
            setListboxItemsource();

        }
        private void setListboxItemsource()
        {
            this.Visibility = Visibility.Visible;
            List<Parameter> hotkeyParameter = new List<Parameter>();
            if (controlList.ItemsSource != null) { controlList.ItemsSource = null; }
            //set as default visible and switch to collapsed in switch statement if its not a parameter type of action (like opening closing hcp)

            switch (Global_Variables.hotKeys.editingHotkey.Action)
            {

                case "Change_TDP":
                    for (int i = -5; i < 6; i++)
                    {
                        if (i != 0)
                        {
                            Parameter parameter = new Parameter();
                            if (i > 0) { parameter.DisplayParameter = "+" + i.ToString(); } else { parameter.DisplayParameter = i.ToString(); }
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
                            if (i > 0) { parameter.DisplayParameter = "+" + j.ToString() + " MHz"; } else { parameter.DisplayParameter = j.ToString() + " MHz"; }
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
                            if (i > 0) { parameter.DisplayParameter = "+" + j.ToString() + "%"; } else { parameter.DisplayParameter = j.ToString() + "%"; }
                            parameter.ParameterValue = j.ToString();
                            hotkeyParameter.Add(parameter);
                        }

                    }
                    break;

                default:
                    this.Visibility = Visibility.Collapsed;
                    break;
            }
            if (this.Visibility == Visibility.Visible)
            {
                if (hotkeyParameter.Count > 0)
                {

                    controlList.ItemsSource = hotkeyParameter;
                    foreach (Parameter param in hotkeyParameter)
                    {
                        if (Global_Variables.hotKeys.editingHotkey.Parameter == param.ParameterValue)
                        {
                            controlList.SelectedItem = param;
                            actionLabel.Content = param.DisplayParameter;
                        }
                    }
                }

            }

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");

            setListboxItemsource();
            Global_Variables.hotKeys.hotkeyActionChangedEvent += HotKeys_hotkeyActionChangedEvent;

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
                        }
                        else
                        {
                            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

                            MainWindow wnd = (MainWindow)Application.Current.MainWindow;
                            wnd.changeUserInstruction("SelectedListBox_Instruction");
                            wnd = null;
                         
                        }

                        break;
                    case "B":
                        MainWindow wnd2 = (MainWindow)Application.Current.MainWindow;
                        wnd2.changeUserInstruction("HomePage_Instruction");
                        wnd2 = null;
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
          

            if (controlList.IsLoaded && controlList.Visibility == Visibility.Visible)
            {
                if (controlList.SelectedItem != null)
                {
                    Parameter selected = (Parameter)controlList.SelectedItem;
                    if (Global_Variables.hotKeys.editingHotkey.Parameter != selected.ParameterValue)
                    {
                        Global_Variables.hotKeys.editingHotkey.Parameter = selected.ParameterValue;
                        actionLabel.Content = selected.DisplayParameter;
                        if (controlList.Visibility == Visibility.Visible)
                        {
                            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        }
                    }

                }


            }

        }


        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
            Global_Variables.hotKeys.hotkeyActionChangedEvent -= HotKeys_hotkeyActionChangedEvent;
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
    public class Parameter
    {
        public string DisplayParameter { get; set; }
        public string ParameterValue { get; set; }
    }
}
