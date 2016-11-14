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
    public class SysConfigController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SysConfig
        public async Task<ActionResult> Index()
        {
            return View(await db.SystemConfiguration.ToListAsync());
        }

        // GET: SysConfig/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemConfiguration systemConfiguration = await db.SystemConfiguration.FindAsync(id);
            if (systemConfiguration == null)
            {
                return HttpNotFound();
            }
            return View(systemConfiguration);
        }

        // GET: SysConfig/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SysConfig/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Key,Value")] SystemConfiguration systemConfiguration)
        {
            if (ModelState.IsValid)
            {
                db.SystemConfiguration.Add(systemConfiguration);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(systemConfiguration);
        }

        // GET: SysConfig/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemConfiguration systemConfiguration = await db.SystemConfiguration.FindAsync(id);
            if (systemConfiguration == null)
            {
                return HttpNotFound();
            }
            return View(systemConfiguration);
        }

        // POST: SysConfig/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Key,Value")] SystemConfiguration systemConfiguration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemConfiguration).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(systemConfiguration);
        }

        // GET: SysConfig/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemConfiguration systemConfiguration = await db.SystemConfiguration.FindAsync(id);
            if (systemConfiguration == null)
            {
                return HttpNotFound();
            }
            return View(systemConfiguration);
        }

        // POST: SysConfig/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SystemConfiguration systemConfiguration = await db.SystemConfiguration.FindAsync(id);
            db.SystemConfiguration.Remove(systemConfiguration);
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
