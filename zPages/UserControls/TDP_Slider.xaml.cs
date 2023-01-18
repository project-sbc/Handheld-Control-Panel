using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Handheld_Control_Panel.Pages.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class TDP_Slider : UserControl
    {
        private string controllerInput { set ; get; }

        public string controllerInputExt
        {
            get
            {
                return controllerInput;
            }
            set
            {
                controllerInput = value;
                handleControllerInput(value);
            }
        }
        public void handleControllerInput(string controllerInput)
        {
            //do work
        }
        public TDP_Slider()
        {
            InitializeComponent();
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {

 
        }
        
   
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            adjustSliderThumbSize();

           
        }
        #region slider thumb size
        //Three routines, one is to loop through elements for sliders in UC
        //one is to find the parent element to get the thumb unit
        //the last is to adjust the thumb size
        private DependencyObject GetElementFromParent(DependencyObject parent, string childname)
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
        private void adjustSliderThumbSize()
        {
            foreach (object child in dockPanel.Children)
            {
                if (child is Slider) { setThumbSize((Slider)child); }
            }
        }
        private void setThumbSize(Slider slider)
        {
            //set thumb size, internet routine
            var SliderThumb = GetElementFromParent(slider as DependencyObject, "HorizontalThumb"); //Make sure to put the right name for your slider layout options are: ("VerticalThumb", "HorizontalThumb")
            if (SliderThumb != null)
            {

                if (SliderThumb is Thumb thumb)
                {

                    thumb.Width = 20;
                    thumb.Height = 30;
                }
                else { }
            }
            else { }
        }
        #endregion

    }
}
