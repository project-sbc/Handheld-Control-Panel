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
using System.Windows;
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

        //good reference to keep even if i dont use it, lookup of values of buttons to Ushort
        static Dictionary<string, ushort> controllerFlagUshortLookup =
   new Dictionary<string, ushort>()
   {
           {"A", 4096},
           {"B", 8192 },
           {"X", 16384 },
           {"Y", 32768 },
           {"LB", 256 },
           {"RB", 512 },
           {"DPadUp", 1},
           {"DPadDown", 2 },
           {"DPadLeft", 4 },
           {"DPadRight", 8},
         {"Start", 16 },
           {"Back", 32 },
           {"LStick", 64 },
            {"RStick", 128 }

   };

       
       

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



        public void addNewHotkey()
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
        private string type { get; set; } = "";
        public string Type
        {

            get
            {
                return type;
            }
            set
            {
                DisplayType = Application.Current.Resources["Hotkeys_Type_" + value].ToString();
                type = value;
            }

        }
        public string DisplayType { get; set; } = "";


        private string action { get; set; } = "";
        public string Action
        {

            get
            {
                return action;
            }
            set
            {
                DisplayAction = Application.Current.Resources["Hotkeys_Action_" + value].ToString();
                action = value;
            }

        }
        public string DisplayAction { get; set; } = "";


        private string hotkey { get; set; } = "";
        public string Hotkey
        {

            get
            {
                return hotkey;
            }
            set
            {
                if (Type == "Controller")
                {
                    DisplayHotkey = convertControllerUshortToString(value);
                }
                else
                {
                    DisplayHotkey = value;
                }
                
                hotkey = value;
            }

        }
        public string DisplayHotkey{ get; set; } = "";

        private string parameter { get; set; } = "";
        public string Parameter
        {

            get
            {
                return parameter;
            }
            set
            {
                if (Action == "OpenProgram")
                {
                    DisplayParameter = Global_Variables.Global_Variables.profiles.getProfileNameById(value);
                }
                else
                {
                    if (Action.Contains("Change"))
                    {
                        int number;
                        if (Int32.TryParse(value, out number))
                        {
                            if (number > 0) { DisplayParameter = "+" + value; }
                        }
                    }
                    else
                    {
                        DisplayParameter = value;
                    }
                }

                parameter = value;
            }

        }
        public string DisplayParameter { get; set; } = "";


        public string convertControllerUshortToString(string hotkey)
        {
            string gamepadCombo = "";
            Gamepad gamepad = new Gamepad();


            ushort uShorthotkey;

            if (ushort.TryParse(hotkey, out uShorthotkey))
            {
                gamepad.Buttons = (GamepadButtonFlags)(uShorthotkey);

                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.A)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "A"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.B)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "B"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.X)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "X"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.Y)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "Y"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "LB"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "RB"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "LStick"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "RStick"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.Start)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "Start"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.Back)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "Back"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "DPadUp"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "DPadDown"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "DPadLeft"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "DPadRight"); }
            }


            return gamepadCombo;

        }
        private string makeGamepadButtonString(string currentValue, string addValue)
        {
            //routine to make string for 
            if (currentValue == "")
            {
                return addValue;
            }
            else
            {
                return currentValue + "+" + addValue;
            }

        }

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
                    Debug.WriteLine(parentNode.SelectSingleNode("Type").InnerText);
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
