using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IoT.Core.AppManager.Models
{

    [XmlRoot(ElementName = "r")]
    public class Relay
    {
        [XmlElement(ElementName = "rn")]
        public string RelayName { get; set; }
        [XmlElement(ElementName = "rs")]
        public string RelayStatus { get; set; }
    }

    [XmlRoot(ElementName = "rl")]
    public class Relays
    {
        [XmlElement(ElementName = "r")]
        public List<Relay> Relay { get; set; }
    }

    [XmlRoot(ElementName = "s")]
    public class Status
    {
        [XmlElement(ElementName = "d")]
        public string DeviceId { get; set; }
        [XmlElement(ElementName = "m")]
        public string MsgId { get; set; }
        [XmlElement(ElementName = "rl")]
        public Relays Relays { get; set; }
    }


}
