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

namespace IotDemoWebApp.Controllers
{
    public class MotionSensorModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MotionSensorModels
        public async Task<ActionResult> Index()
        {
            return View(await db.MotionsSensor.ToListAsync());
        }

        // GET: MotionSensorModels/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MotionSensorModel motionSensorModel = await db.MotionsSensor.FindAsync(id);
            if (motionSensorModel == null)
            {
                return HttpNotFound();
            }
            return View(motionSensorModel);
        }

        // GET: MotionSensorModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MotionSensorModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,MotionValue,MotionTime,Timestamp")] MotionSensorModel motionSensorModel)
        {
            if (ModelState.IsValid)
            {
                db.MotionsSensor.Add(motionSensorModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(motionSensorModel);
        }

        // GET: MotionSensorModels/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MotionSensorModel motionSensorModel = await db.MotionsSensor.FindAsync(id);
            if (motionSensorModel == null)
            {
                return HttpNotFound();
            }
            return View(motionSensorModel);
        }

        // POST: MotionSensorModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,MotionValue,MotionTime,Timestamp")] MotionSensorModel motionSensorModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(motionSensorModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(motionSensorModel);
        }

        // GET: MotionSensorModels/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MotionSensorModel motionSensorModel = await db.MotionsSensor.FindAsync(id);
            if (motionSensorModel == null)
            {
                return HttpNotFound();
            }
            return View(motionSensorModel);
        }

        // POST: MotionSensorModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            MotionSensorModel motionSensorModel = await db.MotionsSensor.FindAsync(id);
            db.MotionsSensor.Remove(motionSensorModel);
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
