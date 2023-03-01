using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.TaskSchedulerWin32;
using Handheld_Control_Panel.Classes.UserControl_Management;
using Handheld_Control_Panel.Styles;
using MahApps.Metro.Controls;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Devices.Radios;

namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class Wifi_Toggle : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        public Wifi_Toggle()
        {
            InitializeComponent();
            setControlValue();
          
        }

        private async void setControlValue()
        {
            bool Wifiresult = await Task.Run(() => GetWifiIsEnabledAsync());
            control.IsOn = Wifiresult;

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");

        }
        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {
                Classes.UserControl_Management.UserControl_Management.handleUserControl(border, control, args.Action);

            }
        }


        private void toggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (control.IsLoaded)
            {
                bool controlValue = control.IsOn;
                ToggleWifi(controlValue);
                
            }
          
        }
        public async Task<bool> GetWifiIsEnabledAsync()
        {
            bool value = false;
            var radios = await Radio.GetRadiosAsync();
            var wifiRadio = radios.FirstOrDefault(radio => radio.Kind == RadioKind.WiFi);
            value = wifiRadio != null && wifiRadio.State == RadioState.On;
            wifiRadio = null;
            radios = null;

            return value;
        }
        public async Task ToggleWifi(bool value)
        {
            var radios = await Radio.GetRadiosAsync();
            var wifiRadio = radios.FirstOrDefault(radio => radio.Kind == RadioKind.WiFi);
            if (wifiRadio != null)
            {
                if (wifiRadio.State == RadioState.Off && value) { wifiRadio.SetStateAsync(RadioState.On); }
                if (wifiRadio.State == RadioState.On && !value) { wifiRadio.SetStateAsync(RadioState.Off); }
            }
            wifiRadio = null;
  
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
        }
    }
}
