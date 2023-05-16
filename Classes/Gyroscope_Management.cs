using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace Handheld_Control_Panel.Classes
{
    public static class Gyroscope_Management
    {
        private static double number = 0;

        public static void startReading()
        {

           
            Accelerometer gyro = Accelerometer.GetDefault();
            AccelerometerReading gyroread= gyro.GetCurrentReading();
           
        }

        private static void Gyro_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            AccelerometerReading reading = args.Reading;
            Debug.WriteLine(reading.AccelerationZ);
            Debug.WriteLine(reading.AccelerationY);
            Debug.WriteLine(number.ToString());
            Debug.WriteLine(reading.AccelerationX);
        }

      
    }
}
