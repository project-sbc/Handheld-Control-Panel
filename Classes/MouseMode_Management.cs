using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using SharpDX.XInput;
using WindowsInput;
using System.Xml;
using WindowsInput.Native;
using System.IO;
using System.Xml.Serialization;
using MahApps.Metro.Controls;

namespace Handheld_Control_Panel.Classes
{
    public static class MouseModeLookup
    {
        public static List<string> MouseModeList = new List<string>()
 {
             {"LeftMouseClick" },
           {"RightMouseClick" },
            {"HCP OSK" },
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
           {"F12" }
          


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
        private double deadzone = Global_Variables.Global_Variables.settings.joystickDeadzone;
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


        public object lockObjectMouseMode = new object();
        public string mouseModeDirectory = AppDomain.CurrentDomain.BaseDirectory + "MouseModes\\";

        public MouseMode_Management()
        {
            //populates list
            if (!Directory.Exists(mouseModeDirectory))
            {
                Directory.CreateDirectory(mouseModeDirectory);
            }

            string[] files = Directory.GetFiles(mouseModeDirectory, "*.xml", SearchOption.TopDirectoryOnly);
            lock (lockObjectMouseMode)
            {
                foreach (string file in files)
                {
                    StreamReader sr = new StreamReader(file);
                    XmlSerializer xmls = new XmlSerializer(typeof(MouseMode));
                    this.Add((MouseMode)xmls.Deserialize(sr));
                    sr.Dispose();
                    xmls = null;

                }
            }

        }

        public void SaveToXML(MouseMode mouseMode)
        {
            //Profile profile = this.Find(o => o.ProfileName == profileName);
            if (mouseMode != null)
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    lock (lockObjectMouseMode)
                    {
                        StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "MouseModes\\" + mouseMode.MouseModeName + ".xml");
                        XmlSerializer xmls = new XmlSerializer(typeof(HotkeyItem));
                        xmls.Serialize(sw, mouseMode);
                        sw.Dispose();
                        xmls = null;
                    }

                }
                );

            }



        }

        public void addNewMouseMode(MouseMode copyMM =null)
        {
            MouseMode mm = new MouseMode();

            if (copyMM != null) { mm= copyMM; }
            string newProfileName = "New Mouse Mode";

            int x = 1;
            if (File.Exists(mouseModeDirectory + newProfileName + ".xml"))
            {
                while (File.Exists(mouseModeDirectory + newProfileName + x.ToString() + ".xml"))
                {
                    x++;
                }
                newProfileName = newProfileName + x.ToString();
            }

            mm.MouseModeName = newProfileName;

            this.Add(mm);
            Global_Variables.Global_Variables.mousemodes.SaveToXML(mm);



        }

    

    public void start_MouseMode()
        {
            //create background thread to handle controller input
            if (activeMouseMode != null)
            {
                getController();
                timerController.Interval = TimeSpan.FromMilliseconds(8);
                timerController.Tick += controller_Tick;
                timerController.Start();
                Global_Variables.Global_Variables.MouseModeEnabled = true;
            }
           
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

        public void checkResumePauseMouseMode(bool toggleOpen)
        {
            //this routine will toggle the mouse mode automatically when HCP is open to allow controller use on the panel
            if (Global_Variables.Global_Variables.MouseModeEnabled)
            {
                if (toggleOpen)
                {
                    timerController.Stop();
                }
                else
                {
                    timerController.Start();
                }
            }
        }

        private void checkSwapController()
        {

            //error number MMM03
            try
            {
                List<Controller> controllerList = new List<Controller>();

                controllerList.Add(new Controller(UserIndex.One));
                controllerList.Add(new Controller(UserIndex.Two));
                controllerList.Add(new Controller(UserIndex.Three));
                controllerList.Add(new Controller(UserIndex.Four));

                foreach (Controller swapController in controllerList)
                {

                    if (swapController != null)
                    {
                        if (swapController.IsConnected)
                        {
                            Gamepad swapGamepad = swapController.GetState().Gamepad;
                            if (swapGamepad.Buttons.HasFlag(GamepadButtonFlags.Start) && swapGamepad.Buttons.HasFlag(GamepadButtonFlags.Back))
                            {

                                controller = swapController;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Log_Writer.writeLog("Controller Management; " + ex.Message, "MMM03");

            }





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
                if (Global_Variables.Global_Variables.mousemodes.activeMouseMode != null)
                {
                    if (controller.IsConnected && activeMouseMode != null)
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
                                mouseScrollX = normalizeJoystickInputScroll(sensitivityScroll, currentGamePad.LeftThumbX);
                                mouseScrollY = normalizeJoystickInputScroll(sensitivityScroll, currentGamePad.LeftThumbY);
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
                                mouseScrollX = normalizeJoystickInputScroll(sensitivityScroll, -currentGamePad.RightThumbX);
                                mouseScrollY = normalizeJoystickInputScroll(sensitivityScroll, currentGamePad.RightThumbY);
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
                                        if (entry.Value == "HCP OSK")
                                        {
                                            if (currentGamePad.Buttons.HasFlag(flag) && !previousGamePad.Buttons.HasFlag(flag))
                                            {
                                                Global_Variables.Global_Variables.mainWindow.toggleOSK();
                                            }
                                           
                                        }
                                        else
                                        {
                                            VirtualKeyCode vkc = keyLookUp[entry.Value];
                                            if (currentGamePad.Buttons.HasFlag(flag) && !previousGamePad.Buttons.HasFlag(flag))
                                            {
                                                try
                                                {
                                                    inputSimulator.Keyboard.KeyPress(vkc);
                                                }
                                                catch
                                                {

                                                }
                                            }
                                            
                                        }
                                      


                                    }
                                    else
                                    {
                                        try
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
                                        catch
                                        {

                                        }
                                    }

                                }


                            }

                        }
                        try
                        {
                            inputSimulator.Mouse.MoveMouseBy((int)mouseX, (int)mouseY);
                            inputSimulator.Mouse.HorizontalScroll(-(int)mouseScrollX);
                            inputSimulator.Mouse.VerticalScroll((int)mouseScrollY);
                        }
                        catch
                        {

                        }

                        previousGamePad = currentGamePad;
                    }
                }
              

            }

        }

        private double normalizeJoystickInput(double sensitivity, double value)
        {
            double returnValue;
            if (Math.Abs(value) > deadzone)
            {
                if (Math.Abs(value) < (deadzone + (0.5 * 32768)))
                {
                    returnValue = 1 + (4 / (0.5 * 32768)) * (Math.Abs(value) - deadzone);
                    returnValue = Math.Round(returnValue, 0, MidpointRounding.ToZero);
                    Debug.WriteLine(returnValue);
                    if (value < 0) { return -returnValue; } else { return returnValue; }
                }
                else
                {
                    returnValue = 5 + sensitivity * Math.Log10(Math.Abs(value) / (deadzone + (0.5 * 32768))) / Math.Log10(32768 / (deadzone + (0.5 * 32768)));
                    //returnValue = (sensitivity * (Math.Abs(value) / 32768) / Math.Sqrt((Math.Abs(value) / 32768 * Math.Abs(value) / 32768) + 1));
                    //returnValue = 2 + 50 * (1 - Math.Exp(-( Math.Abs(value)- (deadzone + (0.4 * 32768))) / 32768));
                    returnValue = Math.Round(returnValue, 0, MidpointRounding.ToZero);
                    if (value < 0) { returnValue = -returnValue; }
                    Debug.WriteLine(returnValue.ToString());
                    return returnValue;
                }


            }
            else
            {
                return 0;
            }
        }
        private bool lastScrollOutput0 = false;
        private double normalizeJoystickInputScroll(double sensitivity, double value)
        {
            double returnValue;
            if (lastScrollOutput0)
            {
                lastScrollOutput0 = false;
                if (Math.Abs(value) > deadzone)
                {
                    returnValue = 5 * (1 - Math.Pow(0.75, (Math.Abs(value) - 1000) / 9000));
                    //returnValue = Math.Pow(Math.Abs(value) / deadzone, 1 / 4.6);
                    //returnValue = Math.Round(returnValue, 0, MidpointRounding.ToZero);
                    Debug.WriteLine(returnValue);
                    if (value < 0) { return -returnValue; } else { return returnValue; }

                }
                else
                {
                    return 0;
                }
            }
            else
            {
                lastScrollOutput0 = true;
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

        public void setCurrentDefaultProfileToFalse(string name)
        {
            //changes 
            foreach (MouseMode profile in this)
            {
                if (profile.MouseModeName == name)
                {
                    profile.DefaultMode = false;
                }
            }

        }
       
        public void deleteProfile(MouseMode profile)
        {
            if (profile != null)
            {
                string name = profile.MouseModeName;

                if (File.Exists(mouseModeDirectory + name + ".xml"))
                {
                    File.Delete(mouseModeDirectory + name + ".xml");
                }

                this.Remove(profile);
            }


        }
              
        public void loadMouseMode(string MouseModeName)
        {
            MouseMode loadingMM = this.Find(o => o.MouseModeName == MouseModeName);
            if (loadingMM != null)
            {
                if (File.Exists(mouseModeDirectory + "\\" + MouseModeName + ".xml"))
                {
                    lock (lockObjectMouseMode)
                    {
                        using (StreamReader sw = new StreamReader(mouseModeDirectory + "\\" + MouseModeName + ".xml"))
                        {
                            XmlSerializer xmls = new XmlSerializer(typeof(MouseMode));
                            loadingMM = (MouseMode)xmls.Deserialize(sw);
                        }
                    }


                }


            }
        }
    
    }

    public class MouseMode
    {
        public string MouseModeName { get; set; } = "";

        public bool HIDHideEnabled { get; set; }
        public bool PowerCycleWithHIDHide { get; set; }
     
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
       

    }

}
