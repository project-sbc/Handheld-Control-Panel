using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using System.Windows.Input;

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


            //set controllist default mode to single select, not multi, will adjust below if it needs to be multi
            controlList.SelectionMode = SelectionMode.Single;
            switch (Global_Variables.hotKeys.editingHotkey.Action)
            {
                case "Change_TDP_Mode":
                    controlList.SelectionMode = SelectionMode.Multiple;
                    for (int i = 5; i < Global_Variables.settings.maxTDP; i=i+5)
                    {
                        Parameter parameter = new Parameter();
                        parameter.DisplayParameter = i.ToString() + " W"; 
                        parameter.ParameterValue = i.ToString();
                        hotkeyParameter.Add(parameter);
                    }
                    break;
                case "Change_TDP":
                    
                    for (int i = -5; i < 6; i++)
                    {
                        if (i != 0)
                        {
                            Parameter parameter = new Parameter();
                            if (i > 0) { parameter.DisplayParameter = "+" + i.ToString() + " W"; } else { parameter.DisplayParameter = i.ToString() + " W"; }
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
                case "Change_Brightness_Mode":
                    controlList.SelectionMode = SelectionMode.Multiple;
                    for (int i = 0; i < 21; i++)
                    {
                        Parameter parameter = new Parameter();
                        int j = i * 5; //multiply by 5 to give a larger value without making the loop complicated
                        parameter.DisplayParameter = j.ToString() + "%";
                        parameter.ParameterValue = j.ToString();
                        hotkeyParameter.Add(parameter);

                    }
                    break;
                case "Change_Volume":
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
                case "Change_FanSpeed":
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
                case "Change_Volume_Mode":
                    controlList.SelectionMode = SelectionMode.Multiple;
                    for (int i = 0; i < 21; i++)
                    {
                        Parameter parameter = new Parameter();
                        int j = i * 5; //multiply by 5 to give a larger value without making the loop complicated
                        parameter.DisplayParameter = j.ToString() + "%";
                        parameter.ParameterValue = j.ToString();
                        hotkeyParameter.Add(parameter);

                    }
                    break;
                case "Change_FanSpeed_Mode":
                    if (Global_Variables.Device.FanCapable)
                    {
                        controlList.SelectionMode = SelectionMode.Multiple;
                        Parameter parameter0 = new Parameter();
                        parameter0.DisplayParameter = "0%";
                        parameter0.ParameterValue = "0";
                        hotkeyParameter.Add(parameter0);
                        for (int i = Global_Variables.Device.MinFanSpeedPercentage; i < 100; i=i+5)
                        {
                            Parameter parameter = new Parameter();
                           
                            parameter.DisplayParameter = i.ToString() + "%";
                            parameter.ParameterValue = i.ToString();
                            hotkeyParameter.Add(parameter);

                        }
                        Parameter parameter100 = new Parameter();
                        parameter100.DisplayParameter = "100%";
                        parameter100.ParameterValue = "100";
                        hotkeyParameter.Add(parameter100);
                    }
                   
                    break;
                case "Change_Resolution_Mode":
                    controlList.SelectionMode = SelectionMode.Multiple;
                    foreach (string resolution in Global_Variables.resolutions)
                    {
                        Parameter parameter = new Parameter();
                        parameter.DisplayParameter = resolution;
                        parameter.ParameterValue = resolution;
                        hotkeyParameter.Add(parameter);
                    }

                    break;
                case "Change_Refresh_Mode":
                    controlList.SelectionMode = SelectionMode.Multiple;
                    foreach (string refresh in Global_Variables.resolution_refreshrates[Global_Variables.resolution])
                    {
                        Parameter parameter = new Parameter();
                        parameter.DisplayParameter = refresh + " Hz";
                        parameter.ParameterValue = refresh;
                        hotkeyParameter.Add(parameter);
                    }

                    break;
                case "Open_Program":

                    foreach (Profile_Main profile in Global_Variables.profiles.Where(o => o.profile_Exe.Exe_Type != "").ToList<Profile_Main>())
                    {
                        Parameter parameter = new Parameter();
                        Debug.WriteLine(profile.profile_Exe.Exe_Type);
                        parameter.DisplayParameter = profile.ProfileName;
                        parameter.ParameterValue = profile.ProfileName;
                        hotkeyParameter.Add(parameter);
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
                    List<string> parameterArray = Global_Variables.hotKeys.editingHotkey.Parameter.Split(";").ToList<string>();
                    foreach (Parameter param in hotkeyParameter)
                    {
                        if (controlList.SelectionMode == SelectionMode.Single)
                        {
                            if (Global_Variables.hotKeys.editingHotkey.Parameter == param.ParameterValue)
                            {
                                controlList.SelectedItem = param;
                                actionLabel.Content = param.DisplayParameter;
                                break;
                            }
                        }
                        else
                        {
                            if (parameterArray.Contains(param.DisplayParameter))
                            {
                                controlList.SelectedItems.Add(param);
                            }
                        }
                      
                    }
                    actionLabel.Content = Global_Variables.hotKeys.editingHotkey.Parameter;
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
                            if (controlList.SelectionMode == SelectionMode.Single)
                            {
                                
                                Global_Variables.mainWindow.changeUserInstruction("HotKeyEditPage_Instruction");
                            }

                        }
                        else
                        {
                            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

                            Global_Variables.mainWindow.changeUserInstruction("SelectedListBox_Instruction");

                         
                        }

                        break;
                    case "B":
                        MainWindow wnd2 = (MainWindow)Application.Current.MainWindow;
                        wnd2.changeUserInstruction("HotKeyEditPage_Instruction");
                        wnd2 = null;
                        if (controlList.Visibility == Visibility.Visible)
                        {
                            
                            if (controlList.SelectionMode == SelectionMode.Single)
                            {
                                if (selectedObject != null)
                                {

                                    controlList.SelectedItem = selectedObject;
                                    actionLabel.Content = selectedObject.DisplayHotkeyAction;
                                }
                            }
                            else
                            {
                                handleListboxChange();
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
                    if (controlList.SelectionMode == SelectionMode.Single)
                    {
                        Parameter selectedItem = (Parameter)controlList.SelectedItem;
                        if (Global_Variables.hotKeys.editingHotkey.Parameter != selectedItem.ParameterValue)
                        {
                            Global_Variables.hotKeys.editingHotkey.Parameter = selectedItem.ParameterValue;
                            actionLabel.Content = selectedItem.DisplayParameter;
                            if (controlList.Visibility == Visibility.Visible && controlList.SelectionMode == SelectionMode.Single)
                            {
                                button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            }
                        }
                    }
                    else
                    {
                        

                        System.Collections.IList selectedItems = controlList.SelectedItems;
                        string totalString="";
                        string totalDisplayString="";
                        foreach (Parameter parameter in selectedItems)
                        {
                            if (totalString == "") { totalString = parameter.ParameterValue; }
                            else { totalString = totalString + ";" + parameter.ParameterValue; }

                            if (totalDisplayString == "") { totalDisplayString = parameter.DisplayParameter; }
                            else { totalDisplayString = totalDisplayString + ";" + parameter.DisplayParameter; }
                        }
                        if (Global_Variables.hotKeys.editingHotkey.Parameter != totalString)
                        {
                            Global_Variables.hotKeys.editingHotkey.Parameter = totalString;
                            actionLabel.Content = totalDisplayString;
                          
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
