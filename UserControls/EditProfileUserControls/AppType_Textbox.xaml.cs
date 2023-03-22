using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.TaskSchedulerWin32;
using Handheld_Control_Panel.Classes.UserControl_Management;
using Handheld_Control_Panel.Classes.Volume_Management;
using Handheld_Control_Panel.Styles;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    public partial class AppType_Textbox : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        public AppType_Textbox()
        {
            InitializeComponent();
            setControlValue();
          
        }

        private void setControlValue()
        {
            if(Global_Variables.profiles.editingProfile.AppType == "Steam")
            {
                controlToggle.Visibility = Visibility.Collapsed;
                controlTextbox.Visibility = Visibility.Collapsed;

                string imageDirectory = Properties.Settings.Default.directorySteam + "\\appcache\\librarycache\\" + Global_Variables.profiles.editingProfile.GameID + "_header";
                if (File.Exists(imageDirectory + ".jpg"))
                {
                    controlImage.Source = new BitmapImage(new Uri(imageDirectory + ".jpg", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    if (File.Exists(imageDirectory + ".png"))
                    {
                        controlImage.Source = new BitmapImage(new Uri(imageDirectory + ".png", UriKind.RelativeOrAbsolute));
                    }

                }
            }
            else
            {


            }

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
                //Classes.UserControl_Management.UserControl_Management.handleUserControl(border, control, args.Action);

            }
        }


      
      
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
           
        }
    }
}
