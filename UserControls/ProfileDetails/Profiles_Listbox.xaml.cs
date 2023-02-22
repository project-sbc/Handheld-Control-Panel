using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Display_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.UserControl_Management;
using Handheld_Control_Panel.Classes.XML_Management;
using Handheld_Control_Panel.Styles;
using MahApps.Metro.Controls;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    public partial class Profiles_Listbox : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        private object selectedObject;

        
        public Profiles_Listbox()
        {
            InitializeComponent();
            //UserControl_Management.setupControl(control);
            //setListboxItemsource();
        }

    
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");
           
            //if (Window.GetWindow(this).ActualWidth < 650) { subText.Visibility = Visibility.Collapsed; }

            setListboxItemsource();
        }

        private void setListboxItemsource(string ID = "")
        {
            control.ItemsSource = null;
            control.ItemsSource = Global_Variables.profiles;
            
            if (Global_Variables.profiles.editingProfile != null) { control.SelectedItem = Global_Variables.profiles.editingProfile; }
        }

     
        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {

                switch(args.Action)
                {
                    case "A":
                        handleListboxChange();
                        break;
                    case "Y":
                        AddProfile.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                    case "X":
                        DeleteProfile.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                    default:
                        Classes.UserControl_Management.UserControl_Management.handleUserControl(border, control, args.Action);
                        if (args.Action == "Highlight" && control.SelectedItem != selectedObject && selectedObject != null) { control.SelectedItem = selectedObject; }
                        break;
                }
               
               
            }
        }

        private void handleListboxChange()
        {
            if (control.IsLoaded)
            {
                if (control.SelectedItem!= null)
                {
                    Global_Variables.profiles.editingProfile = (Profile)control.SelectedItem;

                    MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
                    mainWindow.frame.Navigate(new Uri("Pages\\ProfileDetailsPage.xaml", UriKind.RelativeOrAbsolute));
                }
            }

        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            switch(button.Name)
            {
                case "EditProfile":
                    handleListboxChange();
                    break;
                case "AddProfile":
                    Global_Variables.profiles.addNewProfile();
                    setListboxItemsource();
                    control.SelectedIndex = control.Items.Count - 1;
                    break;
                case "DeleteProfile":
                    if (control.SelectedItem != null)
                    {
                        int index = control.SelectedIndex;
                        Global_Variables.profiles.deleteProfile((Profile)control.SelectedItem);
                        setListboxItemsource();
                        if (control.Items.Count > 0) 
                        { 
                            if (control.Items.Count-1 > index -1 )
                            {
                                control.SelectedIndex = index;
                            }
                            else
                            {
                                control.SelectedIndex = index-1;
                            }
                             
                        } 
                        
                    }
             
                    break;
                default: break;
            }
        }

        private void control_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            handleListboxChange();

        }
    }

  
}
