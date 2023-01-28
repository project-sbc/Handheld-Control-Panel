﻿using Handheld_Control_Panel.Classes.Controller_Management;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Handheld_Control_Panel.Classes
{
    public static class WindowPageUserControl_Management
    {
        //COMMON routines across all pages/windows/usercontrols.... to make it so that a one size fits all method exists
        public static string getWindowPageFromWindowToString(DependencyObject thisObject)
        {
            //get page from object
            int counter = 0;
            DependencyObject parent = thisObject;
            while (!parent.ToString().Contains("Page") && counter < 20)
            {
                counter++;
                parent = parent.GetParentObject();
            }

            //gets window page value so I know what window and page is being used
            string combo = Window.GetWindow(thisObject).ToString() + parent.ToString();
            return combo.Replace(" ", "").Replace(":", "").Replace("Handheld_Control_Panel.", "").Replace("Pages.", ""); 
        }
     
        public static void switchToOuterNavigation(string window)
        {
            //global method to switch the page back to the window navigation
            if (window.Contains("MainWindow"))
            {
                MainWindowNavigation.windowNavigation = true;
            }
            if (window.Contains("QuickAccessMenu"))
            {
                QuickAccessMenuNavigation.windowNavigation = true;
            }
            if (window.Contains("GameLauncher"))
            {
                GameLauncherNavigation.windowNavigation = true;
            }
        }


        public static int[] globalHandlePageControllerInput(string windowpage, string action, List<UserControl> userControls, int highlightedIndex, int selectedIndex, StackPanel stackPanel)
        {
            //global method so that all pages can process there controller input through here, this makes it easier to update across all pages 

            //two things need to be returned: highlight and selected index, that will be done in this array
            int[] intReturn = new int[2];
            intReturn[0] = highlightedIndex;
            intReturn[1] = selectedIndex;

            if (highlightedIndex == -1)
            {
                if (action == "B" || action == "Left")
                {
                    //if nothing selected and 
                    WindowPageUserControl_Management.switchToOuterNavigation(windowpage);
                }
                else
                {
                    //highlight first control
                    Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent("Highlight", windowpage, correctUserControl(userControls[0].ToString()));
                    intReturn[0] = 0;
                }
            }
            else
            {
                if (selectedIndex == -1)
                {
                    if (action == "A")
                    {
                        Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent("Select", windowpage, correctUserControl(userControls[highlightedIndex].ToString()));
                        //set selected index to highlighted index
                        intReturn[1] = highlightedIndex;
                    }
                    if (action == "Up")
                    {
                        if (highlightedIndex > 0)
                        {
                            Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent("Unhighlight", windowpage, correctUserControl(userControls[highlightedIndex].ToString()));
                            Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent("Highlight", windowpage, correctUserControl(userControls[highlightedIndex - 1].ToString()));
                            intReturn[0] = highlightedIndex-1;
                            ((IScrollInfo)stackPanel).MouseWheelUp();
                            userControls[intReturn[0]].BringIntoView();
                
                            
                        }
                    }
                    if (action == "Down")
                    {
                        if (highlightedIndex < userControls.Count-1)
                        {
                            Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent("Unhighlight", windowpage, correctUserControl(userControls[highlightedIndex].ToString()));
                            Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent("Highlight", windowpage, correctUserControl(userControls[highlightedIndex + 1].ToString()));
                            intReturn[0] = highlightedIndex+ 1;
                            ((IScrollInfo)stackPanel).MouseWheelDown();
                            userControls[intReturn[0]].BringIntoView();
                        }
                    }
                    if (action == "B" || action == "Left")
                    {
                        WindowPageUserControl_Management.switchToOuterNavigation(windowpage);
                        Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent("Unhighlight", windowpage, correctUserControl(userControls[highlightedIndex].ToString()));
                        intReturn[0] = -1;
                    }
                }
                else
                {
                    if (action == "B")
                    {
                        //press b on a selected usercontrol means it goes back to being highlighted and selectedindex is -1
                        Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent("Highlight", windowpage, correctUserControl(userControls[selectedIndex].ToString()));
                        intReturn[1] = -1;
                    }
                    else
                    {
                        Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent(action, windowpage, correctUserControl(userControls[selectedIndex].ToString()));
                    }

                }
            }
            return intReturn;
        }

        private static string correctUserControl(string ToString)
        {
            return ToString.Replace("Handheld_Control_Panel.Pages.UserControls.", "");
        }



    }
}