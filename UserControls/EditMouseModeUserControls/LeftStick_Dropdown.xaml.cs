using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Display_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.TaskSchedulerWin32;
using Handheld_Control_Panel.Classes.UserControl_Management;
using Handheld_Control_Panel.Styles;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
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
using static Vanara.Interop.KnownShellItemPropertyKeys;

namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class LeftStick_Dropdown : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private MouseModeAction selectedObject;
        public LeftStick_Dropdown()
        {
            InitializeComponent();
            setListboxItemsource();
          
        }

        private void setListboxItemsource()
        {
            List<MouseModeAction> mouseModeActions = new List<MouseModeAction>();


            MouseModeAction mouseInput = new MouseModeAction();
            mouseInput.DisplayAction = Application.Current.Resources["MouseMode_Action_MouseInput"].ToString();
            mouseInput.Action = "Mouse";

            mouseModeActions.Add(mouseInput);

            MouseModeAction scrollInput = new MouseModeAction();
            scrollInput.DisplayAction = Application.Current.Resources["MouseMode_Action_Scroll"].ToString();
            scrollInput.Action = "Scroll";

            mouseModeActions.Add(scrollInput);

            MouseModeAction wasd = new MouseModeAction();
            wasd.DisplayAction = "WASD";
            wasd.Action = "WASD";

            mouseModeActions.Add(wasd);

            MouseModeAction arrowKeys = new MouseModeAction();
            arrowKeys.DisplayAction = Application.Current.Resources["MouseMode_Action_ArrowKeys"].ToString();
            arrowKeys.Action = "ArrowKeys";

            mouseModeActions.Add(arrowKeys);


            controlList.ItemsSource = mouseModeActions;

            foreach (MouseModeAction mma in mouseModeActions)
            {
                if (Global_Variables.mousemodes.editingMouseMode.mouseMode["LeftThumb"] == mma.Action)
                {
                    controlList.SelectedItem = mma;
                    actionLabel.Content = mma.DisplayAction;
                }
            }


        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");
            controlList.Visibility = Visibility.Collapsed;
        }
              

        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {
                switch(args.Action)
                {
                    case "A":
                        if (controlList.Visibility == Visibility.Visible)
                        {
                            handleListboxChange();
                        }
                        else
                        {
                            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

                            MainWindow wnd = (MainWindow)Application.Current.MainWindow;
                            wnd.changeUserInstruction("SelectedListBox_Instruction");
                            wnd = null;
                         
                        }

                        break;
                    case "B":
                        MainWindow wnd2 = (MainWindow)Application.Current.MainWindow;
                        wnd2.changeUserInstruction("MouseModeEditPage_Instruction");
                        wnd2 = null;
                        if (controlList.Visibility == Visibility.Visible)
                        {
                            

                            if (selectedObject != null)
                            {
                                
                                controlList.SelectedItem = selectedObject;
                                actionLabel.Content = selectedObject.DisplayAction;
                            }
                            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

                        }
  
                        break;
                    default:
                        Classes.UserControl_Management.UserControl_Management.handleUserControl(border, controlList, args.Action);

                        break;
                }

            }
        }
        private void handleListboxChange()
        {
            if (controlList.IsLoaded)
            {
                if (controlList.SelectedItem != null)
                {
                    MouseModeAction mma = (MouseModeAction)controlList.SelectedItem;
                
                    selectedObject = (MouseModeAction)controlList.SelectedItem;
                    if (Global_Variables.mousemodes.editingMouseMode.mouseMode["LeftThumb"] != mma.Action)
                    {
                        Global_Variables.mousemodes.editingMouseMode.mouseMode["LeftThumb"] = mma.Action;
                        actionLabel.Content = mma.DisplayAction;
               
                    }
                    if (controlList.Visibility == Visibility.Visible)
                    {
                        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }



                }

            }

        }


        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (controlList.Visibility == Visibility.Visible)
            {
                controlList.Visibility = Visibility.Collapsed;
                PackIconUnicons icon = (PackIconUnicons)button.Content;
                icon.RotationAngle = 0;
            }
            else
            {
                controlList.Visibility = Visibility.Visible;
                PackIconUnicons icon = (PackIconUnicons)button.Content;
                icon.RotationAngle = 90;
            }
        }


        private void controlList_TouchUp(object sender, TouchEventArgs e)
        {
            handleListboxChange();
        }

        private void controlList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            handleListboxChange();
        }
    }
    public class MouseModeAction
    {
        public string DisplayAction { get; set; }
        public string Action { get; set; }
    }
}
