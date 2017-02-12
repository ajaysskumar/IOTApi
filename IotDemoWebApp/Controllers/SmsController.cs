using IoT.Common.Model.Models;
using IoT.Core.Sms;
using IoT.Core.Sms.TextLocal;
using IotDemoWebApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IotDemoWebApp.Controllers
{
    public class SmsController : ApiController
    {
        
        public SmsController()
        {
            
        }

        [Route("api/sms/send")]
        [HttpPost]
        public IHttpActionResult SendSms()
        {

            IoT.Core.Sms.TwilioSms.TwilioSmsClient twilioClient = new IoT.Core.Sms.TwilioSms.TwilioSmsClient(ConfigurationManager.AppSettings["TwilioAccountSID"], ConfigurationManager.AppSettings["TwilioAuthToken"]);

            twilioClient.SendSMS("+919990014318","This is sample twilio message");

            return Ok();
        }

        [Route("api/sms/contacts")]
        [HttpGet]
        public IHttpActionResult GetContacts()
        {
            List<Admin> admins = new List<Admin>();

            List<SmsContact> contacts = new List<SmsContact>();

            using (ApplicationDbContext context= new ApplicationDbContext())
            {
                admins = context.Admin.Where(x=>x.ShouldRecieve).ToList();


                foreach (var admin in admins)
                {
                    var readings = context.MotionsSensor.Where(x => x.DeviceId == admin.SensorId && x.Timestamp >System.Data.Entity.DbFunctions.AddMinutes(DateTime.UtcNow,-15));

                    if (readings!=null)
                    {
                        int count = readings.Count();

                        decimal averageTemp = readings.Sum(x => x.MotionValue) / count;
                        decimal averageHumidity = readings.Sum(x => x.MotionTime) / count;

                        if (averageTemp<admin.LowerTemperatureThreshold-0.5m || averageTemp > admin.UpperTemperatureThreshold + 0.5m || averageHumidity<admin.LowerHumidityThreshold-0.5m ||averageHumidity>admin.UpperHumidityThreshold+0.5m)
                        {
                            contacts.Add(new SmsContact()
                            {
                                adminName = admin.Name,
                                contactId = admin.Id.ToString(),
                                deviceName =admin.Sensor.DeviceName,
                                message = string.Format("Hi {0}, this is alert to your AC device, Temperature: {1}, Humidity: {2}",admin.Name,averageTemp,averageHumidity),
                                phone = admin.Mobile
                            });
                        }
                    }
                    
                }
            }

            return Ok(contacts);
        }
    }
}
