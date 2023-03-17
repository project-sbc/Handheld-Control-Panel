using RTSSSharedMemoryNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Handheld_Control_Panel.Classes
{
    public static class AutoTDP_Management
    {

        public static void test()
        {
            var OSD = new RTSSSharedMemoryNET.OSD("new");

            int i = 0;

            while (i < 1000)
            {
                var appEntries = OSD.GetAppEntries(AppFlags.Direct3D12);
                foreach (var app in appEntries)
                {
                    string[] osdArr = { app.InstantaneousFrameTime.ToString(), app.InstantaneousFrames.ToString() };
                    OSD.Update(("#" + i.ToString() + " Time: " +DateTime.Now + " " + app.InstantaneousFrames.ToString()));

                }
                Thread.Sleep(450);
                i = i + 1;
            }

        }
    }
}
