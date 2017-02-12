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
using IotDemoWebApp.Models;
using IotDemoWebApp.Utility;
using System.Configuration;

namespace IotDemoWebApp.Controllers
{
    public class WifiSettingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string iotHubConnectionString;

        public WifiSettingsController()
        {
            iotHubConnectionString = ConfigurationManager.AppSettings["iotHubConnectionString"];
        }

        // GET: WifiSettings
        public async Task<ActionResult> Index()
        {
            DevicesProcessor devicesProcessor = new DevicesProcessor(iotHubConnectionString, 1000, "");

            List<DeviceEntity> registeredDevices = await devicesProcessor.GetDevices();

            List<DeviceViewModel> devices = await db.WifiSensor.Select(x=>new DeviceViewModel() {
                Id = x.Id,
                DeviceName = x.DeviceName,
                IsActive = x.IsActive,
                OperationFrequecy = x.OperationFrequecy
            }).ToListAsync();

            registeredDevices.Where(x => devices.Select(y => y.Id).Contains(x.Id)).ToList();

            devices.Where(x => registeredDevices.Select(y => y.Id).Contains(x.Id)).ToList().ForEach(x=> {
                x.IsLinkedToIoTHub = true;
            });

            return View(devices);
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
        public async Task<ActionResult> Create([Bind(Include = "Id,DeviceName,OperationFrequecy,IsActive")] WifiSensor wifiSensor)
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
        public async Task<ActionResult> Edit([Bind(Include = "Id,DeviceName,OperationFrequecy,IsActive")] WifiSensor wifiSensor)
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
