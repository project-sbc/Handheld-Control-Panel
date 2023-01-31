using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
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
                    case "Slider_MaxGPUCLK":
                        slider.Minimum = 400;
                        slider.Maximum = 2200;
                        slider.TickFrequency = 50;
                        slider.SmallChange = 50;
                        slider.LargeChange = 100;
                        slider.Value = Properties.Settings.Default.maxGPUCLK;
                        break;
                    case "Slider_MinTDP":
                        slider.Minimum = 3;
                        slider.Maximum = 25;
                        slider.TickFrequency = 1;
                        slider.SmallChange = 1;
                        slider.LargeChange = 5;
                        slider.Value = Properties.Settings.Default.minTDP;
                        break;
                    case "Slider_MaxTDP":
                        slider.Minimum = 5;
                        slider.Maximum = 65;
                        slider.TickFrequency = 1;
                        slider.SmallChange = 1;
                        slider.LargeChange = 5;
                        slider.Value = Properties.Settings.Default.maxTDP;
                        break;
                    case "Slider_TDP2":
                        slider.Minimum = Properties.Settings.Default.minTDP;
                        slider.Maximum = Properties.Settings.Default.maxTDP;
                        slider.TickFrequency = 1;
                        slider.SmallChange = 1;
                        slider.LargeChange = 5;
                        slider.Value = Global_Variables.Global_Variables.readPL2;
                        break;
                    case "Slider_Volume":
                        slider.Minimum = 0;
                        slider.Maximum = 100;
                        slider.TickFrequency = 1;
                        slider.SmallChange = 1;
                        slider.LargeChange = 10;
                        slider.Value = Global_Variables.Global_Variables.volume;
                        break;
                    case "Slider_EPP":
                        slider.Minimum = 0;
                        slider.Maximum = 100;
                        slider.TickFrequency = 1;
                        slider.SmallChange = 1;
                        slider.LargeChange = 10;
                        slider.Value = Global_Variables.Global_Variables.EPP;
                        break;
                    case "Slider_CoreParking":
                        slider.Minimum = 1;
                        slider.Maximum = Global_Variables.Global_Variables.maxCpuCores;
                        slider.TickFrequency = 1;
                        slider.SmallChange = 1;
                        slider.LargeChange = 2;
                        slider.Value = Global_Variables.Global_Variables.cpuActiveCores;
                        break;
                    case "Slider_Brightness":
                        slider.Minimum = 0;
                        slider.Maximum = 100;
                        slider.TickFrequency = 1;
                        slider.SmallChange = 1;
                        slider.LargeChange = 10;
                        slider.Value = Global_Variables.Global_Variables.brightness;
                        break;
                    case "Slider_FPSLimit":
                        slider.Minimum = 0;
                        slider.Maximum = Properties.Settings.Default.maxRTSSFPSLimit;
                        slider.TickFrequency = 1;
                        slider.SmallChange = 1;
                        slider.LargeChange = 5;
                        slider.Value = Global_Variables.Global_Variables.FPSLimit;
                        break;

                        
                    case "Slider_MaxCPU":
                        slider.Minimum = 1000;
                        slider.Maximum = 5000;
                        slider.TickFrequency = 100;
                        slider.SmallChange = 100;
                        slider.LargeChange = 500;
                        if (Global_Variables.Global_Variables.cpuMaxFrequency == 0)
                        {
                            slider.Value = slider.Maximum;
                        }
                        else
                        {
                            slider.Value = Global_Variables.Global_Variables.cpuMaxFrequency;
                        }
                   

                        break;
                    default:break;
                }









                   
            }



        }

        public static void handleUserControl(Border border, object control, string action)
        {

            ResourceDictionary res = (ResourceDictionary)Application.LoadComponent(new Uri("Styles/ControlStyle.xaml", UriKind.RelativeOrAbsolute));


            switch (action )
            {
                case "Highlight":
                    border.Style = (Style)res["userControlBorderHighlight"];

                    break;
                case "Select":
                    border.Style = (Style)res["userControlBorderSelected"];

                    break;
                case "Unhighlight":
                    border.Style = (Style)res["userControlBorder"];
                    
                    break;

                default: 
                    if (control is Slider)
                    {
                        Slider slider = (Slider)control;
                        switch(action)
                        {
                            case "Up":
                                slider.Value = slider.Value + slider.LargeChange;
                                break;
                            case "Down":
                                slider.Value = slider.Value - slider.LargeChange;
                                break;
                            case "Right":
                                slider.Value = slider.Value + slider.SmallChange;
                                break;
                            case "Left":
                                slider.Value = slider.Value - slider.SmallChange;
                                break;
                            default:break;
                        }

                    }
                    if (control is ToggleSwitch)
                    {
                        if (action == "A")
                        {
                            ToggleSwitch toggleSwitch = (ToggleSwitch)control;
                            toggleSwitch.IsOn = !toggleSwitch.IsOn;
                        }
              

                    }
                    if (control is ListBox)
                    {
                        ListBox listBox = (ListBox)control;
                        switch(action)
                        {
                            case "Left":
                                if (listBox.SelectedIndex > 0) 
                                { 
                                    listBox.SelectedIndex = listBox.SelectedIndex - 1; 
                                    listBox.ScrollIntoView(listBox.SelectedItem); 
                                }
                                break;

                            case "Up":
                                if (listBox.SelectedIndex > 0) 
                                { 
                                    listBox.SelectedIndex = listBox.SelectedIndex - 1; 
                                    listBox.ScrollIntoView(listBox.SelectedItem); 
                                }
                                break;
                            case "Right":
                                if (listBox.SelectedIndex < (listBox.Items.Count -1)) 
                                { 
                                    listBox.SelectedIndex = listBox.SelectedIndex + 1;
                                    listBox.ScrollIntoView(listBox.SelectedItem);
                                }
                                break;
                            case "Down":
                                if (listBox.SelectedIndex < (listBox.Items.Count - 1)) 
                                { 
                                    listBox.SelectedIndex = listBox.SelectedIndex + 1;
                                    listBox.ScrollIntoView(listBox.SelectedItem);
                                }
                                break;
                           
                            default: break;

                        }
                        

                    }
                    break;
            }
        }

        public static void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            string sliderTag = slider.Tag.ToString();
            double sliderValue = slider.Value;
            switch (sliderTag)
            {
                case "Slider_MaxGPUCLK":
                    Properties.Settings.Default.maxGPUCLK = (int)sliderValue;
                    Properties.Settings.Default.Save();
                    break;
                case "Slider_MinTDP":
                    Properties.Settings.Default.minTDP = (int)sliderValue;
                    Properties.Settings.Default.Save();
                    break;
                case "Slider_MaxTDP":
                    Properties.Settings.Default.maxTDP = (int)sliderValue;
                    Properties.Settings.Default.Save();
                    break;
                case "Slider_TDP":
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.TDP_Management.TDP_Management.changeTDP((int)sliderValue, (int)Global_Variables.Global_Variables.readPL2));
                    break;
                case "Slider_TDP2":
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.TDP_Management.TDP_Management.changeTDP((int)Global_Variables.Global_Variables.readPL1, (int)sliderValue));
                    break;
                case "Slider_Volume":
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.Volume_Management.AudioManager.SetMasterVolume((int)sliderValue));
                    break;
                case "Slider_EPP":
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.EPP_Management.EPP_Management.changeEPP((int)sliderValue));
                    break;
                case "Slider_FPSLimit":
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.RTSS.setRTSSFPSLimit((int)sliderValue));
                    break;
                case "Slider_CoreParking":
                    Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.CoreParking_Management.CoreParking_Management.changeActiveCores((int)sliderValue));
                    break;
                case "Slider_MaxCPU":
                    if (sliderValue == slider.Maximum)
                    {
                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.MaxProcFreq_Management.MaxProcFreq_Management.changeCPUMaxFrequency(0));
                    }
                    else
                    {
                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => Classes.MaxProcFreq_Management.MaxProcFreq_Management.changeCPUMaxFrequency((int)sliderValue));
                    }
                    
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

                    thumb.Width = 13;
                    thumb.Height = 18;
                }
                else { }
            }
            else { }
        }
        #endregion

    }
}
