using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using ALK.TripInsight.Model;
using ALK.TripInsight.Worker.Status;
using ALK.TripInsight.Worker.Trip.StopInfo;
using GeoCoordinatePortable;
using Microsoft.Extensions.Logging;

namespace ALK.TripInsight.Worker.RabbitMQ
{
    internal class GPSProcessor
    {
        private const string RABBIT_MQ_URL = "amqp://username:password@my-rabbit-mq:5672";
        private const string RABBIT_MQ_QUEUE_NAME = "MyGPSQueue";
        private const string SQL_DB_CONNECTION_STRING = "MySqlDB";
        private const double DISTANCE_THRESHOLD_MILES = 0.5;

        private readonly RabbitMqBroker _rabbitMqBroker;
        private readonly TripStopInfoProvider _stopProvider;
        private readonly StopStatusRepository _stopStatusWriter;
        private readonly ILogger _logger;

        public GPSProcessor()
        {
            _rabbitMqBroker = new RabbitMqBroker();
            _rabbitMqBroker.Init(RABBIT_MQ_URL, RABBIT_MQ_QUEUE_NAME);

            _stopProvider = new TripStopInfoProvider(SQL_DB_CONNECTION_STRING);
            _stopStatusWriter = new StopStatusRepository(SQL_DB_CONNECTION_STRING);

            LoggerFactory loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger("StopStatus");
        }

        public void ReadQueue()
        {
            _rabbitMqBroker.Subscribe(_processMessage);
        }

        private bool _processMessage(string messageBody, IDictionary<string, object> arguments)
        {
            if (arguments.TryGetValue("ownerId", out object owner) && owner != null)
            {
                var gpsMessage = _deserialize<GpsMessage>(messageBody);
                if (gpsMessage?.Positions?.Count > 0 && gpsMessage.TripId.HasValue)
                {
                    // For the purpose of this demo, just use the first position.
                    var position = gpsMessage.Positions.First();

                    var stop = _stopProvider.GetNextOpenOrArrivedStop(gpsMessage.TripId.Value, Convert.ToInt32(owner));

                    var stopCoordinates = new GeoCoordinate(Convert.ToDouble(stop.Location.Coords.Latitude),
                        Convert.ToDouble(stop.Location.Coords.Latitude));

                    var gpsCoordinates = new GeoCoordinate(Convert.ToDouble(position.GPSCoordinate.Latitude),
                        Convert.ToDouble(position.GPSCoordinate.Longitude));

                    if (stopCoordinates.GetDistanceTo(gpsCoordinates) > DISTANCE_THRESHOLD_MILES)
                    {
                        bool updateSuccess = _stopStatusWriter.UpdateStopStatus(gpsMessage.TripId.Value,
                            stop.StopSequence, StopStatus.Arrived);
                        _logger.LogInformation($"Result of writing stop status was : {updateSuccess.ToString()}");
                    }
                }
            }
            else
            {
                _logger.LogError("Could not get ownerId");
            }
            
            return true;
        }

        private T _deserialize<T>(string message)
        {
            using (var memoryStream = new MemoryStream())
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                memoryStream.Write(bytes, 0, bytes.Length);
                memoryStream.Position = 0;

                var deserializer = new DataContractJsonSerializer(typeof(T));
                return (T)deserializer.ReadObject(memoryStream);
            }
        }
    }
}
