using System;
using System.Collections.Generic;
using System.Text;

namespace IoTDemoApp.Model
{
    public class RelayModel
    {
        public int Id { get; set; }
        public int RelayNumber { get; set; }
        public string RelayDescription { get; set; }
        public bool RelayState { get; set; }
        public int RelayGroupId { get; set; }
    }
}
