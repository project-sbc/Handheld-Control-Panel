
using Handheld_Control_Panel.Classes.Global_Variables;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Xml;
//using static Vanara.Interop.KnownShellItemPropertyKeys;

namespace Handheld_Control_Panel.Classes.XML_Management
{
    public static class Load_XML_File
    {
        public static void load_XML_File()
        {
            //first run of app, check if profile xml is there, add directory to global variables file
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Profiles.xml") == false)
            {
                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Profiles_Template.xml", AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Profiles.xml");
            }


        }
    }
    public static class Manage_XML_Profiles
    {
        //in use stuff
        public static void generateGlobalVariableProfileToExeList()
        {
            //loads the global variable dictionary with 
            if (Global_Variables.Global_Variables.profiles.Count != 0)
            { //Global_Variables.Global_Variables.Profiles.Clear(); }

                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");

                Dictionary<string, string> profiles = new Dictionary<string, string>();

                //Global_Variables.Global_Variables.Profiles.Clear();
                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    if (node.SelectSingleNode("Exe").InnerText != "")
                    {
                        //Global_Variables.Global_Variables.Profiles.Add(node.SelectSingleNode("Exe").InnerText, node.SelectSingleNode("ID").InnerText);
                    }

                }

                xmlDocument = null;



            }
        }
        public static DataTable profilesDatatable()
        {

            //turn this into hotkey table
            DataTable dt = new DataTable();
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");

            dt.Columns.Add("ID");
            dt.Columns.Add("ProfileName");
            dt.Columns.Add("Exe");

            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                string ID = node.SelectSingleNode("ID").InnerText;
                string ProfileName = node.SelectSingleNode("ProfileName").InnerText;
                string Exe = node.SelectSingleNode("Exe").InnerText;

                dt.Rows.Add(node.SelectSingleNode("ID").InnerText, ProfileName, Exe);
            }
            xmlDocument = null;
            return dt;


        }
        public static List<string> generateProfileListHomePage()
        {

            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");

            List<string> profiles = new List<string>();

            profiles.Add("None");
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                profiles.Add(node.SelectSingleNode("ProfileName").InnerText);
            }
            return profiles;
            xmlDocument = null;

        }
        public static void createProfile()
        {

            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNodeTemplate = xmlDocument.SelectSingleNode("//Configuration/ProfileTemplate/Profile");
            XmlNode xmlNodeProfiles = xmlDocument.SelectSingleNode("//Configuration/Profiles");


            string newProfileName = "NewProfile";
            int countProfile = 0;
            XmlNodeList xmlNodesByName = xmlNodeProfiles.SelectNodes("Profile/ProfileName[text()='" + newProfileName + "']");

            if (xmlNodesByName.Count > 0)
            {
                while (xmlNodesByName.Count > 0)
                {
                    countProfile++;
                    xmlNodesByName = xmlNodeProfiles.SelectNodes("Profile/ProfileName[text()='" + newProfileName + countProfile.ToString() + "']");

                }
                newProfileName = newProfileName + countProfile.ToString();
            }


            XmlNode newNode = xmlDocument.CreateNode(XmlNodeType.Element, "Profile", "");
            newNode.InnerXml = xmlNodeTemplate.InnerXml;
            newNode.SelectSingleNode("ProfileName").InnerText = newProfileName;
            newNode.SelectSingleNode("ID").InnerText = getNewIDNumberForProfile();
            xmlNodeProfiles.AppendChild(newNode);



            xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);

            xmlDocument = null;

        }

        public static void createProfileForSteamGame(string profileName, string gameID)
        {

            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNodeTemplate = xmlDocument.SelectSingleNode("//Configuration/ProfileTemplate/Profile");
            XmlNode xmlNodeProfiles = xmlDocument.SelectSingleNode("//Configuration/Profiles");



            XmlNode newNode = xmlDocument.CreateNode(XmlNodeType.Element, "Profile", "");
            newNode.InnerXml = xmlNodeTemplate.InnerXml;
            newNode.SelectSingleNode("ProfileName").InnerText = profileName;
            newNode.SelectSingleNode("ID").InnerText = getNewIDNumberForProfile();
            newNode.SelectSingleNode("LaunchOptions/GameID").InnerText = gameID;
            newNode.SelectSingleNode("LaunchOptions/AppType").InnerText = "Steam";

            xmlNodeProfiles.AppendChild(newNode);



            xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);

            xmlDocument = null;

        }
        public static void createProfileForEpicGame(string profileName, string path, string gameID)
        {

            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNodeTemplate = xmlDocument.SelectSingleNode("//Configuration/ProfileTemplate/Profile");
            XmlNode xmlNodeProfiles = xmlDocument.SelectSingleNode("//Configuration/Profiles");



            XmlNode newNode = xmlDocument.CreateNode(XmlNodeType.Element, "Profile", "");
            newNode.InnerXml = xmlNodeTemplate.InnerXml;
            newNode.SelectSingleNode("ProfileName").InnerText = profileName;
            newNode.SelectSingleNode("ID").InnerText = getNewIDNumberForProfile();
            newNode.SelectSingleNode("LaunchOptions/GameID").InnerText = gameID;
            newNode.SelectSingleNode("LaunchOptions/Path").InnerText = path;
            newNode.SelectSingleNode("LaunchOptions/AppType").InnerText = "EpicGames";

            xmlNodeProfiles.AppendChild(newNode);



            xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);

            xmlDocument = null;

        }
        public static string lookUpExeOfActiveProfile()
        {
            string result = "";
            if (Global_Variables.Global_Variables.profiles.activeProfile != null)
            {
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
                XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ID[text()='" + Global_Variables.Global_Variables.profiles.activeProfile.ID + "']");

                if (xmlSelectedNode != null)
                {
                    result = xmlSelectedNode.ParentNode.SelectSingleNode("Exe").InnerText;
                }

                xmlDocument = null;

            }
            return result;
        }
        public static string getNewIDNumberForProfile()
        {

            //0 is reserved for default profile
            int ID = 1;

            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ID[text()='" + ID.ToString() + "']");


            while (xmlSelectedNode != null)
            {
                ID = ID + 1;
                xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ID[text()='" + ID.ToString() + "']");
            }
            //ID++;
            return ID.ToString();

        }
        public static string removeAsDefaultProfile(string ID)
        {
            string newID = "";
            if (ID == "0")
            {
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
                XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ID[text()='0']");
                if (xmlSelectedNode != null)
                {
                    newID = getNewIDNumberForProfile();
                    xmlSelectedNode.InnerText = newID;
                    xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);
                }

                xmlDocument = null;

            }
            return newID;

        }
        public static void setDefaultProfile(string ID)
        {
            if (ID != "0")
            {
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
                XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ID[text()='0']");

                if (xmlSelectedNode != null)
                {
                    xmlSelectedNode.InnerText = getNewIDNumberForProfile();
                }


                XmlNode xmlNewDefaultNode = xmlNode.SelectSingleNode("Profile/ID[text()='" + ID + "']");
                if (xmlNewDefaultNode != null)
                {
                    xmlNewDefaultNode.InnerText = "0";
                }
                xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);
                xmlDocument = null;
            }



        }


        public static void deleteProfile(string ID)
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNodeProfiles = xmlDocument.SelectSingleNode("//Configuration/Profiles");

            foreach (XmlNode node in xmlNodeProfiles.ChildNodes)
            {
                if (node.SelectSingleNode("ID").InnerText == ID)
                {
                    xmlNodeProfiles.RemoveChild(node);
                    break;
                }

            }

            xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);
            xmlDocument = null;
        }
        public static void saveProfileArray(Dictionary<string, string> result, string ID)
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ID[text()='" + ID + "']");
            if (xmlSelectedNode != null)
            {
                XmlNode parentNode = xmlSelectedNode.ParentNode;

                if (parentNode != null)
                {
                    XmlNode onlineNode = parentNode.SelectSingleNode("Online");
                    foreach (XmlNode node in onlineNode.ChildNodes)
                    {
                        string value;
                        if (result.TryGetValue("Online_" + node.Name, out value))
                        {
                            node.InnerText = value;
                        }
                        else
                        {
                            node.InnerText = "";
                        }


                    }

                    XmlNode offlineNode = parentNode.SelectSingleNode("Offline");
                    foreach (XmlNode node in offlineNode.ChildNodes)
                    {
                        string value;
                        if (result.TryGetValue("Offline_" + node.Name, out value))
                        {
                            node.InnerText = value;
                        }
                        else
                        {
                            node.InnerText = "";
                        }
                    }

                    //profile and exe
                    string profilename;
                    if (result.TryGetValue("ProfileName", out profilename))
                    {
                        parentNode.SelectSingleNode("ProfileName").InnerText = profilename;
                    }
                    string exe;
                    if (result.TryGetValue("Exe", out exe))
                    {
                        parentNode.SelectSingleNode("Exe").InnerText = exe;
                    }

                    XmlNode LaunchOptions = parentNode.SelectSingleNode("LaunchOptions");
                    foreach (XmlNode node in LaunchOptions.ChildNodes)
                    {
                        string value;
                        if (result.TryGetValue(node.Name, out value))
                        {
                            node.InnerText = value;
                        }
                        else
                        {
                            node.InnerText = "";
                        }
                    }


                }


            }
            xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);

            xmlDocument = null;


        }
        public static Dictionary<string, string> loadProfileArray(string ID)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ID[text()='" + ID + "']");
            if (xmlSelectedNode != null)
            {
                XmlNode parentNode = xmlSelectedNode.ParentNode;

                if (parentNode != null)
                {
                    XmlNode onlineNode = parentNode.SelectSingleNode("Online");
                    foreach (XmlNode node in onlineNode.ChildNodes)
                    {
                        result.Add("Online_" + node.Name, node.InnerText);

                    }

                    XmlNode offlineNode = parentNode.SelectSingleNode("Offline");
                    foreach (XmlNode node in offlineNode.ChildNodes)
                    {
                        result.Add("Offline_" + node.Name, node.InnerText);
                    }

                    result.Add("ProfileName", parentNode.SelectSingleNode("ProfileName").InnerText);
                    result.Add("Exe", parentNode.SelectSingleNode("Exe").InnerText);

                    XmlNode LaunchOptions = parentNode.SelectSingleNode("LaunchOptions");
                    foreach (XmlNode node in LaunchOptions.ChildNodes)
                    {
                        result.Add(node.Name, node.InnerText);
                    }

                }

            }

            xmlDocument = null;
            return result;
        }
        public static List<string[]> appListForAppPage()
        {
            List<string[]> result = new List<string[]>();
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNodes = xmlDocument.SelectSingleNode("//Configuration/Profiles");

            foreach (XmlNode node in xmlNodes.ChildNodes)
            {
                if (node.SelectSingleNode("LaunchOptions/AppType").InnerText == "Steam" || ((node.SelectSingleNode("LaunchOptions/AppType").InnerText == "App" || node.SelectSingleNode("LaunchOptions/AppType").InnerText == "EpicGames") && node.SelectSingleNode("LaunchOptions/Path").InnerText != ""))
                {
                    string[] row = new string[5];
                    row[0] = node.SelectSingleNode("ID").InnerText;
                    row[1] = node.SelectSingleNode("ProfileName").InnerText;
                    row[2] = node.SelectSingleNode("LaunchOptions/AppType").InnerText;
                    row[3] = node.SelectSingleNode("LaunchOptions/GameID").InnerText;
                    row[4] = node.SelectSingleNode("LaunchOptions/Path").InnerText;
                    result.Add(row);
                }
            }


            return result;

        }

        public static void openProgram(string ID)
        {


            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ID[text()='" + ID + "']");

            if (xmlSelectedNode != null)
            {
                XmlNode profileNode = xmlSelectedNode.ParentNode;

                string appType = profileNode.SelectSingleNode("LaunchOptions/AppType").InnerText;
                string path = profileNode.SelectSingleNode("LaunchOptions/Path").InnerText;
                if (appType == "Steam")
                {
                    Steam_Management.openSteamGame(path);
                }
                else
                {
                    System.Diagnostics.Process.Start(path);
                }
            }


        }
        public static void syncSteamGameToProfile()
        {
            //gets list of steam games from library.vdf file, then makes profiles for those without one

            Dictionary<string, string> result = Steam_Management.syncSteam_Library();

            if (result.Count > 0)
            {
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");

                foreach (KeyValuePair<string, string> pair in result)
                {
                    XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/LaunchOptions/GameID[text()='" + pair.Key + "']");
                    if (xmlSelectedNode == null)
                    {
                        createProfileForSteamGame(pair.Value, pair.Key);
                    }
                }


                xmlDocument = null;

            }



        }
        public static void syncEpicGameToProfile()
        {
            //gets list of steam games from library.vdf file, then makes profiles for those without one

            List<EpicGamesLauncherItem> result = EpicGames_Management.syncEpic_Library();

            if (result.Count > 0)
            {
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");

                foreach (EpicGamesLauncherItem item in result)
                {
                    XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/LaunchOptions/GameID[text()='" + item.gameID + "']");
                    if (xmlSelectedNode == null)
                    {
                        createProfileForEpicGame(item.gameName, item.installPath, item.gameID);
                    }
                }


                xmlDocument = null;

            }



        }
        public static string getProfileNameByID(string ID)
        {

            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ID[text()='" + ID + "']");
            string returnstring = "";
            if (xmlSelectedNode != null)
            {
                XmlNode parentNode = xmlSelectedNode.ParentNode;

                if (parentNode != null)
                {
                    returnstring = parentNode.SelectSingleNode("ProfileName").InnerText;

                }
            }
            xmlDocument = null;
            return returnstring;
        }

        public static void applyProfile(string ID, bool profileAutoApplied)
        {
            if (ID != "")
            {
                string powerStatus = SystemParameters.PowerLineStatus.ToString();
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
                XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ID[text()='" + ID + "']");
                if (xmlSelectedNode != null)
                {
                    XmlNode parentNode = xmlSelectedNode.ParentNode;

                    if (parentNode != null)
                    {
                        Global_Variables.Global_Variables.profiles.activeProfile.ID = ID;

                        XmlNode powerNode;
                        if (powerStatus == "Online")
                        {
                            powerNode = parentNode.SelectSingleNode("Online");
                        }
                        else
                        {
                            powerNode = parentNode.SelectSingleNode("Offline");
                        }
                        string tdp1 = "";

                        foreach (XmlNode node in powerNode.ChildNodes)
                        {
                            if (node.InnerText != "")
                            {
                                switch (node.Name)
                                {
                                    case "TDP1":
                                        tdp1 = node.InnerText;
                                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => TDP_Management.TDP_Management.changeTDP(Convert.ToInt32(tdp1), Convert.ToInt32(tdp1)));
                                        break;
                                    case "TDP2":
                                        if (tdp1 != "")
                                        {
                                            Classes.Task_Scheduler.Task_Scheduler.runTask(() => TDP_Management.TDP_Management.changeTDP(Convert.ToInt32(tdp1), Convert.ToInt32(node.InnerText)));
                                        }
                                        break;
                                    case "GPUCLK":
                                        if (Global_Variables.Global_Variables.cpuType == "AMD")
                                        {
                                            Classes.Task_Scheduler.Task_Scheduler.runTask(() => GPUCLK_Management.GPUCLK_Management.changeAMDGPUClock(Convert.ToInt32(node.InnerText)));
                                        }
                                        break;
                                    case "EPP":
                                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => EPP_Management.EPP_Management.changeEPP(Convert.ToInt32(node.InnerText)));
                                        break;
                                    case "MAXCPU":
                                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => MaxProcFreq_Management.MaxProcFreq_Management.changeCPUMaxFrequency(Convert.ToInt32(node.InnerText)));
                                        break;
                                    case "ActiveCores":
                                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => CoreParking_Management.CoreParking_Management.changeActiveCores(Convert.ToInt32(node.InnerText)));
                                        break;
                                    case "FPSLimit":
                                        Classes.Task_Scheduler.Task_Scheduler.runTask(() => RTSS.setRTSSFPSLimit(Convert.ToInt32(node.InnerText)));
                                        break;

                                    default: break;
                                }


                            }



                        }

                    }

                    Global_Variables.Global_Variables.profileAutoApplied = profileAutoApplied;
                }
                else
                {
                    //if profile is default and no profile was detected make activeprofile none
                    Global_Variables.Global_Variables.profiles.activeProfile = null;
                    Global_Variables.Global_Variables.profileAutoApplied = false;


                }
                xmlDocument = null;
            }



        }
        //
        #region not in use
        public static List<string> profileListForAppCBO()
        {
            List<string> cboAppProfile = new List<string>();
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");



            foreach (XmlNode node in xmlNode.ChildNodes)
            {

                cboAppProfile.Add(node.SelectSingleNode("ProfileName").InnerText);
            }
            xmlDocument = null;
            return cboAppProfile;


        }



        public static string loadProfileParameter(string powerStatus, string parameter, string profileName)
        {
            string result = "";
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ProfileName[text()='" + profileName + "']");
            if (xmlSelectedNode != null)
            {
                XmlNode parentNode = xmlSelectedNode.ParentNode;
                XmlNode parameterNode = parentNode.SelectSingleNode(powerStatus + "/" + parameter);
                result = parameterNode.InnerText;
            }

            xmlDocument = null;
            return result;
        }
        public static void changeProfileParameter(string powerStatus, string parameter, string profileName, string newValue)
        {
            string result = "";
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ProfileName[text()='" + profileName + "']");
            if (xmlSelectedNode != null)
            {
                XmlNode parentNode = xmlSelectedNode.ParentNode;
                XmlNode parameterNode = parentNode.SelectSingleNode(powerStatus + "/" + parameter);
                parameterNode.InnerText = newValue;
            }

            xmlDocument = null;

        }

        public static void changeProfileName(string oldName, string newName)
        {

            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
            XmlNodeList xmlSelectedNodes = xmlNode.SelectNodes("Profile/ProfileName[text()='" + oldName + "']");
            if (xmlSelectedNodes != null)
            {
                foreach (XmlNode xmlNode1 in xmlSelectedNodes)
                {

                    if (xmlNode1.Name == "ProfileName")
                    {
                        if (xmlNode1.InnerText == oldName) { xmlNode1.InnerText = newName; }

                    }

                }


            }

            xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);

            xmlDocument = null;

        }

        #endregion



    }
    public static class Manage_XML_ControllerHotKeys
        {


            public static string getNewIDNumberForHotKey()
            {
                int ID = 1;

                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/ControllerHotKeys");
                XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("ControllerHotKey/ID[text()='" + ID.ToString() + "']");


                while (xmlSelectedNode == null)
                {
                    ID = ID + 1;
                    xmlSelectedNode = xmlNode.SelectSingleNode("ControllerHotKey/ID[text()='" + ID.ToString() + "']");
                }
                return ID.ToString();

                xmlDocument = null;

            }
            public static void loadGlobalDictionaryForKBHotKeys()
            {
                Dictionary<string, ActionParameter> KBHotKey = new Dictionary<string, ActionParameter>();

                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/ControllerHotKeys");

                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    if (node.SelectSingleNode("Action").InnerText != "" && node.SelectSingleNode("Type").InnerText == "Keyboard")
                    {
                        string hotKey = node.SelectSingleNode("Hotkey").InnerText;
                        string action = node.SelectSingleNode("Action").InnerText;
                        string parameter = node.SelectSingleNode("Parameter").InnerText;

                        ActionParameter actpar = new ActionParameter();
                        actpar.Parameter = parameter;
                        actpar.Action = action;
                        KBHotKey.Add(hotKey, actpar);
                    }

                }
                //clear the list if it isnt null - meaning reset for any changes
                if (Global_Variables.Global_Variables.KBHotKeyDictionary != null) { Global_Variables.Global_Variables.KBHotKeyDictionary.Clear(); }

                Global_Variables.Global_Variables.KBHotKeyDictionary = KBHotKey;

            }
            public static void loadGlobalDictionaryForControllerHotKeys()
            {
                Dictionary<ushort, ActionParameter> controllerHotKey = new Dictionary<ushort, ActionParameter>();

                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/ControllerHotKeys");

                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    if (node.SelectSingleNode("Action").InnerText != "" && node.SelectSingleNode("Type").InnerText == "Controller")
                    {
                        ushort hotKey = Convert.ToUInt16(node.SelectSingleNode("Hotkey").InnerText);
                        string action = node.SelectSingleNode("Action").InnerText;
                        string parameter = node.SelectSingleNode("Parameter").InnerText;

                        ActionParameter actpar = new ActionParameter();
                        actpar.Parameter = parameter;
                        actpar.Action = action;
                        controllerHotKey.Add(hotKey, actpar);
                    }

                }
                //clear the list if it isnt null - meaning reset for any changes
                if (Global_Variables.Global_Variables.controllerHotKeyDictionary != null) { Global_Variables.Global_Variables.controllerHotKeyDictionary.Clear(); }

                Global_Variables.Global_Variables.controllerHotKeyDictionary = controllerHotKey;

            }
            public static void saveHotKeyProfile(string ID, string action, string parameter, string hotkey, string type)
            {

                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/ControllerHotKeys");
                XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("ControllerHotKey/ID[text()='" + ID + "']");
                if (xmlSelectedNode != null)
                {
                    XmlNode parentNode = xmlSelectedNode.ParentNode;
                    XmlNode nodeHotKey = parentNode.SelectSingleNode("HotKey");
                    nodeHotKey.InnerText = hotkey;
                    XmlNode nodeAction = parentNode.SelectSingleNode("Action");
                    nodeAction.InnerText = action;
                    XmlNode nodeParameter = parentNode.SelectSingleNode("Parameter");
                    nodeParameter.InnerText = parameter;
                    XmlNode nodeType = parentNode.SelectSingleNode("Type");
                    nodeType.InnerText = type;
                    xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);
                }

                xmlDocument = null;
                loadGlobalDictionaryForControllerHotKeys();
                loadGlobalDictionaryForKBHotKeys();

            }
            public static DataTable hotkeyDatatable()
            {
                //turn this into hotkey table
                DataTable dt = new DataTable();
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/ControllerHotKeys");

                dt.Columns.Add("ID");
                dt.Columns.Add("Type");
                dt.Columns.Add("Action");
                dt.Columns.Add("HotKeyDisplayText");
                dt.Columns.Add("Parameter");
                dt.Columns.Add("HotKeyValue");

                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    string type = "";
                    if (node.SelectSingleNode("Type").InnerText != "") { type = System.Windows.Application.Current.Resources[node.SelectSingleNode("Type").InnerText].ToString(); }
                    string hotkey = node.SelectSingleNode("Hotkey").InnerText;
                    string action = "";
                    if (node.SelectSingleNode("Action").InnerText != "") { type = System.Windows.Application.Current.Resources[node.SelectSingleNode("Action").InnerText].ToString(); }
                    ushort hotkeyUshort;
                    string hotkeyConverted;
                    if (ushort.TryParse(hotkey, out hotkeyUshort))
                    {
                        hotkeyConverted = HotKey_Management.convertControllerUshortToString(Convert.ToUInt16(hotkeyUshort));
                    }
                    else
                    {
                        hotkeyConverted = hotkey;
                    }

                    dt.Rows.Add(node.SelectSingleNode("ID").InnerText, type, action, hotkeyConverted, node.SelectSingleNode("Parameter").InnerText, hotkey);
                }
                xmlDocument = null;
                return dt;


            }

            public static void createHotKey()
            {
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNodeTemplate = xmlDocument.SelectSingleNode("//Configuration/ControllerHotKeyTemplate/ControllerHotKey");
                XmlNode xmlNodeHotKey = xmlDocument.SelectSingleNode("//Configuration/ControllerHotKeys");

                XmlNode newNode = xmlDocument.CreateNode(XmlNodeType.Element, "ControllerHotKey", "");
                newNode.InnerXml = xmlNodeTemplate.InnerXml;
                newNode.SelectSingleNode("ID").InnerText = getNewIDNumberForHotKey();
                newNode.SelectSingleNode("Type").InnerText = "Controller";
                newNode.SelectSingleNode("HotKey").InnerText = "";
                xmlNodeHotKey.AppendChild(newNode);



                xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);

                xmlDocument = null;

            }
            public static void deleteHotKey(string ID)
            {
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNodeHotkeys = xmlDocument.SelectSingleNode("//Configuration/HotKeys");

                foreach (XmlNode node in xmlNodeHotkeys.ChildNodes)
                {
                    if (node.SelectSingleNode("ID").InnerText == ID)
                    {
                        xmlNodeHotkeys.RemoveChild(node);
                        break;
                    }

                }

                xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);
                xmlDocument = null;
            }

        }

        public static class Manage_XML_Apps
        {
            public static DataTable appList()
            {
                DataTable dt = new DataTable();
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Applications");

                dt.Columns.Add("DisplayName");

                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    if (node.SelectSingleNode("DisplayName") != null)
                    {
                        dt.Rows.Add(node.SelectSingleNode("DisplayName").InnerText);
                    }

                }
                xmlDocument = null;
                return dt;


            }

            public static DataTable appListProfileExe()
            {
                DataTable dt = new DataTable();
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Applications");

                dt.Columns.Add("Profile");
                dt.Columns.Add("Exe");
                if (xmlNode.ChildNodes.Count > 0)
                {
                    foreach (XmlNode node in xmlNode.ChildNodes)
                    {
                        if (node.SelectSingleNode("Profile") != null)
                        {
                            if (node.SelectSingleNode("Profile").InnerText != "")
                            {
                                dt.Rows.Add(node.SelectSingleNode("Profile").InnerText, node.SelectSingleNode("Exe").InnerText);
                            }
                        }

                    }
                }

                xmlDocument = null;
                return dt;


            }

            public static string lookupProfileByAppExe(string exe)
            {

                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNodeList xmlNodes = xmlDocument.SelectSingleNode("//Configuration/Applications").SelectNodes("App/Exe[text()='" + exe + "']");
                string profile = "";


                foreach (XmlNode node in xmlNodes)
                {
                    profile = node.ParentNode.SelectSingleNode("Profile").InnerText;
                }
                xmlDocument = null;
                return profile;


            }
            public static DataTable appListByProfile(string profileName)
            {
                DataTable dt = new DataTable();
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNodeList xmlNodes = xmlDocument.SelectSingleNode("//Configuration/Applications").SelectNodes("App/Profile[text()='" + profileName + "']");

                dt.Columns.Add("DisplayName");

                foreach (XmlNode node in xmlNodes)
                {

                    dt.Rows.Add(node.ParentNode.SelectSingleNode("DisplayName").InnerText);
                }
                xmlDocument = null;
                return dt;


            }
            public static void changeProfileNameInApps(string oldName, string newName)
            {

                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Applications");
                XmlNodeList xmlSelectedNodes = xmlNode.SelectNodes("App/Profile[text()='" + oldName + "']");
                if (xmlSelectedNodes != null)
                {
                    foreach (XmlNode xmlNode1 in xmlSelectedNodes)
                    {

                        if (xmlNode1.Name == "Profile")
                        {
                            if (xmlNode1.InnerText == oldName) { xmlNode1.InnerText = newName; }

                        }

                    }


                }

                xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);

                xmlDocument = null;

            }

            public static void createApp(string ProfileName = "")
            {
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNodeTemplate = xmlDocument.SelectSingleNode("//Configuration/AppTemplate/App");
                XmlNode xmlNodeProfiles = xmlDocument.SelectSingleNode("//Configuration/Applications");

                string newAppName = "NewApp";
                if (ProfileName != "")
                {
                    newAppName = ProfileName + "_App";
                }

                int countApp = 0;
                XmlNodeList xmlNodesByName = xmlNodeProfiles.SelectNodes("App/DisplayName[text()='" + newAppName + "']");

                if (xmlNodesByName.Count > 0)
                {
                    while (xmlNodesByName.Count > 0)
                    {
                        countApp++;
                        xmlNodesByName = xmlNodeProfiles.SelectNodes("App/DisplayName[text()='" + newAppName + countApp.ToString() + "']");

                    }
                    newAppName = newAppName + countApp.ToString();
                }

                XmlNode newNode = xmlDocument.CreateNode(XmlNodeType.Element, "App", "");
                newNode.InnerXml = xmlNodeTemplate.InnerXml;
                newNode.SelectSingleNode("DisplayName").InnerText = newAppName;
                newNode.SelectSingleNode("Profile").InnerText = ProfileName;
                xmlNodeProfiles.AppendChild(newNode);

                xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);

                xmlDocument = null;

            }
            public static void deleteApp(string appName)
            {
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNodeProfiles = xmlDocument.SelectSingleNode("//Configuration/Applications");

                foreach (XmlNode node in xmlNodeProfiles.ChildNodes)
                {
                    if (node.SelectSingleNode("DisplayName").InnerText == appName)
                    {
                        xmlNodeProfiles.RemoveChild(node);
                        break;
                    }

                }

                xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);
                xmlDocument = null;
            }


            public static void saveAppArray(string[] result, string appName)
            {
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Applications");
                XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("App/DisplayName[text()='" + appName + "']");
                if (xmlSelectedNode != null)
                {
                    XmlNode parentNode = xmlSelectedNode.ParentNode;

                    if (parentNode != null)
                    {

                        foreach (XmlNode node in parentNode.ChildNodes)
                        {

                            if (node.Name == "Exe") { node.InnerText = result[0]; }
                            if (node.Name == "Path") { node.InnerText = result[1]; }
                            if (node.Name == "AppType") { node.InnerText = result[2]; }
                            if (node.Name == "GameType") { node.InnerText = result[3]; }
                            if (node.Name == "Image") { node.InnerText = result[4]; }
                            if (node.Name == "Profile") { node.InnerText = result[5]; }

                        }


                    }


                }
                xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);

                xmlDocument = null;


            }
            public static string[] loadAppArray(string appName)
            {
                string[] result = new string[6];
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Applications");
                XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("App/DisplayName[text()='" + appName + "']");
                if (xmlSelectedNode != null)
                {
                    XmlNode parentNode = xmlSelectedNode.ParentNode;

                    if (parentNode != null)
                    {

                        foreach (XmlNode node in parentNode.ChildNodes)
                        {
                            // not needed, already gotten if (node.Name == "DisplayName") { result[0] = node.InnerText; }
                            if (node.Name == "Exe") { result[0] = node.InnerText; }
                            if (node.Name == "Path") { result[1] = node.InnerText; }
                            if (node.Name == "AppType") { result[2] = node.InnerText; }
                            if (node.Name == "GameType") { result[3] = node.InnerText; }
                            if (node.Name == "Image") { result[4] = node.InnerText; }
                            if (node.Name == "Profile") { result[5] = node.InnerText; }


                        }



                    }


                }

                xmlDocument = null;
                return result;
            }

            public static string loadAppParameter(string parameter, string appName)
            {
                string result = "";
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Applications");
                XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("App/DisplayName[text()='" + appName + "']");
                if (xmlSelectedNode != null)
                {

                    XmlNode parameterNode = xmlSelectedNode.SelectSingleNode(parameter);
                    result = parameterNode.InnerText;
                }

                xmlDocument = null;
                return result;
            }
            public static void changeAppParameter(string parameter, string appName, string newValue)
            {

                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Applications");
                XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("App/DisplayName[text()='" + appName + "']");
                if (xmlSelectedNode != null)
                {

                    XmlNode parameterNode = xmlSelectedNode.ParentNode.SelectSingleNode(parameter);

                    parameterNode.InnerText = newValue;
                }
                xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);
                xmlDocument = null;

            }


        }
        
    }

