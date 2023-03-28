using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handheld_Control_Panel.Classes
{
    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }

    
    public static class librehardwaremonitor
    {
        public static void Monitor()
        {
            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = false,
                IsMotherboardEnabled = false,
                IsControllerEnabled = false,
                IsNetworkEnabled = false,
                IsStorageEnabled = false,
                IsBatteryEnabled = true,
                IsPsuEnabled = true
            };

            computer.Open();
            int i = 1;
            while (1==1)
            {
                computer.Accept(new UpdateVisitor());

                foreach (IHardware hardware in computer.Hardware)
                {
                    //Debug.WriteLine("Hardware: {0}", hardware.Name);

                    foreach (IHardware subhardware in hardware.SubHardware)
                    {
                        //Debug.WriteLine("\tSubhardware: {0}", subhardware.Name);

                        foreach (ISensor sensor in subhardware.Sensors)
                        {
                           
                            if (sensor.Name.Contains("Package") || sensor.Name.Contains("ddCPU Total"))
                            {
                                Debug.WriteLine("\t\tSensor: {0}, value: {1}", sensor.Name, sensor.Value);
                            }
                            
                        }
                    }

                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        
                        if (sensor.Name.Contains("Package") || sensor.Name.Contains("ddCPU Total"))
                        {
                            Debug.WriteLine("\t\tSensor: {0}, value: {1}", sensor.Name, sensor.Value);
                        }
                    }
                }
                i = 0;
                Task.Delay(400);
            }
           

            computer.Close();
        }
    }
}
