using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IoTOperations.XmlOps
{
    public static class XmlHelper<T>where T:class
    {
        public static T ParseXmlToObject(string contents)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
            T resultingMessage = (T)serializer.Deserialize(memStream);
            return resultingMessage;
        }
    }
}
