﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Common.Model.Utility
{
    public class RelayGroup
    {
        public int Id { get; set; }
        public string RelayGroupIpAddress { get; set; }
        public string RelayGroupDescription { get; set; }
        public string RelayGroupLocation { get; set; }
        public string RelayGroupMac { get; set; }

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
