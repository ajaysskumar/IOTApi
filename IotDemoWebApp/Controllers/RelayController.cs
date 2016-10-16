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
using IoT.Common.Model.Utility;

namespace IotDemoWebApp.Controllers
{
    public class RelayController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Relay
        public async Task<ActionResult> Index()
        {
            var relay = db.Relay.Include(r => r.RelayGroup);
            return View(await relay.ToListAsync());
        }

        // GET: Relay/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Relay relay = await db.Relay.FindAsync(id);
            if (relay == null)
            {
                return HttpNotFound();
            }
            return View(relay);
        }

        // GET: Relay/Create
        public ActionResult Create()
        {
            ViewBag.RelayGroupId = new SelectList(db.RelayGroup, "Id", "RelayGroupIpAddress");
            return View();
        }

        // POST: Relay/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,RelayName,RelayDescription,RelayState,RelayGroupId")] Relay relay)
        {
            if (ModelState.IsValid)
            {
                db.Relay.Add(relay);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.RelayGroupId = new SelectList(db.RelayGroup, "Id", "RelayGroupIpAddress", relay.RelayGroupId);
            return View(relay);
        }

        // GET: Relay/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Relay relay = await db.Relay.FindAsync(id);
            if (relay == null)
            {
                return HttpNotFound();
            }
            ViewBag.RelayGroupId = new SelectList(db.RelayGroup, "Id", "RelayGroupIpAddress", relay.RelayGroupId);
            return View(relay);
        }

        // POST: Relay/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,RelayName,RelayDescription,RelayState,RelayGroupId")] Relay relay)
        {
            if (ModelState.IsValid)
            {
                db.Entry(relay).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.RelayGroupId = new SelectList(db.RelayGroup, "Id", "RelayGroupIpAddress", relay.RelayGroupId);
            return View(relay);
        }

        // GET: Relay/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Relay relay = await db.Relay.FindAsync(id);
            if (relay == null)
            {
                return HttpNotFound();
            }
            return View(relay);
        }

        // POST: Relay/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Relay relay = await db.Relay.FindAsync(id);
            db.Relay.Remove(relay);
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
