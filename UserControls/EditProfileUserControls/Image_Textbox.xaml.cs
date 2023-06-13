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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class Image_Textbox : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        public Image_Textbox()
        {
            InitializeComponent();
            setControlValue();
          
        }

        private void setControlValue()
        {
            if (Global_Variables.profiles.editingProfile.profile_Exe.Exe_Image_Path != null)
            {
                if (File.Exists(Global_Variables.profiles.editingProfile.profile_Exe.Exe_Image_Path))
                {
                    updateImage();
                    control.IsOn = true;
                }
                else
                {
                    control.IsOn = false;
                }
            }
            else { control.IsOn = false; }
          

          

           

        }
      

        private void updateImage()
        {
            string imageDirectory = Global_Variables.profiles.editingProfile.profile_Exe.Exe_Image_Path;
            if (File.Exists(imageDirectory))
            {
                controlImage.Source = new BitmapImage(new Uri(imageDirectory, UriKind.RelativeOrAbsolute));
                controlTextbox.Text = Global_Variables.profiles.editingProfile.profile_Exe.Exe_Image_Path;
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
                switch(args.Action)
                {
                    case "A":
                        if (control.Visibility == Visibility.Visible)
                        {
                            control.IsOn = !control.IsOn;
                        }
                        break;
                    default:
                        Classes.UserControl_Management.UserControl_Management.handleUserControl(border, control, args.Action);
                        break;
                }
            }
        }

        private void toggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (control.IsLoaded)
            {
                if (control.IsOn)
                {
                    controlTextbox.Visibility = Visibility.Visible;
                    controlButton.Visibility = Visibility.Visible;
                    controlImage.Visibility = Visibility.Visible;
                   


                }
                else
                {
                    controlTextbox.Text = "";
                    controlTextbox.Visibility = Visibility.Collapsed;
                    controlButton.Visibility = Visibility.Collapsed;
                    controlImage.Visibility = Visibility.Collapsed;

               
                }
            }

        }


        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
           
        }

        private void controlButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.InitialDirectory = "C:\\";
            dialog.Filter = "Applications (.exe)|*.exe"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                controlTextbox.Text = dialog.FileName;
                Global_Variables.profiles.editingProfile.profile_Exe.Exe_Path = controlTextbox.Text;
                Global_Variables.profiles.editingProfile.profile_Exe.Exe_Type = "Exe";

            }
        }

        private void control_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Global_Variables.profiles.editingProfile.profile_Exe.Exe_Type == "")
            {
                Global_Variables.profiles.editingProfile.profile_Exe.Exe_Type = "Exe";
            }
            Global_Variables.profiles.editingProfile.profile_Exe.Exe_Path = controlTextbox.Text;
        }
    }
}
