using IoT.Common.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IotDemoWebApp.Models
{
    public class ChartModel
    {
        public DateTime StartTimePeriod { get; set; }
        public DateTime EndTimePeriod { get; set; }

        public string SelectedDeviceId { get; set; }
        public int TimeFrame { get; set; }

        public List<WifiSensor> Sensors { get; set; }
        //public IEnumerable<MotionSensor> SensorData { get; set; }
    }
}