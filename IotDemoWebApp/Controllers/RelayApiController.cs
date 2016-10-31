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
using IoT.Common.Model.Utility;
using IotDemoWebApp.Models;
using AutoMapper;

namespace IotDemoWebApp.Controllers
{
    public class RelayApiController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Relay
        public IEnumerable<RelayView> GetRelay()
        {
            return db.Relay.ProjectToQueryable<RelayView>();
        }

        // GET: api/Relay/5
        [ResponseType(typeof(Relay))]
        public async Task<IEnumerable<RelayView>> GetRelay(int id)
        {
            var relayGroup = await db.RelayGroup.FindAsync(id);
            if (relayGroup == null)
            {
                return new List<RelayView>();
            }

            return relayGroup.Relays.Select(x=>new RelayView {
                Id = x.Id,
                RelayDescription =x.RelayDescription,
                RelayGroupId = x.RelayGroupId,
                RelayNumber = x.RelayNumber,
                RelayState = x.RelayState
            });
        }

        // PUT: api/Relay/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRelay(int id, Relay relay)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != relay.Id)
            {
                return BadRequest();
            }

            db.Entry(relay).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RelayExists(id))
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

        // POST: api/Relay
        [ResponseType(typeof(Relay))]
        public async Task<IHttpActionResult> PostRelay(Relay relay)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Relay.Add(relay);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = relay.Id }, relay);
        }

        // DELETE: api/Relay/5
        [ResponseType(typeof(Relay))]
        public async Task<IHttpActionResult> DeleteRelay(int id)
        {
            Relay relay = await db.Relay.FindAsync(id);
            if (relay == null)
            {
                return NotFound();
            }

            db.Relay.Remove(relay);
            await db.SaveChangesAsync();

            return Ok(relay);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RelayExists(int id)
        {
            return db.Relay.Count(e => e.Id == id) > 0;
        }
    }
}