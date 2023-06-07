using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using System;
using System.Windows;
using System.Windows.Controls;



namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class CloseGame_Button : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";
        public CloseGame_Button()
        {
            try
            {
                InitializeComponent();
                if (RTSS.RTSSRunning())
                {
                  
                    string gameName = "";
                    if (gameName == "")
                    {
                        this.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        controlLabel.Content = gameName;
                    }
                }
                else
                {
                    this.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           

           
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
                Classes.UserControl_Management.UserControl_Management.handleUserControl(border, control, args.Action);

            }
        }


       
      
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
          
        }

       

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        
                 

        }
    }
}
