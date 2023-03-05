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
using Windows.Devices.Radios;

namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class RefreshRate_Dropdown : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private object selectedObject = "";
        public RefreshRate_Dropdown()
        {
            InitializeComponent();
            setControlValue();
          
        }

        private  void setControlValue()
        {
          
            controlList.ItemsSource = Global_Variables.resolution_refreshrates[Global_Variables.Resolution];
            controlList.SelectedItem = Global_Variables.RefreshRate;
            selectedObject = Global_Variables.RefreshRate;
            controlList.Visibility = Visibility.Collapsed;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");
            Global_Variables.valueChanged += Global_Variables_valueChanged;
        }

        private void Global_Variables_valueChanged(object? sender, valueChangedEventArgs e)
        {
            valueChangedEventArgs valueChangedEventArgs = e as valueChangedEventArgs;
            if (valueChangedEventArgs.Parameter == "Resolution")
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    controlList.ItemsSource = Global_Variables.resolution_refreshrates[Global_Variables.Resolution];
                    if (controlList.Items.Contains(Global_Variables.RefreshRate) && controlList.SelectedItem != Global_Variables.RefreshRate && controlList.IsLoaded)
                    {
                        controlList.SelectedItem = Global_Variables.RefreshRate;
                    
                    }


                }));
               
             
            }
            if (valueChangedEventArgs.Parameter == "RefreshRate")
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (controlList.Items.Contains(Global_Variables.RefreshRate) && controlList.SelectedItem != Global_Variables.RefreshRate && controlList.IsLoaded)
                    {
                        controlList.SelectedItem = Global_Variables.Resolution;
                     
                    }


                }));


            }
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
                        wnd2.changeUserInstruction("HomePage_Instruction");
                        wnd2 = null;
                        if (controlList.Visibility == Visibility.Visible)
                        {
                            

                            if (selectedObject != null)
                            {
                                controlList.SelectedItem = selectedObject;
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
                    string refreshrate = controlList.SelectedItem.ToString();
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => Display_Management.SetDisplayRefreshRate(refreshrate));
                    selectedObject = controlList.SelectedItem;
                 
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
            Global_Variables.valueChanged -= Global_Variables_valueChanged;
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
}
