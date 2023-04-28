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
using ControlzEx.Standard;


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
    public class FilterMethods
    {
        public string DisplayFilterMethod { get; set; }
        public string FilterMethod { get; set; }
    }
    public partial class AppLauncherPage : Page
    {

        private static PackIconFontAwesome packIconFontAwesome;
        private static DispatcherTimer spinStopTimer = new DispatcherTimer();

        private string windowpage;
        private List<Profile> items = new List<Profile>();
        private List<Profile> tempList = new List<Profile>();
        private string currentSortMethod = Properties.Settings.Default.appSortMethod;
        private string currentFilterMethod = "Filter_Method_None";

        private List<SortMethods> sortMethods = new List<SortMethods>();
        private List<FilterMethods> filterMethods = new List<FilterMethods>();

        public AppLauncherPage()
        {
            InitializeComponent();
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.SystemTheme + "." + Properties.Settings.Default.systemAccent);

            MainWindow wnd = (MainWindow)Application.Current.MainWindow;
            wnd.changeUserInstruction("AppLauncherPage_Instruction");
            wnd = null;

            updateSortFilterLabel();


    
            SortMethods sm = new SortMethods();
            sm.SortMethod = "Sort_Method_AppType";
            sm.DisplaySortMethod = Application.Current.Resources["Sort_Method_AppType"].ToString();
            sortMethods.Add(sm);

            SortMethods sm1 = new SortMethods();
            sm1.SortMethod = "Sort_Method_ProfileName";
            sm1.DisplaySortMethod = Application.Current.Resources["Sort_Method_ProfileName"].ToString();
            sortMethods.Add(sm1);

            SortMethods sm2 = new SortMethods();
            sm2.SortMethod = "Sort_Method_FrequentlyLaunched";
            sm2.DisplaySortMethod = Application.Current.Resources["Sort_Method_FrequentlyLaunched"].ToString();
            sortMethods.Add(sm2);

            SortMethods sm3 = new SortMethods();
            sm3.SortMethod = "Sort_Method_RecentlyLaunched";
            sm3.DisplaySortMethod = Application.Current.Resources["Sort_Method_RecentlyLaunched"].ToString();
            sortMethods.Add(sm3);

            SortMethods sm4 = new SortMethods();
            sm4.SortMethod = "Sort_Method_Favorite";
            sm4.DisplaySortMethod = Application.Current.Resources["Sort_Method_Favorite"].ToString();
            sortMethods.Add(sm4);


            FilterMethods fm0 = new FilterMethods();
            fm0.FilterMethod = "Filter_Method_None";
            fm0.DisplayFilterMethod = Application.Current.Resources["Filter_Method_None"].ToString();
            filterMethods.Add(fm0);

            FilterMethods fm = new FilterMethods();
            fm.FilterMethod = "Filter_Method_Favorite";
            fm.DisplayFilterMethod = Application.Current.Resources["Filter_Method_Favorite"].ToString();
            filterMethods.Add(fm);

            FilterMethods fm1 = new FilterMethods();
            fm1.FilterMethod = "Filter_Method_Steam";
            fm1.DisplayFilterMethod = Application.Current.Resources["Filter_Method_Steam"].ToString(); 
            filterMethods.Add(fm1);

            FilterMethods fm2 = new FilterMethods();
            fm2.FilterMethod = "Filter_Method_EpicGames";
            fm2.DisplayFilterMethod = Application.Current.Resources["Filter_Method_EpicGames"].ToString();
            filterMethods.Add(fm2);

            FilterMethods fm3 = new FilterMethods();
            fm3.FilterMethod = "Filter_Method_Battlenet";
            fm3.DisplayFilterMethod = Application.Current.Resources["Filter_Method_Battlenet"].ToString();
            filterMethods.Add(fm3);

            FilterMethods fm4 = new FilterMethods();
            fm4.FilterMethod = "Filter_Method_Applications";
            fm4.DisplayFilterMethod = Application.Current.Resources["Filter_Method_Applications"].ToString();
            filterMethods.Add(fm4);

            FilterMethods fm5 = new FilterMethods();
            fm5.FilterMethod = "Filter_Method_GOGGalaxy";
            fm5.DisplayFilterMethod = Application.Current.Resources["Filter_Method_GOGGalaxy"].ToString();
            filterMethods.Add(fm5);

            FilterMethods fm6 = new FilterMethods();
            fm6.FilterMethod = "Filter_Method_MicrosoftStore";
            fm6.DisplayFilterMethod = Application.Current.Resources["Filter_Method_MicrosoftStore"].ToString();
            filterMethods.Add(fm6);
                        
            controlListFilter.ItemsSource = filterMethods;
            controlListFilter.SelectedIndex = 0;
            controlListSort.ItemsSource = sortMethods;
            controlListSort.SelectedValue = currentSortMethod;

            dpFilter.Visibility = Visibility.Collapsed;
            dpSort.Visibility = Visibility.Collapsed;


        }
        private void controlList_TouchUp(object sender, TouchEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            if (lb.Name.Contains("Sort"))
            {
                changeSortMethod();
            }
            else
            {
                changeFilterMethod();
            }
    
        }
   
        private void controlList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            if (lb.Name.Contains("Sort"))
            {
                changeSortMethod();
            }
            else
            {
                changeFilterMethod();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //get unique window page combo from getwindow to string
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            //subscribe to controller input events
            Controller_Window_Page_UserControl_Events.pageControllerInput += handleControllerInputs;


            loadValues();


            if (controlList.Items.Contains(Global_Variables.profiles.editingProfile))
            {
                controlList.SelectedItem = Global_Variables.profiles.editingProfile;
                controlList.ScrollIntoView(controlList.SelectedItem);
            }


        }
        private void loadValues()
        {
            // add panels to wrap panel
            if (items.Count > 0)
            {
                items.Clear();
            }

            items =  Global_Variables.profiles.Where(o => o.AppType != "").ToList();
        
       

            applySortAndFilter();
        }

        private void applySortAndFilter()
        {
            tempList = items;
            switch (currentFilterMethod)
            {

                case "Filter_Method_None":
                    tempList = items;
                    break;
                case "Filter_Method_Favorite":
                    tempList = items.Where(o => o.Favorite == true).ToList<Profile>();
                    break;
                case "Filter_Method_Applications":
                    tempList = items.Where(o => o.AppType == "Exe").ToList<Profile>();
                    break;
                default:
                    tempList = items.Where(o => o.AppType == Application.Current.Resources[currentFilterMethod].ToString()).ToList<Profile>();
                    break;

            }

            switch (currentSortMethod)
            {
                case "Sort_Method_AppType":
                    tempList = tempList.OrderBy(o => o.AppType).ThenBy(p => p.ProfileName).ToList();
                    break;
                case "Sort_Method_RecentlyLaunched":
                    tempList = tempList.OrderByDescending(o => o.LastLaunched).ThenBy(p => p.ProfileName).ToList();
                    break;
                case "Sort_Method_FrequentlyLaunched":
                    tempList = tempList.OrderByDescending(o => o.NumberLaunches).ThenBy(p => p.ProfileName).ToList();

                    break;

                case "Sort_Method_Favorite":
                    tempList = tempList.OrderByDescending(o => o.Favorite).ThenBy(p => p.ProfileName).ToList();
                    break;
                case "Sort_Method_ProfileName":
                    tempList = tempList.OrderBy(o => o.ProfileName).ToList();
                    break;
                default:

                    break;
            }


            if (tempList.Count > 0)
            {
                controlList.ItemsSource = tempList;
                controlList.Items.Refresh();
                if (controlList.Items.Count > 0)
                {
                    if (controlList.SelectedIndex < 0)
                    {
                        controlList.SelectedIndex = 0;
                    }
                }
            }
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
                    Profile lbai = controlList.SelectedItem as Profile;
                    int index = controlList.SelectedIndex;
                    switch (action)
                    {
                        case "A":
                            //if the sort or filter list is open change that, otherwise launch a game
                            if (dpSort.Visibility == Visibility.Visible)
                            {
                                changeSortMethod();
                                dpSort.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                if (dpFilter.Visibility == Visibility.Visible)
                                {
                                    changeFilterMethod();
                                    dpFilter.Visibility = Visibility.Collapsed;
                                }
                                else
                                {
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
                                            spinStopTimer.Interval = new TimeSpan(0, 0, 15);

                                            spinStopTimer.Tick += spinner_Stop_Tick;

                                            spinStopTimer.Start();


                                        }



                                    }
                                }
                            }
                           
                            break;

                        case "X":
                            Global_Variables.profiles.editingProfile = lbai;
                            Global_Variables.mainWindow.navigateFrame("ProfileEditPage");
                            break;
                        case "Start":
                            if (dpSort.Visibility == Visibility.Collapsed)
                            {
                                dpSort.Visibility = Visibility.Visible;
                                dpFilter.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                dpSort.Visibility = Visibility.Collapsed;
                            }
                            break;
                        case "Back":
                            if (dpFilter.Visibility == Visibility.Collapsed)
                            {
                                dpFilter.Visibility = Visibility.Visible;
                                dpSort.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                dpFilter.Visibility = Visibility.Collapsed;
                            }
                            break;
                        case "Y":
                            Global_Variables.profiles.changeProfileFavorite(lbai.ID);
                            if (currentFilterMethod.Contains("Favorite") || currentSortMethod.Contains("Favorite"))
                            {
                                applySortAndFilter();
                            }
                            else
                            {
                                controlList.Items.Refresh();
                            }
                            
                            break;
                        case "Up":
                            if (dpSort.Visibility == Visibility.Visible)
                            {
                                handleListBoxIndexChange(controlListSort, -1);
                            }
                            else
                            {
                                if (dpFilter.Visibility == Visibility.Visible)
                                {
                                    handleListBoxIndexChange(controlListFilter, -1);
                                }
                                else
                                {
                                    handleListBoxIndexChange(controlList, -3);
                                }
                            }
                            
                            break;
                        case "Down":
                            if (dpSort.Visibility == Visibility.Visible)
                            {
                                handleListBoxIndexChange(controlListSort, 1);
                            }
                            else
                            {
                                if (dpFilter.Visibility == Visibility.Visible)
                                {
                                    handleListBoxIndexChange(controlListFilter, 1);
                                }
                                else
                                {
                                    handleListBoxIndexChange(controlList, 3);
                                }
                            }
                            break;
                        case "Left":
                            handleListBoxIndexChange(controlList, -1);
                            break;
                        case "Right":
                            handleListBoxIndexChange(controlList, 1);
                            break;
                        case "LB":
                            handleListBoxIndexChange(controlList, -15);
                            break;
                        case "RB":
                            handleListBoxIndexChange(controlList, 15);
                            break;
                        default: break;

                    }

                }
                

            }

        }

        private void handleListBoxIndexChange(ListBox lb, int change)
        {
            int selectedIndex = lb.SelectedIndex;
            int upperIndex = lb.Items.Count-1;
            if (change < 0 )
            {
                if (selectedIndex >= -change)
                {
                    lb.SelectedIndex = selectedIndex + change;
                    lb.ScrollIntoView(lb.SelectedItem);
                }
                else
                {
                    if (selectedIndex != 0)
                    {
                        lb.SelectedIndex = 0;
                        lb.ScrollIntoView(lb.SelectedItem);
                    }
                    else
                    {
                        lb.SelectedIndex = lb.Items.Count-1;
                        lb.ScrollIntoView(lb.SelectedItem);
                    }
                }

            }
            if (change > 0 )
            {
                if ((upperIndex - selectedIndex) >= change)
                {
                    lb.SelectedIndex = selectedIndex + change;
                    lb.ScrollIntoView(lb.SelectedItem);
                }
                else 
                {
                    if (selectedIndex != upperIndex)
                    {
                        lb.SelectedIndex = upperIndex;
                        lb.ScrollIntoView(lb.SelectedItem);
                    }
                    else
                    {
                        lb.SelectedIndex = 0;
                        lb.ScrollIntoView(lb.SelectedItem);
                    }
                }

               
            }

        }

        private void changeSortMethod()
        {
            if (controlListSort.SelectedItem != null)
            {
                SortMethods sm = controlListSort.SelectedItem as SortMethods;
                currentSortMethod = sm.SortMethod;
                applySortAndFilter();
                updateSortFilterLabel();
                Properties.Settings.Default.appSortMethod = currentSortMethod;
                Properties.Settings.Default.Save();

            }

          

        }

        private void applygarbageFilter()
        {
            switch (currentFilterMethod)
            {

                case "Filter_Method_None":
                    controlList.ItemsSource = items;
                    break;
                case "Filter_Method_Favorite":
                    controlList.ItemsSource = items.Where(o => o.Favorite == true);
                    break;
                case "Filter_Method_Applications":
                    controlList.ItemsSource = items.Where(o => o.AppType == "Exe");
                    break;
                default:
                    controlList.ItemsSource = items.Where(o => o.AppType == Application.Current.Resources[currentFilterMethod].ToString());
                    break;

            }
            controlList.Items.Refresh();

        }
        private void updateSortFilterLabel()
        {
            sortLabel.Content = Application.Current.Resources["Sort_Method_SortBy"].ToString() + ": " + Application.Current.Resources[currentSortMethod].ToString() + "; " + Application.Current.Resources["Filter_Method_Filter"].ToString() + ": " + Application.Current.Resources[currentFilterMethod].ToString();
        }

        private void changeFilterMethod()
        {

            if (controlListFilter.SelectedItem != null)
            {
                FilterMethods fm = controlListFilter.SelectedItem as FilterMethods;
                currentFilterMethod = fm.FilterMethod;
                applySortAndFilter();
                updateSortFilterLabel();


            }
                 

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.pageControllerInput -= handleControllerInputs;
        }
    }
   
}
