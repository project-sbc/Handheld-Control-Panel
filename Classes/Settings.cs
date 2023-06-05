using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Handheld_Control_Panel.Classes
{
    public static class Load_Settings
    {
        public static Settings loadSettings(string filename)
        {
            if (File.Exists(filename))
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    XmlSerializer xmls = new XmlSerializer(typeof(Settings));
                    return xmls.Deserialize(sr) as Settings;
                }
            }

            
            Settings newSettings = new Settings();
            return newSettings;
        }
    }

    public class Settings
    {
        public string systemAccent { get; set; } = "Teal";
        public string language { get; set; } = "English";
        public string IntelMMIOMSR { get; set; } = "MMIO+MSR";
        public int minTDP { get; set; } = 5;
        public int maxTDP { get; set; } = 35;
        public bool checkUpdatesAtStartUp { get; set; } = true;
        public bool dockWindowRight { get; set; } = true;
        public int maxGPUCLK { get; set; } = 1500;


        public List<TestXMLSaving> testXMLSavings = new List<TestXMLSaving>();




        public void Save(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(Settings));
                xmls.Serialize(sw, this);
            }
        }

       
    }

    public class TestXMLSaving
    {
        public string profilename { get; set; }
        public string test1 { get; set; } = "test1";
        public string test2 { get; set; } = "test2";
        public string test3 { get; set; } = "test3";

    }
}
