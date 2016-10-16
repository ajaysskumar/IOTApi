using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IoTOperations.Sensors
{
    [XmlRoot(ElementName = "Relay")]
    public class Relay
    {
        [XmlElement(ElementName = "RelayName")]
        public string RelayName { get; set; }
        [XmlElement(ElementName = "RelayStatus")]
        public string RelayStatus { get; set; }
    }

    [XmlRoot(ElementName = "Relays")]
    public class Relays
    {
        [XmlElement(ElementName = "Relay")]
        public List<Relay> Relay { get; set; }
    }


}
