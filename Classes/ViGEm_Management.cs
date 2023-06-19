using Nefarius.ViGEm.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nefarius.ViGEm.Client;
using Windows.Media.Protection.PlayReady;
using Nefarius.ViGEm.Client.Targets;

namespace Handheld_Control_Panel.Classes
{
    public static class ViGEm_Management
    {
        // initializes the SDK instance
        public static ViGEmClient vigemClient = null;  
        public static IXbox360Controller x360 = null;  
        public static bool startViGEm()
        {
            //this checks if ViGEm is installed and will activate it if its set to enable
            try
            {
                if (Global_Variables.Global_Variables.settings.useHIDHideAndVIGEM)
                {
                    vigemClient = new ViGEmClient();
                    x360 = vigemClient.CreateXbox360Controller();
                    x360.Connect();
                
                }

                return true;
            }
            catch 
            {
                
                return false;
            }
            
        }
      

    }
}
