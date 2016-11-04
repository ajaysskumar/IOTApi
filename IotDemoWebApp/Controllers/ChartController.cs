﻿using System;
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
    }
}