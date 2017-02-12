using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Common.Model.Models
{
    public class MotionSensor
    {
        public int Id { get; set; }


        public decimal MotionValue { get; set; }

        public decimal MotionTime { get; set; }

        public string DeviceId { get; set; }

        public DateTime Timestamp { get; set; }

        //public virtual WifiSensor Device { get; set; }
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
        //[NotMapped]
        public string MsgId { get; set; }
        public string Message { get; set; }
        //public string Timestamp { get; set; }
        public string MethodName { get; set; }
        public string MyProperty { get; set; }
        public string Error { get; set; }
        public string Input { get; set; }
    }

    public class Admin
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Mobile { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public bool ShouldRecieve { get; set; }
        public decimal UpperTemperatureThreshold { get; set; }
        public decimal LowerTemperatureThreshold { get; set; }
        public decimal UpperHumidityThreshold { get; set; }
        public decimal LowerHumidityThreshold { get; set; }
        
        public String SensorId { get; set; }
        //public DateTime LastSmsRecievedTime { get; set; }

        public virtual WifiSensor Sensor { get; set; }

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

    public class SystemConfiguration
    {
        public int Id { get; set; }

        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }
    }

    public class DeviceEntity {

        public string Id { get; set; }
        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; }
        public string PrimaryThumbPrint { get; set; }
        public string SecondaryThumbPrint { get; set; }
        public string ConnectionString { get; set; }
        public string ConnectionState { get; set; }
        public DateTime LastActivityTime { get; set; }
        public DateTime LastConnectionStateUpdatedTime { get; set; }
        public DateTime LastStateUpdatedTime { get; set; }
        public int MessageCount { get; set; }
        public string State { get; set; }
        public string SuspensionReason { get; set; }

        public int CompareTo(DeviceEntity other)
        {
            return string.Compare(this.Id, other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return $"Device ID = {this.Id}, Primary Key = {this.PrimaryKey}, Secondary Key = {this.SecondaryKey}, Primary Thumbprint = {this.PrimaryThumbPrint}, Secondary Thumbprint = {this.SecondaryThumbPrint}, ConnectionString = {this.ConnectionString}, ConnState = {this.ConnectionState}, ActivityTime = {this.LastActivityTime}, LastConnState = {this.LastConnectionStateUpdatedTime}, LastStateUpdatedTime = {this.LastStateUpdatedTime}, MessageCount = {this.MessageCount}, State = {this.State}, SuspensionReason = {this.SuspensionReason}\r\n";
        }
    }
}
