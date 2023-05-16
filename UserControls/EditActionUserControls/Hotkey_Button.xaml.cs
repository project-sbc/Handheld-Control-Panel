using ControlzEx.Theming;
using Handheld_Control_Panel.Classes;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Display_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using Handheld_Control_Panel.Classes.TaskSchedulerWin32;
using Handheld_Control_Panel.Classes.UserControl_Management;
using Handheld_Control_Panel.Styles;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

using static Vanara.Interop.KnownShellItemPropertyKeys;

namespace Handheld_Control_Panel.UserControls
{
    /// <summary>
    /// Interaction logic for TDP_Slider.xaml
    /// </summary>
    public partial class Hotkey_Button : UserControl
    {
        private string windowpage = "";
        private string usercontrol = "";

        //var for hotkey input
        private DispatcherTimer gamepadTimer = new DispatcherTimer(DispatcherPriority.Render);
        private DispatcherTimer fiveSecondTimeOutTimer = new DispatcherTimer();
        private ushort controllerButtons = 0;
        private DateTime gamepadTimerTickCounter;
        private ushort currentGamepad = 0;
        private ushort previousGamepad = 0;
        public Hotkey_Button()
        {
            InitializeComponent();
        
          
        }

      
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput += handleControllerInputs;
            windowpage = WindowPageUserControl_Management.getWindowPageFromWindowToString(this);
            usercontrol = this.ToString().Replace("Handheld_Control_Panel.Pages.UserControls.","");
            Global_Variables.hotKeys.hotkeyClearedEvent += HotKeys_hotkeyClearedEvent;
            if (Global_Variables.hotKeys.editingHotkey.DisplayHotkey != "")
            {
                control.Content = Global_Variables.hotKeys.editingHotkey.DisplayHotkey;
            }
            else
            {
                control.Content = "...";
            }

        }

        private void HotKeys_hotkeyClearedEvent(object? sender, EventArgs e)
        {
            //event triggered when hotkey is changed from controller to keyboard or vice versa
            control.Content = "...";
        }

        private void handleControllerInputs(object sender, EventArgs e)
        {
            controllerUserControlInputEventArgs args= (controllerUserControlInputEventArgs)e;
            if (args.WindowPage == windowpage && args.UserControl==usercontrol)
            {
                if (args.Action == "A" && control.Visibility == Visibility.Visible)
                {
                    control.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
                else
                {
                    Classes.UserControl_Management.UserControl_Management.handleUserControl(border, control, args.Action);
                }

            }
        }
       


        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller_Window_Page_UserControl_Events.userControlControllerInput -= handleControllerInputs;
            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (control.IsLoaded)
            {
                startKB_Controller_Timer();
            }

        }

        #region gamepad timer routines
        private void gamepad_Tick(object sender, EventArgs e)
        {
            currentGamepad = ((ushort)Controller_Management.currentGamePad.Buttons);


           
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
                    Global_Variables.hotKeys.editingHotkey.Type = "Controller";
                    Global_Variables.hotKeys.editingHotkey.Hotkey = controllerButtons.ToString();
                    control.Content = Global_Variables.hotKeys.editingHotkey.DisplayHotkey;
                    stopKB_Controller_Timer(false);
                    return;
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


                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "LB"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "RB"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "LStick"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "RStick"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.Start)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "Start"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.Back)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "Back"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.A)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "A"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.B)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "B"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.X)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "X"); }
                if (gamepad.Buttons.HasFlag(GamepadButtonFlags.Y)) { gamepadCombo = makeGamepadButtonString(gamepadCombo, "Y"); }
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

        private void startKB_Controller_Timer()
        {
            //set variables to stop controller / kb input from being registered
            Controller_Management.suspendEventsForGamepadHotKeyProgramming = true;
            MouseKeyHook.programmingKeystroke = true;

            //set up separate timer to get controller input
            gamepadTimer.Tick += gamepad_Tick;
            gamepadTimer.Interval = TimeSpan.FromMilliseconds(70);
            gamepadTimer.Start();

            // setup 5 second timeout timer
            fiveSecondTimeOutTimer.Tick += fiveSecondTimeOutTimer_Tick;
            fiveSecondTimeOutTimer.Interval = new TimeSpan(0, 0, 5);

            //subscribe to event that tracks keyboard presses for a completed hotkey
            MouseKeyHook.keyboardEvents.keyboardStringPress += handleKeyboardStringPress;

            //set the control to the ... signifying its listening
            control.Content = "...";

            //start the 5 second timeout timer (after 5 seconds it stops and reverts the value back if no controller/kb input detected)
            fiveSecondTimeOutTimer.Start();

        }
        private void stopKB_Controller_Timer(bool timedOut)
        {
            //set variables to allow the normal function of keyboard/controller input
            Controller_Management.suspendEventsForGamepadHotKeyProgramming = false;
            MouseKeyHook.programmingKeystroke = false;

            //stop the specific timer for tracking controller hotkey inputs AND unsubscribe to prevent threads from staying open after closure
            gamepadTimer.Stop();
            gamepadTimer.Tick -= gamepad_Tick;
            MouseKeyHook.keyboardEvents.keyboardStringPress -= handleKeyboardStringPress;
            if (timedOut)
            {
                control.Content = Global_Variables.hotKeys.editingHotkey.DisplayHotkey;
            }
        }
      
      

        private void handleKeyboardStringPress(object sender, EventArgs args)
        {
            control.Content = (string)sender;
            Global_Variables.hotKeys.editingHotkey.Hotkey = (string)sender;
            Global_Variables.hotKeys.editingHotkey.Type = "Keyboard";
            stopKB_Controller_Timer(false);
        }
        
      

        #endregion
        #region keyboard timer
        private void fiveSecondTimeOutTimer_Tick(object sender, EventArgs e)
        {
            control.Content = Global_Variables.hotKeys.editingHotkey.DisplayHotkey;
            stopKB_Controller_Timer(true);
        }


        #endregion


    
    }
   
}
