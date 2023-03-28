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
    public partial class Bluetooth_Toggle : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        public Bluetooth_Toggle()
        {
            InitializeComponent();
            setControlValue();
          
        }

        private async void setControlValue()
        {
            bool BTresult = await Task.Run(() => GetBTIsEnabledAsync());
            control.IsOn = BTresult;

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
                ToggleBT(controlValue);
                
            }
          
        }
        public async Task<bool> GetBTIsEnabledAsync()
        {//error number BTT01
            try
            {
                bool value = false;
                var radios = await Radio.GetRadiosAsync();
                var btRadio = radios.FirstOrDefault(radio => radio.Kind == RadioKind.Bluetooth);
                value = btRadio != null && btRadio.State == RadioState.On;
                btRadio = null;
                radios = null;

                return value;
            }
            catch (Exception ex)
            {
                Log_Writer.writeLog(usercontrol + "; " + ex.Message, "BTT01");

                return false;
            }

        }
        public async Task ToggleBT(bool value)
        {
            var radios = await Radio.GetRadiosAsync();
            var btRadio = radios.FirstOrDefault(radio => radio.Kind == RadioKind.Bluetooth);
            if (btRadio != null)
            {
                if (btRadio.State == RadioState.Off && value) { btRadio.SetStateAsync(RadioState.On); }
                if (btRadio.State == RadioState.On && !value) { btRadio.SetStateAsync(RadioState.Off); }
            }
            btRadio = null;
  
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
        }
    }
}
