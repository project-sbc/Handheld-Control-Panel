using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.UserControl_Management;
using Handheld_Control_Panel.Styles;
using MahApps.Metro.Controls;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class HotKey_Textbox : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";


        //var for hotkey input
        private DispatcherTimer gamepadTimer = new DispatcherTimer(DispatcherPriority.Render);
        private DispatcherTimer keyboardTimer = new DispatcherTimer();
        private ushort controllerButtons = 0;
        private int gamepadTimerTickCounter = 0;
        private ushort currentGamepad = 0;
        private ushort previousGamepad = 0;
      
        public HotKey_Textbox()
        {
            InitializeComponent();
            //UserControl_Management.setupControl(control);
            control.Content = Global_Variables.hotKeys.editingHotkey.DisplayHotkey;
         
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");
            
        }

       
        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {
                if (args.Action == "A" && control.Visibility == Visibility.Visible)
                {
                    button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
                else
                {
                    Classes.UserControl_Management.UserControl_Management.handleUserControl(border, control, args.Action);
                }

               
            }
        }

      


        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (button.IsLoaded)
            {
                if (Global_Variables.hotKeys.editingHotkey.Type == "Controller")
                {
                    startGamepadTimer();
                }
                else
                {
                    startKBTimer();
                }
            }

        }

        #region gamepad timer routines
        private void gamepad_Tick(object sender, EventArgs e)
        {
            currentGamepad = ((ushort)Controller_Management.currentGamePad.Buttons);

            gamepadTimerTickCounter = gamepadTimerTickCounter + 1;

            //timeout after 100 ticks or just over 5 seconds
            if (gamepadTimerTickCounter > 100)
            {
                stopGamepadTimer(true);
                controllerButtons = 0;
            }
            //start timer to read controller for inputs
            if (currentGamepad >= previousGamepad)
            {
                if (currentGamepad > 0)
                {
                    controllerButtons = currentGamepad;
                }

            }
            else
            {
                if (currentGamepad < previousGamepad && previousGamepad != 4096)
                {
                    //as soon as the button combo is LESS than the previous gamepad button AND isn't the A button (value of 4096) we know they have finished pressing all the buttons and we can now figure out what the combo is
                    stopGamepadTimer(false);
                    //string gamepadCombo = convertControllerUshortToString(controllerButtons.ToString());

                    
                    Global_Variables.hotKeys.editingHotkey.Hotkey = controllerButtons.ToString();
                    control.Content = Global_Variables.hotKeys.editingHotkey.DisplayHotkey;
                }

            }
            previousGamepad = currentGamepad;




        }

        private string convertControllerUshortToString(string hotkey)
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


        private void startGamepadTimer()
        {
            Controller_Management.suspendEventsForGamepadHotKeyProgramming = true;
            gamepadTimer.Tick += gamepad_Tick;
            gamepadTimer.Interval = TimeSpan.FromMilliseconds(70);
            gamepadTimerTickCounter = 0;
            Thread.Sleep(70);
            gamepadTimer.Start();
            control.Content = "...";
        }
        private void stopGamepadTimer(bool timedOut)
        {
            gamepadTimer.Stop();
            Controller_Management.suspendEventsForGamepadHotKeyProgramming = false;
            if (timedOut)
            {
                control.Content = Global_Variables.hotKeys.editingHotkey.DisplayHotkey;
            }
        }

        private void handleKeyboardStringPress(object sender, EventArgs args)
        {
            control.Content = (string)sender;
            MouseKeyHook.keyboardEvents.keyboardStringPress -= handleKeyboardStringPress;
            keyboardTimer.Stop();
        }
        private void startKBTimer()
        {

            MouseKeyHook.programmingKeystroke = true;
            keyboardTimer.Tick += keyboard_Tick;
            keyboardTimer.Interval = new TimeSpan(0, 0, 5);

            MouseKeyHook.keyboardEvents.keyboardStringPress += handleKeyboardStringPress;


            keyboardTimer.Start();
        }
        private void stopKBTimer()
        {
            keyboardTimer.Stop();
            MouseKeyHook.programmingKeystroke = false;

        }

        #endregion
        #region keyboard timer
        private void keyboard_Tick(object sender, EventArgs e)
        {
            control.Content = Global_Variables.hotKeys.editingHotkey.DisplayHotkey;
            stopKBTimer();
        }


        #endregion
    }
}
