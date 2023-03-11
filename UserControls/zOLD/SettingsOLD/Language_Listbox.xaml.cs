using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
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

namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class Language_Listbox : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private object selectedObject;
        public ReadOnlyObservableCollection<Theme> themes = ThemeManager.Current.Themes;

        public Language_Listbox()
        {
            InitializeComponent();
            //UserControl_Management.setupControl(control);
            setLanguageItems();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");
           
            if (Window.GetWindow(this).ActualWidth < 650) { subText.Visibility = Visibility.Collapsed; }
        }

        private void setLanguageItems()
        {
            control.Items.Add("English");
            control.Items.Add("中文");
            control.Items.Add("Pусский");
            
            control.SelectedItem = Properties.Settings.Default.language;

        }

      

        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {
                if (args.Action == "A")
                {
                    handleListboxChange();
                }
                else
                {
                    Classes.UserControl_Management.UserControl_Management.handleUserControl(border, control, args.Action);
                    if (args.Action == "Highlight" && control.SelectedItem != selectedObject && selectedObject != null) { control.SelectedItem = selectedObject; }
                }
               
            }
        }

        private void handleListboxChange()
        {
            if (control.IsLoaded)
            {
               if (control.SelectedItem != null)
                {
                    string selectedItem = control.SelectedValue.ToString();

                    Properties.Settings.Default.language = selectedItem;
                    Properties.Settings.Default.Save();
                    selectedObject = selectedItem;
                    System.Windows.Application.Current.Resources.MergedDictionaries.Remove(Global_Variables.languageDict);
                    switch (selectedItem)
                    {
                        default:
                        case "English":
                            Global_Variables.languageDict.Source = new Uri("StringResources/StringResources.xaml", UriKind.RelativeOrAbsolute);
                            break;
                        case "中文":
                            Global_Variables.languageDict.Source = new Uri("StringResources/StringResources.zh-Hans.xaml", UriKind.RelativeOrAbsolute);
                            break;
                        case "Pусский":
                            Global_Variables.languageDict.Source = new Uri("StringResources/StringResources.ru.xaml", UriKind.RelativeOrAbsolute);
                            break;


                    }
                    System.Windows.Application.Current.Resources.MergedDictionaries.Add(Global_Variables.languageDict);
            
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

  
}
