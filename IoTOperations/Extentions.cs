using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTOperations
{
    public static class Extentions
    {
        public static string Append(this String str,String valueToAppend)
        {
            return String.Concat(valueToAppend.ToCharArray(),valueToAppend);
        }
    }
}
