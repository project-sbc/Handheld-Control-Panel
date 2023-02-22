using Gma.System.MouseKeyHook;
using Handheld_Control_Panel.Classes.Controller_Management;
using Handheld_Control_Panel.Classes.Global_Variables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace Handheld_Control_Panel.Classes
{
    public static class MouseKeyHook
    {
        public static IKeyboardMouseEvents m_GlobalHook;
        public static string runningKeyStroke = "";
        public static bool programmingKeystroke = false;
        public static keyboardEvents keyboardEvents = new keyboardEvents();
        public static void Subscribe()
        {
            // Note: for the application hook, use the Hook.AppEvents() instead
            m_GlobalHook = Hook.GlobalEvents();
     
            m_GlobalHook.KeyDown += GlobalHook_KeyEvent;
            m_GlobalHook.KeyUp += GlobalHook_KeyEvent;
        }



        private static void GlobalHook_KeyEvent(object? sender, KeyEventArgs e)
        {

            
            KeyEventArgsExt args = (KeyEventArgsExt)e;

            if (args.IsKeyDown) 
            { 
                if (runningKeyStroke == "") 
                { 
                    runningKeyStroke = e.KeyCode.ToString(); 
                } 
                else 
                { 
                    if(!runningKeyStroke.Contains(e.KeyCode.ToString()))
                    {
                        runningKeyStroke = runningKeyStroke + "+" + e.KeyCode.ToString();
                    }
                   
                }
                Debug.WriteLine(runningKeyStroke); 
                if (Global_Variables.Global_Variables.KBHotKeyDictionary.Count != null)
                {
                    if (Global_Variables.Global_Variables.KBHotKeyDictionary.ContainsKey(runningKeyStroke) && !programmingKeystroke)
                    {
                        args.SuppressKeyPress = true;
                        ActionParameter action = Global_Variables.Global_Variables.KBHotKeyDictionary[runningKeyStroke];
                        //HotKey_Management.runHotKeyAction(action);
                        runningKeyStroke = "";

                    }
                }
              
            }
            if (args.IsKeyUp)
            {
                keyboardEvents.raiseKeyboardStringPress(runningKeyStroke);
                runningKeyStroke = "";
            }

            //args.SuppressKeyPress = true;
        }



        public static void Unsubscribe()
        {
            m_GlobalHook.KeyDown -= GlobalHook_KeyEvent;
            m_GlobalHook.KeyUp -= GlobalHook_KeyEvent;

            //It is recommened to dispose it
            m_GlobalHook.Dispose();
        }




    }
    public class keyboardEvents
    {

        public event EventHandler keyboardStringPress;
        public void raiseKeyboardStringPress(string keyCombo)
        {
            
            keyboardStringPress?.Invoke(keyCombo, EventArgs.Empty);
        }

    
    }
}
