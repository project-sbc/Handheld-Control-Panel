using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.UserControls;
using Microsoft.Win32.TaskScheduler;
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
using MahApps.Metro;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlzEx.Theming;
using System.Windows.Controls.Primitives;
using Handheld_Control_Panel.Classes.Global_Variables;

namespace Handheld_Control_Panel.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class ActionPage : Page
    {
        private string windowpage;

        public ActionPage()
        {
            InitializeComponent();
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.SystemTheme + "." + Properties.Settings.Default.systemAccent);

            MainWindow wnd = (MainWindow)Application.Current.MainWindow;
            wnd.changeUserInstruction("ActionPage_Instruction");
            wnd = null;
          
        }
      
      
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //get unique window page combo from getwindow to string
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            //subscribe to controller input events
            Controller_Window_Page_UserControl_Events.pageControllerInput += handleControllerInputs;


            controlList.ItemsSource = Global_Variables.hotKeys;

            if (controlList.Items.Count > 0)
            {
                if (Global_Variables.hotKeys.editingHotkey != null)
                {
                    controlList.SelectedItem = Global_Variables.hotKeys.editingHotkey;
                }
                else
                {
                    controlList.SelectedIndex = 0;
                }
            }

        }

        private void handleControllerInputs(object sender, EventArgs e)
        {
            //get action from custom event args for controller
            controllerPageInputEventArgs args = (controllerPageInputEventArgs)e;
            string action = args.Action;

            if (args.WindowPage == windowpage)
            {
                //global method handles the event tracking and returns what the index of the highlighted and selected usercontrolshould be
                if (controlList.SelectedItem != null)
                {
                    HotkeyItem hotkey = controlList.SelectedItem as HotkeyItem;
                    int index = controlList.SelectedIndex;
                    switch (action)
                    {
                        case "A":
                            Global_Variables.hotKeys.editingHotkey = (HotkeyItem)controlList.SelectedItem;
                            MainWindow wnd = (MainWindow)Application.Current.MainWindow;
                            wnd.navigateFrame("ActionEditPage");
                            break;

                        case "Start":
                            Global_Variables.hotKeys.addNewHotkey();
                            controlList.Items.Refresh();
                            break;
                        case "Delete_Action":

                            Global_Variables.hotKeys.deleteHotkey(hotkey);
                            controlList.Items.Refresh();
                            Global_Variables.hotKeys.generateGlobalControllerHotKeyList();
                            Global_Variables.hotKeys.generateGlobalKeyboardHotKeyList();
                            if (controlList.Items.Count > 0) { if (index > 0) { controlList.SelectedIndex = index - 1; } else { controlList.SelectedIndex = 0; } };
                            break;
                        case "X":
                            Notification_Management.ShowYesNoPrompt(Application.Current.Resources["Prompt_DeleteSelectedAction"].ToString(), Notification.Wpf.NotificationType.Warning, "Delete_Action");
                            break;
                        case "Up":
                            if (index > 0) { controlList.SelectedIndex = index - 1; } else { controlList.SelectedIndex = controlList.Items.Count - 1; }
                            controlList.ScrollIntoView(controlList.SelectedItem);
                            break;
                        case "Down":
                            if (index < controlList.Items.Count - 1) { controlList.SelectedIndex = index + 1; } else { controlList.SelectedIndex = 0; }
                            controlList.ScrollIntoView(controlList.SelectedItem);
                            break;
                        default: break;

                    }

                }
                else
                {
                    if (action == "Y")
                    {
                        Global_Variables.hotKeys.addNewHotkey();
                        controlList.Items.Refresh();
                    }
                    if (action == "Up" || action == "Down")
                    {
                        if (controlList.Items.Count > 0) { controlList.SelectedIndex = 0; controlList.ScrollIntoView(controlList.SelectedItem); }
                    
                    }
                }


            }

        }

     

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.pageControllerInput -= handleControllerInputs;
        }
    }
}
