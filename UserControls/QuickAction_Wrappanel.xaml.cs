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
using MahApps.Metro.IconPacks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Handheld_Control_Panel.Classes.Volume_Management;

namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class QuickAction_Wrappanel : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private object selectedObject;
        

        public QuickAction_Wrappanel()
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
           
            
        }

        private async void setListboxItemsource()
        {
            List<quickactionItem> items = new List<quickactionItem>();

            quickactionItem qaiWifi = new quickactionItem();
            qaiWifi.ID = "Toggle_Wifi";
            qaiWifi.iconKind = PackIconMaterialKind.Wifi;
            Task<bool> wifi = QuickAction_Management.GetWifiIsEnabledAsync();
            if (!wifi.Result) { qaiWifi.disabled = PackIconUniconsKind.LineAlt; }

            quickactionItem qaiBT = new quickactionItem();
            qaiBT.ID = "Toggle_BT";
            qaiBT.iconKind = PackIconMaterialKind.Bluetooth ;
            Task<bool> bt = QuickAction_Management.GetBluetoothIsEnabledAsync();
            if (!bt.Result) { qaiBT.disabled = PackIconUniconsKind.LineAlt; }

            quickactionItem qaiVolume = new quickactionItem();
            qaiVolume.ID = "Toggle_Volume";
            qaiVolume.iconKind = PackIconMaterialKind.VolumeHigh;
            AudioManager.GetMasterVolumeMute();
            if (Global_Variables.muteVolume) { qaiVolume.iconKind = PackIconMaterialKind.VolumeMute; }

            quickactionItem qai3 = new quickactionItem();
            qai3.ID = "Toggle_Controller";
            qai3.iconKind = PackIconMaterialKind.MicrosoftXboxController;

            quickactionItem qai4 = new quickactionItem();
            qai4.ID = "Toggle_MouseMode";
            qai4.iconKind = PackIconMaterialKind.Mouse;

            items.Add(qaiWifi);
            items.Add(qaiBT);
            items.Add(qaiVolume);
            items.Add(qai3);
            items.Add(qai4);

            control.ItemsSource = items;

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
                    quickactionItem qai = (quickactionItem)control.SelectedItem;
                    
                    
   
                    switch (qai.ID)
                    {
                        case "Toggle_Wifi":
                            QuickAction_Management.ToggleWifi();
                            if (qai.disabled == PackIconUniconsKind.None) { qai.disabled = PackIconUniconsKind.LineAlt; } else { qai.disabled = PackIconUniconsKind.None; }
                            break;
                        case "Toggle_BT":
                            QuickAction_Management.ToggleBT();
                            if (qai.disabled == PackIconUniconsKind.None) { qai.disabled = PackIconUniconsKind.LineAlt; } else { qai.disabled = PackIconUniconsKind.None; }
                            break;
                        case "Toggle_Volume":
                            AudioManager.GetMasterVolumeMute();
                            if (Global_Variables.muteVolume) { qai.iconKind = PackIconMaterialKind.VolumeMute; } else { qai.iconKind = PackIconMaterialKind.VolumeHigh; }
                            break;

                    }
                    control.Items.Refresh();
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
    class quickactionItem
    {
        public string ID { get; set; }
        public PackIconMaterialKind iconKind { get; set; }
        public PackIconUniconsKind disabled { get; set; }
    }
}
