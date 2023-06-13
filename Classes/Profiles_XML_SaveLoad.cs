using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ControlzEx.Standard;
using Handheld_Control_Panel.Classes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Handheld_Control_Panel.Classes
{
    public static class Profiles_XML_SaveLoad
    {
        private static string hcpDirectory = AppDomain.CurrentDomain.BaseDirectory + "Profiles\\";
        private static object lockObject = new object();

        public static void Save_XML(string folderName, string objType, object objClass)
        {
            if (!Directory.Exists(hcpDirectory + folderName))
            {
                Directory.CreateDirectory(hcpDirectory + folderName);
            }

            lock (lockObject)
            {
                StreamWriter sw = new StreamWriter(hcpDirectory + folderName + "\\" + objType  +".xml");
                XmlSerializer xmls = null;
                switch(objType)
                {
                    case "Profile_Main":
                        xmls = new XmlSerializer(typeof(Profile_Main));
                        Profile_Main objMain = (Profile_Main)objClass;
                        xmls.Serialize(sw, objMain);
                        objMain = null;
                        break;
                    case "Profile_Info":
                        xmls = new XmlSerializer(typeof(Profile_Info));
                        Profile_Info objInfo = (Profile_Info)objClass;
                        xmls.Serialize(sw, objInfo);
                        objInfo = null;
                        break;
                    case "Profile_Exe":
                        xmls = new XmlSerializer(typeof(Profile_Exe));
                        Profile_Exe objExe = (Profile_Exe)objClass;
                        xmls.Serialize(sw, objExe);
                        objExe = null;
                        break;
                    case "Profile_Parameters":
                        xmls = new XmlSerializer(typeof(Profile_Parameters));
                        Profile_Parameters objParameters = (Profile_Parameters)objClass;
                        xmls.Serialize(sw, objParameters);
                        objParameters = null;
                        break;
                    case "Profile_ControllerMapping":
                        xmls = new XmlSerializer(typeof(Profile_ControllerMapping));
                        Profile_ControllerMapping objCM = (Profile_ControllerMapping)objClass;
                        xmls.Serialize(sw, objCM);
                        objCM = null;
                        break;
                }
                sw.Dispose();
                xmls = null;
                
            }
        }
        public static object Load_XML(string folderPath, string objType)
        {
            string filePath = hcpDirectory + folderPath + "\\" + objType + ".xml";
            object objObject = null;
            if (File.Exists(filePath))
            {
                lock (lockObject)
                {
                    StreamReader sr = new StreamReader(filePath);
                    XmlSerializer xmls = new XmlSerializer(typeof(Profile));

                    switch (objType)
                    {
                        case "Profile_Main":
                            xmls = new XmlSerializer(typeof(Profile_Main));
                            objObject = ((Profile_Main)xmls.Deserialize(sr));
                            break;
                        case "Profile_Info":
                            xmls = new XmlSerializer(typeof(Profile_Info));
                            objObject = ((Profile_Info)xmls.Deserialize(sr));
                            break;
                        case "Profile_Exe":
                            xmls = new XmlSerializer(typeof(Profile_Exe));
                            objObject = ((Profile_Exe)xmls.Deserialize(sr));
                            break;
                        case "Profile_Parameters":
                            xmls = new XmlSerializer(typeof(Profile_Parameters));
                            objObject = ((Profile_Parameters)xmls.Deserialize(sr));
                            break;
                        case "Profile_ControllerMapping":
                            xmls = new XmlSerializer(typeof(Profile_ControllerMapping));
                            objObject = ((Profile_ControllerMapping)xmls.Deserialize(sr));
                            break;
                    }
                    
                    sr.Dispose();
                    xmls = null;
                }
            }
            return objObject;
        }
      

    }
}
