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

    public class DeviceEntityViewModel
	{
        public string DeviceId { get; set; }
        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; }
    }

    public class DeviceViewModel : WifiSensor
    {
        public Boolean IsLinkedToIoTHub { get; set; }
    }

    public class SmsContact
    {
        [Required]
        public string message { get; set; }

        [Required]
        public string phone { get; set; }
        [Required]
        public string email { get; set; }

        [Required]
        public string adminName { get; set; }
        [Required]
        public string deviceName { get; set; }
        [Required]
        public string contactId { get; set; }
    }
}