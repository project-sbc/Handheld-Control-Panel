using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Handheld_Control_Panel.Classes
{
    public static class Open_Program
    {
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

    }
}
