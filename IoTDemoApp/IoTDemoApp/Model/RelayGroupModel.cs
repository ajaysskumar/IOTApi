using System;
using System.Collections.Generic;
using System.Text;
using Java.Lang;

namespace IoTDemoApp.Model
{
    public class RelayGroupModel
    {
        public int Id { get; set; }
        public string RelayGroupIpAddress { get; set; }
        public string RelayGroupDescription { get; set; }
        public string RelayGroupLocation { get; set; }
        public string RelayGroupMac { get; set; }

        public static explicit operator RelayGroupModel(Java.Lang.Object v)
        {
            throw new NotImplementedException();
        }
    }
}
