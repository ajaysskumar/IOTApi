using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using IotDemoWebApp.Models;
using System.Web.Http.Cors;
using MongoDB.Bson;
using MongoDB.Driver;
using OnActuate.Iot.Core.Logging;
using System.Text.RegularExpressions;

namespace IotDemoWebApp.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MotionSensorController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        protected static IMongoClient _client;
        protected static IMongoDatabase _database;

        // GET: api/MotionSensor
        public IQueryable<MotionSensor> GetMotionsSensor()
        {
            return db.MotionsSensor;
        }

        [HttpGet]
        [Route("api/gettop")]
        public IEnumerable<MotionSensor> GetTop(int top,string sensorId)
        {
            int timeframe = top * 60 * 60;
            int numberOfPoints = top*60 / 12;

            DateTime currenteDate = DateTime.UtcNow.AddHours(-top);

            List<MotionSensor> data = new List<MotionSensor>();

            List<MotionSensor> dataPoints = new List<MotionSensor>();

            List<List<MotionSensor>> dataPointGroups = new List<List<MotionSensor>>();

            var queryData = db.MotionsSensor.Where(x => x.Timestamp>=currenteDate && x.DeviceId==sensorId).OrderByDescending(x => x.Timestamp);

            foreach (var item in queryData)
            {
                data.Add(new MotionSensor {
                    Id = item.Id,
                    MotionTime = item.MotionTime,
                    MotionValue = item.MotionValue,
                    Timestamp = item.Timestamp
                });
            }

            var totalDataPoints = data.Count();

            int groupStrength = totalDataPoints / numberOfPoints;

            dataPointGroups = splitList(data, numberOfPoints);

            foreach (var list in dataPointGroups)
            {
                int listCount = list.Count;
                MotionSensor model = new MotionSensor();
                model.MotionValue =(list.Sum(x => Convert.ToDecimal(x.MotionValue))/ listCount).ToString();
                model.MotionTime = (list.Sum(x => Convert.ToDecimal(x.MotionTime)) / listCount).ToString();
                double numOfElements = list.Count / 2;
                model.Timestamp = list[Convert.ToInt32(Math.Floor(numOfElements))].Timestamp;

                dataPoints.Add(model);
            }

            return dataPoints;
        }

        [HttpGet]
        [Route("api/gettopdatapoints")]
        public IEnumerable<MotionSensor> GetTopDatapoints(int top,string sensorId)
        {
            int timeframe = top * 60 * 60;
            int numberOfPoints = top * 60 / 12;

            DateTime currenteDate = DateTime.UtcNow.AddHours(-top);

            List<MotionSensor> data = new List<MotionSensor>();

            List<MotionSensor> dataPoints = new List<MotionSensor>();

            List<List<MotionSensor>> dataPointGroups = new List<List<MotionSensor>>();

            var queryData = db.MotionsSensor.Where(x => x.DeviceId==sensorId && x.Timestamp >= currenteDate).OrderByDescending(x => x.Timestamp);

            foreach (var item in queryData)
            {
                data.Add(new MotionSensor
                {
                    Id = item.Id,
                    MotionTime = item.MotionTime,
                    MotionValue = item.MotionValue,
                    Timestamp = item.Timestamp
                });
            }

            //var totalDataPoints = data.Count();

            //int groupStrength = totalDataPoints / numberOfPoints;

            //dataPointGroups = splitList(data, numberOfPoints);

            //foreach (var list in dataPointGroups)
            //{
            //    int listCount = list.Count;
            //    MotionSensorModel model = new MotionSensorModel();
            //    model.MotionValue = list.Sum(x => x.MotionValue) / listCount;
            //    model.MotionTime = list.Sum(x => x.MotionTime) / listCount;
            //    double numOfElements = list.Count / 2;
            //    model.Timestamp = list[Convert.ToInt32(Math.Floor(numOfElements))].Timestamp;

            //    dataPoints.Add(model);
            //}

            return data;
        }

        // GET: api/MotionSensor/5
        [ResponseType(typeof(MotionSensor))]
        public async Task<IHttpActionResult> GetMotionSensorModel(int id)
        {
            MotionSensor motionSensorModel = await db.MotionsSensor.FindAsync(id);
            if (motionSensorModel == null)
            {
                return NotFound();
            }

            return Ok(motionSensorModel);
        }

        // PUT: api/MotionSensor/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMotionSensorModel(int id, MotionSensor motionSensorModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != motionSensorModel.Id)
            {
                return BadRequest();
            }

            db.Entry(motionSensorModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MotionSensorModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/MotionSensor
        //[ResponseType(typeof(MotionSensor))]
        public async Task<IHttpActionResult> PostMotionSensorModel([FromUri]MotionSensor motionSensorModel)
        {
            decimal temperature = 0.00m;
            decimal humidity = 0.00m;

            if (!decimal.TryParse(motionSensorModel.MotionValue,out temperature) && !decimal.TryParse(motionSensorModel.MotionValue, out humidity))
            {
                string method = Helper.GetCurrentMethod();
                string objectString = Helper.ConvertObjectToXML(motionSensorModel);

                db.Trace.Add(new Trace {
                    Error = "invalid model",
                    Input = objectString,
                    MethodName = method,
                    Timestamp = DateTime.UtcNow.ToString()
                });
                await db.SaveChangesAsync();

                return Ok(db.WifiSensor.Where(x => x.Id == motionSensorModel.DeviceId).FirstOrDefault().OperationFrequecy);
            }

            //string colons : from mac id
            motionSensorModel.DeviceId = motionSensorModel.DeviceId.Replace(":", "");

            motionSensorModel.Timestamp = DateTime.UtcNow;
            try
            {
                //Send SMS
                var admins = db.Admin.Where(x => x.ShouldRecieve == true);

                foreach (var admin in admins)
                {
                    if (temperature>=admin.Threshold && (DateTime.Now-admin.LastSmsRecievedTime).Minutes>=15 )
                    {
                        Helper.SendSMS(ConfigurationManager.AppSettings.Get("AccountID"), ConfigurationManager.AppSettings.Get("SmsEmail"), ConfigurationManager.AppSettings.Get("Password"), admin.Mobile, string.Format("Hey there...Your threshold temperature {0} has reached. Please get adjustments in temperature. Current Temperature is{1} ",admin.Threshold,temperature));
                    }
                }

                db.MotionsSensor.Add(motionSensorModel);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //IotDemoEventSourceManager.Log.Info( Helper.ConvertObjectToXML(motionSensorModel), DateTime.UtcNow.ToString(), ex.Message, Helper.GetCurrentMethod());
                return Ok(db.WifiSensor.Where(x => x.Id == motionSensorModel.DeviceId).FirstOrDefault().OperationFrequecy);
            }

            return Ok(db.WifiSensor.Where(x=>x.Id == motionSensorModel.DeviceId).FirstOrDefault().OperationFrequecy);
        }

        // DELETE: api/MotionSensor/5
        [ResponseType(typeof(MotionSensor))]
        public async Task<IHttpActionResult> DeleteMotionSensorModel(int id)
        {
            MotionSensor motionSensorModel = await db.MotionsSensor.FindAsync(id);
            if (motionSensorModel == null)
            {
                return NotFound();
            }

            db.MotionsSensor.Remove(motionSensorModel);
            await db.SaveChangesAsync();

            return Ok(motionSensorModel);
        }

        // DELETE: api/MotionSensor
        //[ResponseType(typeof(MotionSensorModel))]
        [HttpPost]
        [Route("api/flushdb")]
        public async Task<IHttpActionResult> FlushDb(int startId,int stopId)
        {
            var dbSet = db.MotionsSensor.Where(x => x.Id >= startId && x.Id <= stopId);
            foreach (var item in dbSet)
            {
                db.MotionsSensor.Remove(item);
            }
            await db.SaveChangesAsync();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MotionSensorModelExists(int id)
        {
            return db.MotionsSensor.Count(e => e.Id == id) > 0;
        }

        [HttpPost]
        [Route("api/insertmongo")]
        public async Task<IHttpActionResult> Insert([FromUri]MotionSensor model)
        {
            string uri = "mongodb://ajaysskumar:123456@ds042729.mlab.com:42729/iot";

            _client = new MongoClient(uri);

            model.Timestamp = DateTime.UtcNow;

            _database = _client.GetDatabase("iot");

            var document  = model.ToBsonDocument<MotionSensor>();

            var collection = _database.GetCollection<BsonDocument>("datapoints");
            
            await collection.InsertOneAsync(document);

            return Ok(document);
        }

        public static List<List<MotionSensor>> splitList(List<MotionSensor> locations, int nSize = 5)
        {
            var list = new List<List<MotionSensor>>();

            for (int i = 0; i < locations.Count; i += nSize)
            {
                list.Add(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            }

            return list;
        }
    }
}