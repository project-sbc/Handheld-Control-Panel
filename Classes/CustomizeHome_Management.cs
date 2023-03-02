using MahApps.Metro.IconPacks;
using SharpDX;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Handheld_Control_Panel.Classes
{
    public class CustomizeHome_Management: List<HomePageItem>
    {
        
        public HomePageItem editingItem = null;

        public CustomizeHome_Management()
        {
            //populates list
            List<string> list = Properties.Settings.Default.qamUserControls.Split(';').ToList(); 
            foreach(string item in list)
            {
             
                if (item != "")
                {
                    HomePageItem hpi = new HomePageItem();
                    hpi.UserControl = item.Substring(0,item.Length - 1);
                    hpi.DisplayUsercontrol = Application.Current.Resources[hpi.UserControl.ToString()].ToString();
                    string boolValue = item.Substring(item.Length - 1, 1);
                    if (boolValue == "1") { hpi.Enabled= true; } else { hpi.Enabled= false; }
                    hpi.UpArrowTag = hpi.UserControl + "_Up";
                    hpi.DownArrowTag = hpi.UserControl + "_Down";
                    hpi.EnableMovementTag = hpi.UserControl + "_EnableMovement";
                    this.Add(hpi);
                }
            }
        }

        public void saveList()
        {
            string newList = "";
            foreach (HomePageItem hpi in this)
            {
                newList = newList + hpi.UserControl.ToString();
                if (hpi.Enabled) { newList = newList + "1;"; } else { newList = newList + "0;"; }

            }
            Properties.Settings.Default.qamUserControls= newList;
            Properties.Settings.Default.Save();

        }

    }

    public class HomePageItem
    {
        public string UserControl { get; set; }
        public string DisplayUsercontrol { get; set; }
        public bool Enabled { get; set; } = true;
        public PackIconUniconsKind iconKind { get; set; } = PackIconUniconsKind.Bars;
        
        public string UpArrowTag { get; set; }
        public string DownArrowTag { get; set; }


        public Visibility updownVisibility { get; set; } = Visibility.Collapsed;
        public Visibility enableMovementVisibility { get; set; } = Visibility.Visible;
        public string EnableMovementTag { get; set; }

  

    }
}
