using System;
using System.Collections.Generic;
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

        [Route("api/sendsms")]
        public IHttpActionResult SendSms()
        {
            int status = Helper.SendSMS("CI00178342", "ajay.kumar@onactuate.com", "eicO0kC5","+919811631863","Hello Sample message");

            return Ok(status);
        }
    }
}
