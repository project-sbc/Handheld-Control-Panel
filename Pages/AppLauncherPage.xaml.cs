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
using Handheld_Control_Panel.Classes.XML_Management;
using System.Drawing;
using System.Windows.Interop;
using System.IO;
using MahApps.Metro.IconPacks;
using System.Windows.Threading;
using Handheld_Control_Panel.Classes.Task_Scheduler;


namespace Handheld_Control_Panel.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    /// 
    public class SortMethods
    {
        public string DisplaySortMethod { get; set; }
        public string SortMethod { get; set; }
    }

    public partial class AppLauncherPage : Page
    {

        private static PackIconFontAwesome packIconFontAwesome;
        private static DispatcherTimer spinStopTimer = new DispatcherTimer();

        private string windowpage;
        private List<ListBoxAppItem> items = new List<ListBoxAppItem>();

        private string currentSortMethod = Properties.Settings.Default.appSortMethod;

        private List<SortMethods> sortMethods = new List<SortMethods>();

        public AppLauncherPage()
        {
            InitializeComponent();
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.SystemTheme + "." + Properties.Settings.Default.systemAccent);

            MainWindow wnd = (MainWindow)Application.Current.MainWindow;
            wnd.changeUserInstruction("AppLauncherPage_Instruction");
            wnd = null;



            SortMethods sm = new SortMethods();
            sm.SortMethod = "App Type";
            sm.DisplaySortMethod = Application.Current.Resources["Sort_Method_AppType"].ToString();
            sortMethods.Add(sm);

            SortMethods sm1 = new SortMethods();
            sm1.SortMethod = "Profile Name";
            sm1.DisplaySortMethod = Application.Current.Resources["Sort_Method_ProfileName"].ToString();
            sortMethods.Add(sm1);

            SortMethods sm2 = new SortMethods();
            sm2.SortMethod = "Frequently Launched";
            sm2.DisplaySortMethod = Application.Current.Resources["Sort_Method_FrequentlyLaunched"].ToString();
            sortMethods.Add(sm2);

            SortMethods sm3 = new SortMethods();
            sm3.SortMethod = "Recently Launched";
            sm3.DisplaySortMethod = Application.Current.Resources["Sort_Method_RecentlyLaunched"].ToString();
            sortMethods.Add(sm3);
        }
      
      
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //get unique window page combo from getwindow to string
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            //subscribe to controller input events
            Controller_Window_Page_UserControl_Events.pageControllerInput += handleControllerInputs;


            loadValues();

         

        }
        private void loadValues()
        {
            // add panels to wrap panel
            if (items.Count > 0)
            {
                items.Clear();
            }


            foreach (Profile profile in Global_Variables.profiles)
            {
                if (profile.AppType != "")
                {
                    ListBoxAppItem lbai = new ListBoxAppItem();
                    lbai.ID = profile.ID;
                    lbai.ProfileName = profile.ProfileName;
                    lbai.programType = profile.AppType;
                    lbai.LastLaunched = profile.LastLaunched;
                    lbai.NumberLaunches = profile.NumberLaunches;
                    switch (profile.AppType)
                    {
                        case "Steam":
                            string imageDirectory = Properties.Settings.Default.directorySteam + "\\appcache\\librarycache\\" + profile.GameID + "_header";
                            if (File.Exists(imageDirectory + ".jpg"))
                            {
                                lbai.imageSteam = new BitmapImage(new Uri(imageDirectory + ".jpg", UriKind.RelativeOrAbsolute));
                            }
                            else
                            {
                                if (File.Exists(imageDirectory + ".png"))
                                {
                                    lbai.imageSteam = new BitmapImage(new Uri(imageDirectory + ".png", UriKind.RelativeOrAbsolute));
                                }

                            }
                            if (lbai.imageSteam != null)
                            {
                                items.Add(lbai);
                            }

                            break;
                        default:
                            if (profile.Path != null)
                            {

                                if (File.Exists(profile.Path))
                                {
                                    lbai.Exe = profile.Exe;
                                    using (Icon ico = Icon.ExtractAssociatedIcon(profile.Path))
                                    {
                                        lbai.image = Imaging.CreateBitmapSourceFromHIcon(ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                                    }
                                    items.Add(lbai);
                                }
                            }


                            break;




                    }



                }



            }
            switch(currentSortMethod)
            {
                case "App Type":
                    controlList.ItemsSource = items.OrderBy(o => o.programType).ToList();
                    break;
                case "Recently Launched":
                    items = items.OrderBy(o => o.NumberLaunches).ToList();
                    items.Reverse();
                    controlList.ItemsSource = items;
         
                    break;
                case "Frequently Launched":
                    items = items.OrderBy(o => o.LastLaunched).ToList();
                    items.Reverse();
                    controlList.ItemsSource = items;
                    break;
                case "Profile Name":
                    controlList.ItemsSource = items.OrderBy(o => o.ProfileName).ToList();
                    break;
                default:
                    controlList.ItemsSource = items;
                    break;

            }
            controlList.Items.Refresh();
        }

        private void spinner_Stop_Tick(object sender, EventArgs e)
        {
            spinStopTimer.Stop();
            if (controlList.SelectedItem != null)
            {
               packIconFontAwesome.Visibility = Visibility.Hidden;
               packIconFontAwesome.Spin = false;

            }
         
        }


        private childItem FindVisualChild<childItem>(DependencyObject obj)
where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
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
                    ListBoxAppItem lbai = controlList.SelectedItem as ListBoxAppItem;
                    int index = controlList.SelectedIndex;
                    switch (action)
                    {
                        case "A":
                            if (controlList.SelectedItem != null)
                            {
                              
                                if (packIconFontAwesome != null)
                                {
                                    packIconFontAwesome.Spin = false;
                                    packIconFontAwesome.Visibility = Visibility.Collapsed;
                                }

                            

                                // IsSynchronizedWithCurrentItem set to True for this to work
                                ListBoxItem myListBoxItem =
                                    (ListBoxItem)(controlList.ItemContainerGenerator.ContainerFromItem(controlList.SelectedItem));

                                // Getting the ContentPresenter of myListBoxItem
                                ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListBoxItem);

                                // Finding textBlock from the DataTemplate that is set on that ContentPresenter
                                DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;

                                Global_Variables.profiles.openProgram(lbai.ID);

                                packIconFontAwesome = (PackIconFontAwesome)myDataTemplate.FindName("fontAwesomeIcon", myContentPresenter);
                                if (packIconFontAwesome != null)
                                {
                                    packIconFontAwesome.Spin = true;
                                    packIconFontAwesome.Visibility = Visibility.Visible;
                                    if (lbai.imageSteam != null)
                                    {
                                        spinStopTimer.Interval = new TimeSpan(0, 0, 15);
                                    }
                                    else
                                    {
                                        spinStopTimer.Interval = new TimeSpan(0, 0, 5);
                                    }

                                    spinStopTimer.Tick += spinner_Stop_Tick;

                                    spinStopTimer.Start();


                                }
                                


                            }
                            break;

                        case "X":
                            changeSortMethod();
                            break;
                        case "Up":
                            if (index > 2) { controlList.SelectedIndex = index - 3; controlList.ScrollIntoView(controlList.SelectedItem); }
                            break;
                        case "Down":
                            if (index +3  < controlList.Items.Count - 1) { controlList.SelectedIndex = index + 3; controlList.ScrollIntoView(controlList.SelectedItem); }
                            break;
                        case "Left":
                            if (index > 0) { controlList.SelectedIndex = index - 1; controlList.ScrollIntoView(controlList.SelectedItem); }
                            break;
                        case "Right":
                            if (index < controlList.Items.Count - 1) { controlList.SelectedIndex = index + 1; controlList.ScrollIntoView(controlList.SelectedItem); }
                            break;
                        default: break;

                    }

                }
                else
                {
                    controlList.SelectedIndex = 0;
                    if (args.Action == "X")
                    {
                            changeSortMethod();
                    }
                }
              

            }

        }

        private void changeSortMethod()
        {
            foreach(SortMethods sm in sortMethods) 
            {
               if (currentSortMethod == sm.SortMethod)
                {
                    int index = sortMethods.IndexOf(sm);
                    if (index == sortMethods.Count - 1)
                    {
                        currentSortMethod = sortMethods[0].SortMethod;

                    }
                    else
                    {
                        currentSortMethod = sortMethods[index + 1].SortMethod;
                    }
                    loadValues();
                    break;
                }
            }

        }

      

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.pageControllerInput -= handleControllerInputs;
        }
    }
    public class ListBoxAppItem
    {
        public string ID { get; set; }
        public string ProfileName { get; set; }
        public ImageSource image { get; set; }
        public ImageSource imageSteam { get; set; }
        public int NumberLaunches { get; set; }
        public int LastLaunched { get; set; }
        public string programType { get; set; }
        public string Exe { get; set; }
  
    }
}
