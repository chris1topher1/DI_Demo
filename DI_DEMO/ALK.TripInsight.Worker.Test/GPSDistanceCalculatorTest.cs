using System;
using ALK.TripInsight.Worker.GPS;
using ALK.TripInsight.Model;
using NUnit.Framework;

namespace ALK.TripInsight.Worker.Test
{
    [TestFixture]
    public class GpsDistanceCalculatorTest
    {
        private IGpsDistanceCalculator _distanceCalculator;

        [SetUp]
        public void Setup()
        {
            _distanceCalculator = new GpsDistanceCalculator();
        }

        [Test]
        public void Test_Known_Distance_Short()
        {
            var dist = _distanceCalculator.CalculateDistance(40.3815041, -74.6517602, 40.1779972, -74.6576088);
            Assert.IsTrue(Math.Round(dist) == 14);
        }

        [Test]
        public void Test_Known_Distance_Long()
        {
            var dist = _distanceCalculator.CalculateDistance(33.9972885, -118.7977026, 40.1779972, -74.6576088);
            Assert.IsTrue(Math.Round(dist) == 2447);
        }

        [Test]
        public void Test_Known_Distance_String_Conversion()
        {
            var gps1 = new GPSCoordinate()
            {
                Latitude = "33.9972885",
                Longitude = "-118.7977026"
            };

            var gps2 = new GPSCoordinate()
            {
                Latitude = "40.1779972",
                Longitude = "-74.6576088"
            };

            var dist = _distanceCalculator.CalculateDistance(Convert.ToDouble(gps1.Latitude),
                Convert.ToDouble(gps1.Longitude), Convert.ToDouble(gps2.Latitude), Convert.ToDouble(gps2.Longitude));

            Assert.IsTrue(Math.Round(dist) == 2447);
        }
    }
}
