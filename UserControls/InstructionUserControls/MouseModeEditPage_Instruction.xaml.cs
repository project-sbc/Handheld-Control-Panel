using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.TaskSchedulerWin32;
using Handheld_Control_Panel.Classes.UserControl_Management;
using Handheld_Control_Panel.Styles;
using MahApps.Metro.Controls;
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
    public partial class MouseModeEditPage_Instruction : UserControl
    {
 
        public MouseModeEditPage_Instruction()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Tag != "")
            {
                Controller_Management.buttonEvents.raiseControllerInput(button.Tag.ToString());

            }

        }
    }
}
