using IoT.Common.Logging;
using IoT.Common.Model.Models;
using IoT.Common.Model.Utility;
using IoT.Core.AppManager.Models;
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

using IoTAppHelper = IoT.Core.AppManager.Helpers;

namespace IoTSensorPolling
{
    public partial class IoTSensorSubscriptionService : ServiceBase
    {
        private static System.Timers.Timer aTimer;

        //private static IoTApiClient client;

        static int _pollInterval = 0;
        static int _measureInterval = 0;

        static MqttClient _mqttClient = new MqttClient(
                IoT.Core.AppManager.Helpers.SystemConfiguration.MqttServerAddress,
                System.Environment.MachineName,
                IoT.Core.AppManager.Helpers.SystemConfiguration.MqttServerUserName,
                IoT.Core.AppManager.Helpers.SystemConfiguration.MqttServerPassword);



        static List<MotionSensor> temperatureData = new List<MotionSensor>();

        static List<EmailMappingModel> emailRecipientList = new List<EmailMappingModel>();

        static List<Admin> recipientList = new List<Admin>();

        static DateTime currenteDate = DateTime.UtcNow.AddSeconds(-MeasureInterval);
        static Status currentRelayStatus = null;
        static string _relayGroupMac = "18:FE:34:D4:7F:85";
        static string _sensorIdMac = "18:FE:34:D4:7F:85";
        static decimal _minThreshold = 24;
        static decimal _maxThreshold = 26;

        private static int PollInterval { get { return _pollInterval * 1000; } }
        private static int MeasureInterval { get { return _measureInterval; } }
        private static string CurrentActiveRelayGroupMac { get { return _relayGroupMac; } }

        private static string CurrentActiveSensorIdMac { get { return _sensorIdMac; } }
        private static decimal MinThreshold { get { return _minThreshold; } }
        private static decimal MaxThreshold { get { return _maxThreshold; } }
        public IoTSensorSubscriptionService()
        {
            InitializeComponent();
            _mqttClient = new MqttClient(
                IoT.Core.AppManager.Helpers.SystemConfiguration.MqttServerAddress,
                System.Environment.MachineName,
                IoT.Core.AppManager.Helpers.SystemConfiguration.MqttServerUserName,
                IoT.Core.AppManager.Helpers.SystemConfiguration.MqttServerPassword);
            _mqttClient.Start();
            _mqttClient.RegisterOurSubscriptions("relayActionConfirmation/18:FE:34:D4:7F:85");
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
                IoT.Common.Logging.IoTEventSourceManager.Log.Debug("inside On Start - Before Db context", "WindowsServiceLog");

                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    _pollInterval = int.Parse(context.SystemConfiguration.Where(x => x.Key == "PollInterval").FirstOrDefault()?.Value);
                    _measureInterval = int.Parse(context.SystemConfiguration.Where(x => x.Key == "MeasureInterval").FirstOrDefault()?.Value);

                    //_relayGroupMac = context.SystemConfiguration.Where(x => x.Key == "CurrentActiveRelayGroupMac").FirstOrDefault()?.Value;
                    //_sensorIdMac = context.SystemConfiguration.Where(x => x.Key == "CurrentActiveSensorIdMac").FirstOrDefault()?.Value;
                    //_minThreshold = decimal.Parse(context.SystemConfiguration.Where(x => x.Key == "MinThreshold").FirstOrDefault()?.Value);
                    //_maxThreshold = decimal.Parse(context.SystemConfiguration.Where(x => x.Key == "MaxThreshold").FirstOrDefault()?.Value);


                    if (_pollInterval == 0)
                    {
                        IoT.Common.Logging.IoTEventSourceManager.Log.Debug("pollinterval : value not defined for the configuration or it is 0.", "WindowsServiceLog");
                        throw new Exception("Value not defined for the configuration or it is 0.");
                    }
                }
            }
            catch (Exception ex)
            {
                IoTEventSourceManager.Log.Debug("WindowsServiceLog", ex.Message);
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
            IoTEventSourceManager.Log.Debug("Inside On timed Interval", "WindowsServiceLog");

            temperatureData = new List<MotionSensor>();

            emailRecipientList = new List<EmailMappingModel>();

            recipientList = new List<Admin>();

            currenteDate = DateTime.UtcNow.AddSeconds(-MeasureInterval);

            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {

                    _relayGroupMac = context.SystemConfiguration.Where(x => x.Key == "CurrentActiveRelayGroupMac").FirstOrDefault()?.Value;
                    _sensorIdMac = context.SystemConfiguration.Where(x => x.Key == "CurrentActiveSensorIdMac").FirstOrDefault()?.Value;
                    _minThreshold = decimal.Parse(context.SystemConfiguration.Where(x => x.Key == "MinThreshold").FirstOrDefault()?.Value);
                    _maxThreshold = decimal.Parse(context.SystemConfiguration.Where(x => x.Key == "MaxThreshold").FirstOrDefault()?.Value);

                    IoTEventSourceManager.Log.Debug(string.Format("{0}-{1}-{2}-{3}", _relayGroupMac, _sensorIdMac, _minThreshold, _maxThreshold), "WindowsServiceLog");

                    //recipientList = context.Admin.Where(x => x.ShouldRecieve).ToList();

                    //foreach (var recipient in recipientList)
                    //{
                    var tempPoints = context.MotionsSensor.Where(x => x.DeviceId == CurrentActiveSensorIdMac && x.Timestamp >= currenteDate).OrderBy(x => x.Timestamp).ToList();

                    if (tempPoints.Count > 0)
                    {
                        int pointsCount = tempPoints.Count;

                        decimal averageTemp = tempPoints.Sum(x => decimal.Parse(x.MotionValue)) / pointsCount;
                        decimal averageHumid = tempPoints.Sum(x => decimal.Parse(x.MotionTime)) / pointsCount;

                        if (averageTemp<MinThreshold)
                        {

                        }

                        try
                        {
                            if (!_mqttClient.ClientConnected)
                            {
                                _mqttClient.Start();
                                _mqttClient.RegisterOurSubscriptions(string.Format("relayActionConfirmation/{0}",CurrentActiveRelayGroupMac));
                            }

                            try
                            {
                                currentRelayStatus = _mqttClient.GetRelayGroupStatus(CurrentActiveRelayGroupMac);
                            }
                            catch (Exception ex)
                            {
                                IoTEventSourceManager.Log.Debug(ex.Message, "WindowsServiceLog");
                            }

                            if (currentRelayStatus != null)
                            {
                                string msgId = Guid.NewGuid().ToString();

                                string statusToSet = IoTAppHelper.AppHelper.GetStatus(currentRelayStatus.Relays.Relay.Where(x => x.RelayName == "1").FirstOrDefault().RelayStatus == "1" ? false : true);

                                IoTEventSourceManager.Log.Debug(string.Format("Request to Relay : {0}; Status to set : {1}; MsgId : {2}", currentRelayStatus.DeviceId, statusToSet, msgId), "WindowsServiceLog");

                                if (
                                    (averageTemp>MinThreshold || averageTemp<MaxThreshold)
                                    && currentRelayStatus.Relays.Relay.Where(x=>x.RelayName=="1").FirstOrDefault().RelayStatus==IoTAppHelper.SystemConfiguration.DeviceConfig.SwitchOff
                                    )
                                {
                                    IoTEventSourceManager.Log.Debug(string.Format("Switch On Request to Relay : {0}; Status to set : {1}; MsgId : {2}", currentRelayStatus.DeviceId, IoTAppHelper.SystemConfiguration.DeviceConfig.SwitchOn, msgId), "WindowsServiceLog");

                                    _mqttClient.PublishSomething(IoTAppHelper.AppHelper.GetNodeMCUPin(1).ToString(), IoTAppHelper.SystemConfiguration.DeviceConfig.SwitchOn, msgId, string.Format("relayActionRequest/{0}", currentRelayStatus.DeviceId));
                                }
                                if(
                                    (averageTemp < MinThreshold || averageTemp > MaxThreshold)
                                    && currentRelayStatus.Relays.Relay.Where(x => x.RelayName == "1").FirstOrDefault().RelayStatus == IoTAppHelper.SystemConfiguration.DeviceConfig.SwitchOn
                                    )
                                {
                                    IoTEventSourceManager.Log.Debug(string.Format("Switch Off Request to Relay : {0}; Status to set : {1}; MsgId : {2}", currentRelayStatus.DeviceId, IoTAppHelper.SystemConfiguration.DeviceConfig.SwitchOff, msgId), "WindowsServiceLog");

                                    _mqttClient.PublishSomething(IoTAppHelper.AppHelper.GetNodeMCUPin(1).ToString(), IoTAppHelper.SystemConfiguration.DeviceConfig.SwitchOff, msgId, string.Format("relayActionRequest/{0}", currentRelayStatus.DeviceId));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            IoTEventSourceManager.Log.Debug(ex.Message, "WindowsServiceLog");
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                IoTEventSourceManager.Log.Debug(ex.Message, "WindowsServiceLog");
            }

            try
            {
                if (!_mqttClient.ClientConnected)
                {
                    _mqttClient.Start();
                    _mqttClient.RegisterOurSubscriptions("relayActionConfirmation/18:FE:34:D4:7F:85");
                }

                try
                {
                    currentRelayStatus = _mqttClient.GetRelayGroupStatus(CurrentActiveRelayGroupMac);
                }
                catch (Exception ex)
                {
                    IoTEventSourceManager.Log.Debug(ex.Message, "WindowsServiceLog");
                }

                if (currentRelayStatus != null)
                {
                    string msgId = Guid.NewGuid().ToString();

                    string statusToSet = IoTAppHelper.AppHelper.GetStatus(currentRelayStatus.Relays.Relay.Where(x => x.RelayName == "1").FirstOrDefault().RelayStatus == "1" ? false : true);

                    IoTEventSourceManager.Log.Debug(string.Format("Request to Relay : {0}; Status to set : {1}; MsgId : {2}", currentRelayStatus.DeviceId, statusToSet, msgId), "WindowsServiceLog");

                    if (currentRelayStatus.Relays.Relay.Where(x => x.RelayName == "1").FirstOrDefault().RelayStatus == IoT.Core.AppManager.Helpers.SystemConfiguration.DeviceConfig.SwitchOff)
                    {
                        IoTEventSourceManager.Log.Debug(string.Format("Switch On Request to Relay : {0}; Status to set : {1}; MsgId : {2}", currentRelayStatus.DeviceId, IoTAppHelper.SystemConfiguration.DeviceConfig.SwitchOn, msgId), "WindowsServiceLog");

                        _mqttClient.PublishSomething(IoTAppHelper.AppHelper.GetNodeMCUPin(1).ToString(), IoTAppHelper.SystemConfiguration.DeviceConfig.SwitchOn, msgId, string.Format("relayActionRequest/{0}", currentRelayStatus.DeviceId));
                    }
                    else
                    {
                        IoTEventSourceManager.Log.Debug(string.Format("Switch Off Request to Relay : {0}; Status to set : {1}; MsgId : {2}", currentRelayStatus.DeviceId, IoTAppHelper.SystemConfiguration.DeviceConfig.SwitchOff, msgId), "WindowsServiceLog");

                        _mqttClient.PublishSomething(IoTAppHelper.AppHelper.GetNodeMCUPin(1).ToString(), IoTAppHelper.SystemConfiguration.DeviceConfig.SwitchOff, msgId, string.Format("relayActionRequest/{0}", currentRelayStatus.DeviceId));
                    }
                }
            }
            catch (Exception ex)
            {
                IoTEventSourceManager.Log.Debug(ex.Message, "WindowsServiceLog");
            }
            //}
        }
    }
}
