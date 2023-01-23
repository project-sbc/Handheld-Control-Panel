using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Handheld_Control_Panel.Classes.UserControl_Management
{
    public static class UserControl_Management
    {
        public static void setupControl(object control)
        {
            if (control is Slider)
            {
                Slider slider = (Slider)control;
                slider.IsSnapToTickEnabled = true;

                switch (slider.Tag)
                {
                    case "Slider_TDP":
                        slider.Minimum = Properties.Settings.Default.minTDP;
                        slider.Maximum = Properties.Settings.Default.maxTDP;
                        slider.TickFrequency = 1;
                        slider.SmallChange = 1;
                        slider.LargeChange = 5;
                        slider.Value = Global_Variables.Global_Variables.readPL1;
                        break;
                    case "Slider_TDP2":
                        slider.Minimum = Properties.Settings.Default.minTDP;
                        slider.Maximum = Properties.Settings.Default.maxTDP;
                        slider.TickFrequency = 1;
                        slider.SmallChange = 1;
                        slider.LargeChange = 5;
                        slider.Value = Global_Variables.Global_Variables.readPL2;
                        break;
                    default:break;
                }









                   
            }



        }

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
        
            Slider slider = (Slider)sender;
            string sliderTag = slider.Tag.ToString();
            double sliderValue = slider.Value;
            switch (sliderTag)
            {
                case "Slider_TDP":
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.TDP_Management.TDP_Management.changeTDP((int)sliderValue, (int)Global_Variables.Global_Variables.readPL2));
                    break;
                case "Slider_TDP2":
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.TDP_Management.TDP_Management.changeTDP((int)Global_Variables.Global_Variables.readPL1, (int)sliderValue));
                    break;
                default: break;


            }
        }

        #region set slider thumbsize
        private static DependencyObject GetElementFromParent(DependencyObject parent, string childname)
        {
            //Internet routine for finding thumb of slider
            //Use element parent for thumb size control on slider
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement childframeworkelement && childframeworkelement.Name == childname)
                    return child;

                var FindRes = GetElementFromParent(child, childname);
                if (FindRes != null)
                    return FindRes;
            }
            return null;
        }
            

        public static void setThumbSize(Slider slider)
        {
            //set thumb size, internet routine
            var SliderThumb = GetElementFromParent(slider as DependencyObject, "HorizontalThumb"); //Make sure to put the right name for your slider layout options are: ("VerticalThumb", "HorizontalThumb")
            if (SliderThumb != null)
            {

                if (SliderThumb is Thumb thumb)
                {

                    thumb.Width = 16;
                    thumb.Height = 24;
                }
                else { }
            }
            else { }
        }
        #endregion

    }
}
