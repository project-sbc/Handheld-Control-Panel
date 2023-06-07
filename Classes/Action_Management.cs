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
using MahApps.Metro.IconPacks;
using System.Windows.Threading;
using WindowsInput;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;

namespace Handheld_Control_Panel.Classes
{
    public class Action_Management: List<HotkeyItem>
    {
        public object lockObjectActions = new object();
        public string actionDirectory = AppDomain.CurrentDomain.BaseDirectory + "Actions\\";
        public HotkeyItem editingHotkey = null;

        public event EventHandler hotkeyClearedEvent;
        public void raiseHotkeyClearedEvent()
        {
            hotkeyClearedEvent?.Invoke(null, EventArgs.Empty);
        }
        public event EventHandler hotkeyActionChangedEvent;
        public void raiseHotkeyActionChangedEvent()
        {
            hotkeyActionChangedEvent?.Invoke(null, EventArgs.Empty);
        }

        public void updateLanguage()
        {
            foreach (HotkeyItem hki in Global_Variables.Global_Variables.hotKeys)
            {
                hki.Action = hki.Action;
            }

        }

        public void loadHotKey(string ID)
        {
            HotkeyItem loadingHKI = this.Find(o => o.ID == ID);
            if (loadingHKI != null)
            {
                if (File.Exists(actionDirectory + "\\" + ID + ".xml"))
                {
                    lock (lockObjectActions)
                    {
                        using (StreamReader sw = new StreamReader(actionDirectory + "\\" + ID + ".xml"))
                        {
                            XmlSerializer xmls = new XmlSerializer(typeof(HotkeyItem));
                            loadingHKI = (HotkeyItem)xmls.Deserialize(sw);
                        }
                    }


                }


            }
        }

        public void generateGlobalControllerHotKeyList()
        {
            Global_Variables.Global_Variables.controllerHotKeyDictionary.Clear();

            Dictionary<ushort, ActionParameter> returnDictionary = new Dictionary<ushort, ActionParameter>();

            foreach(HotkeyItem hki in this)
            {
                if (hki.Type == "Controller")
                {
                    ushort hotkey;
                    if (ushort.TryParse(hki.Hotkey, out hotkey))
                    {
                        ActionParameter ap = new ActionParameter();
                        ap.Action = hki.Action;
                        ap.Parameter = hki.Parameter;
                        if (!returnDictionary.ContainsKey(hotkey))
                        {
                            returnDictionary.Add(hotkey, ap);
                        }

                    }
                }
              
            }

            Global_Variables.Global_Variables.controllerHotKeyDictionary = returnDictionary;
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

        public void deleteHotkey(HotkeyItem hotKey)
        {
            if (hotKey != null)
            {
                string hotkeyID = hotKey.ID;
                               
                if (File.Exists(actionDirectory + hotkeyID + ".xml"))
                {
                    File.Delete(actionDirectory + hotkeyID + ".xml");
                }

                this.Remove(hotKey);
            }

        }


        public void generateGlobalKeyboardHotKeyList()
        {

            Global_Variables.Global_Variables.KBHotKeyDictionary.Clear();
       

            Dictionary<string, ActionParameter> returnDictionary = new Dictionary<string, ActionParameter>();

            foreach (HotkeyItem hki in this)
            {
                if (hki.Type == "Keyboard")
                {
                    ActionParameter ap = new ActionParameter();
                    ap.Action = hki.Action;
                    ap.Parameter = hki.Parameter;
                    if (!returnDictionary.ContainsKey(hki.Hotkey))
                    {
                        returnDictionary.Add(hki.Hotkey, ap);
                    }
                }
              
            }

            Global_Variables.Global_Variables.KBHotKeyDictionary = returnDictionary;


        }

        public Action_Management()
        {
            //populates list
            if (!Directory.Exists(actionDirectory))
            {
                Directory.CreateDirectory(actionDirectory);
            }

            string[] files = Directory.GetFiles(actionDirectory, "*.xml", SearchOption.TopDirectoryOnly);
            lock (lockObjectActions)
            {
                foreach (string file in files)
                {
                    StreamReader sr = new StreamReader(file);
                    XmlSerializer xmls = new XmlSerializer(typeof(HotkeyItem));
                    this.Add((HotkeyItem)xmls.Deserialize(sr));
                    sr.Dispose();
                    xmls = null;

                }
            }
           
                
        }
        public void SaveToXML(HotkeyItem hki)
        {
            //Profile profile = this.Find(o => o.ProfileName == profileName);
            if (hki != null)
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    lock (lockObjectActions)
                    {
                        StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Actions\\" + hki.ID + ".xml");
                        XmlSerializer xmls = new XmlSerializer(typeof(HotkeyItem));
                        xmls.Serialize(sw, hki);
                        sw.Dispose();
                        xmls = null;
                    }
           
                }


                    );

            }



        }
     


        public void addNewHotkey()
        {
            HotkeyItem hki = new HotkeyItem();

            string newProfileName = "New Action";
            
            int x = 1;
            if (File.Exists(actionDirectory + newProfileName + ".xml"))
            {
                while (File.Exists(actionDirectory + newProfileName + x.ToString() + ".xml"))
                {
                    x++;
                }
                newProfileName = newProfileName + x.ToString();
            }

            hki.ID = newProfileName;

            this.Add(hki);
            Global_Variables.Global_Variables.hotKeys.SaveToXML(hki);



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
                if (value != "")
                {
                    DisplayType = Application.Current.Resources["Hotkeys_Type_" + value].ToString();
                }
                if (value == "Controller")
                {
                    Kind = PackIconMaterialKind.MicrosoftXboxController;
                }
                else
                {
                    Kind = PackIconMaterialKind.Keyboard;
                }
                type = value;
            }

        }
        public string DisplayType { get; set; } = "";

        public PackIconMaterialKind Kind { get; set; }
        public bool AddHomePage { get; set; }
        private string action { get; set; } = "";
        public string Action
        {

            get
            {
                return action;
            }
            set
            {
                if (value != "")
                {
                    DisplayAction = Application.Current.Resources["Hotkeys_Action_" + value].ToString();
                }
               
                action = value;

                switch (action)
                {

                    case "Toggle_HCP_OSK":
                       
                        break;
                    case "Toggle_AutoTDP":
                        
                        break;
                    case "Toggle_Windows_OSK":
                       
                        break;
                    case "Show_Hide_HCP":
                      
                        break;
                    case "Show_Hide_HCP_ProcessSuspend":
                       
                        break;
                    case "Change_FanSpeed":
                        
                        break;
                    case "Change_FanSpeed_Mode":

                        break;
                    case "Change_TDP":

                        break;
                    case "Change_TDP_Mode":

                        break;
                    case "Open_Steam_BigPicture":
                      
                        break;
                    case "Open_Playnite":
                     
                        break;
                    case "Change_Brightness":

                        break;
                    case "Change_Brightness_Mode":

                        break;
                    case "Change_Volume_Mode":

                        break;
                    case "Change_Refresh_Mode":
                       
                        break;
                    case "Change_Resolution_Mode":
                      
                        break;
                    case "Change_Volume":
                    
                        break;
                    case "Change_GPUCLK":
                  
                        break;
                    case "Toggle_MouseMode":
                     
                        break;
                    case "Toggle_Controller":
                     
                        break;
                    case "Desktop":
                       
                        break;
                    default: break;
                }

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
                if (value != "")
                {
                    if (Type == "Controller")
                    {
                        DisplayHotkey = convertControllerUshortToString(value);
                    }
                    else
                    {
                        DisplayHotkey = value;
                    }
                }
           
                
                hotkey = value;
            }

        }
        public string DisplayHotkey{ get; set; } = "";

        private string parameter { get; set; } = "";

        public string DisplayParameter { get; set; }
        public string Parameter
        {

            get
            {
                return parameter;
            }
            set
            {
                if (value != "")
                {
                    if (Action == "Open_Program")
                    {
                        DisplayParameter = value;
                    }
                    else
                    {
                        if (Action.Contains("Change"))
                        {
                            int number;
                            if (Int32.TryParse(value, out number))
                            {
                                if (number > 0) { DisplayParameter = "+" + value; } else { DisplayParameter = value; }
                            }
                        }
                        else
                        {
                            DisplayParameter = value;
                        }
                    }
                }
                else { DisplayParameter = ""; }
            

                parameter = value;
            }

        }
      


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
                

      
    }
   
}
