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

namespace Handheld_Control_Panel.Classes.MouseMode_Management
{
    public static class MouseMode_Management
    {
        public static InputSimulator inputSimulator;
        public static Controller controller;
        public static DispatcherTimer timerController = new DispatcherTimer(DispatcherPriority.Render);
        public static Gamepad currentGamePad;
        public static Gamepad previousGamePad;
        public static int sensitivity = 10;
        public static void start_MouseMode()
        {
            //create background thread to handle controller input
            getController();
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
                    short xCon = currentGamePad.LeftThumbX;      
                    short yCon = currentGamePad.LeftThumbY;
                    double x = 0;
                    double y = 0;
                    if (xCon > 0)
                    {
                        x = (sensitivity /Math.Log(32768))*Math.Log(xCon+1);
                        x = Math.Round(x, 0);
                    }
                    else
                    {
                        if (xCon < 0)
                        {
                            x = (sensitivity / Math.Log(32768)) * Math.Log(Math.Abs(xCon) + 1);
                            x = -1*Math.Round(x, 0);
                        }
                    }
                    if (yCon > 0)
                    {
                        y = (sensitivity / Math.Log(32768)) * Math.Log(yCon + 1);
                        y = Math.Round(y, 0);
                    }
                    else
                    {
                        if (yCon < 0)
                        {
                            y = (sensitivity / Math.Log(32768)) * Math.Log(Math.Abs(yCon) + 1);
                            y = -1 * Math.Round(y, 0);
                        }
                    }
                    inputSimulator.Mouse.MoveMouseBy((int)x,(int)y);
                 

                    previousGamePad = currentGamePad;
                }

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
