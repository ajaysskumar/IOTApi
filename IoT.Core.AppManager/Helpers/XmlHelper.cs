using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IoT.Core.AppManager.Helpers
{
    public static class XmlHelper<T> where T: class
    {
        public static T ConvertToObject(string xmlString)
        {
            try
            {
                if (xmlString==null)
                {
                    return null;
                }
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                StringReader rdr = new StringReader(xmlString);
                T resultingMessage = (T)serializer.Deserialize(rdr);

                return resultingMessage;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
    }
}
