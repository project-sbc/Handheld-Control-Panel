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

    
    public class librehardwaremonitor
    {
        public void Monitor()
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

            while (1==1)
            {
                computer.Accept(new UpdateVisitor());

                foreach (IHardware hardware in computer.Hardware)
                {
                    Debug.WriteLine("Hardware: {0}", hardware.Name);

                    foreach (IHardware subhardware in hardware.SubHardware)
                    {
                        Debug.WriteLine("\tSubhardware: {0}", subhardware.Name);

                        foreach (ISensor sensor in subhardware.Sensors)
                        {
                            Debug.WriteLine("\t\tSensor: {0}, value: {1}", sensor.Name, sensor.Value);
                        }
                    }

                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        Debug.WriteLine("\tSensor: {0}, value: {1}", sensor.Name, sensor.Value);
                    }
                }
                Task.Delay(400);
            }
           

            computer.Close();
        }
    }
}
