using Handheld_Control_Panel.Classes;
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

namespace Handheld_Control_Panel.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class AutoFanPage : Page
    {
        private string windowpage;
        private double[] dataX = new double[] { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
        private double[] dataY = new double[] { 0, 0, 0, 0, 0, 0, 0, 30, 50, 70, 100 };
        private int xIndex = 0;


        public AutoFanPage()
        {
            InitializeComponent();
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.SystemTheme + "." + Properties.Settings.Default.systemAccent);

            MainWindow wnd = (MainWindow)Application.Current.MainWindow;
            wnd.changeUserInstruction("HotKeyPage_Instruction");
            wnd = null;



            loadSettingFanCurve();
            setUpPlot();
            plotFanCurve();
            updateLabels();

            
        }
        private void handleInputs(string action)
        {
            switch(action)
            {
                case "Left":
                    if (xIndex > 0)
                    {
                        xIndex = xIndex - 1;
                        updateLabels();
                    }
                    break;
                case "Right":
                    if (xIndex < 10)
                    {
                        xIndex = xIndex + 1;
                        updateLabels();
                    }
                    
                    break;
                case "Up":
                    if (dataY[xIndex] < Global_Variables.Device.MinFanSpeed)
                    {
                        dataY[xIndex] = Global_Variables.Device.MinFanSpeed;
                    }
                    else
                    {
                        if (dataY[xIndex] < 100)
                        {
                            dataY[xIndex] = dataY[xIndex] + 1;
                        }
                    }
                    plotFanCurve();
                    break;
                case "Down":
                    if (dataY[xIndex] <= Global_Variables.Device.MinFanSpeed)
                    {
                        dataY[xIndex] = 0;
                    }
                    else
                    {
                        dataY[xIndex] = dataY[xIndex] - 1;
                    }
                    plotFanCurve();
                    break;
         
                default: break;

            }
            updateLabels();
        }

        private void updateLabels()
        {
            TemperatureLabel.Content = Application.Current.Resources["FanPage_Temperature"] + ": " + dataX[xIndex].ToString();
            FanSpeedLabel.Content = Application.Current.Resources["FanPage_FanSpeed"] + ": " + dataY[xIndex].ToString();
        }
        private void setUpPlot()
        {

            fanCurvePlot.Plot.Title(Application.Current.Resources["FanPage_FanCurve"].ToString());
            fanCurvePlot.Plot.XLabel(Application.Current.Resources["FanPage_Temperature"].ToString());
            fanCurvePlot.Plot.YLabel(Application.Current.Resources["FanPage_FanSpeed"].ToString());
            fanCurvePlot.Plot.SetAxisLimits(-2, 102, -2, 102);
        }


        private void plotFanCurve()
        {

            fanCurvePlot.Plot.Clear();
            fanCurvePlot.Plot.AddScatter(dataX, dataY);
            fanCurvePlot.Refresh();
        }
        private void saveSettingFanCurve()
        {
            string saveDataY = "";
            foreach(double d in dataY)
            {
                saveDataY = saveDataY + d.ToString() + ",";

            }
            Properties.Settings.Default.fanCurve = saveDataY;
            Properties.Settings.Default.Save();

        }
        private void loadSettingFanCurve()
        {
            if (Properties.Settings.Default.fanCurve != "")
            {
                string[] dataYstring = Properties.Settings.Default.fanCurve.Split(',').ToArray();

                if (dataYstring.Length == 11)
                {
                    for (int i = 0; i < 11; i++)
                    {
                        dataY[i] = Convert.ToDouble(dataYstring[i]);
                    }

                }

            }
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
                    saveSettingFanCurve();
                }


            }

        }

     

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.pageControllerInput -= handleControllerInputs;
        }
    }
}
