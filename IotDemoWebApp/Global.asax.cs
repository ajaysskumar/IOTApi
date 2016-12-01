using IoT.Common.SemanticLogging;
using IoT.Common.Model.Models;
using IoT.Common.Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using IoT.Common.Logging;

namespace IotDemoWebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //Initialize logging
            LoggingManager.InitializeLogger("IoTEventSourceManager", System.Diagnostics.Tracing.EventLevel.LogAlways);
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Relay, RelayView>().ReverseMap();
                cfg.CreateMap<RelayGroup, RelayGroupView>().ReverseMap();
            });

            IoTEventSourceManager.Log.Info(Guid.NewGuid().ToString(),"Application Started");
        }
    }
}
