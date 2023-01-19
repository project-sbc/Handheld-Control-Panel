using Handheld_Control_Panel.Pages.UserControls;
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

namespace Handheld_Control_Panel.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }
        public void controllerInput(string action)
        {
            Debug.WriteLine("Yes");

            foreach(object child in dockPanel.Children)
            {
                if (child is UserControl)
                {
                    selectUserControl((UserControl)child, action);
                    Debug.WriteLine(child.GetType().ToString());
                }
            }
        }

        private void selectUserControl(UserControl uc, string action)
        {
            switch(uc.ToString())
            {
                case "TDP_Slider":
                
                    break;
                default: break;

            }

        }

    }
}
