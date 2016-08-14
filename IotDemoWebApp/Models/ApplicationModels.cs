using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IotDemoWebApp.Models
{
    public class MotionSensorModel
    {
        public int Id { get; set; }


        public decimal MotionValue { get; set; }

        public decimal MotionTime { get; set; }

        public DateTime Timestamp { get; set; }
    }
}