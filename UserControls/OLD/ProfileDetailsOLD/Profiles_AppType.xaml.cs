using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.UserControl_Management;
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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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
    public partial class Profiles_AppType : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";

        public Profiles_AppType()
        {
            InitializeComponent();
            //UserControl_Management.setupControl(control);
            control.Text = Global_Variables.profiles.editingProfile.Path;
            loadAppType();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");
            
        }

        private void loadAppType()
        {
            TextBlock textBox = (TextBlock)subText.Content;
            switch (Global_Variables.profiles.editingProfile.AppType)
            {
                case "Exe":
                    simpleIcon.Visibility = Visibility.Collapsed;
                    control.Text = Global_Variables.profiles.editingProfile.Path;
                    break;
                case "Steam":
                    materialIcon.Visibility = Visibility.Collapsed;
                    control.Visibility= Visibility.Collapsed;
                    button.Visibility = Visibility.Collapsed;
                    textBox.Text = "Steam: " + Global_Variables.profiles.editingProfile.GameID;

                    string imageDirectory = Properties.Settings.Default.directorySteam + "\\appcache\\librarycache\\" + Global_Variables.profiles.editingProfile.GameID + "_header";
                    if (File.Exists(imageDirectory + ".jpg"))
                    {
                        imageControl.Source = new BitmapImage(new Uri(imageDirectory + ".jpg", UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        if (File.Exists(imageDirectory + ".png"))
                        {
                            imageControl.Source = new BitmapImage(new Uri(imageDirectory + ".png", UriKind.RelativeOrAbsolute));
                        }

                    }
                    break;
                case "Epic":
                    materialIcon.Visibility = Visibility.Collapsed;
                    control.Visibility = Visibility.Collapsed;
                    button.Visibility = Visibility.Collapsed;
                    simpleIcon.Kind = MahApps.Metro.IconPacks.PackIconSimpleIconsKind.EpicGames;
                    textBox.Text = "Epic Games: " + Global_Variables.profiles.editingProfile.GameID;
                    break;

                default:
                    simpleIcon.Visibility = Visibility.Collapsed;
                    break;

            }
        }

        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {
                if (args.Action == "Y" && control.Visibility == Visibility.Visible)
                {
                    button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
                else
                {
                    Classes.UserControl_Management.UserControl_Management.handleUserControl(border, control, args.Action);
                }

               
            }
        }

        private void control_LostFocus(object sender, RoutedEventArgs e)
        {
            if(Global_Variables.profiles.editingProfile != null )
            {
                Global_Variables.profiles.editingProfile.Path = control.Text;
                if (control.Text == "")
                {
                    Global_Variables.profiles.editingProfile.AppType = "";
                }
                else
                {
                    Global_Variables.profiles.editingProfile.AppType = "Exe";
                }
            }
           
        }

    


        private void button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.InitialDirectory = "C:\\";
            dialog.Filter = "Applications (.exe)|*.exe"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                control.Text = dialog.FileName;
                Global_Variables.profiles.editingProfile.Path= control.Text;
                Global_Variables.profiles.editingProfile.AppType= "Exe";
           
            }

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
        }
    }
}
