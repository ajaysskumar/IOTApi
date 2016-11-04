using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace IoT.Core.AppManager
{

    [XmlRoot(ElementName = "Relay")]
    public class Relay
    {
        [XmlElement(ElementName = "RelayNumber")]
        public string RelayNumber { get; set; }
        [XmlElement(ElementName = "RelayStatus")]
        public string RelayStatus { get; set; }
    }

    [XmlRoot(ElementName = "Relays")]
    public class Relays
    {
        [XmlElement(ElementName = "Relay")]
        public Relay Relay { get; set; }
    }

}


