using RTSSSharedMemoryNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Handheld_Control_Panel.Classes
{
    public class TEMPTEST
    {
        public TEMPTEST() 
        {
            MessageBox.Show("initialized");
        }

        public void littleTest()
        {
            MessageBox.Show("TEST LITTLE");
        }
        public void gameRunning()
        {
            MessageBox.Show("will it work");


            MessageBox.Show("start the routine");
            string gameRunning = "";
            try
            {
                MessageBox.Show("rtss running?");
                if (RTSS.RTSSRunning())
                {
                    MessageBox.Show("about to start osd");
                    RTSSSharedMemoryNET.OSD OSD = new RTSSSharedMemoryNET.OSD("checkgamerunning2");
                                       
                    MessageBox.Show("Get app entries is next");
                    var appEntries = OSD.GetAppEntries(AppFlags.Direct3D12);

                    //appEntries = OSD.GetAppEntries(appFlag);
                    MessageBox.Show("Get ready to loop");
       
                    foreach (var app in appEntries)
                    {
                        MessageBox.Show(app.Name);
                        string[] gamedir = app.Name.Split('\\');
                        if (gamedir.Length > 0)
                        {
                            MessageBox.Show("splitting name");
                            string currGameName = gamedir[gamedir.Length - 1];
                            gameRunning = currGameName.Substring(0, currGameName.Length - 4);
                            MessageBox.Show(gameRunning);
                            break;
                        }



                    }
                    MessageBox.Show("dispose");
                    OSD.Dispose();

                }




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Log_Writer.writeLog(ex.Message, "OSDM01");
       
            }
          
        }
    }
}
