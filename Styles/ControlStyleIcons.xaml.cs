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
using System.Windows.Shapes;

namespace Handheld_Control_Panel.Styles
{
    /// <summary>
    /// Interaction logic for ControlStyle.xaml
    /// </summary>
    public partial class ControlStyleIcons : ResourceDictionary
    {
        public ControlStyleIcons()
        {
            InitializeComponent();
        }

        public void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           
        }
    }
}
