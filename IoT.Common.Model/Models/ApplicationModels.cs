using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Common.Model.Models
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

        [Display(Name ="Is Active")]
        public bool IsActive { get; set; }

    }

    public class Trace
    {

        public int Id { get; set; }
        public string InstanceName { get; set; }
        public DateTime Timestamp { get; set; }
        public int Level { get; set; }
        public Guid MsgId { get; set; }
        public string Message { get; set; }
        //public string Timestamp { get; set; }
        public string MethodName { get; set; }
        public string Error { get; set; }
        public string Input { get; set; }
    }

    public class Admin
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Mobile { get; set; }
        public bool ShouldRecieve { get; set; }
        public decimal Threshold { get; set; }
        public DateTime LastSmsRecievedTime { get; set; }

    }

    public class RelayGroup
    {
        public int Id { get; set; }
        public string RelayGroupIpAddress { get; set; }
        public string RelayGroupDescription { get; set; }
        public string RelayGroupLocation { get; set; }
        public string RelayGroupMac { get; set; }

        [Display(Name ="Is Active")]
        public bool IsActive { get; set; }

        public virtual ICollection<Relay> Relays { get; set; }

    }

    public class Relay
    {
        public int Id { get; set; }

        [Required]
        public int RelayNumber { get; set; }
        public string RelayDescription { get; set; }

        public bool RelayState { get; set; }
        public int RelayGroupId { get; set; }

        public virtual RelayGroup RelayGroup { get; set; }
    }

    public class Message
    {

        public Message()
        {
            this.MsgId = Guid.NewGuid();
        }

        public Guid MsgId { get; set; }
        public int RelayId { get; set; }
        public string Status { get; set; }
        public int CurrentRelayStatus { get; set; }

        public virtual Relay Relay { get; set; }
    }

    public class RequestLog
    {
        public int Id { get; set; }
        public Guid MsgId { get; set; }
        public int RelayId { get; set; }
        public string RelayGroupMac { get; set; }
        public string Status { get; set; }
        public int CurrentRelayStatus { get; set; }
        public DateTime RequestStartTime { get; set; }
        public DateTime RequestEndTime { get; set; }
        public virtual Relay Relay { get; set; }

    }

    public class RequestModel
    {
        public int RequestId { get; set; }
        //public Guid MsgId { get; set; }
        //public int UserId { get; set; }
        public int RelayNumber { get; set; }
        public string RelayGroupIp { get; set; }

    }
}
