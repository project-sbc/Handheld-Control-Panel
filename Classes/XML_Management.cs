using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Handheld_Control_Panel.Classes;

namespace Handheld_Control_Panel.Classes
{
    public static class XML_Management
    {
        private static string hcpDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static object lockObject = new object();


        public static void Save_Profile(Profile profile)
        {
            lock (lockObject)
            {
                StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Profiles\\" + profile.ProfileName + ".xml");
                XmlSerializer xmls = new XmlSerializer(typeof(Profile));
                xmls.Serialize(sw, profile);
                sw.Dispose();
                xmls = null;
            }
        }
        public static Profile Load_Profile(string name)
        {
            string filePath = hcpDirectory + "Profiles\\" + name + ".xml";
            Profile objObject = null;
            if (File.Exists(filePath))
            {
                lock (lockObject)
                {
                    StreamReader sr = new StreamReader(filePath);
                    XmlSerializer xmls = new XmlSerializer(typeof(Profile));
                    objObject = ((Profile)xmls.Deserialize(sr));
                    sr.Dispose();
                    xmls = null;
                }
            }
            return objObject;
        }
        public static void Save_MouseMode(MouseMode mouseMode)
        {
            lock (lockObject)
            {
                StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "MouseModes\\" + mouseMode.MouseModeName + ".xml");
                XmlSerializer xmls = new XmlSerializer(typeof(MouseMode));
                xmls.Serialize(sw, mouseMode);
                sw.Dispose();
                xmls = null;
            }
        }

        public static MouseMode Load_MouseMode(string name)
        {
            string filePath = hcpDirectory + "MouseModes\\" + name + ".xml";
            MouseMode objObject = null;
            if (File.Exists(filePath))
            {
                lock (lockObject)
                {
                    StreamReader sr = new StreamReader(filePath);
                    XmlSerializer xmls = new XmlSerializer(typeof(MouseMode));
                    objObject = ((MouseMode)xmls.Deserialize(sr));
                    sr.Dispose();
                    xmls = null;
                }
            }
            return objObject;
        }
        public static void Save_Action(QuickAction hotkeyItem)
        {
            lock (lockObject)
            {
                StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Actions\\" + hotkeyItem.ID + ".xml");
                XmlSerializer xmls = new XmlSerializer(typeof(QuickAction));
                xmls.Serialize(sw, hotkeyItem);
                sw.Dispose();
                xmls = null;
            }
        }
        public static QuickAction Load_Action(string name)
        {
            string filePath = hcpDirectory + "Actions\\" + name + ".xml";
            QuickAction objObject = null;
            if (File.Exists(filePath))
            {
                lock (lockObject)
                {
                    StreamReader sr = new StreamReader(filePath);
                    XmlSerializer xmls = new XmlSerializer(typeof(QuickAction));
                    objObject = ((QuickAction)xmls.Deserialize(sr));
                    sr.Dispose();
                    xmls = null;
                }
            }
            return objObject;
        }


    }
}
