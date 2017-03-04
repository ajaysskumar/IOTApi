using IoT.Common.Logging;
using IoT.Common.Model.Models;
using IoT.Core.Email;
using IoT.Core.Sms;
using IotDemoWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

                            decimal averageTemp = tempPoints.Sum(x => x.MotionValue) / pointsCount;
                            decimal averageHumid = tempPoints.Sum(x => x.MotionTime) / pointsCount;

                            EmailMappingModel model = new EmailMappingModel
                            {
                                //Admin = recipient,
                                AdminName = recipient.Name,
                                Email = recipient.Email,
                                IntervalTime = MeasureInterval,
                                AverageHumidity = averageHumid,
                                AverageTemperature = averageTemp,
                                UpperThreshold = recipient.UpperTemperatureThreshold,
                                LowerThreshold = recipient.LowerTemperatureThreshold
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

        [Route("api/sms/contacts")]
        [HttpGet]
        public IHttpActionResult GetContacts()
        {
            List<Admin> admins = new List<Admin>();

            List<SmsContact> contacts = new List<SmsContact>();

            try
            {
                int notificationTimeInterval = Convert.ToInt32(ConfigurationManager.AppSettings["NotificationTimeInterval"]);

                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    admins = context.Admin.Where(x => x.ShouldRecieve).ToList();


                    foreach (var admin in admins)
                    {
                        var readings = context.MotionsSensor.Where(x => x.DeviceId == admin.SensorId && x.Timestamp > System.Data.Entity.DbFunctions.AddMinutes(DateTime.UtcNow, -notificationTimeInterval));

                        if (readings != null)
                        {
                            int count = readings.Count();

                            if (count != 0)
                            {
                                decimal averageTemp = readings.Sum(x => x.MotionValue) / count;
                                decimal averageHumidity = readings.Sum(x => x.MotionTime) / count;

                                if (averageTemp < admin.LowerTemperatureThreshold - 0.5m || averageTemp > admin.UpperTemperatureThreshold + 0.5m || averageHumidity < admin.LowerHumidityThreshold - 0.5m || averageHumidity > admin.UpperHumidityThreshold + 0.5m)
                                {
                                    contacts.Add(new SmsContact()
                                    {
                                        adminName = admin.Name,
                                        contactId = admin.Id.ToString(),
                                        deviceName = admin.Sensor.DeviceName,
                                        message = string.Format("Hi {0}, this is alert to your AC device, Temperature: {1}, Humidity: {2}", admin.Name, Math.Round(Convert.ToDecimal(averageTemp), 2), Math.Round(Convert.ToDecimal(averageHumidity), 2)),
                                        phone = admin.Mobile,
                                        email = admin.Email
                                    });
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(contacts);
        }

        [Route("api/sms/send")]
        [HttpPost]
        public async Task<IHttpActionResult> SendSms(SmsContact contact)
        {

            try
            {
                HttpClient smsSender = new HttpClient();

                if (!ModelState.IsValid)
                {
                    return BadRequest("model state not valid");
                }
                var data = new
                {
                    url= ConfigurationManager.AppSettings["SmsGatewayRequestUrl"],
                    username = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["SmsGatewayUser"]),
                    password = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["SmsGatewayPassword"]),
                    type = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["SmsGatewayRequestType"]),
                    dlr = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["SmsGatewayRequestDelivery"]),
                    source = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["SmsGatewayRequestSource"]),
                    destination = HttpUtility.UrlEncode(String.Format("91{0}",contact.phone)),
                    message = HttpUtility.UrlEncode(contact.message)
                };

                string encodedPostUrl = string.Format("{0}?username={1}&password={2}&type={3}&dlr={4}&destination={5}&source={6}&message={7}", data.url, data.username, data.password, data.type, data.dlr, data.destination, data.source, data.message);

                HttpResponseMessage responseMessage = await smsSender.PostAsync(encodedPostUrl, new StringContent(""));

                if (responseMessage.IsSuccessStatusCode)
                {
                    string messageResponseString = await responseMessage.Content.ReadAsStringAsync();

                    if (messageResponseString.Split('|')[0]=="1701")
                    {
                        return Ok(messageResponseString);
                    }
                    else
                    {
                        return BadRequest("Someting wrong happened. Response returned : " +messageResponseString );
                    }
                }

            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }

            return Ok();
        }
    }
}
