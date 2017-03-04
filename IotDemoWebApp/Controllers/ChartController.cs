using IoT.Common.Model.Models;
using IotDemoWebApp.Models;
using IotDemoWebApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IotDemoWebApp.Controllers
{
    public class ChartController : Controller
    {
        // GET: Chart
        public ActionResult Index()
        {
            return View();
        }

        [Route("chart2")]
        public ActionResult Chart() 
        {
           
            return View();
        }

        [Route("gchart")]
        public ActionResult GoogleChart()
        {
            return View("GChart");
        }

        [Route("chartcon")]
        public ActionResult ChartCon()
        {
            return View();
        }

        [Route("vischart")]
        public ActionResult VisChart()
        {
            return View("VisChart");
        }

        [Route("chart/v1")]
        public ActionResult ChartV1(string sensorId = "", int timeFrame = 1)
        {

            ChartHelper chartHelper = new ChartHelper();

            ChartModel model = new ChartModel();

            model.TimeFrame = timeFrame;

            model.StartTimePeriod = DateTime.Now.AddHours(-1);
            model.EndTimePeriod = DateTime.Now;

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                model.Sensors = context.WifiSensor.Where(x => x.IsActive).ToList();
                if (model.Sensors != null && model.Sensors.Count > 0)
                {
                    if (string.IsNullOrEmpty(sensorId))
                    {
                        model.SelectedDeviceId = model.Sensors.FirstOrDefault().Id;
                    }
                    else
                    {
                        model.SelectedDeviceId = sensorId;
                    }
                }

            }

            return View(model);
        }

        [Route("chart/v2")]
        public ActionResult ChartV2(string sensorId = "", int timeFrame = 1)
        {
            
            ChartHelper chartHelper = new ChartHelper();

            ChartModel model = new ChartModel();

            model.TimeFrame = timeFrame;

            model.StartTimePeriod = DateTime.Now.AddHours(-1);
            model.EndTimePeriod = DateTime.Now;

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                model.Sensors = context.WifiSensor.Where(x => x.IsActive).ToList();
                if (model.Sensors != null && model.Sensors.Count > 0)
                {
                    if (string.IsNullOrEmpty(sensorId))
                    {
                        model.SelectedDeviceId = model.Sensors.FirstOrDefault().Id;
                    }
                    else
                    {
                        model.SelectedDeviceId = sensorId;
                    }
                }

            }

            return View(model);
        }

        [Route("chart/v3")]
        public ActionResult ChartV3(string sensorId = "", int timeFrame = 1)
        {

            ChartHelper chartHelper = new ChartHelper();

            ChartModel model = new ChartModel();

            model.TimeFrame = timeFrame;

            model.StartTimePeriod = DateTime.Now.AddHours(-1);
            model.EndTimePeriod = DateTime.Now;

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                model.Sensors = context.WifiSensor.Where(x => x.IsActive).ToList();
                if (model.Sensors != null && model.Sensors.Count > 0)
                {
                    if (string.IsNullOrEmpty(sensorId))
                    {
                        model.SelectedDeviceId = model.Sensors.FirstOrDefault().Id;
                    }
                    else
                    {
                        model.SelectedDeviceId = sensorId;
                    }
                }

            }

            return View(model);
        }

        [Route("chart/v4")]
        public ActionResult ChartV4(string sensorId = "", int timeFrame = 1)
        {

            ChartHelper chartHelper = new ChartHelper();

            ChartModel model = new ChartModel();

            model.TimeFrame = timeFrame;

            model.StartTimePeriod = DateTime.Now.AddHours(-1);
            model.EndTimePeriod = DateTime.Now;

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                model.Sensors = context.WifiSensor.Where(x => x.IsActive).ToList();
                if (model.Sensors != null && model.Sensors.Count > 0)
                {
                    if (string.IsNullOrEmpty(sensorId))
                    {
                        model.SelectedDeviceId = model.Sensors.FirstOrDefault().Id;
                    }
                    else
                    {
                        model.SelectedDeviceId = sensorId;
                    }
                }

            }

            return View(model);
        }
    }
}