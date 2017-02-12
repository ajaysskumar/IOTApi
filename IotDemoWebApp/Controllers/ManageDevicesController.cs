using IoT.Common.Model.Models;
using IotDemoWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure.Devices;
using System.Configuration;
using System.Threading;
using IotDemoWebApp.Utility;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Common;

namespace IotDemoWebApp.Controllers
{
    public class ManageDevicesController : Controller
    {
        // GET: ManageDevices
        private string iotHubConnectionString;
        private int devicesMaxCount;
        private RegistryManager registryManager;

        #region fields
        private const int MAX_TTL_VALUE = 365;
        private const int MAX_COUNT_OF_DEVICES = 1000;

        private bool devicesListed = false;
        private static int eventHubPartitionsCount;
        private static string activeIoTHubConnectionString;
        private string iotHubHostName;

        private static string cloudToDeviceMessage;

        private static int deviceSelectedIndexForEvent = 0;
        private static int deviceSelectedIndexForC2DMessage = 0;
        private static int deviceSelectedIndexForDeviceMethod = 0;

        private static CancellationTokenSource ctsForDataMonitoring;
        private static CancellationTokenSource ctsForFeedbackMonitoring;
        private static CancellationTokenSource ctsForDeviceMethod;

        private const string DEFAULT_CONSUMER_GROUP = "$Default";
        #endregion

        public ManageDevicesController()
        {
            iotHubConnectionString = ConfigurationManager.AppSettings["iotHubConnectionString"];
            this.registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);
        }
        public async Task<ActionResult> Index()
        {
            DevicesProcessor devicesProcessor = new DevicesProcessor(iotHubConnectionString, 1000, "");

            List<DeviceEntity> devices = await devicesProcessor.GetDevices();

            return View(devices);
        }

        [HttpGet]
        public ActionResult Create() {

            DeviceEntityViewModel entity = new DeviceEntityViewModel()
            {
                PrimaryKey = CryptoKeyGenerator.GenerateKey(32),
                SecondaryKey = CryptoKeyGenerator.GenerateKey(32),
            };
            return View(entity);
        }

        [HttpPost]
        public ActionResult Create(DeviceEntityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("","invalid model state");
            }

            DeviceEntity deviceModel = new DeviceEntity()
            {
                Id = model.DeviceId,
                PrimaryKey = model.PrimaryKey,
                SecondaryKey = model.SecondaryKey
            };

            Device device = new Device(model.DeviceId);

            device.Authentication = new AuthenticationMechanism();

            device.Authentication.SymmetricKey.PrimaryKey = model.PrimaryKey;
            device.Authentication.SymmetricKey.SecondaryKey = model.SecondaryKey;

            registryManager.AddDeviceAsync(device);

            return RedirectToAction("Index");
        }

        public ActionResult AutoGenerateKeys(bool selected) {
            return null;
        }
    }
}