using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Common.Model.Utility
{
    public class RelayGroupView
    {
        public int Id { get; set; }
        public string RelayGroupIpAddress { get; set; }
        public string RelayGroupDescription { get; set; }
        public string RelayGroupLocation { get; set; }
        public string RelayGroupMac { get; set; }

    }

    public class RelayView
    {
        public int Id { get; set; }
        public int RelayNumber { get; set; }
        public string RelayDescription { get; set; }

        public bool RelayState { get; set; }
        public int RelayGroupId { get; set; }

        private Relay _relay;
        
    }
}
