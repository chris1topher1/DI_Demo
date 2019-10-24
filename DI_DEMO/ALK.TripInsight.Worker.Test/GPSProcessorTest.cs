using ALK.TripInsight.Worker.RabbitMQ;
using ALK.TripInsight.Worker.Status;
using ALK.TripInsight.Worker.Trip.StopInfo;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace ALK.TripInsight.Worker.Test
{
    [TestFixture]
    public class GPSrocessorTest
    {   /*
        private Mock<ITripStopInfoProvider> _tripStopInfoProvider;
        private Mock<IRabbitMqBroker> _rabbitMqBroker;
        private Mock<IStopStatusRepository> _stopStatusRepo;
        private Mock<IDistanceCalculator> _distanceCalculator;
        private Mock<ILogger<GPSProcessor>> _logger;
        private GPSProcessor _gpsProcessor;
        
        [SetUp]
        public void Setup()
        {
            _tripStopInfoProvider = new Mock<ITripStopInfoProvider>();
            _tripStopInfoProvider.Setup(r => r.GetNextOpenOrArrivedStop(It.IsAny<long>(), It.IsAny<int>())).Returns((long tripId, int ownerId) =>
            {
                return MockData.GetTripStopInfo(false);
            });

            _rabbitMqBroker = new Mock<IRabbitMqBroker>();
            _stopStatusRepo = new Mock<IStopStatusRepository>();
            _distanceCalculator = new Mock<IDistanceCalculator>();
            _logger = new Mock<ILogger<GPSProcessor>>();

            _gpsProcessor = new GPSProcessor(_rabbitMqBroker.Object, _tripStopInfoProvider.Object, _stopStatusRepo.Object, _distanceCalculator.Object, _logger.Object);
        }
        
        [TestCase(null, null)]
        [TestCase("", null)]
        public void Test_Bad_Inputs(string messageBody, IDictionary<string, object> arguments)
        {
            Assert.DoesNotThrow(() => _gpsProcessor.ProcessMessage(string.Empty, null));
        }

        [TestCase]
        public void Test_With_Real_GPS_Message()
        {
            string message = MockData.GetMockGPSMessage();
            var arguments = MockData.GetGPSArguments();
            Assert.DoesNotThrow(() => _gpsProcessor.ProcessMessage(message, arguments));
        }*/
    }
}
