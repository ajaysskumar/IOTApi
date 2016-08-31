using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace IotDemoWebApp
{
    public static class Helper
    {
        //public static IList<IList<T>> Split<T>(IList<T> source)
        //{
        //    return source
        //        .Select((x, i) => new { Index = i, Value = x })
        //        .GroupBy(x => x.Index / 3)
        //        .Select(x => x.Select(v => v.Value).ToList())
        //        .ToList();
        //}

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        public static string ConvertObjectToXML(object objectToConvert)
        {
            XmlDocument xmlDoc = new XmlDocument();   //Represents an XML document, 
                                                      // Initializes a new instance of the XmlDocument class.          
            XmlSerializer xmlSerializer = new XmlSerializer(objectToConvert.GetType());
            // Creates a stream whose backing store is memory. 
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, objectToConvert);
                xmlStream.Position = 0;
                //Loads the XML document from the specified string.
                xmlDoc.Load(xmlStream);
                return xmlDoc.InnerXml;
            }
        }
    }
}