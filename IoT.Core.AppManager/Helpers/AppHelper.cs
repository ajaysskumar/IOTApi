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

        public static int GetNodeMCUPin(int pinNumber)
        {
            switch (pinNumber)
            {
                case 1: return 16;
                case 2: return 4;
                case 3: return 5;
                case 4: return 12;
                case 5: return 13;
                case 6: return 14;
                default: return 0;
            }
        }
    }
}
