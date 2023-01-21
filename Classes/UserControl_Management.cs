using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Handheld_Control_Panel.Classes
{
    public static class UserControl_Management
    {
        public static void handleUserControl(Border border, Border leftBorder,object control, string action)
        {
            switch (action)
            {
                case "Highlight":
                    leftBorder.BorderBrush = System.Windows.Media.Brushes.Black;
                    border.BorderBrush = System.Windows.Media.Brushes.Transparent;
                    break;
                case "Select":
                    border.BorderBrush = System.Windows.Media.Brushes.Black;
                    break;
                case "Unhighlight":
                    border.BorderBrush = System.Windows.Media.Brushes.Transparent;
                    leftBorder.BorderBrush = System.Windows.Media.Brushes.Transparent;
                    break;

                default: break;

            }
            

        }

    }
}
