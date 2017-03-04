using IoT.Common.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IotDemoWebApp.Utility
{
    public class ChartHelper
    {

        public IEnumerable<MotionSensor> GetChartData(int top, int lastRecord, string sensorId, ApplicationDbContext context)
        {
            int timeframe = top * 60 * 60;
            int numberOfPoints = top * 60 / 12;

            DateTime currenteDate = DateTime.UtcNow.AddHours(-top);

            MotionSensor[] queryData = new MotionSensor[200];

            List<MotionSensor> data = new List<MotionSensor>();

            List<List<MotionSensor>> dataPointGroups = new List<List<MotionSensor>>();


            queryData = context.MotionsSensor.Where(x => x.DeviceId == sensorId && x.Timestamp >= currenteDate && x.Id > lastRecord).OrderBy(x => x.Timestamp).ToArray();



            int recordCount = queryData.Count();

            var counter = 20;

            while (counter >= 20 && recordCount > counter)
            {
                var item = queryData[counter];
                data.Add(new MotionSensor()
                {
                    Id = item.Id,
                    MotionTime = item.MotionTime,
                    MotionValue = item.MotionValue,
                    Timestamp = item.Timestamp.ToUniversalTime()
                });

                if (recordCount - counter >= 20)
                {
                    counter += 20;
                }
                else
                {
                    data.Add(new MotionSensor()
                    {
                        Id = queryData[recordCount - 1].Id,
                        MotionTime = queryData[recordCount - 1].MotionTime,
                        MotionValue = queryData[recordCount - 1].MotionValue,
                        Timestamp = queryData[recordCount - 1].Timestamp
                    });

                    break;
                }
            }

            if (recordCount < 24)
            {
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
            }
            return data;
        }
    }
}