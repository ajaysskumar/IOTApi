using AttendanceTracker.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AttendanceTracker.Portal.Controllers
{
    public class SystemController : ApiController
    {
        public ApplicationDbContext context = new ApplicationDbContext();

        public SystemController()
        {

        }

        [HttpPost]
        [Route("api/updatestatus")]
        public IHttpActionResult AddHost([FromBody]Host[] hosts)
        {
            try
            {
                var existingHosts = context.Host;

                foreach (var host in hosts)
                {
                    if (existingHosts.Where(x => x.Id.ToLower().Equals(host.Id.ToLower())).FirstOrDefault() == null)
                    {
                        context.Host.Add(host);
                    }
                    else
                    {
                        context.HostStatus.Add(new HostStatus
                        {
                            HostId = host.Id,
                            LastAccessedIP = host.LastAccessedIP,
                            LastStatusChecked = host.LastStatusChecked,
                            Status = host.Status
                        });
                    }

                }

                context.SaveChanges();

                return Ok("Changes Saved");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
