using Handheld_Control_Panel.Classes.Controller_Management;
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
                //QAMNavigation.windowQAMNavigation = true;
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
                //highlight first control
                Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent("Highlight", windowpage, correctUserControl(userControls[0].ToString()));
                intReturn[0] = 0;
            }
            else
            {
                if (selectedIndex != -1)
                {
                    switch (action)
                    {
                        case "B":
                            intReturn[1] = -1;
                            Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent(action, windowpage, correctUserControl(userControls[highlightedIndex].ToString()));

                            break;
                        case "A":
                            intReturn[1] = -1;
                            Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent(action, windowpage, correctUserControl(userControls[highlightedIndex].ToString()));

                            break;
                     
                        default:
                            Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent(action, windowpage, correctUserControl(userControls[highlightedIndex].ToString()));
                            break;

                    }
                }
                else
                {
                    switch (action)
                    {
                        case "A":
                            if (userControls[highlightedIndex].ToString().Contains("Dropdown"))
                            {
                                intReturn[1] = highlightedIndex;
                                Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent(action, windowpage, correctUserControl(userControls[highlightedIndex].ToString()));
                            }

                            break;
                        case "Up":
                            if (highlightedIndex > 0)
                            {
                                Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent("Unhighlight", windowpage, correctUserControl(userControls[highlightedIndex].ToString()));
                                Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent("Highlight", windowpage, correctUserControl(userControls[highlightedIndex - 1].ToString()));
                                intReturn[0] = highlightedIndex - 1;
                                if (intReturn[0] == 0) { ((IScrollInfo)stackPanel).MouseWheelUp(); }

                                userControls[intReturn[0]].BringIntoView();
                            }
                            break;
                        case "Down":
                            if (highlightedIndex < userControls.Count - 1)
                            {
                                Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent("Unhighlight", windowpage, correctUserControl(userControls[highlightedIndex].ToString()));
                                Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent("Highlight", windowpage, correctUserControl(userControls[highlightedIndex + 1].ToString()));
                                intReturn[0] = highlightedIndex + 1;
                                if (intReturn[0] == userControls.Count - 1) { ((IScrollInfo)stackPanel).MouseWheelUp(); }
                                //((IScrollInfo)stackPanel).MouseWheelDown();
                                userControls[intReturn[0]].BringIntoView();
                            }
                            break;
                        default:
                            Controller_Window_Page_UserControl_Events.raiseUserControlControllerInputEvent(action, windowpage, correctUserControl(userControls[highlightedIndex].ToString()));
                            break;

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
