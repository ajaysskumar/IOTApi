using IoT.Common.Model.Models;
using IoT.Common.Model.Utility;
using IoT.Core.Email;
using IoTOperations.Sensors;
using IoTOperations.ServiceHelper;
using IoTSensorPolling.PollModel;
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
    public partial class IoTPollService : ServiceBase
    {
        private static System.Timers.Timer aTimer;

        private static IoTApiClient client;

        static int _pollInterval = 0;
        static int _measureInterval = 0;


        private static int PollInterval { get { return _pollInterval * 1000; } }
        private static int MeasureInterval { get { return _measureInterval; } }
        //private ApplicationDbContext _context;
        public IoTPollService()
        {
            InitializeComponent();
            //_context = new ApplicationDbContext();
        }

        protected override void OnStart(string[] args)
        {


            // Create a timer with a ten second interval.
            aTimer = new System.Timers.Timer(10000);

            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            // Set the Interval to 2 seconds (2000 milliseconds).


            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    _pollInterval = int.Parse(context.SystemConfiguration.Where(x => x.Key == "PollInterval").FirstOrDefault().Value);
                    _measureInterval = int.Parse(context.SystemConfiguration.Where(x => x.Key == "MeasureInterval").FirstOrDefault().Value);

                    if (_pollInterval == 0)
                    {
                        throw new Exception("Value not defined for the configuration or it is 0.");
                    }
                }
            }
            catch (Exception ex)
            {
                this.Stop();
            }

            aTimer.Interval = PollInterval;
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
            List<MotionSensor> temperatureData = new List<MotionSensor>();

            List<EmailMappingModel> emailRecipientList = new List<EmailMappingModel>();

            List<Admin> recipientList = new List<Admin>();

            DateTime currenteDate = DateTime.UtcNow.AddSeconds(-MeasureInterval);

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                recipientList = context.Admin.Where(x => x.ShouldRecieve).ToList();

                foreach (var recipient in recipientList)
                {
                    var tempPoints = context.MotionsSensor.Where(x => x.DeviceId == recipient.SensorId && x.Timestamp >= currenteDate).OrderBy(x => x.Timestamp).ToList();

                    if (tempPoints.Count > 0)
                    {
                        int pointsCount = tempPoints.Count;

                        decimal averageTemp = tempPoints.Sum(x => decimal.Parse(x.MotionValue)) / pointsCount;
                        decimal averageHumid = tempPoints.Sum(x => decimal.Parse(x.MotionTime)) / pointsCount;

                        EmailMappingModel model = new EmailMappingModel
                        {
                            Admin = recipient,
                            AverageHumidity = averageHumid,
                            AverageTemperature = averageTemp
                        };

                        emailRecipientList.Add(model);
                    }



                }
            }

            foreach (var recipient in emailRecipientList)
            {
                if (recipient.AverageTemperature < recipient.Admin.Threshold)
                {
                    EmailClient client = new EmailClient();
                    client.SendEmail(recipient.AverageTemperature, recipient.AverageHumidity, MeasureInterval, recipient.Admin.Email, recipient.Admin.Name);
                }
            }
        }
    }
}
