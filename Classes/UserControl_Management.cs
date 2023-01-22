using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Handheld_Control_Panel.Classes.UserControl_Management
{
    public static class UserControl_Management
    {
        public static void handleUserControl(Border border, Border leftBorder,object control, string action)
        {
            
            


            switch (action )
            {
                case "Highlight":
                    if (Properties.Settings.Default.SystemTheme == "Light")
                    {
                        leftBorder.BorderBrush = System.Windows.Media.Brushes.Black;
                        border.BorderBrush = System.Windows.Media.Brushes.Transparent;
                    }
                    else
                    {
                        leftBorder.BorderBrush = System.Windows.Media.Brushes.White;
                        border.BorderBrush = System.Windows.Media.Brushes.Transparent;
                    }

                    break;
                case "Select":
                    if (Properties.Settings.Default.SystemTheme == "Light")
                    {
                        border.BorderBrush = System.Windows.Media.Brushes.Black;
                    }
                    else
                    {
                        border.BorderBrush = System.Windows.Media.Brushes.White;
                    }
                  
                    break;
                case "Unhighlight":
                    border.BorderBrush = System.Windows.Media.Brushes.Transparent;
                    leftBorder.BorderBrush = System.Windows.Media.Brushes.Transparent;
                    break;

                default: break;

            }
            

        }

        public static void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Debug.WriteLine("YES Dog");
        }
    }
}
