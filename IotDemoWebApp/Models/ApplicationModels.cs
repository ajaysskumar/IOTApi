using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IotDemoWebApp.Models
{
    public class MotionSensor
    {
        public int Id { get; set; }


        public string MotionValue { get; set; }

        public string MotionTime { get; set; }

        public string DeviceId { get; set; }

        public DateTime Timestamp { get; set; }

        public virtual WifiSensor Device { get; set; }
    }

    public class WifiSensor
    {
        public string Id { get; set; }

        public string DeviceName { get; set; }

        public int OperationFrequecy { get; set; }
        
    }

    public class Trace {

        public int Id { get; set; }
        public string Input { get; set; }
        public string Timestamp { get; set; }
        public string Error { get; set; }
        public string MethodName { get; set; }
    }
}