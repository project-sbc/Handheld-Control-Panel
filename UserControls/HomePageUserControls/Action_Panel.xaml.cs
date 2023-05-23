using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.TaskSchedulerWin32;
using Handheld_Control_Panel.Classes.UserControl_Management;
using Handheld_Control_Panel.Styles;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
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

using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class Action_Panel : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private Dictionary<string, Action_Panel_Items> valueChangedParameters = new Dictionary<string, Action_Panel_Items>();
        private List<Action_Panel_Items> apiList = new List<Action_Panel_Items>();

        public Action_Panel()
        {
            InitializeComponent();
            //setControlValue();
            UserControl_Management.setupControl(null);


            loadActionPanelList();


        }
      

        private void Global_Variables_valueChanged(object? sender, valueChangedEventArgs e)
        {
            valueChangedEventArgs valueChangedEventArgs = (valueChangedEventArgs)e;
            if (valueChangedParameters.ContainsKey(valueChangedEventArgs.Parameter))
            {
                this.Dispatcher.BeginInvoke(() => {
                    if (controlList.IsLoaded)
                    {
                        Action_Panel_Items api = valueChangedParameters[valueChangedEventArgs.Parameter];
                        switch(valueChangedEventArgs.Parameter)
                        {
                            case "RefreshRate":
                                api.text = Global_Variables.RefreshRate + " Hz";
                                break;
                            case "Resolution":
                                api.text = Global_Variables.Resolution;
                                break;
                            case "Brightness":
                                api.text = Global_Variables.Brightness.ToString() + " %";
                                break;
                            case "Volume":
                                api.text = "  " + Global_Variables.Volume.ToString() + " %";
                                break;
                            case "FanSpeed":
                                api.text = Global_Variables.FanSpeed.ToString() + " %";
                                break;
                            case "TDP1":
                                api.text = Global_Variables.ReadPL1.ToString() + " W";
                                break;
                        }

                        controlList.Items.Refresh();
                    }

                });
            }

        }

        private void loadActionPanelList()
        {
            foreach (HotkeyItem hki in Global_Variables.hotKeys)
            {
                if (hki.AddHomePage)
                {
                    Action_Panel_Items api = new Action_Panel_Items();
                    api.hki = hki;
                    api.data = Application.Current.Resources["Path_Data_" + hki.Action].ToString();

                    switch (hki.Action)
                    {
                        case "Toggle_AMD_RSR":
                            api.text = "    RSR";
                            if (ADLX_Management.GetRSRState() == 1)
                            {
                                api.visibilitySlash = Visibility.Visible;
                            }
                            else
                            {
                                api.visibilitySlash = Visibility.Hidden;
                            }
                            break;
                        case "Toggle_MouseMode":
                            if (!Global_Variables.MouseModeEnabled)
                            {
                                api.visibilitySlash = Visibility.Visible;
                            }

                            break;
                        case "Change_Refresh_Mode":
                            api.text = Global_Variables.RefreshRate;
                            valueChangedParameters.Add("RefreshRate",api);
                            break;
                        case "Change_Resolution_Mode":
                            api.text = Global_Variables.Resolution;
                            valueChangedParameters.Add("Resolution", api);
                            break;
                        case "Change_Brightness_Mode":
                            api.text = Global_Variables.Brightness + " %";
                            valueChangedParameters.Add("Brightness", api);
                            break;
                        case "Change_GPUCLK":
                            api.text = hki.DisplayParameter + " MHz";
                            break;
                        case "Change_Brightness":
                            api.text = hki.DisplayParameter + " %";
                            break;
                        case "Change_FanSpeed_Mode":
                            api.text = Global_Variables.FanSpeed + " %";
                            valueChangedParameters.Add("FanSpeed", api);
                            break;
                        case "Change_FanSpeed":
                            api.text = hki.DisplayParameter + " %";
                            break;
                        case "Change_Volume_Mode":
                            api.text ="  " + Global_Variables.Volume + " %";
                            valueChangedParameters.Add("Volume", api);
                            break;
                        case "Change_Volume":
                            api.text = hki.DisplayParameter + " %";
                            break;
                        case "Change_TDP":
                            api.text = hki.DisplayParameter + " W";
                            break;
                        case "Change_TDP_Mode":
                            api.text = Global_Variables.ReadPL1.ToString() + " W";
                            valueChangedParameters.Add("TDP1", api);
                            break;
                        case "Open_Program":
                            
                            break;
                        default:
                            

                            break;
                    }
                    if (hki.Action != "")
                    {
                        apiList.Add(api);
                    }
                }

            }

            controlList.ItemsSource = apiList;


        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //UserControl_Management.setThumbSize(control);

            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.", "");
            Global_Variables.valueChanged += Global_Variables_valueChanged;

        }

        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args = (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl == usercontrol)
            {
                if (controlList.SelectedItem != null)
                {
                    Profile lbai = controlList.SelectedItem as Profile;
                    int index = controlList.SelectedIndex;
                    switch (args.Action)
                    {
                        case "A":
                            handleActionButtonPressed();

                            break;

                        case "Up":
                            handleListBoxIndexChange(controlList, -3);

                            break;
                        case "Down":
                            handleListBoxIndexChange(controlList, 3);
                            break;
                        case "Left":
                            handleListBoxIndexChange(controlList, -1);
                            break;
                        case "Right":
                            handleListBoxIndexChange(controlList, 1);
                            break;
                        case "LB":
                            handleListBoxIndexChange(controlList, -15);
                            break;
                        case "RB":
                            handleListBoxIndexChange(controlList, 15);
                            break;
                        default:
                            Classes.UserControl_Management.UserControl_Management.handleUserControl(border, null, args.Action);
                            break;

                    }

                }

            }
        }

        private void handleActionButtonPressed()
        {
            if (controlList.SelectedItem != null)
            {
                Action_Panel_Items api = controlList.SelectedItem as Action_Panel_Items;

                if (api != null)
                {
                    if (api.hki != null)
                    {
                        ActionParameter ap = new ActionParameter();
                        ap.Action = api.hki.Action;
                        ap.Parameter = api.hki.Parameter;

                        QuickAction_Management.runHotKeyAction(ap);

                        switch (ap.Action)
                        {
                            case "Toggle_AMD_RSR":
                                if (api.visibilitySlash == Visibility.Visible)
                                {
                                    api.visibilitySlash = Visibility.Hidden;
                                }
                                else
                                {
                                    api.visibilitySlash = Visibility.Visible;
                                }
                                break;

                        }
                        controlList.Items.Refresh();
                    }

                }

            }
        }

      

        private void handleListBoxIndexChange(ListBox lb, int change)
        {
            int selectedIndex = lb.SelectedIndex;
            int upperIndex = lb.Items.Count - 1;
            if (change < 0)
            {
                if (selectedIndex >= -change)
                {
                    lb.SelectedIndex = selectedIndex + change;
                    lb.ScrollIntoView(lb.SelectedItem);
                }
                else
                {
                    if (selectedIndex != 0)
                    {
                        lb.SelectedIndex = 0;
                        lb.ScrollIntoView(lb.SelectedItem);
                    }
                    else
                    {
                        lb.SelectedIndex = lb.Items.Count - 1;
                        lb.ScrollIntoView(lb.SelectedItem);
                    }
                }

            }
            if (change > 0)
            {
                if ((upperIndex - selectedIndex) >= change)
                {
                    lb.SelectedIndex = selectedIndex + change;
                    lb.ScrollIntoView(lb.SelectedItem);
                }
                else
                {
                    if (selectedIndex != upperIndex)
                    {
                        lb.SelectedIndex = upperIndex;
                        lb.ScrollIntoView(lb.SelectedItem);
                    }
                    else
                    {
                        lb.SelectedIndex = 0;
                        lb.ScrollIntoView(lb.SelectedItem);
                    }
                }


            }

        }



        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;

        }

        private void controlList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //update text to show what the action is
            if (this.IsLoaded)
            {
                if (controlList.SelectedItem != null)
                {
                    Action_Panel_Items api = controlList.SelectedItem as Action_Panel_Items;
                    if (api != null)
                    {
                        if (api.hki != null)
                        {
                            actionName.Text = api.hki.DisplayAction;
                        }
                        
                    }
                    
                }

            }
        }
    }
    public class Action_Panel_Items
    {
        public HotkeyItem hki { get; set; }
        public string data { get; set; }
        public string text { get; set; }
        public Visibility canvasVisibility { get; set; } = Visibility.Visible;
        public Visibility imageVisibility { get; set; } = Visibility.Collapsed;
        public Visibility visibilitySlash { get; set; } = Visibility.Collapsed;
        public Image image { get; set; }

    }
    
}
