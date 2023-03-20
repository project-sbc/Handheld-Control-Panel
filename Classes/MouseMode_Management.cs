using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Handheld_Control_Panel.Classes.Global_Variables;
using SharpDX;
using SharpDX.XInput;
using Linearstar.Windows.RawInput;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using WindowsInput;
using System.Xml;
using System.Drawing;
using System.Collections;
using WindowsInput.Native;
using System.Windows.Media.Media3D;


namespace Handheld_Control_Panel.Classes
{
    public static class MouseModeLookup
    {
        public static List<string> MouseModeList = new List<string>()
 {
           {"A"},
           {"B"},
           {"C"},
           {"D"},
           {"E" },
           {"F" },
      {"G" },
           {"H" },
           {"I" },
           {"J" },
           {"K" },
           {"L" },
           {"M" },
           {"N" },
           {"O" },
           {"P" },
           {"Q" },
           {"R" },
           {"S" },
           {"T" },
           {"U" },
           {"V" },
           {"W" },
           {"X" },
           {"Y" },
           {"Z" },

           {"1" },
           {"2" },
           {"3" },
           {"4" },
           {"5" },
           {"6" },
           {"7" },
           {"8" },
           {"9" },
           {"0" },

           {"PERIOD" },
           {"COMMA" },

           {"SPACE"},
           {"RETURN" },

           {"UP" },
           {"DOWN" },
           {"LEFT" },
           {"RIGHT" },

           {"F1" },
           {"F2" },
           {"F3" },
           {"F4" },
           {"F5" },
           {"F6" },
           {"F7" },
           {"F8" },
           {"F9" },
           {"F10" },
           {"F11" },
           {"F12" },
           {"LeftMouseClick" },
           {"RightMouseClick" }



 };
    }

    public class MouseMode_Management: List<MouseMode>
    {
        public InputSimulator inputSimulator = new InputSimulator();
        public Controller controller;
        public DispatcherTimer timerController = new DispatcherTimer(DispatcherPriority.Render);
        public Gamepad currentGamePad;
        public Gamepad previousGamePad;

        public double sensitivityScroll = 1;
        private int joystickButtonPressSensivitiy = 6000;
       
        public MouseMode activeMouseMode = new MouseMode();
        public MouseMode editingMouseMode = new MouseMode();
        public Dictionary<string, VirtualKeyCode> keyLookUp =
 new Dictionary<string, VirtualKeyCode>()
 {
           {"A", VirtualKeyCode.VK_A },
           {"B", VirtualKeyCode.VK_B },
           {"C", VirtualKeyCode.VK_C },
           {"D", VirtualKeyCode.VK_D },
           {"E", VirtualKeyCode.VK_E },
           {"F", VirtualKeyCode.VK_F },
           {"G", VirtualKeyCode.VK_G },
           {"H", VirtualKeyCode.VK_H },
           {"I", VirtualKeyCode.VK_I },
           {"J", VirtualKeyCode.VK_J },
           {"K", VirtualKeyCode.VK_K },
           {"L", VirtualKeyCode.VK_L },
           {"M", VirtualKeyCode.VK_M },
           {"N", VirtualKeyCode.VK_N },
           {"O", VirtualKeyCode.VK_O },
           {"P", VirtualKeyCode.VK_P },
           {"Q", VirtualKeyCode.VK_Q },
           {"R", VirtualKeyCode.VK_R },
           {"S", VirtualKeyCode.VK_S },
           {"T", VirtualKeyCode.VK_T },
           {"U", VirtualKeyCode.VK_U },
           {"V", VirtualKeyCode.VK_V },
           {"W", VirtualKeyCode.VK_W },
           {"X", VirtualKeyCode.VK_X },
           {"Y", VirtualKeyCode.VK_Y },
           {"Z", VirtualKeyCode.VK_Z },

           {"1", VirtualKeyCode.VK_1 },
           {"2", VirtualKeyCode.VK_2 },
           {"3", VirtualKeyCode.VK_3 },
           {"4", VirtualKeyCode.VK_4 },
           {"5", VirtualKeyCode.VK_5 },
           {"6", VirtualKeyCode.VK_6 },
           {"7", VirtualKeyCode.VK_7 },
           {"8", VirtualKeyCode.VK_8 },
           {"9", VirtualKeyCode.VK_9 },
           {"0", VirtualKeyCode.VK_0 },

           {"PERIOD", VirtualKeyCode.OEM_PERIOD },
           {"COMMA", VirtualKeyCode.OEM_COMMA },

           {"SPACE", VirtualKeyCode.SPACE },
           {"RETURN", VirtualKeyCode.RETURN },

           {"UP", VirtualKeyCode.UP },
           {"DOWN", VirtualKeyCode.DOWN },
           {"LEFT", VirtualKeyCode.LEFT },
           {"RIGHT", VirtualKeyCode.RIGHT },

           {"F1", VirtualKeyCode.F1 },
           {"F2", VirtualKeyCode.F2 },
           {"F3", VirtualKeyCode.F3 },
           {"F4", VirtualKeyCode.F4 },
           {"F5", VirtualKeyCode.F5 },
           {"F6", VirtualKeyCode.F6 },
           {"F7", VirtualKeyCode.F7 },
           {"F8", VirtualKeyCode.F8 },
           {"F9", VirtualKeyCode.F9 },
           {"F10", VirtualKeyCode.F10 },
           {"F11", VirtualKeyCode.F11 },
           {"F12", VirtualKeyCode.F12 },



 };
        private Dictionary<string, GamepadButtonFlags> buttonLookup = new Dictionary<string, GamepadButtonFlags>()
        {
            { "A", GamepadButtonFlags.A },
            {"B", GamepadButtonFlags.B },
            {"X", GamepadButtonFlags.X },
            {"Y", GamepadButtonFlags.Y },
            {"LB", GamepadButtonFlags.LeftShoulder },
            {"RB", GamepadButtonFlags.RightShoulder},
            {"DPadUp", GamepadButtonFlags.DPadUp},
            {"DPadDown", GamepadButtonFlags.DPadDown},
            {"DPadLeft", GamepadButtonFlags.DPadLeft},
            {"DPadRight", GamepadButtonFlags.DPadRight},
            {"Start", GamepadButtonFlags.Start},
            {"Back", GamepadButtonFlags.Back},
            {"L3", GamepadButtonFlags.LeftThumb},
            {"R3", GamepadButtonFlags.RightThumb}

        };

        public MouseMode_Management()
        {
            
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/MouseModeConfigurations");

            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                MouseMode mouseMode = new MouseMode();
                mouseMode.LoadProfile(node.SelectSingleNode("ID").InnerText, xmlDocument);
                if (mouseMode.DefaultMode) { activeMouseMode = mouseMode; ; activeMouseMode.ActiveProfile = true; }
                this.Add(mouseMode);
            }
            xmlDocument = null;
        }

        public void start_MouseMode()
        {
            //create background thread to handle controller input
            getController();
            timerController.Interval = TimeSpan.FromMilliseconds(10);
            timerController.Tick += controller_Tick;
            timerController.Start();
            Global_Variables.Global_Variables.MouseModeEnabled = true;
        }
        public bool status_MouseMode()
        {
            return timerController.IsEnabled;
        }
        public bool toggle_MouseMode()
        {
            bool mouseMode = false;

            if (timerController.IsEnabled)
            {
                end_MouseMode();
            }
            else
            {
                mouseMode = true;
                start_MouseMode();
            }
            return mouseMode;
        }
        public void end_MouseMode()
        {
            timerController.Stop();
            timerController.Tick -= controller_Tick;
            inputSimulator = null;
            Global_Variables.Global_Variables.MouseModeEnabled = false;
        }
      
        private void controller_Tick(object sender, EventArgs e)
        {
            //start timer to read and compare controller inputs
            //Controller input handler
            if (inputSimulator == null) { inputSimulator = new InputSimulator(); }

            if (controller == null)
            {
                getController();
                if (controller.IsConnected == false)
                {
                    getController();
                }
            }
            else if (!controller.IsConnected)
            {
                getController();
            }


            if (controller != null)
            {
                if (controller.IsConnected)
                {
                    currentGamePad = controller.GetState().Gamepad;

                    double mouseX = 0;
                    double mouseY = 0;
                    double mouseScrollX = 0;
                    double mouseScrollY = 0;

                    switch (activeMouseMode.mouseMode["LeftThumb"])
                    {
                        case "Mouse":
                            mouseX = normalizeJoystickInput(Global_Variables.Global_Variables.mousemodes.activeMouseMode.MouseSensitivity, currentGamePad.LeftThumbX);
                            mouseY = normalizeJoystickInput(Global_Variables.Global_Variables.mousemodes.activeMouseMode.MouseSensitivity, -currentGamePad.LeftThumbY);
                            break;
                        case "Scroll":
                            mouseScrollX = normalizeJoystickInput(sensitivityScroll, currentGamePad.LeftThumbX);
                            mouseScrollY = normalizeJoystickInput(sensitivityScroll, currentGamePad.LeftThumbY);
                            break;
                        case "WASD":
                            if (currentGamePad.LeftThumbX > joystickButtonPressSensivitiy)
                            {
                                if (!inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_D))
                                {
                                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_D);
                                }
                            }
                            else
                            {
                                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_D))
                                {
                                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_D);
                                }
                            }
                            if (currentGamePad.LeftThumbX < -joystickButtonPressSensivitiy)
                            {
                                if (!inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_A))
                                {
                                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_A);
                                }
                            }
                            else
                            {
                                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_A))
                                {
                                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_A);
                                }
                            }
                            if (currentGamePad.LeftThumbY > joystickButtonPressSensivitiy)
                            {
                                if (!inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_W))
                                {
                                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_W);
                                }
                            }
                            else
                            {
                                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_W))
                                {
                                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_W);
                                }
                            }
                            if (currentGamePad.LeftThumbY < -joystickButtonPressSensivitiy)
                            {
                                if (!inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_S))
                                {
                                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_S);
                                }
                            }
                            else
                            {
                                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_S))
                                {
                                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_S);
                                }
                            }

                            break;
                        case "ArrowKeys":
                            if (currentGamePad.LeftThumbX > joystickButtonPressSensivitiy)
                            {
                                if (!inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.RIGHT))
                                {
                                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.RIGHT);
                                }
                            }
                            else
                            {
                                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.RIGHT))
                                {
                                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.RIGHT);
                                }
                            }
                            if (currentGamePad.LeftThumbX < -joystickButtonPressSensivitiy)
                            {
                                if (!inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.LEFT))
                                {
                                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.LEFT);
                                }
                            }
                            else
                            {
                                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.LEFT))
                                {
                                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.LEFT);
                                }
                            }
                            if (currentGamePad.LeftThumbY > joystickButtonPressSensivitiy)
                            {
                                if (!inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.UP))
                                {
                                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.UP);
                                }
                            }
                            else
                            {
                                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.UP))
                                {
                                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.UP);
                                }
                            }
                            if (currentGamePad.LeftThumbY < -joystickButtonPressSensivitiy)
                            {
                                if (!inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.DOWN))
                                {
                                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.DOWN);
                                }
                            }
                            else
                            {
                                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.DOWN))
                                {
                                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.DOWN);
                                }
                            }
                            break;
                        default: break;
                    }

                    switch (activeMouseMode.mouseMode["RightThumb"])
                    {
                        case "Mouse":
                            mouseX = normalizeJoystickInput(Global_Variables.Global_Variables.mousemodes.activeMouseMode.MouseSensitivity, currentGamePad.RightThumbX);
                            mouseY = normalizeJoystickInput(Global_Variables.Global_Variables.mousemodes.activeMouseMode.MouseSensitivity, -currentGamePad.RightThumbY);
                            break;
                        case "Scroll":
                            mouseScrollX = normalizeJoystickInput(sensitivityScroll, -currentGamePad.RightThumbX);
                            mouseScrollY = normalizeJoystickInput(sensitivityScroll, currentGamePad.RightThumbY);
                            break;
                        case "WASD":
                            if (currentGamePad.RightThumbX > joystickButtonPressSensivitiy)
                            {
                                if (!inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_D))
                                {
                                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_D);
                                }
                            }
                            else
                            {
                                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_D))
                                {
                                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_D);
                                }
                            }
                            if (currentGamePad.RightThumbX < -joystickButtonPressSensivitiy)
                            {
                                if (!inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_A))
                                {
                                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_A);
                                }
                            }
                            else
                            {
                                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_A))
                                {
                                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_A);
                                }
                            }
                            if (currentGamePad.RightThumbY > joystickButtonPressSensivitiy)
                            {
                                if (!inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_W))
                                {
                                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_W);
                                }
                            }
                            else
                            {
                                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_W))
                                {
                                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_W);
                                }
                            }
                            if (currentGamePad.RightThumbY < -joystickButtonPressSensivitiy)
                            {
                                if (!inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_S))
                                {
                                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_S);
                                }
                            }
                            else
                            {
                                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.VK_S))
                                {
                                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_S);
                                }
                            }

                            break;
                        case "ArrowKeys":
                            if (currentGamePad.RightThumbX > joystickButtonPressSensivitiy)
                            {
                                if (!inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.RIGHT))
                                {
                                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.RIGHT);
                                }
                            }
                            else
                            {
                                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.RIGHT))
                                {
                                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.RIGHT);
                                }
                            }
                            if (currentGamePad.RightThumbX < -joystickButtonPressSensivitiy)
                            {
                                if (!inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.LEFT))
                                {
                                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.LEFT);
                                }
                            }
                            else
                            {
                                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.LEFT))
                                {
                                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.LEFT);
                                }
                            }
                            if (currentGamePad.RightThumbY > joystickButtonPressSensivitiy)
                            {
                                if (!inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.UP))
                                {
                                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.UP);
                                }
                            }
                            else
                            {
                                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.UP))
                                {
                                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.UP);
                                }
                            }
                            if (currentGamePad.RightThumbY < -joystickButtonPressSensivitiy)
                            {
                                if (!inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.DOWN))
                                {
                                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.DOWN);
                                }
                            }
                            else
                            {
                                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.DOWN))
                                {
                                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.DOWN);
                                }
                            }
                            break;
                        default: break;
                    }


                    foreach (KeyValuePair<string, string> entry in activeMouseMode.mouseMode)
                    {
                        if (!entry.Key.Contains("Thumb") && entry.Key != "LT" && entry.Key != "RT")
                        {
                            if (entry.Value != "")
                            {
                                GamepadButtonFlags flag = buttonLookup[entry.Key];
                                if (!entry.Value.Contains("Mouse"))
                                {
                                    VirtualKeyCode vkc = keyLookUp[entry.Value];
                                    if (currentGamePad.Buttons.HasFlag(flag) && !previousGamePad.Buttons.HasFlag(flag))
                                    {
                                        inputSimulator.Keyboard.KeyPress(vkc);
                                    }
                                    if (1==0)
                                    {
                                        if (currentGamePad.Buttons.HasFlag(flag))
                                        {
                                            if (!inputSimulator.InputDeviceState.IsKeyDown(vkc))
                                            {
                                                inputSimulator.Keyboard.KeyDown(vkc);
                                            }
                                        }
                                        else
                                        {
                                            if (inputSimulator.InputDeviceState.IsKeyDown(vkc))
                                            {
                                                inputSimulator.Keyboard.KeyUp(vkc);
                                            }
                                        }
                                    }
                                   
                                
                                }
                                else
                                {
                                    switch (entry.Value)
                                    {
                                        case "LeftMouseClick":
                                            if (currentGamePad.Buttons.HasFlag(flag) && !previousGamePad.Buttons.HasFlag(flag))
                                            {
                                                inputSimulator.Mouse.LeftButtonDown();
                                            }
                                            if (!currentGamePad.Buttons.HasFlag(flag) && previousGamePad.Buttons.HasFlag(flag))
                                            {
                                                inputSimulator.Mouse.LeftButtonUp();
                                            }
                                            break;
                                        case "RightMouseClick":
                                            if (currentGamePad.Buttons.HasFlag(flag) && !previousGamePad.Buttons.HasFlag(flag))
                                            {
                                                inputSimulator.Mouse.RightButtonDown();
                                            }
                                            if (!currentGamePad.Buttons.HasFlag(flag) && previousGamePad.Buttons.HasFlag(flag))
                                            {
                                                inputSimulator.Mouse.RightButtonUp();
                                            }
                                            break;
                                        default: break;

                                    }
                                }

                            }


                        }

                    }

                    inputSimulator.Mouse.MoveMouseBy((int)mouseX, (int)mouseY);
                    inputSimulator.Mouse.HorizontalScroll((int)mouseScrollX);
                    inputSimulator.Mouse.VerticalScroll((int)mouseScrollY);

                    previousGamePad = currentGamePad;
                }

            }

        }

        private double normalizeJoystickInput(double sensitivity, double value)
        {
            double returnValue;
            if (value != 0)
            {
                returnValue = (sensitivity * (value / 32768) / Math.Sqrt((value / 32768 * value / 32768) + 1));
                returnValue = Math.Round(returnValue, 0);
                return returnValue;
            }
            else
            {
                return 0;
            }
        }
        private void getController()
        {
            int controllerNum = 1;
            //get controller used, loop controller number if less than 5, so if controller is connected make num = 5 to get out of while loop
            while (controllerNum < 5)
            {
                switch (controllerNum)
                {
                    default:
                        break;
                    case 1:
                        controller = new Controller(UserIndex.One);
                        break;
                    case 2:
                        controller = new Controller(UserIndex.Two);
                        break;
                    case 3:
                        controller = new Controller(UserIndex.Three);
                        break;
                    case 4:
                        controller = new Controller(UserIndex.Four);
                        break;
                    case 5:
                        end_MouseMode();
                        break;
                }
                if (controller == null)
                {
                    controllerNum++;
                }
                else
                {
                    if (controller.IsConnected)
                    {
                        break;
                    }
                    else
                    {
                        controllerNum++;
                    }
                }
            }
        }

        public void setCurrentDefaultProfileToFalse(string ID)
        {
            //changes 
            foreach (MouseMode profile in this)
            {
                if (profile.ID == ID)
                {
                    profile.DefaultMode = false;
                }
            }

        }
        public string getProfileNameById(string ID)
        {
            string name = "";
            foreach (MouseMode profile in this)
            {
                if (profile.ID == ID)
                {
                    name = profile.MouseModeName;
                }
            }
            return name;
        }
        public void deleteProfile(MouseMode profile)
        {
            if (profile != null)
            {
                string ID = profile.ID;

                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
                XmlNode xmlNodeProfiles = xmlDocument.SelectSingleNode("//Configuration/MouseModeConfigurations");

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

        public void addNewProfile(MouseMode copyProfile)
        {

            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNodeTemplate = xmlDocument.SelectSingleNode("//Configuration/MouseModeTemplate/MouseModeConfiguration");

            XmlNode xmlNodeProfiles = xmlDocument.SelectSingleNode("//Configuration/MouseModeConfigurations");

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
            if (copyProfile != null) { newProfileName = copyProfile.MouseModeName; }
            int countProfile = 0;
            XmlNodeList xmlNodesByName = xmlNodeProfiles.SelectNodes("MouseModeConfiguration/MouseModeName[text()='" + newProfileName + "']");

            if (xmlNodesByName.Count > 0)
            {
                while (xmlNodesByName.Count > 0)
                {
                    countProfile++;
                    xmlNodesByName = xmlNodeProfiles.SelectNodes("MouseModeConfiguration/MouseModeName[text()='" + newProfileName + countProfile.ToString() + "']");

                }
                newProfileName = newProfileName + countProfile.ToString();
            }


            XmlNode newNode = xmlDocument.CreateNode(XmlNodeType.Element, "MouseModeConfiguration", "");
            newNode.InnerXml = xmlNodeTemplate.InnerXml;
            newNode.SelectSingleNode("MouseModeName").InnerText = newProfileName;
            newNode.SelectSingleNode("ID").InnerText = getNewIDNumberForProfile(xmlDocument);
            newNode.SelectSingleNode("DefaultMode").InnerText = "False";


            xmlNodeProfiles.AppendChild(newNode);

            MouseMode profile = new MouseMode();
            this.Add(profile);
            profile.LoadProfile(newNode.SelectSingleNode("ID").InnerText, xmlDocument);

            xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);

            xmlDocument = null;

        }

        public string getNewIDNumberForProfile(XmlDocument xmlDocument)
        {
            //gets ID for new profiles
            int ID = 0;

            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/MouseModeConfigurations");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("MouseModeConfiguration/ID[text()='" + ID.ToString() + "']");

            while (xmlSelectedNode != null)
            {
                ID = ID + 1;
                xmlSelectedNode = xmlNode.SelectSingleNode("MouseModeConfiguration/ID[text()='" + ID.ToString() + "']");
            }
            //ID++;
            return ID.ToString();

        }

    }

    public class MouseMode
    {
        public string MouseModeName { get; set; } = "";

        public bool HIDHideEnabled { get; set; }
        public bool PowerCycleWithHIDHide { get; set; }
        public string ID { get; set; } = "";

        public double MouseSensitivity { get; set; }
        public double ScrollSensitivity { get; set; }

        public bool DefaultMode
        {
            get { return defaultMode; }
            set
            {
                defaultMode = value;
                if (value == true) { VisibilityDefaultProfile = Visibility.Visible; } else { VisibilityDefaultProfile = Visibility.Collapsed; }
            }
        }
        public bool defaultMode { get; set; }
        public bool activeMode { get; set; }
        public bool ActiveProfile
        {
            get { return activeMode; }
            set
            {
                activeMode = value;
                if (value == true) { VisibilityActiveProfile = Visibility.Visible; } else { VisibilityActiveProfile = Visibility.Collapsed; }
            }
        }
        public Visibility VisibilityActiveProfile { get; set; } = Visibility.Collapsed;
        public Visibility VisibilityDefaultProfile { get; set; } = Visibility.Collapsed;
        public Dictionary<string, string> mouseMode = new Dictionary<string, string>();



        public void applyProfile()
        {
            //remove active profile flag for current
            if (Global_Variables.Global_Variables.mousemodes.activeMouseMode != null)
            {
                Global_Variables.Global_Variables.mousemodes.activeMouseMode.ActiveProfile = false;

            }
            //apply active profile flag
            Global_Variables.Global_Variables.mousemodes.activeMouseMode = this;
            ActiveProfile = true;
        }
        public void SaveToXML()
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/MouseModeConfigurations");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("MouseModeConfiguration/ID[text()='" + ID + "']");

            if (xmlSelectedNode != null)
            {
                XmlNode parentNode = xmlSelectedNode.ParentNode;

                if (parentNode != null)
                {
                    foreach(XmlNode childNode in parentNode.ChildNodes)
                    {
                        switch(childNode.Name)
                        {
                            case "MouseModeName":
                                childNode.InnerText = MouseModeName;
                                break;
                            case "DefaultMode":
                                //do nothing, will handle below
                                break;
                            case "ID"://donothing
                                      break;
                            case "HIDHideEnabled":
                                childNode.InnerText = HIDHideEnabled.ToString();
                                break;
                            case "PowerCycleWithHIDHide":
                                childNode.InnerText = PowerCycleWithHIDHide.ToString();
                                break;
                            case "MouseSensitivity":
                                childNode.InnerText = MouseSensitivity.ToString();
                               
                                break;
                            case "ScrollSensitivity":
                                childNode.InnerText = ScrollSensitivity.ToString();
                                break;
                            default:
                                childNode.InnerText = mouseMode[childNode.Name];

                                break;
                        }
                    }
                    

                    //if ID isnt 0, which is the default profile, and its been saved to be the default profile, then make this ID 0 and change the other profile
                    if (DefaultMode.ToString() != parentNode.SelectSingleNode("DefaultMode").InnerText && DefaultMode)
                    {
                        //check to see if a default profile exists
                        XmlNode xmlCurrentDefault = xmlNode.SelectSingleNode("MouseModeConfiguration/DefaultMode[text()='True']");
                        if (xmlCurrentDefault != null)
                        {
                            //if not null set to false
                            xmlCurrentDefault.InnerText = "False";
                            //get the ID and change status in profiles list
                            string curDefID = xmlCurrentDefault.ParentNode.SelectSingleNode("ID").InnerText;
                            Global_Variables.Global_Variables.mousemodes.setCurrentDefaultProfileToFalse(curDefID);
                        }

                    }
                    parentNode.SelectSingleNode("DefaultMode").InnerText = DefaultMode.ToString();

                }


            }
            xmlDocument.Save(Global_Variables.Global_Variables.xmlFile);

            xmlDocument = null;


        }

        public void LoadProfile(string loadID, XmlDocument xmlDocument = null)
        {
            if (xmlDocument == null)
            {
                xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            }


            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/MouseModeConfigurations");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("MouseModeConfiguration/ID[text()='" + loadID + "']");

            if (xmlSelectedNode != null)
            {
                XmlNode parentNode = xmlSelectedNode.ParentNode;

                if (parentNode != null)
                {
                    MouseModeName = parentNode.SelectSingleNode("MouseModeName").InnerText;
                    ID = loadID;
                    if (parentNode.SelectSingleNode("DefaultMode").InnerText == "True") { DefaultMode = true; } else { DefaultMode = false; }
                    if (parentNode.SelectSingleNode("HIDHideEnabled").InnerText == "True") { HIDHideEnabled = true; } else { HIDHideEnabled = false; }
                    if (parentNode.SelectSingleNode("PowerCycleWithHIDHide").InnerText == "True") { PowerCycleWithHIDHide = true; } else { PowerCycleWithHIDHide = false; }

             
                    double dblSen;
                    if (double.TryParse(parentNode.SelectSingleNode("MouseSensitivity").InnerText, out dblSen))
                    {
                        MouseSensitivity = dblSen;
                    }
                    else { MouseSensitivity = 20; }

                    double dblSSen;
                    if (double.TryParse(parentNode.SelectSingleNode("MouseSensitivity").InnerText, out dblSSen))
                    {
                        ScrollSensitivity = dblSSen;
                    }
                    else { ScrollSensitivity = 1; }

                    mouseMode.Clear();
                    foreach (XmlNode node in parentNode.ChildNodes)
                    {

                        if (node.Name != "MouseSensitivity" && node.Name != "ScrollSensitivity" && node.Name != "MouseModeName" && node.Name != "ID" && node.Name != "DefaultMode" && node.Name != "HIDHideEnabled" && node.Name != "PowerCycleWithHIDHide")
                        {
                            mouseMode.Add(node.Name, node.InnerText);
                        }

                    }



                }


            }

            xmlDocument = null;

        }

    }

}
