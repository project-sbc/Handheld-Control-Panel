using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Handheld_Control_Panel.Classes;
using System.Threading;
using System.Diagnostics;

namespace Handheld_Control_Panel.Classes.TDP_Management
{



    public static class TDP_Management
    {
        private static Object objLock = new Object();
        public static string BaseDir = AppDomain.CurrentDomain.BaseDirectory;

        //Read TDP routines
        public static void readTDP()
        {
        
            try
            {
                //add small delay to prevent write and read operations from interfering
                //Log_Writer.writeLog("Read TDP start: determine CPU type");
                Thread.Sleep(100);
   
                //Log_Writer.writeLog("CPU type is " + cpuType);
                if (Global_Variables.Global_Variables.cpuType == "Intel")
                {
                    if (Properties.Settings.Default.IntelMMIOMSR.Contains("MMIO"))
                    { //Log_Writer.writeLog("Read TDP MMIO: read MMIO intel");
                        runIntelReadTDPMMIOKX();
                    }
                    if (Properties.Settings.Default.IntelMMIOMSR == "MSR")
                    { //Log_Writer.writeLog("Read TDP start: read MSR intel");
                        runIntelReadTDPMSR();
                    }
                    //if (Properties.Settings.Default.IntelMMIOMSR == "MSRCMD") { runIntelReadTDPMSRCMD(); }
                }
                else
                {
                    if (Global_Variables.Global_Variables.cpuType == "AMD")
                    {// Log_Writer.writeLog("Read TDP start: read AMD");
                        runAMDReadTDP();
                    }
                }
                Thread.Sleep(200);
                


                if (Properties.Settings.Default.autoApplySetTDP)
                {
                    Task_Scheduler.Task_Scheduler.runTask(() => TDP_Management.changeTDP((int)Global_Variables.Global_Variables.setPL1, (int)Global_Variables.Global_Variables.setPL2));
                }


            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  Reading TDP: " + ex.Message;
                Log_Writer.writeLog(errorMsg);
                MessageBox.Show(errorMsg);

                //return "Error";
            }

        }
        //Change TDP routines - Intel
        public static void changeTDP(int pl1TDP, int pl2TDP)
        {
            //Return Success as default value, otherwise alert calling routine to error
            try
            {
                //check to make sure input TDP is not above maximum set and minimum 5
                if (pl1TDP < Properties.Settings.Default.minTDP) { pl1TDP = Properties.Settings.Default.minTDP; }
                if (pl2TDP < Properties.Settings.Default.minTDP) { pl2TDP = Properties.Settings.Default.minTDP; }
                if (pl1TDP > Properties.Settings.Default.maxTDP) { pl1TDP = Properties.Settings.Default.maxTDP; }
                if (pl2TDP > Properties.Settings.Default.maxTDP) { pl2TDP = Properties.Settings.Default.maxTDP; }


                if (Global_Variables.Global_Variables.cpuType == "Intel")
                {
                    if (Properties.Settings.Default.IntelMMIOMSR.Contains("MMIO"))
                    {
                        //Log_Writer.writeLog("Start intel change TDP MMIO");
                        runIntelTDPChangeMMIOKX(pl1TDP, pl2TDP);
                    }
                    if (Properties.Settings.Default.IntelMMIOMSR.Contains("MSR"))
                    { //Log_Writer.writeLog("Start intel change TDP MSR");
                        runIntelTDPChangeMSR(pl1TDP, pl2TDP);
                    }

                }
                else
                {
                    if (Global_Variables.Global_Variables.cpuType == "AMD")
                    {
                        //Log_Writer.writeLog("Start AMD change TDP");
                        //6800U MessageBox.Show("change TDP to " + pl1TDP.ToString() + ", " + pl2TDP.ToString() + ", " + processorName);
                        runAMDTDPChange(pl1TDP, pl2TDP);
                    }
                }
                Global_Variables.Global_Variables.setPL1 = pl1TDP;
                Global_Variables.Global_Variables.setPL2 = pl2TDP;

                //read tdp after changing
                readTDP();
                
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  Changing TDP Intel or AMD handler: " + ex.Message;
                Log_Writer.writeLog(errorMsg);
                MessageBox.Show(errorMsg);

            }

        }

        static void runIntelTDPChangeMMIOKX(int pl1TDP, int pl2TDP)
        {
            string processKX = "";
            string hexPL1 = "";
            string hexPL2 = "";
            string commandArgumentsPL1 = "";
            string commandArgumentsPL2 = "";
            try
            {

                processKX = BaseDir + "\\Resources\\Intel\\KX\\KX.exe";
                hexPL1 = convertTDPToHexMMIO(pl1TDP);
                hexPL2 = convertTDPToHexMMIO(pl2TDP);
                //Log_Writer.writeLog("Change TDP MMIO processKX=" + processKX + "; Hex PL1 PL2=" + hexPL1 + "," + hexPL2 );
                if (hexPL1 != "Error" && hexPL2 != "Error" && Global_Variables.Global_Variables.MCHBAR != null)
                {
                    lock (objLock)
                    {
                        commandArgumentsPL1 = " /wrmem16 " + Global_Variables.Global_Variables.MCHBAR + "a0 0x" + hexPL1;
                        //Log_Writer.writeLog("Change TDP MMIO commandargumentPL1=" + commandArgumentsPL1);
                        Run_CLI.Run_CLI.RunCommand(commandArgumentsPL1, true, processKX);
                        Thread.Sleep(500);
                        commandArgumentsPL2 = " /wrmem16 " + Global_Variables.Global_Variables.MCHBAR + "a4 0x" + hexPL2;
                        //Log_Writer.writeLog("Change TDP MMIO commandargumentPL2=" + commandArgumentsPL2);
                        Run_CLI.Run_CLI.RunCommand(commandArgumentsPL2, true, processKX);
                        //Log_Writer.writeLog("Change TDP MMIO complete");
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  Run Intel TDP Change MMIOKX: " + ex.Message + " hexPL1 and PL2 are: " + hexPL1 + ", " + hexPL2 + ", processKX is " + processKX + ", commandarguments are " + commandArgumentsPL1 + " and " + commandArgumentsPL2;
                Log_Writer.writeLog(errorMsg);
                MessageBox.Show(errorMsg);

            }


        }


        static void runIntelTDPChangeMSR(int pl1TDP, int pl2TDP)
        {

            string processMSR = "";
            string hexPL1 = "";
            string hexPL2 = "";
            string commandArguments = "";
            try
            {

                hexPL1 = convertTDPToHexMSR(pl1TDP);
                hexPL2 = convertTDPToHexMSR(pl2TDP);

                if (hexPL1 != "Error" && hexPL2 != "Error" && Global_Variables.Global_Variables.MCHBAR != null)
                {
                    lock (objLock)
                    {
                        if (hexPL1.Length < 3)
                        {
                            if (hexPL1.Length == 1) { hexPL1 = "00" + hexPL1; }
                            if (hexPL1.Length == 2) { hexPL1 = "0" + hexPL1; }
                        }
                        if (hexPL2.Length < 3)
                        {
                            if (hexPL2.Length == 1) { hexPL2 = "00" + hexPL2; }
                            if (hexPL2.Length == 2) { hexPL2 = "0" + hexPL2; }
                        }

                        commandArguments = " -s write 0x610 0x00438" + hexPL2 + " 0x00dd8" + hexPL1;
                        processMSR = BaseDir + "\\Resources\\Intel\\MSR\\msr-cmd.exe";
                        //Log_Writer.writeLog("Change TDP MSR processMSR=" + processMSR + "; Hex PL1 PL2=" + hexPL1 + "," + hexPL2);
                        Run_CLI.Run_CLI.RunCommand(commandArguments, false, processMSR);
                        //Log_Writer.writeLog("Change TDP MSR complete");
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  Run Intel TDP Change MSR: " + ex.Message + " Hex pl1 and pl2 are " + hexPL1 + " and " + hexPL2 + ", commandargument is " + commandArguments + ", processMSR is " + processMSR;
                Log_Writer.writeLog(errorMsg);
                MessageBox.Show(errorMsg);

            }


        }
        //End change TDP routines

        public static void checkDriverBlockRegistry()
        {
            if (Global_Variables.Global_Variables.cpuType == "Intel")
            {
                RegistryKey myKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\CI\\Config", true);
                if (myKey != null)
                {
                    if (myKey.GetValue("VulnerableDriverBlocklistEnable") == "1")
                    {
                        var msgBox = MessageBox.Show("","", MessageBoxButton.YesNo);
                        if (msgBox == MessageBoxResult.Yes)
                        {
                            myKey.SetValue("VulnerableDriverBlocklistEnable", "0", RegistryValueKind.String);
                        }
                      
                    }

                    myKey.Close();
                }
            }
        }

        public static void determineCPU()
        {
            //Get the processor name to determine intel vs AMD
            object processorNameRegistry = Registry.GetValue("HKEY_LOCAL_MACHINE\\hardware\\description\\system\\centralprocessor\\0", "ProcessorNameString", null);
            string processorName;
           
            if (processorNameRegistry != null)
            {
                //If not null, find intel or AMD string and clarify type. For Intel determine MCHBAR for rw.exe
                processorName = processorNameRegistry.ToString();
                if (processorName.IndexOf("Intel") >= 0) {Global_Variables.Global_Variables.cpuType = "Intel"; }
                if (processorName.IndexOf("AMD") >= 0) { Global_Variables.Global_Variables.cpuType = "AMD"; }
                Global_Variables.Global_Variables.processorName = processorName;

                if (Global_Variables.Global_Variables.cpuType == "Intel" && Properties.Settings.Default.IntelMMIOMSR.Contains("MMIO"))
                {
                    determineIntelMCHBAR();
                }

            }
        }

        static void determineIntelMCHBAR()
        {

            //Get the processor model to determine MCHBAR, INTEL ONLY
            object processorModelRegistry = Registry.GetValue("HKEY_LOCAL_MACHINE\\hardware\\description\\system\\centralprocessor\\0", "Identifier", null);
            string processorModel = null;
            if (processorModelRegistry != null)
            {
                //If not null, convert to string and determine MCHBAR for rw.exe
                processorModel = processorModelRegistry.ToString();
                if (processorModel.IndexOf("Model 140") >= 0) { Global_Variables.Global_Variables.MCHBAR = "0xFEDC59"; } else { Global_Variables.Global_Variables.MCHBAR = "0xFED159"; };
            }

        }

        //MMIO Stuff here
        static string convertTDPToHexMMIO(int tdp)
        {
            //Convert integer TDP value to Hex for rw.exe
            //Must use formula (TDP in watt   *1000/125) +32768 and convert to hex
            try
            {
                int newTDP = (tdp * 1000 / 125) + 32768;
                return newTDP.ToString("X");

            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  convert MMIO TDP To Hex: " + ex.Message;
                Log_Writer.writeLog(errorMsg);
                MessageBox.Show(errorMsg);
                return "Error";
            }
        }


        static void runIntelReadTDPMMIOKX()
        {
            string processKX = "";
            string commandArgumentsPL1 = "";
            string resultPL1 = "";
            string commandArgumentsPL2 = "";
            string resultPL2 = "";

            try
            {
                processKX = BaseDir + "\\Resources\\Intel\\KX\\KX.exe";
                if (Global_Variables.Global_Variables.MCHBAR != null)
                {
                    lock (objLock)
                    {
                        commandArgumentsPL1 = " /rdmem16 " + Global_Variables.Global_Variables.MCHBAR + "a0";
                        //Log_Writer.writeLog("Read TDP MMIO processKX=" + processKX + "; commandarugmentPL1=" + commandArgumentsPL1 );
                        resultPL1 = Run_CLI.Run_CLI.RunCommand(commandArgumentsPL1, true, processKX);

                        if (resultPL1 != null)
                        {
                            //Log_Writer.writeLog("Read TDP MMIO resultpl1=" + resultPL1);
                            double dblPL1 = Convert.ToDouble(parseHexFromResultMMIOConvertToTDPKX(resultPL1, true));
                            Global_Variables.Global_Variables.readPL1 = dblPL1;
                            //Log_Writer.writeLog("Read TDP MMIO pl1=" + dblPL1.ToString());
                        }
                        Thread.Sleep(300);
                        commandArgumentsPL2 = " /rdmem16 " + Global_Variables.Global_Variables.MCHBAR + "a4";
                        //Log_Writer.writeLog("Read TDP MMIO processKX=" + processKX + "; commandarugmentPL2=" + commandArgumentsPL2);
                        resultPL2 = Run_CLI.Run_CLI.RunCommand(commandArgumentsPL2, true, processKX);
                        if (resultPL2 != null)
                        {
                            //Log_Writer.writeLog("Read TDP MMIO resultpl2=" + resultPL2);
                            double dblPL2 = Convert.ToDouble(parseHexFromResultMMIOConvertToTDPKX(resultPL2, false));
                            Global_Variables.Global_Variables.readPL2 = dblPL2;
                            //Log_Writer.writeLog("Read TDP MMIO pl2=" + dblPL2.ToString());
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Unable to get MCHBAR for intel CPU");

                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs: Reading intel tdp: " + ex.Message + ", processKX is " + processKX + ", result pl1 and pl2 are " + resultPL1 + ", " + resultPL2 + ", command arguments are " + commandArgumentsPL1 + " and " + commandArgumentsPL2;
                Log_Writer.writeLog(errorMsg);
                MessageBox.Show(errorMsg);

            }

        }
        static string parseHexFromResultMMIOConvertToTDPKX(string result, bool isPL1)
        {
            try
            {
                int FindString;
                string hexResult;
                float intResult;
                if (isPL1)
                {
                    FindString = result.IndexOf("Memory Data") + 22;
                    hexResult = result.Substring(FindString, 7).Trim();
                    intResult = (Convert.ToInt32(hexResult, 16) - 32768) / 8;
                    return intResult.ToString();

                }
                else
                {
                    FindString = result.IndexOf("Memory Data") + 22;
                    hexResult = result.Substring(FindString, 7).Trim();
                    intResult = (Convert.ToInt32(hexResult, 16) - 32768) / 8;
                    return intResult.ToString();
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  Parse intel tdp from result: " + ex.Message;
                Log_Writer.writeLog(errorMsg);
                MessageBox.Show(errorMsg);
                return "Error";
            }



        }
        //MSR stuff here


        static string parseHexFromResultMSRConvertToTDP(string result, bool isPL1)
        {
            int FindString = -1;
            string hexResult = "";
            try
            {

                float intResult;
                if (isPL1)
                {
                    FindString = result.IndexOf("0x00000610") + 29;
                    hexResult = result.Substring(FindString, 3).Trim();
                    intResult = (Convert.ToInt32(hexResult, 16)) / 8;
                    return intResult.ToString();

                }
                else
                {
                    FindString = result.IndexOf("0x00000610") + 18;
                    hexResult = result.Substring(FindString, 3).Trim();
                    intResult = (Convert.ToInt32(hexResult, 16)) / 8;
                    return intResult.ToString();
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  Parse intel tdp from result: " + ex.Message + "hexresult is " + hexResult + " and intFindString is " + FindString.ToString();
                Log_Writer.writeLog(errorMsg);
                MessageBox.Show(errorMsg);
                return "Error";
            }



        }
        static void runIntelReadTDPMSR()
        {
            try
            {
                string processMSR = BaseDir + "\\Resources\\Intel\\MSR\\msr-cmd.exe";

                lock (objLock)
                {
                    string commandArguments = " read 0x610";
                    //Log_Writer.writeLog("Read TDP MSR processMSR=" + processMSR + "; commandarugmentPL1=" + commandArguments);
                    string result = Run_CLI.Run_CLI.RunCommand(commandArguments, true, processMSR);
                    if (result != null)
                    {
                        //Log_Writer.writeLog("Read TDP MSR result=" + result);
                        double dblPL1 = Convert.ToDouble(parseHexFromResultMSRConvertToTDP(result, true));
                        Global_Variables.Global_Variables.readPL1 = dblPL1;

                        double dblPL2 = Convert.ToDouble(parseHexFromResultMSRConvertToTDP(result, false));
                        Global_Variables.Global_Variables.readPL2 = dblPL2;
                        //Log_Writer.writeLog("Read TDP MSR PL1=" + dblPL1.ToString() + "; PL1=" + dblPL2.ToString());
                    }
                }


            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs: Reading intel tdp MSR: " + ex.Message;
                Log_Writer.writeLog(errorMsg);
                MessageBox.Show(errorMsg);
            }

        }
        static string convertTDPToHexMSR(int tdp)
        {
            //Convert integer TDP value to Hex for rw.exe
            //Must use formula (TDP in watt   *1000/125) +32768 and convert to hex
            try
            {
                int newTDP = (tdp * 8);
                return newTDP.ToString("X");

            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  convert MSR TDP To Hex: " + ex.Message;
                Log_Writer.writeLog(errorMsg);
                MessageBox.Show(errorMsg);
                return "Error";
            }
        }
        //MSR stuff above
        static void runAMDReadTDP()
        {
            string processRyzenAdj = "";
            string result = "";
            string commandArguments = "";
    
            if (Global_Variables.Global_Variables.processorName.Contains("APU 0405"))
            {
                Global_Variables.Global_Variables.readPL1 = Global_Variables.Global_Variables.setPL1;
                Global_Variables.Global_Variables.readPL2 = Global_Variables.Global_Variables.setPL2;
            }
            else
            {
                try
                {
                    processRyzenAdj = BaseDir + "\\Resources\\AMD\\RyzenAdj\\ryzenadj.exe";

                    lock (objLock)
                    {
                        commandArguments = " -i";
                        //Log_Writer.writeLog("Read TDP AMD processRyzenAj=" + processRyzenAdj + "; commandarugment=" + commandArguments);

                        result = Run_CLI.Run_CLI.RunCommand(commandArguments, true, processRyzenAdj);

                        if (result != null)
                        {

                            using (StringReader reader = new StringReader(result))
                            {
                                string line;
                                string tdp;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    double n = 0;
                                    if (line.Contains("STAPM LIMIT"))
                                    {

                                        tdp = line;
                                        tdp = tdp.Replace("|", "");
                                        tdp = tdp.Replace("STAPM LIMIT", "");
                                        tdp = tdp.Replace(" ", "");
                                        tdp = tdp.Replace("stapm-limit", "");
                                        tdp = tdp.Substring(0, tdp.IndexOf("."));
                                        Global_Variables.Global_Variables.readPL1 = Convert.ToDouble(tdp);
                                    }
                                    if (line.Contains("PPT LIMIT SLOW"))
                                    {
                                        tdp = line;
                                        tdp = tdp.Replace("|", "");
                                        tdp = tdp.Replace("PPT LIMIT SLOW", "");
                                        tdp = tdp.Replace(" ", "");
                                        tdp = tdp.Replace("slow-limit", "");
                                        tdp = tdp.Substring(0, tdp.IndexOf("."));
                                        Global_Variables.Global_Variables.readPL2 = Convert.ToDouble(tdp);
                                        break;
                                    }
                                }
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    string errorMsg = "Error: ChangeTDP.cs:  Run AMD TDP Read: " + ex.Message + ", processRyzenAdj is " + processRyzenAdj + ", result is " + result + ", commandargument is " + commandArguments;
                    Log_Writer.writeLog(errorMsg);
                    MessageBox.Show(errorMsg);

                }
            }



        }
        static void runAMDTDPChange(int pl1TDP, int pl2TDP)
        {
            string processRyzenAdj = "";
            string result = "";
            string commandArguments = "";
            try
            {
                processRyzenAdj = BaseDir + "\\Resources\\AMD\\RyzenAdj\\ryzenadj.exe";
                //6800U MessageBox.Show("going to change TDP");
                lock (objLock)
                {
                    commandArguments = " --stapm-limit=" + (pl1TDP * 1000).ToString() + " --slow-limit=" + (pl2TDP * 1000).ToString() + " --fast-limit=" + (pl2TDP * 1000).ToString();
         
                    result = Run_CLI.Run_CLI.RunCommand(commandArguments, true, processRyzenAdj);
                    Thread.Sleep(100);
                    commandArguments = " --apu-slow-limit=" + (pl1TDP * 1000).ToString();

                    result = Run_CLI.Run_CLI.RunCommand(commandArguments, true, processRyzenAdj);
                    Thread.Sleep(100);
                    //6800U MessageBox.Show(result);
                    //Log_Writer.writeLog("Read TDP AMD complete");
                }


            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  Run AMD TDP Change: " + ex.Message + ", processRyzenAdj is " + processRyzenAdj + ", result is " + result + ", commandargument is " + commandArguments; ;
                Log_Writer.writeLog(errorMsg);
                MessageBox.Show(errorMsg);

            }


        }

   

    }
 

}

