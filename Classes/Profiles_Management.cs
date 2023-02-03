using SharpDX;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Handheld_Control_Panel.Classes
{
    public class Profiles_Management: List<Profile>
    {
        //public List<Profile> profiles= new List< Profile>();
        public Profile activeProfile=null;
        public Profile editingProfile = null;
        public Profile defaultProfile = null;
        public Profiles_Management()
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");

            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                Profile profile = new Profile();

                profile.LoadProfile(node.SelectSingleNode("ID").InnerText);
                if (node.SelectSingleNode("ID").InnerText == "0") { defaultProfile = profile; }
                this.Add(profile);
            }
            
            xmlDocument = null;          
        }

    }

    public class Profile
    {
        public string ID { get; set; }
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

                }


            }
            xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);

            xmlDocument = null;


        }

        public void LoadProfile(string ID)
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
                    ID = ID;

                }


            }
            Debug.WriteLine("done with profile");
            xmlDocument = null;

        }
    }
}
