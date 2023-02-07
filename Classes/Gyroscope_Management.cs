using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace Handheld_Control_Panel.Classes
{
    public static class Gyroscope_Management
    {
        private static double number = 0;
        private static void ReadingChanged(object sender, AccelerometerReadingChangedEventArgs e)
        {

            //Accelerometer acc = Accelerometer.GetDefault();
            //Accelerometer gyro = Accelerometer.GetDefault();

            //gyro.ReadingChanged += ReadingChanged;
            //AccelerometerReading reading = e.Reading;
            //Debug.WriteLine( reading.AccelerationZ);
            //Debug.WriteLine(reading.AccelerationY);
            //number =number  +reading.AccelerationX;
            //Debug.WriteLine(number.ToString());
            //Debug.WriteLine(reading.AccelerationX);
        }
    }
}
