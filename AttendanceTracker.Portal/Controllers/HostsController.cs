using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AttendanceTracker.Portal.Models;
using System.Data.Entity.SqlServer;

namespace AttendanceTracker.Portal.Controllers
{
    //[Authorize]
    public class HostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Hosts
        public ActionResult Index()
        {
            var data = db.Host.Select(x => new HostViewModel {
                EmployeeName = x.EmployeeName,

                HostName =x.HostName,

                Id =x.Id.ToUpper(),

                LastAccessedIP = x.HostStatus
                .OrderByDescending(y => y.LastStatusChecked)
                .FirstOrDefault()==null?"":x.HostStatus
                .OrderByDescending(y => y.LastStatusChecked)
                .FirstOrDefault().LastAccessedIP,

                Status = x.HostStatus
                .OrderByDescending(y=>y.LastStatusChecked)
                .FirstOrDefault() == null ? 0 : x.HostStatus
                .OrderByDescending(y => y.LastStatusChecked)
                .FirstOrDefault().Status,

                LastStatusChecked = x.HostStatus.OrderByDescending(y => y.LastStatusChecked).FirstOrDefault() == null ? DateTime.MinValue : SqlFunctions.DateAdd("second", 19800, x.HostStatus.OrderByDescending(y => y.LastStatusChecked).FirstOrDefault().LastStatusChecked).Value,

                TotalConnectedHoursToday = x.HostStatus.Where(y => y.LastStatusChecked.Day == DateTime.UtcNow.Day && y.LastStatusChecked.Month == DateTime.UtcNow.Month && y.LastStatusChecked.Year == DateTime.UtcNow.Year)
                .OrderByDescending(y => y.LastStatusChecked)
                .FirstOrDefault()==null?0: x.HostStatus.Where(y => y.LastStatusChecked.Day == DateTime.UtcNow.Day && y.LastStatusChecked.Month == DateTime.UtcNow.Month && y.LastStatusChecked.Year == DateTime.UtcNow.Year)
                .OrderByDescending(y => y.LastStatusChecked)
                .FirstOrDefault().LastStatusChecked.Hour- x.HostStatus
                .Where(y => y.LastStatusChecked.Day == DateTime.UtcNow.Day && y.LastStatusChecked.Month == DateTime.UtcNow.Month && y.LastStatusChecked.Year == DateTime.UtcNow.Year).OrderBy(y => y.LastStatusChecked)
                .FirstOrDefault().LastStatusChecked.Hour,

                CurrentDayLoginTime = x.HostStatus
                .Where(y => y.LastStatusChecked.Day == DateTime.UtcNow.Day && y.LastStatusChecked.Month == DateTime.UtcNow.Month && y.LastStatusChecked.Year == DateTime.UtcNow.Year)
                .OrderByDescending(y => y.LastStatusChecked)
                .FirstOrDefault() == null ? "NA" : SqlFunctions.DateAdd("second", 19800, x.HostStatus.Where(y => y.LastStatusChecked.Day == DateTime.UtcNow.Day && y.LastStatusChecked.Month == DateTime.UtcNow.Month && y.LastStatusChecked.Year == DateTime.UtcNow.Year)
                .OrderBy(y => y.LastStatusChecked)
                .FirstOrDefault().LastStatusChecked).Value.ToString()
            }).OrderByDescending(z=>z.TotalConnectedHoursToday).ToList();
            return View(data);
        }

        // GET: Hosts/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Host host = db.Host.Find(id);
            if (host == null)
            {
                return HttpNotFound();
            }
            return View(host);
        }

        // GET: Hosts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Hosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HostName,EmployeeName")] Host host)
        {
            if (ModelState.IsValid)
            {
                db.Host.Add(host);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(host);
        }

        // GET: Hosts/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Host host = db.Host.Find(id);
            if (host == null)
            {
                return HttpNotFound();
            }
            return View(host);
        }

        // POST: Hosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HostName,EmployeeName")] Host host)
        {
            if (ModelState.IsValid)
            {
                db.Entry(host).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(host);
        }

        // GET: Hosts/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Host host = db.Host.Find(id);
            if (host == null)
            {
                return HttpNotFound();
            }
            return View(host);
        }

        // POST: Hosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Host host = db.Host.Find(id);
            db.Host.Remove(host);
            db.SaveChanges();
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
