﻿using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.UserControls;
using Microsoft.Win32.TaskScheduler;
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
using MahApps.Metro;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlzEx.Theming;
using System.Windows.Controls.Primitives;
using Handheld_Control_Panel.Classes.Global_Variables;
using YamlDotNet.Core.Tokens;
using ScottPlot;
using System.Reflection;
using LibreHardwareMonitor.Hardware;
using System.Windows.Threading;

namespace Handheld_Control_Panel.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class AutoFanPage : Page
    {
        private string windowpage;
        private double[] dataXtemp = new double[21] { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100 };
        private double[] dataXpower = new double[] { };
        private double[] dataYtemp = new double[] { };
        private double[] dataYpower = new double[] { };
        private int xIndex= 0;

        private DispatcherTimer updateTempPower = new DispatcherTimer();


        public AutoFanPage()
        {
            InitializeComponent();
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.SystemTheme + "." + Properties.Settings.Default.systemAccent);

            MainWindow wnd = (MainWindow)Application.Current.MainWindow;
            wnd.changeUserInstruction("HotKeyPage_Instruction");
            wnd = null;


            if (Properties.Settings.Default.fanAutoModeTemp)
            {
                spPackagePowerPlot.Visibility = Visibility.Collapsed;
            }
            else
            {
                spTempPlot.Visibility = Visibility.Collapsed;
            }

            loadSettingFanCurve();
            setUpPlot();
            plotFanCurve();
            updateLabels();
            if (Properties.Settings.Default.SystemTheme == "Dark")
            {
                fanCurvePackagePowerPlot.plt.Style(ScottPlot.Style.Gray2);
                fanCurveTemperaturePlot.plt.Style(ScottPlot.Style.Gray2);
            }
            else
            {
                fanCurvePackagePowerPlot.plt.Style(ScottPlot.Style.Light2);
                fanCurveTemperaturePlot.plt.Style(ScottPlot.Style.Light2);
            }

            computer.Open();
            updateTempPower.Interval = new TimeSpan(0, 0, 1);
            updateTempPower.Tick += UpdateTempPower_Tick;
            updateTempPower.Start();
        }

        private void UpdateTempPower_Tick(object? sender, EventArgs e)
        {
            computer.Accept(new UpdateVisitor());
            if (Properties.Settings.Default.fanAutoModeTemp)
            {
                getLibre_cpuTemperature();
            }
            else
            {
                getLibre_packagepower();
            }
        }

        private void handleInputs(string action)
        {
           
           
                if (spPackagePowerPlot.Visibility == Visibility.Visible)
                {
                    switch (action)
                    {

                        case "Left":
                            if (xIndex > 0)
                            {
                                xIndex = xIndex - 1;
                                updateLabels();
                            }
                            break;
                        case "Right":
                            if (xIndex < (dataXpower.Length - 1))
                            {
                                xIndex = xIndex + 1;
                                updateLabels();
                            }

                            break;
                        case "Up":
                            if (dataYpower[xIndex] < Global_Variables.Device.MinFanSpeedPercentage)
                            {
                                dataYpower[xIndex] = Global_Variables.Device.MinFanSpeedPercentage;
                            }
                            else
                            {
                                if (dataYpower[xIndex] < 100)
                                {
                                    dataYpower[xIndex] = dataYpower[xIndex] + 1;
                                }
                            }
                            plotFanCurve();
                            break;
                        case "Down":
                            if (dataYpower[xIndex] <= Global_Variables.Device.MinFanSpeedPercentage)
                            {
                                dataYpower[xIndex] = 0;
                            }
                            else
                            {
                                dataYpower[xIndex] = dataYpower[xIndex] - 1;
                            }
                            plotFanCurve();
                            break;

                        default: break;

                    }
                }
                else
                {
                    switch (action)
                    {

                        case "Left":
                            if (xIndex > 0)
                            {
                                xIndex = xIndex - 1;
                                updateLabels();
                            }
                            break;
                        case "Right":
                            if (xIndex < (dataXtemp.Length - 1))
                            {
                                xIndex = xIndex + 1;
                                updateLabels();
                            }

                            break;
                        case "Up":
                            if (dataYtemp[xIndex] < Global_Variables.Device.MinFanSpeedPercentage)
                            {
                                dataYtemp[xIndex] = Global_Variables.Device.MinFanSpeedPercentage;
                            }
                            else
                            {
                                if (dataYtemp[xIndex] < 100)
                                {
                                    dataYtemp[xIndex] = dataYtemp[xIndex] + 1;
                                }
                            }
                            plotFanCurve();
                            break;
                        case "Down":
                            if (dataYtemp[xIndex] <= Global_Variables.Device.MinFanSpeedPercentage)
                            {
                                dataYtemp[xIndex] = 0;
                            }
                            else
                            {
                                dataYtemp[xIndex] = dataYtemp[xIndex] - 1;
                            }
                            plotFanCurve();
                            break;

                        default: break;

                    }
                }
           
            updateLabels();
        }

        private void updateLabels()
        {
            if (spTempPlot.Visibility == Visibility.Visible)
            {
                TemperatureLabel.Content = Application.Current.Resources["FanPage_Temperature"] + ": " + dataXtemp[xIndex].ToString();
                FanSpeedLabel_Temperature.Content = Application.Current.Resources["FanPage_FanSpeed"] + ": " + dataYtemp[xIndex].ToString();
            }
            if (spPackagePowerPlot.Visibility == Visibility.Visible)
            {
                PackagePowerLabel.Content = Application.Current.Resources["FanPage_CPUPower"] + ": " + dataXpower[xIndex].ToString();
                FanSpeedLabel_Power.Content = Application.Current.Resources["FanPage_FanSpeed"] + ": " + dataYpower[xIndex].ToString();
            }
        }
        private void setUpPlot()
        {

            fanCurvePackagePowerPlot.Plot.Title(Application.Current.Resources["FanPage_FanCurve"].ToString());
            fanCurvePackagePowerPlot.Plot.XLabel(Application.Current.Resources["FanPage_CPUPower"].ToString());
            fanCurvePackagePowerPlot.Plot.YLabel(Application.Current.Resources["FanPage_FanSpeed"].ToString());
            fanCurvePackagePowerPlot.Plot.SetAxisLimits(-2, dataXpower[dataXpower.Length - 1] + 2, -2, 102);


            fanCurveTemperaturePlot.Plot.Title(Application.Current.Resources["FanPage_FanCurve"].ToString());
            fanCurveTemperaturePlot.Plot.XLabel(Application.Current.Resources["FanPage_Temperature"].ToString());
            fanCurveTemperaturePlot.Plot.YLabel(Application.Current.Resources["FanPage_FanSpeed"].ToString());

            fanCurveTemperaturePlot.Plot.SetAxisLimits(-2, 102, -2, 102);
        }


        private void plotFanCurve()
        {
            if (spPackagePowerPlot.Visibility == Visibility.Visible)
            {
                fanCurvePackagePowerPlot.Plot.Clear();
                fanCurvePackagePowerPlot.Plot.AddScatter(dataXpower, dataYpower);
                fanCurvePackagePowerPlot.Refresh();
                fanCurvePackagePowerPlot.Render();
        

            }
            else
            {
                fanCurveTemperaturePlot.Plot.Clear();
                fanCurveTemperaturePlot.Plot.AddScatter(dataXtemp, dataYtemp);
                fanCurveTemperaturePlot.Refresh();
                fanCurveTemperaturePlot.Render();
              
            }
        }
        private void saveSettingFanCurveTemperature()
        {
            string saveDataY = "";
            foreach(double d in dataYtemp)
            {
                saveDataY = saveDataY + d.ToString() + ",";

            }
            Properties.Settings.Default.fanCurveTemperature = saveDataY;
            Properties.Settings.Default.Save();

        }

        private void saveSettingFanCurvePower()
        {
            string saveDataY = "";
            foreach (double d in dataYpower)
            {
                saveDataY = saveDataY + d.ToString() + ",";

            }
            Properties.Settings.Default.fanCurvePackagePower = saveDataY;
            Properties.Settings.Default.Save();

        }
        private void loadSettingFanCurve()
        {
            dataYtemp = Fan_Functions.getTemperatureYValues();
            dataXpower = Fan_Functions.getPowerXValues();
            dataYpower = Fan_Functions.getPowerYValues();

        }
      
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //get unique window page combo from getwindow to string
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            //subscribe to controller input events
            Controller_Window_Page_UserControl_Events.pageControllerInput += handleControllerInputs;



        }

        private void handleControllerInputs(object sender, EventArgs e)
        {
            //get action from custom event args for controller
            controllerPageInputEventArgs args = (controllerPageInputEventArgs)e;
            string action = args.Action;

            if (args.WindowPage == windowpage)
            {
                //global method handles the event tracking and returns what the index of the highlighted and selected usercontrolshould be
                if (args.Action == "Up" || args.Action == "Down" || args.Action == "Left" || args.Action == "Right")
                { handleInputs(action); }
                if (args.Action == "Start")
                {
                    if (spPackagePowerPlot.Visibility == Visibility.Visible)
                    {
                        saveSettingFanCurvePower();
                    }
                    else{
                        saveSettingFanCurveTemperature();
                    }
                    AutoFan_Management.loadXandYvalues();
                }
                if (args.Action == "X")
                {
                    xIndex = 0;
                    if (spPackagePowerPlot.Visibility == Visibility.Visible)
                    {
                        spPackagePowerPlot.Visibility = Visibility.Collapsed;
                        spTempPlot.Visibility = Visibility.Visible;
                        Properties.Settings.Default.fanAutoModeTemp = true;
                    }
                    else
                    {
                        spPackagePowerPlot.Visibility = Visibility.Visible;
                        spTempPlot.Visibility = Visibility.Collapsed;
                        Properties.Settings.Default.fanAutoModeTemp = false;
                    }
                    Properties.Settings.Default.Save();
                    plotFanCurve();
                    updateLabels();
                }


            }

        }

     

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.pageControllerInput -= handleControllerInputs;
            updateTempPower.Stop();
            updateTempPower.Tick -= UpdateTempPower_Tick;
        }


       

        private static Computer computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = false,
            IsMemoryEnabled = false,
            IsMotherboardEnabled = false,
            IsControllerEnabled = false,
            IsNetworkEnabled = false,
            IsStorageEnabled = false,
            IsBatteryEnabled = false,
            IsPsuEnabled = false
        };
        
    
        private void getLibre_packagepower()
        {
            foreach (Hardware hardware in computer.Hardware)
            {
                ISensor package = hardware.Sensors.FirstOrDefault(c => c.Name == "Package");
                if (package != null)
                {
                    updateLabel.Content = Application.Current.Resources["Usercontrol_FanCurrentPackagePower"] + " " + Math.Round((double)package.Value,1).ToString() + " W";

                   
                    break;
                }
            }

        }


        private void getLibre_cpuTemperature()
        {

            foreach (Hardware hardware in computer.Hardware)
            {
                ISensor temperature = hardware.Sensors.FirstOrDefault(c => c.Name == "Core (Tctl/Tdie)");
                if (temperature != null)
                {
                    updateLabel.Content = Application.Current.Resources["Usercontrol_FanCurrentTemperature"] + " " + Math.Round((double)temperature.Value, 1).ToString() + " C";

                    break;
                }
                else
                {
                    foreach (IHardware subhardware in hardware.SubHardware)
                    {
                        Debug.WriteLine("\tSubhardware: {0}", subhardware.Name);

                        ISensor temperature2 = subhardware.Sensors.FirstOrDefault(c => c.Name == "Core (Tctl/Tdie)");
                        if (temperature2 != null)
                        {
                            updateLabel.Content = Application.Current.Resources["Usercontrol_FanCurrentTemperature"] + " " + Math.Round((double)temperature.Value, 1).ToString() + " C";

                            break;
                        }
                    }
                }
            }

        }
    }
}
