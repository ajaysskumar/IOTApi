using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IoT.Common.Model.Models;

namespace IotDemoWebApp.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        public async Task<ActionResult> Index()
        {
            var admin = db.Admin.Include(a => a.Sensor);
            return View(await admin.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = await db.Admin.FindAsync(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            ViewBag.SensorId = new SelectList(db.WifiSensor, "Id", "DeviceName");
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Mobile,Email,ShouldRecieve,UpperTemperatureThreshold,LowerTemperatureThreshold,UpperHumidityThreshold,LowerHumidityThreshold,SensorId")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Admin.Add(admin);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.SensorId = new SelectList(db.WifiSensor, "Id", "DeviceName", admin.SensorId);
            return View(admin);
        }

        // GET: Admin/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = await db.Admin.FindAsync(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            ViewBag.SensorId = new SelectList(db.WifiSensor, "Id", "DeviceName", admin.SensorId);
            return View(admin);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Mobile,Email,ShouldRecieve,UpperTemperatureThreshold,LowerTemperatureThreshold,UpperHumidityThreshold,LowerHumidityThreshold,SensorId")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(admin).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.SensorId = new SelectList(db.WifiSensor, "Id", "DeviceName", admin.SensorId);
            return View(admin);
        }

        // GET: Admin/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = await db.Admin.FindAsync(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Admin admin = await db.Admin.FindAsync(id);
            db.Admin.Remove(admin);
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
