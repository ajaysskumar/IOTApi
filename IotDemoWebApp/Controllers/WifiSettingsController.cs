using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IotDemoWebApp.Models;
using IoT.Common.Model.Models;

namespace IotDemoWebApp.Controllers
{
    public class WifiSettingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WifiSettings
        public async Task<ActionResult> Index()
        {
            return View(await db.WifiSensor.ToListAsync());
        }

        // GET: WifiSettings/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WifiSensor wifiSensor = await db.WifiSensor.FindAsync(id);
            if (wifiSensor == null)
            {
                return HttpNotFound();
            }
            return View(wifiSensor);
        }

        // GET: WifiSettings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WifiSettings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,DeviceName,OperationFrequecy")] WifiSensor wifiSensor)
        {
            if (ModelState.IsValid)
            {
                db.WifiSensor.Add(wifiSensor);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(wifiSensor);
        }

        // GET: WifiSettings/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WifiSensor wifiSensor = await db.WifiSensor.FindAsync(id);
            if (wifiSensor == null)
            {
                return HttpNotFound();
            }
            return View(wifiSensor);
        }

        // POST: WifiSettings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,DeviceName,OperationFrequecy")] WifiSensor wifiSensor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wifiSensor).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(wifiSensor);
        }

        // GET: WifiSettings/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WifiSensor wifiSensor = await db.WifiSensor.FindAsync(id);
            if (wifiSensor == null)
            {
                return HttpNotFound();
            }
            return View(wifiSensor);
        }

        // POST: WifiSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            WifiSensor wifiSensor = await db.WifiSensor.FindAsync(id);
            db.WifiSensor.Remove(wifiSensor);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
