using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
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
    public partial class AccentColor_Picker : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        public ReadOnlyObservableCollection<Theme> themes = ThemeManager.Current.Themes;
        public AccentColor_Picker()
        {
            InitializeComponent();
            //UserControl_Management.setupControl(control);
            setThemeItems();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");
           
            if (Window.GetWindow(this).ActualWidth < 650) { subText.Visibility = Visibility.Collapsed; }
        }

        private void setThemeItems()
        {
            foreach(Theme theme in themes)
            {
                if (theme.ToString().Contains("(Light)"))
                {
                    control.Items.Add(accentShape(theme));
                    
                }
      
            }
          
            foreach (Rectangle lbi in control.Items)
            {
                if (lbi.Tag.ToString().Contains(Properties.Settings.Default.systemAccent))
                {
                    control.SelectedItem = lbi;
                }

            }


        }

        private Rectangle accentShape(Theme theme)
        {
            Rectangle accentShape = new Rectangle();
            accentShape.RadiusX = 4;
            accentShape.RadiusY=4;
            accentShape.Width = 30;
            accentShape.Height = 30;
            accentShape.VerticalAlignment= VerticalAlignment.Center;
            accentShape.HorizontalAlignment = HorizontalAlignment.Center;
            accentShape.Margin = new Thickness(3,6,8,6);
            accentShape.Fill = theme.ShowcaseBrush;
            accentShape.Tag = theme.DisplayName;
            return accentShape;
        }

        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {
                Classes.UserControl_Management.UserControl_Management.handleUserControl(border, control, args.Action);
            }
        }

        private void control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.IsLoaded)
            {
                Rectangle themeShape = (Rectangle)control.SelectedItem;
                string theme = themeShape.Tag.ToString();
                theme = theme.Substring(0, theme.IndexOf("(")).Trim();
                ThemeManager.Current.ChangeTheme(Window.GetWindow(this), Properties.Settings.Default.SystemTheme + "." + theme);
                Properties.Settings.Default.systemAccent = theme;
                Properties.Settings.Default.Save();
            }
            
        }
    }

  
}
