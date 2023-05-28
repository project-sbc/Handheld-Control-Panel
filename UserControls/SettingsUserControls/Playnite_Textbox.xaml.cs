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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;


namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class Playnite_Textbox : System.Windows.Controls.UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        public Playnite_Textbox()
        {
            InitializeComponent();
            setControlValue();
          
        }

        private void setControlValue()
        {
           controlTextbox.Text = Properties.Settings.Default.directoryPlaynite;
           

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
                        controlButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                        break;
                    default:
                        Classes.UserControl_Management.UserControl_Management.handleUserControl(border, controlButton, args.Action);
                        break;
                }
            }
        }

      


        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
           
        }

        private void controlButton_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.InitialDirectory = "C:\\";
                DialogResult result = fbd.ShowDialog();
             
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                   
                    Properties.Settings.Default.directoryPlaynite = fbd.SelectedPath;
                    Properties.Settings.Default.Save();
                  
                }
            }


        }

        private void control_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                Properties.Settings.Default.directoryPlaynite = controlTextbox.Text;
                Properties.Settings.Default.Save();
            }

        }
    }
}
