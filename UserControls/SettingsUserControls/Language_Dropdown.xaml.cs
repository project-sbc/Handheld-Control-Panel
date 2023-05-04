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
using System.Collections.ObjectModel;
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
    public partial class Language_Dropdown : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private object selectedObject = "";
        
        public Language_Dropdown()
        {
            InitializeComponent();
            setControlValue();
          
        }

        private  void setControlValue()
        {
            controlList.Items.Add("English");
            controlList.Items.Add("简体中文");
            controlList.Items.Add("Pусский");
            controlList.Items.Add("日本語");
            controlList.Items.Add("Português (Brasil)");
            controlList.Items.Add("한국어");
            controlList.SelectedItem = Properties.Settings.Default.language;


            controlList.Visibility = Visibility.Collapsed;
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
                switch(args.Action)
                {
                    case "A":
                        if (controlList.SelectedItem.ToString() != Properties.Settings.Default.language)
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
                    string selectedItem = controlList.SelectedValue.ToString();

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
                        case "简体中文":
                            Global_Variables.languageDict.Source = new Uri("StringResources/StringResources.zh-Hans.xaml", UriKind.RelativeOrAbsolute);
                            break;
                        case "Pусский":
                            Global_Variables.languageDict.Source = new Uri("StringResources/StringResources.ru.xaml", UriKind.RelativeOrAbsolute);
                            break;
                        case "日本語":
                            Global_Variables.languageDict.Source = new Uri("StringResources/StringResources.jp.xaml", UriKind.RelativeOrAbsolute);
                            break;
                        case "Português (Brasil)":
                            Global_Variables.languageDict.Source = new Uri("StringResources/StringResources.pt-br.xaml", UriKind.RelativeOrAbsolute);
                            break;
                        case "한국어":
                            Global_Variables.languageDict.Source = new Uri("StringResources/StringResources.kr.xaml", UriKind.RelativeOrAbsolute);
                            break;
                    }
                   
                    System.Windows.Application.Current.Resources.MergedDictionaries.Add(Global_Variables.languageDict);
                    Global_Variables.homePageItems.populateList();
                    Global_Variables.hotKeys.updateLanguage();
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
}
