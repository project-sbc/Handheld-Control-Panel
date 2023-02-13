using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
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
    public partial class Profiles_Exe : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";

        public Profiles_Exe()
        {
            InitializeComponent();
            //UserControl_Management.setupControl(control);
            controlText.Text = Global_Variables.profiles.editingProfile.Exe;
            loadListbox();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");
            
        }

        private void loadListbox()
        {
            List<string> listProcesses = new List<string>();

            Process[] pList = Process.GetProcesses();
            foreach (Process p in pList)
            {
                if (p.MainWindowHandle != IntPtr.Zero)
                {
                    if (!listProcesses.Contains(p.ProcessName))
                    {
                        listProcesses.Add(p.ProcessName);
                    }
                }
            }

            control.ItemsSource = listProcesses;
        }

        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {
                if (args.Action == "Y")
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
            if(Global_Variables.profiles.editingProfile != null && controlText.Text != "")
            {
                Global_Variables.profiles.editingProfile.Exe = controlText.Text;
            }
           
        }

        private void handleListboxChange()
        {
            if (this.IsLoaded)
            {
                if (control.SelectedItem != null)
                {
                    controlText.Text = control.SelectedItem.ToString();
                    Global_Variables.profiles.editingProfile.Exe = controlText.Text;
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
                string exe = dialog.SafeFileName;
                if (exe.Length > 4)
                {
                    controlText.Text = exe.Substring(0, exe.Length - 4);
                }
            }

        }
    }
}
