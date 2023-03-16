using SharpDX;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Handheld_Control_Panel.Classes
{
    public class Profiles_Management: List<Profile>
    {
        
        public Profile activeProfile=null;
        public Profile editingProfile = null;
        public Profile defaultProfile = null;
        public Profiles_Management()
        {
            //populates list
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");

           
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                Profile profile = new Profile();

                profile.LoadProfile(node.SelectSingleNode("ID").InnerText, xmlDocument);
                if (node.SelectSingleNode("DefaultProfile").InnerText == "True") { defaultProfile = profile; }
                this.Add(profile);
            }
           // this.OrderBy(o => o.ProfileName);
            xmlDocument = null;          
        }

        public void setCurrentDefaultProfileToFalse(string ID)
        {
            //changes 
            foreach (Profile profile in this)
            {
                if (profile.ID == ID)
                {
                    profile.DefaultProfile = false;
                }
            }

        }
        public string getProfileNameById(string ID)
        {
            string name = "";
            foreach (Profile profile in this)
            {
                if (profile.ID == ID)
                {
                    name = profile.ProfileName;
                }
            }
            return name;
        }
        public void deleteProfile(Profile profile)
        {
            if (profile != null)
            {
                string ID = profile.ID;

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

                this.Remove(profile);
            }
          

        }

        public void addNewProfile(Profile copyProfile)
        {
         
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNodeTemplate = xmlDocument.SelectSingleNode("//Configuration/ProfileTemplate/Profile");

            XmlNode xmlNodeProfiles = xmlDocument.SelectSingleNode("//Configuration/Profiles");

            if (copyProfile != null)
            {

                foreach (XmlNode node in xmlNodeProfiles.ChildNodes)
                {
                    if (node.SelectSingleNode("ID").InnerText == copyProfile.ID)
                    {
                        xmlNodeTemplate = node;
                        break;
                    }

                }
            }

            string newProfileName = "NewProfile";
            if (copyProfile != null) { newProfileName = copyProfile.ProfileName; }
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
            newNode.SelectSingleNode("ID").InnerText = getNewIDNumberForProfile(xmlDocument);
            newNode.SelectSingleNode("DefaultProfile").InnerText = "False";


            xmlNodeProfiles.AppendChild(newNode);

            Profile profile = new Profile();
            this.Add(profile);
            profile.LoadProfile(newNode.SelectSingleNode("ID").InnerText, xmlDocument);

            xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);

            xmlDocument = null;

        }

        public string getNewIDNumberForProfile(XmlDocument xmlDocument)
        {
            //gets ID for new profiles
            int ID = 0;

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
    }

    public class Profile
    {
        public string ID { get; set; }
        public bool DefaultProfile
        {
            get { return defaultProfile; }
            set
            {
                defaultProfile = value;
                if (value == true) { VisibilityDefaultProfile = Visibility.Visible; } else { VisibilityDefaultProfile = Visibility.Collapsed; }
            }
        }
        public bool defaultProfile { get; set; }

        public Visibility VisibilityDefaultProfile { get; set; } = Visibility.Collapsed;
        public string ProfileName { get; set; } = "";
        public string Exe { get; set; } = "";
        public string Resolution { get; set; } = "";
        public string RefreshRate { get; set; } = "";
        public string Path { get; set; } = "";
        public string AppType { get; set; } = "";
        public string GameID { get; set; } = "";
        public string Offline_TDP1 { get; set; } = "";
        public string Offline_TDP2 { get; set; } = "";
        public string Offline_ActiveCores { get; set; } = "";
        public string Offline_MAXCPU { get; set; } = "";
        public string Offline_FPSLimit { get; set; } = "";
        public string Offline_EPP { get; set; } = "";
        public string Offline_GPUCLK { get; set; } = "";
        public string Online_TDP1 { get; set; } = "";
        public string Online_TDP2 { get; set; } = "";
        public string Online_ActiveCores { get; set; } = "";
        public string Online_MAXCPU { get; set; } = "";
        public string Online_FPSLimit { get; set; } = "";
        public string Online_EPP { get; set; } = "";
        public string Online_GPUCLK { get; set; } = "";


        public void SaveToXML()
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
                    onlineNode.SelectSingleNode("TDP1").InnerText = Online_TDP1;
                    onlineNode.SelectSingleNode("TDP2").InnerText = Online_TDP2;
                    onlineNode.SelectSingleNode("ActiveCores").InnerText = Online_ActiveCores;
                    onlineNode.SelectSingleNode("MAXCPU").InnerText = Online_MAXCPU;
                    onlineNode.SelectSingleNode("FPSLimit").InnerText = Online_FPSLimit;
                    onlineNode.SelectSingleNode("EPP").InnerText = Online_EPP;
                    onlineNode.SelectSingleNode("GPUCLK").InnerText = Online_GPUCLK;

                    XmlNode offlineNode = parentNode.SelectSingleNode("Offline");
                    offlineNode.SelectSingleNode("TDP1").InnerText = Offline_TDP1;
                    offlineNode.SelectSingleNode("TDP2").InnerText = Offline_TDP2;
                    offlineNode.SelectSingleNode("ActiveCores").InnerText = Offline_ActiveCores;
                    offlineNode.SelectSingleNode("MAXCPU").InnerText = Offline_MAXCPU;
                    offlineNode.SelectSingleNode("FPSLimit").InnerText = Offline_FPSLimit;
                    offlineNode.SelectSingleNode("EPP").InnerText = Offline_EPP;
                    offlineNode.SelectSingleNode("GPUCLK").InnerText = Offline_GPUCLK;

                    XmlNode LaunchOptions = parentNode.SelectSingleNode("LaunchOptions");
                    LaunchOptions.SelectSingleNode("Resolution").InnerText = Resolution;
                    LaunchOptions.SelectSingleNode("RefreshRate").InnerText = RefreshRate;
                    LaunchOptions.SelectSingleNode("Path").InnerText = Path;
                    LaunchOptions.SelectSingleNode("AppType").InnerText = AppType;
                    LaunchOptions.SelectSingleNode("GameID").InnerText = GameID;


                    parentNode.SelectSingleNode("ProfileName").InnerText = ProfileName;
                    parentNode.SelectSingleNode("Exe").InnerText = Exe;

                    //if ID isnt 0, which is the default profile, and its been saved to be the default profile, then make this ID 0 and change the other profile
                    if (DefaultProfile.ToString() != parentNode.SelectSingleNode("DefaultProfile").InnerText && DefaultProfile)
                    {
                        //check to see if a default profile exists
                        XmlNode xmlCurrentDefault = xmlNode.SelectSingleNode("Profile/DefaultProfile[text()='True']");
                        if (xmlCurrentDefault != null)
                        {
                            //if not null set to false
                            xmlCurrentDefault.InnerText = "False";
                            //get the ID and change status in profiles list
                            string curDefID = xmlCurrentDefault.ParentNode.SelectSingleNode("ID").InnerText;
                            Global_Variables.Global_Variables.profiles.setCurrentDefaultProfileToFalse(curDefID);
                        }
                        
                    }
                    parentNode.SelectSingleNode("DefaultProfile").InnerText = DefaultProfile.ToString();

                }


            }
            xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);

            xmlDocument = null;


        }

        public void LoadProfile(string loadID, XmlDocument xmlDocument=null)
        {
            if (xmlDocument == null)
            {
                xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            }


            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ID[text()='" + loadID + "']");

            if (xmlSelectedNode != null)
            {
                XmlNode parentNode = xmlSelectedNode.ParentNode;

                if (parentNode != null)
                {
                    XmlNode onlineNode = parentNode.SelectSingleNode("Online");
                    Online_TDP1 = onlineNode.SelectSingleNode("TDP1").InnerText;
                    Online_TDP2 = onlineNode.SelectSingleNode("TDP2").InnerText;
                    Online_ActiveCores = onlineNode.SelectSingleNode("ActiveCores").InnerText;
                    Online_MAXCPU = onlineNode.SelectSingleNode("MAXCPU").InnerText;
                    Online_FPSLimit = onlineNode.SelectSingleNode("FPSLimit").InnerText;
                    Online_EPP = onlineNode.SelectSingleNode("EPP").InnerText;
                    Online_GPUCLK = onlineNode.SelectSingleNode("GPUCLK").InnerText;

                    XmlNode offlineNode = parentNode.SelectSingleNode("Offline");
                    Offline_TDP1 = offlineNode.SelectSingleNode("TDP1").InnerText;
                    Offline_TDP2 = offlineNode.SelectSingleNode("TDP2").InnerText;
                    Offline_ActiveCores = offlineNode.SelectSingleNode("ActiveCores").InnerText;
                    Offline_MAXCPU = offlineNode.SelectSingleNode("MAXCPU").InnerText;
                    Offline_FPSLimit = offlineNode.SelectSingleNode("FPSLimit").InnerText;
                    Offline_EPP = offlineNode.SelectSingleNode("EPP").InnerText;
                    Offline_GPUCLK = offlineNode.SelectSingleNode("GPUCLK").InnerText;

                    XmlNode LaunchOptions = parentNode.SelectSingleNode("LaunchOptions");
                    Resolution = LaunchOptions.SelectSingleNode("Resolution").InnerText;
                    RefreshRate = LaunchOptions.SelectSingleNode("RefreshRate").InnerText;
                    Path = LaunchOptions.SelectSingleNode("Path").InnerText;
                    AppType = LaunchOptions.SelectSingleNode("AppType").InnerText;
                    GameID = LaunchOptions.SelectSingleNode("GameID").InnerText;

                    ProfileName = parentNode.SelectSingleNode("ProfileName").InnerText;
                    Exe = parentNode.SelectSingleNode("Exe").InnerText;
                    if (parentNode.SelectSingleNode("DefaultProfile").InnerText == "True") { DefaultProfile = true; } else { DefaultProfile = false; }
                    ID = loadID;
                    

                }


            }
            
            xmlDocument = null;

        }

        public void applyProfile()
        {
            string powerStatus = SystemParameters.PowerLineStatus.ToString();


            switch (powerStatus)
            {
                case "Online":
                    if (Online_ActiveCores != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => CoreParking_Management.CoreParking_Management.changeActiveCores(Convert.ToInt32(Online_ActiveCores))); }
                    if (Online_EPP != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => EPP_Management.EPP_Management.changeEPP(Convert.ToInt32(Online_EPP))); }
                    if (Online_FPSLimit != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => RTSS.setRTSSFPSLimit(Convert.ToInt32(Online_FPSLimit))); }
                    if (Online_GPUCLK != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => GPUCLK_Management.GPUCLK_Management.changeAMDGPUClock(Convert.ToInt32(Online_EPP))); }
                    if (Online_MAXCPU != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => MaxProcFreq_Management.MaxProcFreq_Management.changeCPUMaxFrequency(Convert.ToInt32(Online_EPP))); }
                    if (Online_TDP1 != "" && Online_TDP2 == "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => TDP_Management.TDP_Management.changeTDP(Convert.ToInt32(Online_TDP1), Convert.ToInt32(Online_TDP1))); }
                    if (Online_TDP1 != "" && Online_TDP2 != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => TDP_Management.TDP_Management.changeTDP(Convert.ToInt32(Online_TDP1), Convert.ToInt32(Online_TDP2))); }
                
                    break;
                case "Offline":
                    if (Offline_ActiveCores != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => CoreParking_Management.CoreParking_Management.changeActiveCores(Convert.ToInt32(Offline_ActiveCores))); }
                    if (Offline_EPP != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => EPP_Management.EPP_Management.changeEPP(Convert.ToInt32(Offline_EPP))); }
                    if (Offline_FPSLimit != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => RTSS.setRTSSFPSLimit(Convert.ToInt32(Offline_FPSLimit))); }
                    if (Offline_GPUCLK != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => GPUCLK_Management.GPUCLK_Management.changeAMDGPUClock(Convert.ToInt32(Offline_EPP))); }
                    if (Offline_MAXCPU != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => MaxProcFreq_Management.MaxProcFreq_Management.changeCPUMaxFrequency(Convert.ToInt32(Offline_EPP))); }
                    if (Offline_TDP1 != "" && Offline_TDP2 == "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => TDP_Management.TDP_Management.changeTDP(Convert.ToInt32(Offline_TDP1), Convert.ToInt32(Offline_TDP1))); }
                    if (Offline_TDP1 != "" && Offline_TDP2 != "") { Classes.Task_Scheduler.Task_Scheduler.runTask(() => TDP_Management.TDP_Management.changeTDP(Convert.ToInt32(Offline_TDP1), Convert.ToInt32(Offline_TDP2))); }

                    break;

                default: break;
            }


        }
    }
}
