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

        private void setListboxItemsource()
        {
            List<quickactionItem> items = new List<quickactionItem>();

            quickactionItem qai = new quickactionItem();
            qai.ID = "Toggle_Wifi";
            qai.iconKind = PackIconMaterialKind.Wifi;

            quickactionItem qai0 = new quickactionItem();
            qai0.ID = "Toggle_BT";
            qai0.iconKind = PackIconMaterialKind.Bluetooth ;

            quickactionItem qai2 = new quickactionItem();
            qai2.ID = "Toggle_Volume";
            qai2.iconKind = PackIconMaterialKind.VolumeHigh;

            quickactionItem qai3 = new quickactionItem();
            qai3.ID = "Toggle_Controller";
            qai3.iconKind = PackIconMaterialKind.MicrosoftXboxController;

            quickactionItem qai4 = new quickactionItem();
            qai4.ID = "Toggle_MouseMode";
            qai4.iconKind = PackIconMaterialKind.Mouse;

            items.Add(qai);
            items.Add(qai0);
            items.Add(qai2);
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

    }
}
