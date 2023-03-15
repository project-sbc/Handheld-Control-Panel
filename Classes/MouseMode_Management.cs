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

namespace Handheld_Control_Panel.Classes.MouseMode_Management
{
    public static class MouseMode_Management
    {
        public static InputSimulator inputSimulator;
        public static Controller controller;
        public static DispatcherTimer timerController = new DispatcherTimer(DispatcherPriority.Render);
        public static Gamepad currentGamePad;
        public static Gamepad previousGamePad;
        private static int sensitivity =20;
        private static int joystickButtonPressSensivitiy =6000;
        private static Dictionary<string, string> mouseMode = new Dictionary<string, string>();

        private static Dictionary<string, VirtualKeyCode> keyLookUp =
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
        private static Dictionary<string, GamepadButtonFlags> buttonLookup = new Dictionary<string, GamepadButtonFlags>()
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



        public static void start_MouseMode()
        {
            //create background thread to handle controller input
            getController();
            loadConfiguration();
            timerController.Interval = TimeSpan.FromMilliseconds(10);
            timerController.Tick += controller_Tick;
            inputSimulator = new InputSimulator();
            timerController.Start();
            
        }
        public static bool status_MouseMode()
        {
            return timerController.IsEnabled;
        }
        public static bool toggle_MouseMode()
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
        public static void end_MouseMode()
        {
            timerController.Stop();
            timerController.Tick -= controller_Tick;
            inputSimulator = null;

        }
        public static void loadConfiguration()
        {
            mouseMode.Clear();
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(Global_Variables.Global_Variables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/MouseModeConfigurations/MouseModeConfiguration");

            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                mouseMode.Add(node.Name, node.InnerText);

            }
            xmlDocument = null;
            

        }

        private static void controller_Tick(object sender, EventArgs e)
        {
            //start timer to read and compare controller inputs
            //Controller input handler

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

                    switch(mouseMode["LeftThumb"])
                    {
                        case "Mouse":
                            mouseX = normalizeJoystickInput(sensitivity, currentGamePad.LeftThumbX);
                            mouseY = normalizeJoystickInput(sensitivity, -currentGamePad.LeftThumbY);
                            break;
                        case "Scroll":
                            mouseScrollX = normalizeJoystickInput(sensitivity, currentGamePad.LeftThumbX);
                            mouseScrollY = normalizeJoystickInput(sensitivity, -currentGamePad.LeftThumbY);
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

                    switch (mouseMode["RightThumb"])
                    {
                        case "Mouse":
                            mouseX = normalizeJoystickInput(sensitivity, currentGamePad.RightThumbX);
                            mouseY = normalizeJoystickInput(sensitivity, -currentGamePad.RightThumbY);
                            break;
                        case "Scroll":
                            mouseScrollX = normalizeJoystickInput(sensitivity, currentGamePad.RightThumbX);
                            mouseScrollY = normalizeJoystickInput(sensitivity, -currentGamePad.RightThumbY);
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


                    foreach (KeyValuePair<string,string> entry in mouseMode)
                    {
                        if (!entry.Key.Contains("Thumb") && entry.Key != "LT" && entry.Key != "RT")
                        {
                            if (entry.Value != "")
                            {
                                GamepadButtonFlags flag = buttonLookup[entry.Key];
                                if (!entry.Value.Contains("Mouse"))
                                {
                                    VirtualKeyCode vkc = keyLookUp[entry.Value];
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
                                else
                                {
                                    switch(entry.Value)
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

                    inputSimulator.Mouse.MoveMouseBy((int)mouseX,(int)mouseY);
                    inputSimulator.Mouse.HorizontalScroll((int)mouseScrollX);
                    inputSimulator.Mouse.VerticalScroll((int)mouseScrollY);

                    previousGamePad = currentGamePad;
                }

            }

        }

        private static double normalizeJoystickInput(int sensitivty, double value)
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
        private static void getController()
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

    }

   
}
