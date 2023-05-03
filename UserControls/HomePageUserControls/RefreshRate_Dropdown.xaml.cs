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
            //error number RRDD01
            try
            {
                Log_Writer.writeLog("current res: " + Global_Variables.resolution);
                Log_Writer.writeLog("# dictionary items: " + Global_Variables.resolution_refreshrates.Count.ToString());
                List<string> refreshrates = Global_Variables.resolution_refreshrates[Global_Variables.Resolution.Trim()];
                controlList.ItemsSource = refreshrates;
                controlList.SelectedItem = Global_Variables.RefreshRate;
                selectedObject = Global_Variables.RefreshRate;
                controlList.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Log_Writer.writeLog(usercontrol + "; " + ex.Message, "RRDD01");

            }
           

           
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
            //error number RRDD02
            try
            {

                valueChangedEventArgs valueChangedEventArgs = e as valueChangedEventArgs;
                if (valueChangedEventArgs.Parameter == "Resolution")
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (Global_Variables.resolution_refreshrates.ContainsKey(Global_Variables.Resolution.Trim()))
                        {
                            controlList.ItemsSource = Global_Variables.resolution_refreshrates[Global_Variables.Resolution];
                            if (controlList.Items.Contains(Global_Variables.RefreshRate) && controlList.SelectedItem != Global_Variables.RefreshRate && controlList.IsLoaded)
                            {
                                controlList.SelectedItem = Global_Variables.RefreshRate;

                            }
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
            catch (Exception ex)
            {
                Log_Writer.writeLog(usercontrol + "; " + ex.Message, "RRDD02");

            }

           
        }

        private void handleControllerInputs(object sender, EventArgs e)
        {
            //error number RRDD03
            try
            {
                controllerUserControlInputEventArgs args = (controllerUserControlInputEventArgs)e;
                if (args.WindowPage == windowpage && args.UserControl == usercontrol)
                {
                    switch (args.Action)
                    {
                        case "A":
                            if (controlList.SelectedItem.ToString() != Global_Variables.refreshRate)
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
            catch (Exception ex)
            {
                Log_Writer.writeLog(usercontrol + "; " + ex.Message, "RRDD03");

            }

           
        }
        private void handleListboxChange()
        {
            //error number RRDD04
            try
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
            catch (Exception ex)
            {
                Log_Writer.writeLog(usercontrol + "; " + ex.Message, "RRDD04");

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
