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

namespace IotDemoWebApp.Controllers
{
    public class WifiSensorsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/WifiSensors
        public IQueryable<WifiSensor> GetWifiSensors()
        {
            return db.WifiSensor;
        }

        // GET: api/WifiSensors/5
        [ResponseType(typeof(WifiSensor))]
        public async Task<IHttpActionResult> GetWifiSensor(string id)
        {
            WifiSensor wifiSensor = await db.WifiSensor.FindAsync(id);
            if (wifiSensor == null)
            {
                return NotFound();
            }

            return Ok(wifiSensor);
        }

        // PUT: api/WifiSensors/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutWifiSensor(string id, WifiSensor wifiSensor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != wifiSensor.Id)
            {
                return BadRequest();
            }

            db.Entry(wifiSensor).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WifiSensorExists(id))
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

        // POST: api/WifiSensors
        [ResponseType(typeof(WifiSensor))]
        public async Task<IHttpActionResult> PostWifiSensor([FromUri]WifiSensor wifiSensor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.WifiSensor.Add(wifiSensor);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (WifiSensorExists(wifiSensor.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = wifiSensor.Id }, wifiSensor);
        }

        // DELETE: api/WifiSensors/5
        [ResponseType(typeof(WifiSensor))]
        public async Task<IHttpActionResult> DeleteWifiSensor(string id)
        {
            WifiSensor wifiSensor = await db.WifiSensor.FindAsync(id);
            if (wifiSensor == null)
            {
                return NotFound();
            }

            db.WifiSensor.Remove(wifiSensor);
            await db.SaveChangesAsync();

            return Ok(wifiSensor);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WifiSensorExists(string id)
        {
            return db.WifiSensor.Count(e => e.Id == id) > 0;
        }
    }
}