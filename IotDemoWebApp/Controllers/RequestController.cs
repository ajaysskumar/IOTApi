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
using IoT.Common.Model.Models;

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
                RelayNumber = request.Relay.RelayNumber
            };
            //request.RelayGroup.Relays = null;

            return Ok(requestModel);
        }

        // GET: api/Request
        [Route("api/getrelaygrouprequest")]
        public IHttpActionResult GetRelayGroupRequest(string relayGroupMac)
        {
            RequestLog request = db.RequestLog.OrderBy(x => x.RequestStartTime).Where(x => x.Status == "Initiated" && x.RelayGroupMac==relayGroupMac).FirstOrDefault();

            if (request==null)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            string requestString = string.Format("{0}-{1}-{2}",request.Relay.RelayNumber,request.CurrentRelayStatus,request.Id);

            request.Status = RequestStatus.PickedUp;

            db.SaveChanges();

            return Ok(requestString);
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
        public async Task<IHttpActionResult> PutRequestLog([FromUri]int id,int currentStatus)
        {
            RequestLog requestLog = await db.RequestLog.FindAsync(id);

            requestLog.Status = RequestStatus.Completed;
            requestLog.CurrentRelayStatus = currentStatus;

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
        //[ResponseType(typeof(RequestLog))]
        public async Task<IHttpActionResult> PostRequestLog([FromUri]int relayId,int opCode, string msgId)
        {
            var relay = db.Relay.Where(x => x.Id == relayId).FirstOrDefault();

            RequestLog requestLog = new RequestLog {
                CurrentRelayStatus = opCode,
                MsgId = Guid.Parse(msgId),
                RelayId = relayId,
                RelayGroupMac = relay.RelayGroup.RelayGroupMac,
                RequestStartTime = DateTime.UtcNow,
                RequestEndTime = DateTime.UtcNow,
                Status = RequestStatus.Initiated
            };

            db.RequestLog.Add(requestLog);
            await db.SaveChangesAsync();

            return Ok(requestLog.Id);
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

        //private string returnResultString(IEnumerable<RequestLog> requests, int numberOfRelays)
        //{

        //}
    }
}