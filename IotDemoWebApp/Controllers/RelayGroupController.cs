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
using IoT.Common.Model.Models;

namespace IotDemoWebApp.Controllers
{
    public class RelayGroupController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RelayGroup
        public async Task<ActionResult> Index()
        {
            return View(await db.RelayGroup.ToListAsync());
        }

        // GET: RelayGroup/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RelayGroup relayGroup = await db.RelayGroup.FindAsync(id);
            if (relayGroup == null)
            {
                return HttpNotFound();
            }
            return View(relayGroup);
        }

        // GET: RelayGroup/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RelayGroup/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,RelayGroupIpAddress,RelayGroupDescription,RelayGroupLocation,RelayGroupMac")] RelayGroup relayGroup)
        {
            if (ModelState.IsValid)
            {
                db.RelayGroup.Add(relayGroup);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(relayGroup);
        }

        // GET: RelayGroup/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RelayGroup relayGroup = await db.RelayGroup.FindAsync(id);
            if (relayGroup == null)
            {
                return HttpNotFound();
            }
            return View(relayGroup);
        }

        // POST: RelayGroup/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,RelayGroupIpAddress,RelayGroupDescription,RelayGroupLocation,RelayGroupMac")] RelayGroup relayGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(relayGroup).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(relayGroup);
        }

        // GET: RelayGroup/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RelayGroup relayGroup = await db.RelayGroup.FindAsync(id);
            if (relayGroup == null)
            {
                return HttpNotFound();
            }
            return View(relayGroup);
        }

        // POST: RelayGroup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RelayGroup relayGroup = await db.RelayGroup.FindAsync(id);
            db.RelayGroup.Remove(relayGroup);
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
