using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Core.AppManager.Helpers
{
    public static class AppHelper
    {
        public static string GetStatus(bool status)
        {
            if (status == true)
                return "1";
            else
                return "0";
        }
    }
}
