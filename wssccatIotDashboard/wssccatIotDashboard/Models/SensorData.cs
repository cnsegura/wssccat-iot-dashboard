using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wssccatIotDashboard.Models
{
    public class SensorData
    {
        public int BarometricPressure { get; set; } 
        //public float CelciusTemperature { get; set; } 
        public int FahrenheitTemperature { get; set; } 
        public int Humidity { get; set; } 
        public string TimeStamp { get; set; } 
        public string ClientName { get; set; }
    }
}