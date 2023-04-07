using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace Handheld_Control_Panel.Classes.Display_Management
{
    public class ResolutionRefresh
        {
        public string resolution { get; set; }
    public string refreshrate { get; set; }
        }

    public static class Display_Management
    {
        public static string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
        private static Object objLock = new Object();

        //event is for refresh rate to update when the resolution changes
        public static event EventHandler resolutionChangedEvent;
        public static void raiseResolutionChangedEvent()
        {
            resolutionChangedEvent?.Invoke(null, EventArgs.Empty);
        }
        public static event EventHandler resolutionProfileChangedEvent;
        public static void raiseResolutionProfileChangedEvent()
        {
            resolutionProfileChangedEvent?.Invoke(null, EventArgs.Empty);
        }
        public static void getCurrentDisplaySettings()
        {
            
            string commandArguments = " /S";
            string result = QResCLIResult(commandArguments);
            try
            {
                               
                if (result != null)
                {
                    //get actual resolution by dividing scaled width/height with scaling factor
                    double primaryWidth = Screen.PrimaryScreen.Bounds.Width;
                    double primaryHeight = Screen.PrimaryScreen.Bounds.Height;
                    string resolution = primaryWidth.ToString() + "x" + primaryHeight.ToString();
                    if (Global_Variables.Global_Variables.Resolution != resolution)
                    {
                        Global_Variables.Global_Variables.Resolution = resolution;
                    }

                    //display refresh rate
                    int findStartStringRate = result.IndexOf("@") + 1;
                    int findEndStringRate = result.IndexOf("Hz") - 2;

                    string displayRate = result.Substring(findStartStringRate, 1 + findEndStringRate - findStartStringRate).Trim();
                    if (Global_Variables.Global_Variables.RefreshRate != displayRate)
                    {
                       Global_Variables.Global_Variables.RefreshRate = displayRate;
                    }
        
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeDisplaySettings.cs:  Get current display settings: " + ex.Message + " Result: " + result;
                Log_Writer.writeLog(errorMsg);
                System.Windows.MessageBox.Show(errorMsg);

            }





        }
        private static string SetDPICLIResult(string commandArguments)
        {
            string result = "";
            string processSDPI = BaseDir + "\\Resources\\SetDPI\\SetDPI.exe";

            try
            {
                lock (objLock)
                {
                    result = Run_CLI.Run_CLI.RunCommand(commandArguments, true, processSDPI);
                    Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeDisplaySettings.cs:  set DPI CLI: " + ex.Message + " Result: " + result;
                Log_Writer.writeLog(errorMsg);
                System.Windows.Forms.MessageBox.Show(errorMsg);
            }

            return result;

        }
        private static string QResCLIResult(string commandArguments)
        {
            string result = "";
            string processQRes = BaseDir + "\\Resources\\QRes\\QRes.exe";

            try
            {
                lock (objLock)
                {
                    Task.Delay(100);
                    result = Run_CLI.Run_CLI.RunCommand(commandArguments, true, processQRes,1000);
                    Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeDisplaySettings.cs:  Get current display settings: " + ex.Message + " Result: " + result;
                Log_Writer.writeLog(errorMsg);
                //MessageBox.Show(errorMsg);
            }

            return result;

        }

        public static void generateDisplayResolutionAndRateListOLD()
        {
            Global_Variables.Global_Variables.resolutions.Clear();
            //Global_Variables.Global_Variables.refreshRates.Clear();
            string commandArguments = " /L";
            string result = QResCLIResult(commandArguments);

            string resolution;
            string refreshrate;

            using (StringReader reader = new StringReader(result))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.IndexOf("@") > 0)
                    {
                        resolution = line.Substring(0, line.IndexOf(",")).Trim();
                        refreshrate = line.Substring(line.IndexOf("@") + 1, 4).Trim();
                        if (!Global_Variables.Global_Variables.resolutions.Contains(resolution)) { Global_Variables.Global_Variables.resolutions.Insert(0,resolution); }
                       // if (!Global_Variables.Global_Variables.refreshRates.Contains(refreshrate)) { Global_Variables.Global_Variables.refreshRates.Add(refreshrate); }
                    }
                }
            }

            Global_Variables.Global_Variables.scalings.Clear();
            Global_Variables.Global_Variables.scalings.Add("100");
            Global_Variables.Global_Variables.scalings.Add("125");
            Global_Variables.Global_Variables.scalings.Add("150");
            Global_Variables.Global_Variables.scalings.Add("175");
            Global_Variables.Global_Variables.scalings.Add("200");
            Global_Variables.Global_Variables.scalings.Add("225");
        }
        public static void generateDisplayResolutionAndRateListOLD2()
        {
            Global_Variables.Global_Variables.resolution_refreshrates.Clear();
            string commandArguments = " /L";
            string result = QResCLIResult(commandArguments);

            string resolution;
            string currResolution="";
            string refreshrate;


            List<string> resultList = result.Split(Environment.NewLine).ToList();

            List<string> refreshrates = new List<string>();

            foreach (string line in resultList)
            {
                if (line.IndexOf("@") > 0)
                {
                
                    resolution = line.Substring(0, line.IndexOf(",")).Trim();

                    if (currResolution != resolution)
                    {
                        if (currResolution != "")
                        {
                            Global_Variables.Global_Variables.resolution_refreshrates.Add(currResolution, refreshrates);
                            refreshrates = new List<string>();
                            Global_Variables.Global_Variables.resolutions.Insert(0, currResolution);
                            currResolution = resolution;
                          
                        }
                        else
                        {
                            currResolution = resolution;
                        }

                    }

                    refreshrate = line.Substring(line.IndexOf("@") + 1, 4).Trim();
                    if (!refreshrates.Contains(refreshrate)) { refreshrates.Add(refreshrate); }
                    if (resultList.IndexOf(line) == resultList.Count - 2)
                    {
                        Global_Variables.Global_Variables.resolution_refreshrates.Add(resolution, refreshrates);
                        if (!Global_Variables.Global_Variables.resolutions.Contains(resolution)) { Global_Variables.Global_Variables.resolutions.Insert(0, resolution); }
                       

                    }
                }

            }

           

            Global_Variables.Global_Variables.scalings.Clear();
            Global_Variables.Global_Variables.scalings.Add("100");
            Global_Variables.Global_Variables.scalings.Add("125");
            Global_Variables.Global_Variables.scalings.Add("150");
            Global_Variables.Global_Variables.scalings.Add("175");
            Global_Variables.Global_Variables.scalings.Add("200");
            Global_Variables.Global_Variables.scalings.Add("225");
        }
        public static void generateDisplayResolutionAndRateList()
        {
            Global_Variables.Global_Variables.resolution_refreshrates.Clear();
            string commandArguments = " /L";
            string result = QResCLIResult(commandArguments);

            string resolution;
            string currResolution = "";
            string refreshrate;

            string[] resultList = result.Split(Environment.NewLine).ToArray();
            List<ResolutionRefresh> resolutionRefreshList = new List<ResolutionRefresh>();
                       
            foreach (string line in resultList)
            {
                if (line.IndexOf("@") > 0)
                {
                    ResolutionRefresh rr = new ResolutionRefresh();
                    rr.resolution = line.Substring(0, line.IndexOf(",")).Trim();
            
                    rr.refreshrate = line.Substring(line.IndexOf("@") + 1, 4).Trim();

                    Log_Writer.writeLog(rr.resolution + " " + rr.refreshrate);
                   
                    if (!Global_Variables.Global_Variables.resolutions.Contains(rr.resolution)) { Global_Variables.Global_Variables.resolutions.Insert(0,rr.resolution); Debug.WriteLine(rr.resolution); }
                    resolutionRefreshList.Add(rr);
                }

            }

            foreach (string GVresolution in Global_Variables.Global_Variables.resolutions)
            {
                List<string> refreshrates = new List<string>();
                foreach(ResolutionRefresh rr in resolutionRefreshList)
                {
                    if (rr.resolution == GVresolution)
                    {
                        Log_Writer.writeLog(rr.resolution + " Added to array");
                        if (!refreshrates.Contains(rr.refreshrate))
                        {
                            Log_Writer.writeLog(rr.refreshrate + "Hz Added to resolution");
                            refreshrates.Add(rr.refreshrate);
                        }
                      
                    }
                }

                Global_Variables.Global_Variables.resolution_refreshrates.Add(GVresolution,refreshrates);

            }



            Global_Variables.Global_Variables.scalings.Clear();
            Global_Variables.Global_Variables.scalings.Add("100");
            Global_Variables.Global_Variables.scalings.Add("125");
            Global_Variables.Global_Variables.scalings.Add("150");
            Global_Variables.Global_Variables.scalings.Add("175");
            Global_Variables.Global_Variables.scalings.Add("200");
            Global_Variables.Global_Variables.scalings.Add("225");
        }

        public static void SetDisplayResolution(string resolution)
        {
            string resolutionX = resolution.Substring(0, resolution.IndexOf("x"));
            string resolutionY = resolution.Substring(resolution.IndexOf("x") + 1, resolution.Length - (1 + resolution.IndexOf("x")));
            string commandArguments = " /X:" + resolutionX + " /Y:" + resolutionY;
            string result = "";

            //check if current refresh rate is supported at the display resolution, otherwise change
            if (!Global_Variables.Global_Variables.resolution_refreshrates[resolution].Contains(Global_Variables.Global_Variables.RefreshRate))
            {

                commandArguments = commandArguments + " /R:" + Global_Variables.Global_Variables.resolution_refreshrates[resolution][0] + " /D";
            }
            else
            {
                commandArguments = commandArguments + " /R:" + Global_Variables.Global_Variables.RefreshRate + " /D";
            }
            try
            {
                result = QResCLIResult(commandArguments);
                Task.Delay(200);
                getCurrentDisplaySettings();
                if (result.Contains("Mode Ok"))
                {
                    //MainWindow.setWindowDock();
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeDisplaySettings.cs:  Set display resolution: " + ex.Message + " Result: " + result;
                Log_Writer.writeLog(errorMsg);
                System.Windows.Forms.MessageBox.Show(errorMsg);

            }




        }

        public static void SetDisplayScaling(string scaling)
        {
            if (scaling != "Default")
            {
                string commandArguments = " " + scaling;
                string result = "";

                try
                {
                    result = SetDPICLIResult(commandArguments);
                    Global_Variables.Global_Variables.Scaling = scaling;
                    
                    Global_Variables.Global_Variables.mainWindow.setWindowSizePosition();
            
                }
                catch (Exception ex)
                {
                    string errorMsg = "Error: ChangeDisplaySettings.cs:  Set display resolution: " + ex.Message + " Result: " + result;
                    Log_Writer.writeLog(errorMsg);
                    System.Windows.MessageBox.Show(errorMsg);

                }


            }




        }
        public static void SetDisplayRefreshRate(string refresh)
        {
     
            string commandArguments = " /R:" + refresh + " /D";
            string result = "";
            try
            {
                result = QResCLIResult(commandArguments);
                Task.Delay(200);
                getCurrentDisplaySettings();

            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeDisplaySettings.cs:  Set refresh rate: " + ex.Message + " Result: " + result;
                Log_Writer.writeLog(errorMsg);
                //MessageBox.Show(errorMsg);

            }





        }
    }
}
