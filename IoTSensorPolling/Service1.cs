using IoTOperations.Sensors;
using IoTOperations.ServiceHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IoTSensorPolling
{
    public partial class Service1 : ServiceBase
    {
        private static System.Timers.Timer aTimer;

        //string url = "http://192.168.100.186";

        private static IoTApiClient client = new IoTApiClient("http://192.168.100.186");
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            client = new IoTApiClient("http://192.168.100.186");

            // Create a timer with a ten second interval.
            aTimer = new System.Timers.Timer(10000);

            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            // Set the Interval to 2 seconds (2000 milliseconds).
            aTimer.Interval = 2000;
            aTimer.Enabled = true;

            Console.WriteLine("Press the Enter key to exit the program.");
            Console.ReadLine();

            // If the timer is declared in a long-running method, use
            // KeepAlive to prevent garbage collection from occurring
            // before the method ends.
            //GC.KeepAlive(aTimer);
        }

        protected override void OnStop()
        {
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            List<Relay> relays = client.GetRelayStatus().Result;
            client.ToggleSwitch();
        }
    }
}
