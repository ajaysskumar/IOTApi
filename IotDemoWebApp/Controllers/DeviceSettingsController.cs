using IoT.Common.Model.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace IotDemoWebApp.Controllers
{
    //[RoutePrefix("api")]
    public class DeviceSettingsController : ApiController
    {
        private string iotHubConnectionString;
        private RegistryManager registryManager;
        private string iotHubHostName;

        public DeviceSettingsController()
        {
            this.iotHubConnectionString = ConfigurationManager.AppSettings["iotHubConnectionString"];
            this.registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);

            this.iotHubHostName = ConfigurationManager.AppSettings["iotHubHostName"];
        }

        [HttpGet]
        //[Route("device")]
        public async Task<IHttpActionResult> Get([FromUri]string deviceId)
        {

                Device device = await registryManager.GetDeviceAsync(deviceId);

                string[] connectionString = CreateDeviceConnectionString(device).Split(';');

                string sasKey = connectionString[1].TrimStart("SharedAccessSignature=".ToCharArray());
                string hostName = connectionString[0].Split('=')[1];

                return Ok(new {
                    sasKey = sasKey,
                    hostName = hostName
                });
        }

        [HttpGet]
        [Route("api/getdeviceusername")]
        public async Task<IHttpActionResult> GetDeviceUsername([FromUri] string deviceId) {
            try
            {
                return Ok(string.Format("{0}/{1}",iotHubHostName,deviceId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Route("api/gethubservername")]
        public async Task<IHttpActionResult> GetHubServer([FromUri] string deviceId)
        {
            try
            {
                return Ok(iotHubHostName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Route("api/getsaskey")]
        public async Task<IHttpActionResult> GetDeviceSASKey([FromUri] string deviceId, decimal ttlValue)
        {
            try
            {
                var selectedDevice = await registryManager.GetDeviceAsync(deviceId);

                if (selectedDevice==null)
                {
                    throw new Exception("Device not found in the system please register device");
                }

                var sasBuilder = new SharedAccessSignatureBuilder()
                {
                    Key =  selectedDevice.Authentication.SymmetricKey.SecondaryKey,
                    Target = String.Format("{0}/devices/{1}", iotHubHostName, WebUtility.UrlEncode(deviceId)),
                    TimeToLive = TimeSpan.FromDays(Convert.ToDouble(ttlValue))
                };

                string iotHubPassword = sasBuilder.ToSignature();
                return Ok(iotHubPassword);


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Route("api/getdatapointfrequency")]
        public async Task<IHttpActionResult> GetDatapointFrequency([FromUri] string deviceId)
        {
            try
            {
                int operationFrequency = 30;
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    operationFrequency  = context.WifiSensor.Where(x => x.Id.ToLower() == deviceId.ToLower()).FirstOrDefault().OperationFrequecy;
                }
                return Ok(operationFrequency*1000);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        #region private helper methods
        private String CreateDeviceConnectionString(Device device)
        {
            StringBuilder deviceConnectionString = new StringBuilder();

            var hostName = String.Empty;
            var tokenArray = iotHubConnectionString.Split(';');
            for (int i = 0; i < tokenArray.Length; i++)
            {
                var keyValueArray = tokenArray[i].Split('=');
                if (keyValueArray[0] == "HostName")
                {
                    hostName = tokenArray[i] + ';';
                    break;
                }
            }

            if (!String.IsNullOrWhiteSpace(hostName))
            {
                deviceConnectionString.Append(hostName);
                deviceConnectionString.AppendFormat("DeviceId={0}", device.Id);

                if (device.Authentication != null)
                {
                    if ((device.Authentication.SymmetricKey != null) && (device.Authentication.SymmetricKey.PrimaryKey != null))
                    {
                        deviceConnectionString.AppendFormat(";SharedAccessKey={0}", device.Authentication.SymmetricKey.PrimaryKey);
                    }
                    else
                    {
                        deviceConnectionString.AppendFormat(";x509=true");
                    }
                }

                //if (this.protocolGatewayHostName.Length > 0)
                //{
                //    deviceConnectionString.AppendFormat(";GatewayHostName=ssl://{0}:8883", this.protocolGatewayHostName);
                //}
            }

            return deviceConnectionString.ToString();
        }

        private string deviceConnectionStringWithSAS(string sas,string deviceId)
        {
            // Format of Device Connection String with SAS Token:
            // "HostName=<iothub_host_name>;CredentialType=SharedAccessSignature;DeviceId=<device_id>;SharedAccessSignature=SharedAccessSignature sr=<iot_host>/devices/<device_id>&sig=<token>&se=<expiry_time>";
            return String.Format("HostName={0};DeviceId={1};SharedAccessSignature={2}", iotHubHostName, deviceId, sas);
        }
        #endregion
    }
}
