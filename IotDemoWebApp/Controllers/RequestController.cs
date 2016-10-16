using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using IotDemoWebApp.Models;
using IoT.Common.Model.Utility;

namespace IotDemoWebApp.Controllers
{
    public class RequestApiController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Request
        public IQueryable<RequestLog> GetRequestLog()
        {
            return db.RequestLog;
        }

        // GET: api/Request
        [Route("api/requestToProcess")]
        public IHttpActionResult GetRequestToProcess()
        {
            RequestLog request = db.RequestLog.OrderBy(x=>x.RequestStartTime).Where(x=>x.Status=="Initiated").FirstOrDefault();

            if (request.Relay == null)
            {
                request.Relay = db.Relay.Where(x => x.Id == request.RelayId).FirstOrDefault();
            }

            //request.RelayGroup = request.Relay.RelayGroup;

            RequestModel requestModel = new RequestModel {
                RequestId = request.Id,
                RelayGroupIp = request.Relay.RelayGroup.RelayGroupIpAddress,
                RelayName = request.Relay.RelayName
            };
            //request.RelayGroup.Relays = null;

            return Ok(requestModel);
        }



        // GET: api/Request/5
        [ResponseType(typeof(RequestLog))]
        public async Task<IHttpActionResult> GetRequestLog(int id)
        {
            RequestLog requestLog = await db.RequestLog.FindAsync(id);
            if (requestLog == null)
            {
                return NotFound();
            }

            return Ok(requestLog);
        }

        // PUT: api/Request/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRequestLog(int id, IoT.Common.Model.Utility.RequestLog requestLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != requestLog.Id)
            {
                return BadRequest();
            }

            db.Entry(requestLog).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestLogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Request
        [ResponseType(typeof(RequestLog))]
        public async Task<IHttpActionResult> PostRequestLog([FromBody]Message message)
        {
            RequestLog requestLog = new RequestLog();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            requestLog.MsgId = message.MsgId;
            requestLog.RelayId = 1;
            requestLog.RequestStartTime = DateTime.UtcNow;
            requestLog.RequestEndTime = DateTime.UtcNow;
            requestLog.Status = "Initiated";

            db.RequestLog.Add(requestLog);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = requestLog.Id }, requestLog);
        }

        // DELETE: api/Request/5
        [ResponseType(typeof(RequestLog))]
        public async Task<IHttpActionResult> DeleteRequestLog(int id)
        {
            RequestLog requestLog = await db.RequestLog.FindAsync(id);
            if (requestLog == null)
            {
                return NotFound();
            }

            db.RequestLog.Remove(requestLog);
            await db.SaveChangesAsync();

            return Ok(requestLog);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RequestLogExists(int id)
        {
            return db.RequestLog.Count(e => e.Id == id) > 0;
        }
    }
}