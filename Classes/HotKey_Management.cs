using Handheld_Control_Panel.Classes.Global_Variables;
using MahApps.Metro.Controls;
using SharpDX;
using SharpDX.XInput;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Handheld_Control_Panel.Classes
{
    public class HotKey_Management: List<HotkeyItem>
    {
      
        public HotkeyItem editingHotkey = null;

        public void generateGlobalControllerHotKeyList()
        {
            Dictionary<ushort, ActionParameter> returnDictionary = Global_Variables.Global_Variables.controllerHotKeyDictionary;

            returnDictionary.Clear();

            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/ControllerHotKeys");

            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                if (node.SelectSingleNode("Type").InnerText == "Controller")
                {
                    ushort hotkey;
                    if (ushort.TryParse(node.SelectSingleNode("Hotkey").InnerText, out hotkey))
                    {
                        ActionParameter ap = new ActionParameter();
                        ap.Action = node.SelectSingleNode("Action").InnerText;
                        ap.Action = node.SelectSingleNode("Parameter").InnerText;
                        returnDictionary.Add(hotkey, ap);
                    }

                }

            }
            xmlDocument = null;
        }

        public void generateGlobalKeyboardHotKeyList()
        {
            Dictionary<string, ActionParameter> returnDictionary = Global_Variables.Global_Variables.KBHotKeyDictionary;

            returnDictionary.Clear();

            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/ControllerHotKeys");

            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                if (node.SelectSingleNode("Type").InnerText == "Keyboard")
                {
                    ActionParameter ap = new ActionParameter();
                    string hotkey = node.SelectSingleNode("Hotkey").InnerText;
                    ap.Action = node.SelectSingleNode("Action").InnerText;
                    ap.Action = node.SelectSingleNode("Parameter").InnerText;
                    returnDictionary.Add(hotkey, ap);

                }

            }

            xmlDocument = null;
          
        }

        public HotKey_Management()
        {
            //populates list
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/ControllerHotKeys");

            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                HotkeyItem hotkey = new HotkeyItem();

                hotkey.LoadProfile(node.SelectSingleNode("ID").InnerText, xmlDocument);
         
                this.Add(hotkey);
            }
            
            xmlDocument = null;          
        }



        public void addNewProfile()
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNodeTemplate = xmlDocument.SelectSingleNode("//Configuration/ControllerHotKeyTemplate/ControllerHotKey");
            XmlNode xmlNodeHotKeys = xmlDocument.SelectSingleNode("//Configuration/ControllerHotKeys");
               
            XmlNode newNode = xmlDocument.CreateNode(XmlNodeType.Element, "ControllerHotKey", "");
            newNode.InnerXml = xmlNodeTemplate.InnerXml;
            
            newNode.SelectSingleNode("ID").InnerText = getNewIDNumberForHotkey(xmlDocument);
            xmlNodeHotKeys.AppendChild(newNode);

            HotkeyItem hotkey = new HotkeyItem();
            this.Add(hotkey);
            hotkey.LoadProfile(newNode.SelectSingleNode("ID").InnerText, xmlDocument);

            xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);

            xmlDocument = null;

        }

        public string getNewIDNumberForHotkey(XmlDocument xmlDocument)
        {
            //gets ID for new profiles
            int ID = 0;

            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Hotkeys");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("ControllerHotKey/ID[text()='" + ID.ToString() + "']");

            while (xmlSelectedNode != null)
            {
                ID = ID + 1;
                xmlSelectedNode = xmlNode.SelectSingleNode("ControllerHotKey/ID[text()='" + ID.ToString() + "']");
            }
            //ID++;
            return ID.ToString();

        }
    }

    public class HotkeyItem
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string Action { get; set; } = "";
        public string Parameter { get; set; } = "";
        public string Hotkey { get; set; } = "";
        public void SaveToXML()
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/ControllerHotKeys");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("ControllerHotKey/ID[text()='" + ID + "']");

            if (xmlSelectedNode != null)
            {
                XmlNode parentNode = xmlSelectedNode.ParentNode;

                if (parentNode != null)
                {
                    parentNode.SelectSingleNode("Type").InnerText = Type;
                    parentNode.SelectSingleNode("Action").InnerText = Action;
                    parentNode.SelectSingleNode("Parameter").InnerText = Parameter;
                    parentNode.SelectSingleNode("Hotkey").InnerText = Hotkey;

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


            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/ControllerHotKeys");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("ControllerHotKey/ID[text()='" + loadID + "']");

            if (xmlSelectedNode != null)
            {
                XmlNode parentNode = xmlSelectedNode.ParentNode;

                if (parentNode != null)
                {
                   
                    Type = parentNode.SelectSingleNode("Type").InnerText;
                    Action = parentNode.SelectSingleNode("Action").InnerText;
                    Parameter = parentNode.SelectSingleNode("Parameter").InnerText;
                    Hotkey = parentNode.SelectSingleNode("Hotkey").InnerText;
                    ID = loadID;
                }
            }            
            xmlDocument = null;

        }
    }
}
