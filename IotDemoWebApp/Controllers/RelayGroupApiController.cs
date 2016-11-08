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
using IoT.Common.Model.Models;

namespace IotDemoWebApp.Controllers
{
    public class RelayGroupApiController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/RelayGroup
        public IQueryable<RelayGroupView> GetRelayGroup()
        {
            return db.RelayGroup.ProjectToQueryable<RelayGroupView>();
        }

        // GET: api/RelayGroup/5
        [ResponseType(typeof(RelayGroup))]
        public async Task<IHttpActionResult> GetRelayGroup(int id)
        {
            RelayGroup relayGroup = await db.RelayGroup.FindAsync(id);
            if (relayGroup == null)
            {
                return NotFound();
            }

            return Ok(relayGroup);
        }

        // PUT: api/RelayGroup/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRelayGroup(int id, RelayGroup relayGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != relayGroup.Id)
            {
                return BadRequest();
            }

            db.Entry(relayGroup).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RelayGroupExists(id))
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

        // POST: api/RelayGroup
        [ResponseType(typeof(RelayGroup))]
        public async Task<IHttpActionResult> PostRelayGroup(RelayGroup relayGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RelayGroup.Add(relayGroup);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = relayGroup.Id }, relayGroup);
        }

        // DELETE: api/RelayGroup/5
        [ResponseType(typeof(RelayGroup))]
        public async Task<IHttpActionResult> DeleteRelayGroup(int id)
        {
            RelayGroup relayGroup = await db.RelayGroup.FindAsync(id);
            if (relayGroup == null)
            {
                return NotFound();
            }

            db.RelayGroup.Remove(relayGroup);
            await db.SaveChangesAsync();

            return Ok(relayGroup);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RelayGroupExists(int id)
        {
            return db.RelayGroup.Count(e => e.Id == id) > 0;
        }
    }
}