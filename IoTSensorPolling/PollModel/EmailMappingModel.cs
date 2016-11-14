using IoT.Common.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTSensorPolling.PollModel
{
    public class EmailMappingModel
    {
        public Admin Admin { get; set; }
        public decimal AverageTemperature { get; set; }
        public decimal AverageHumidity { get; set; }
    }
}
