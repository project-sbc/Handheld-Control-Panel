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
using MahApps.Metro.Controls;


namespace Handheld_Control_Panel.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class CustomizeHomePage : Page
    {
        private string windowpage;
        private List<UserControl> userControls = new List<UserControl>();

        private HomePageItem selectedItem;

        
        public CustomizeHomePage()
        {
            InitializeComponent();
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.SystemTheme + "." + Properties.Settings.Default.systemAccent);


            MainWindow wnd = (MainWindow)Application.Current.MainWindow;
            wnd.changeUserInstruction("CustomizeHomePage_Instruction");
            wnd = null;

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //get unique window page combo from getwindow to string
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            //subscribe to controller input events
            Controller_Window_Page_UserControl_Events.pageControllerInput += handleControllerInputs;
          

            controlList.ItemsSource = Global_Variables.homePageItems;

            if (controlList.Items.Count > 0) { controlList.SelectedIndex = 0; }

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
                    HomePageItem hpi = controlList.SelectedItem as HomePageItem;
                    int index = controlList.SelectedIndex;
                    switch (action)
                    {
                        case "A":
                            hpi.Enabled = !hpi.Enabled;
                            controlList.Items.Refresh();
                            break;

                        case "X":
                            if (hpi.updownVisibility == Visibility.Visible)
                            {
                                hpi.updownVisibility = Visibility.Collapsed;
                                hpi.enableMovementVisibility = Visibility.Visible;
                            }
                            else
                            {
                                hpi.updownVisibility = Visibility.Visible;
                                hpi.enableMovementVisibility = Visibility.Collapsed;
                            }
                            controlList.Items.Refresh();
                            break;

                        case "Up":
                            if (hpi.updownVisibility ==Visibility.Visible)
                            {
                                moveHomePageItem(hpi, index, true);
                            }
                            else
                            {
                                if (index > 0) 
                                { 
                                    controlList.SelectedIndex = index - 1;

                                    controlList.ScrollIntoView(controlList.SelectedItem);
                                }
                                else
                                {
                                    //if at top of list, go to bottom
                                    controlList.SelectedIndex = controlList.Items.Count -1;
                                    controlList.ScrollIntoView(controlList.SelectedItem);
                                }
                            }
                            break;
                        case "Down":
                            if (hpi.updownVisibility == Visibility.Visible)
                            {
                                moveHomePageItem(hpi, index, false);
                            }
                            else
                            {
                                if (index < controlList.Items.Count -1) 
                                { 
                                    controlList.SelectedIndex = index + 1;
                            

                                    controlList.ScrollIntoView(controlList.SelectedItem);

                                }
                                else
                                {
                                    controlList.SelectedIndex =0;


                                    controlList.ScrollIntoView(controlList.SelectedItem);
                                }
                            }
                            break;


                    }

                }
              
   
            }

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.pageControllerInput -= handleControllerInputs;
            Global_Variables.homePageItems.saveList();
        }

        private void moveHomePageItem(HomePageItem hpi, int index, bool up)
        {
            if (up)
            {
                if (index > 0)
                {
                    Global_Variables.homePageItems.Remove(hpi);
                    Global_Variables.homePageItems.Insert(index - 1, hpi);
                    controlList.Items.Refresh();
                }
                else
                {
                    Global_Variables.homePageItems.Remove(hpi);
                    Global_Variables.homePageItems.Insert(Global_Variables.homePageItems.Count, hpi);
                    controlList.Items.Refresh();
                }
            }
            else
            {
                if (index < controlList.Items.Count - 1)
                {
                    Global_Variables.homePageItems.Remove(hpi);
                    Global_Variables.homePageItems.Insert(index+1, hpi);
                    controlList.Items.Refresh();
                }
                else
                {
                    Global_Variables.homePageItems.Remove(hpi);
                    Global_Variables.homePageItems.Insert(0, hpi);
                    controlList.Items.Refresh();
                }
            }
            controlList.ScrollIntoView(controlList.SelectedItem);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string buttonTag = button.Tag.ToString();

            if (controlList.SelectedItem != null && buttonTag != "")
            {
                HomePageItem hpi = controlList.SelectedItem as HomePageItem;

                if (buttonTag.Contains(hpi.UserControl))
                {
                    int index = controlList.SelectedIndex;
                    if (buttonTag.Contains("_Up"))
                    {
                        moveHomePageItem(hpi, index, true);
                    }
                    if (buttonTag.Contains("_Down"))
                    {
                        moveHomePageItem(hpi, index, false);
                    }
                    if (buttonTag.Contains("_EnableMovement"))
                    {
                        hpi.updownVisibility = Visibility.Visible;
                        hpi.enableMovementVisibility = Visibility.Collapsed;
                        controlList.Items.Refresh();

                    }
                }
               
                
            }
        }

        private void controlList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (controlList.SelectedItem != null)
            {
                HomePageItem newHomePageItem = controlList.SelectedItem as HomePageItem;
                if (newHomePageItem != selectedItem)
                {
                    if (selectedItem !=null)
                    {
                        selectedItem.updownVisibility = Visibility.Collapsed;
                        selectedItem.enableMovementVisibility = Visibility.Visible;
                        controlList.Items.Refresh();
                    }
                    selectedItem = newHomePageItem;
                    
                }
            }

          
        }
    }
}
