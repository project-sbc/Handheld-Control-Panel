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
using System.Windows.Interop;
using MahApps.Metro.IconPacks;
using System.Runtime.InteropServices;


namespace Handheld_Control_Panel.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    /// 


    public partial class AboutPage : Page
    {
        private string windowpage;
        private List<infopageitem> infopageitems = new List<infopageitem>();
        public AboutPage()
        {
            InitializeComponent();
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.SystemTheme + "." + Properties.Settings.Default.systemAccent);

            MainWindow wnd = (MainWindow)Application.Current.MainWindow;
            wnd.changeUserInstruction("SelectBack_Instruction");
            wnd = null;

            infopageitem GitHub = new infopageitem();
            GitHub.displayitem = Application.Current.Resources["UserControl_GitHubLink"].ToString();
            GitHub.item = "Handheld Control Panel GitHub Link";
            GitHub.Kind = PackIconMaterialKind.DockRight;
            infopageitems.Add(GitHub);

            infopageitem Tutorials = new infopageitem();
            Tutorials.displayitem = Application.Current.Resources["UserControl_Tutorials"].ToString();
            Tutorials.item = "Tutorials";
            Tutorials.Kind = PackIconMaterialKind.DockRight;
            infopageitems.Add(Tutorials);


            infopageitem Donate = new infopageitem();
            Donate.displayitem = Application.Current.Resources["UserControl_Donate"].ToString();
            Donate.item = "Donate";
            Donate.Kind = PackIconMaterialKind.DockRight;
            infopageitems.Add(Donate);

            infopageitem OtherSoftware = new infopageitem();
            OtherSoftware.displayitem = Application.Current.Resources["UserControl_OtherSoftware"].ToString();
            OtherSoftware.item = "OtherSoftware";
            OtherSoftware.Kind = PackIconMaterialKind.DockRight;
            infopageitems.Add(OtherSoftware);


            controlList.ItemsSource = infopageitems;
        }

        

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //get unique window page combo from getwindow to string
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            //subscribe to controller input events
            Controller_Window_Page_UserControl_Events.pageControllerInput += handleControllerInputs;

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
                    infopageitem ipi = controlList.SelectedItem as infopageitem;
                    int index = controlList.SelectedIndex;
                    switch (action)
                    {
                        case "A":
                            handleListChange();
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
                    
                    if (action == "Up" || action == "Down")
                    {
                        if (controlList.Items.Count > 0) { controlList.SelectedIndex = 0; controlList.ScrollIntoView(controlList.SelectedItem); }
                    
                    }
                }


            }

        }


        private void handleListChange()
        {
            if (controlList.SelectedItem != null)
            {
                infopageitem ipi = controlList.SelectedItem as infopageitem;
                int index = controlList.SelectedIndex;

                MainWindow wnd = (MainWindow)Application.Current.MainWindow;
                var mainWindowHandle = new WindowInteropHelper(wnd).Handle;
                switch (ipi.item)
                {
                    case "Tutorials":
                       
                        break;


                  
                    default: break;

                }

            }
          
           

        }
     

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.pageControllerInput -= handleControllerInputs;
        }

        private void controlList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            handleListChange();
        }
    }
    public class infopageitem
    {
        public string displayitem { get; set; }
        public string displayitemdescription { get; set; }
        public string item { get; set; }
        public PackIconMaterialKind Kind { get; set; }
      
    }
}
