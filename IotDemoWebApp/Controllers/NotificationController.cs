using IoT.Common.Logging;
using IoT.Common.Model.Models;
using IoT.Core.Email;
using IotDemoWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static IoT.Core.AppManager.Helpers.SystemConfiguration;

namespace IotDemoWebApp.Controllers
{
    public class NotificationController : ApiController
    {
        static int _pollInterval = 0;
        static int _measureInterval = 0;


        private static int PollInterval { get { return _pollInterval; } }
        private static int MeasureInterval { get { return _measureInterval; } }

        public IHttpActionResult PostNotification()
        {
            List<MotionSensor> temperatureData = new List<MotionSensor>();

            List<EmailMappingModel> emailRecipientList = new List<EmailMappingModel>();

            List<Admin> recipientList = new List<Admin>();

            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    _measureInterval = _measureInterval = int.Parse(context.SystemConfiguration.Where(x => x.Key == "MeasureInterval").FirstOrDefault().Value);

                    DateTime currenteDate = DateTime.UtcNow.AddSeconds(-MeasureInterval);

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
                                //Admin = recipient,
                                AdminName = recipient.Name,
                                Email = recipient.Email,
                                IntervalTime = MeasureInterval,
                                AverageHumidity = averageHumid,
                                AverageTemperature = averageTemp,
                                UpperThreshold = recipient.UpperThreshold,
                                LowerThreshold = recipient.LowerThreshold
                            };

                            emailRecipientList.Add(model);
                        }
                    }
                    foreach (var recipient in emailRecipientList)
                    {
                        if (recipient.AverageTemperature < recipient.LowerThreshold || recipient.AverageTemperature>recipient.UpperThreshold)
                        {
                            EmailClient client = new EmailClient();
                            client.SendEmail(recipient.AverageTemperature, recipient.AverageHumidity, recipient.IntervalTime, recipient.Email, recipient.AdminName);
                        }
                    }
                    return Ok(emailRecipientList);
                }
            }
            catch (Exception ex)
            {
                IoTEventSourceManager.Log.Error(ex.Message, ApplicationConstants.IoTApiName);
                return BadRequest(ex.Message);
            }
        }

        public IHttpActionResult GetNotification()
        {
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    _pollInterval = int.Parse(context.SystemConfiguration.Where(x => x.Key == "PollInterval").FirstOrDefault().Value);
                    return Ok(PollInterval); 
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
