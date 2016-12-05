using IoT.Common.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IotDemoWebApp.Models
{
    public class EmailMappingModel
    {
        public String AdminName { get; set; }
        public int IntervalTime { get; set; }
        public string Email { get; set; }
        public decimal AverageTemperature { get; set; }
        public decimal AverageHumidity { get; set; }
        public decimal UpperThreshold { get; set; }
        public decimal LowerThreshold { get; set; }
    }
}