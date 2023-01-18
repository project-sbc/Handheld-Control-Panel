using Handheld_Control_Panel.Classes.Global_Variables;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handheld_Control_Panel.Classes
{
    public static class HotKey_Management
    {
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


        public static ushort convertStringToControllerUshort(string hotkey)
        {
            ushort gamepadCombo = 0;
            if (hotkey != "")
            {
                
                if (hotkey.Contains("+"))
                {
                    string[] buttons = hotkey.Split("+");
                    
                    foreach (string button in buttons)
                    {
                        gamepadCombo = (ushort)(gamepadCombo + controllerFlagUshortLookup[button]);
                    }
                }
                else { gamepadCombo = controllerFlagUshortLookup[hotkey]; }
            }

            return gamepadCombo;

        }
        public static string convertControllerUshortToString(ushort hotkey)
        {
            string gamepadCombo = "";
            Gamepad gamepad = new Gamepad();
            gamepad.Buttons = (GamepadButtonFlags)(hotkey);


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

            return gamepadCombo;


        }
        private static string makeGamepadButtonString(string currentValue, string addValue)
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

        public static void runHotKeyAction(ActionParameter actionParameter)
        {
            switch (actionParameter.Action)
            {
                case "Show_Hide_HCP":
                    //Controller_Management.Controller_Management..RaiseOpenAppEvent();
                    break;
             
                default: break;
            }
        }
    }
}
