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
    public partial class Customize_QAM : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private object selectedObject;
        

        public Customize_QAM()
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
            ucItem uc1 = new ucItem();
            uc1.DisplayName = "Yes";
            ucItem uc2 = new ucItem();
            uc2.DisplayName = "Yes";
            ucItem uc3 = new ucItem();
            uc3.DisplayName = "Yes";
            controlL.Items.Add(uc1);
            controlL.Items.Add(uc2);
            controlL.Items.Add(uc3);
            controlR.Items.Add(uc1);
            controlR.Items.Add(uc2);
            controlR.Items.Add(uc3);
        


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
                        Classes.UserControl_Management.UserControl_Management.handleUserControl(border, controlL, args.Action);
                        if (args.Action == "Highlight" && controlL.SelectedItem != selectedObject && selectedObject != null) { controlL.SelectedItem = selectedObject; }

                        break;


                }


               
            }
        }

        private void handleListboxChange()
        {
            if (controlL.IsLoaded && controlL.Visibility == Visibility.Visible)
            {
                if (controlL.SelectedItem != null)
                {
                    HotKeyTypes selected = (HotKeyTypes)controlL.SelectedItem;
                 
                        
                   
                    selectedObject = controlL.SelectedItem;
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

    class ucItem
    {
        public string ID { get; set; }
        public string DisplayName { get; set; }
    }
  
}
