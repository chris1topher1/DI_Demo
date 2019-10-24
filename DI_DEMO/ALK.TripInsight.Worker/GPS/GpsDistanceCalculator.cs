using GeoCoordinatePortable;

namespace ALK.TripInsight.Worker.GPS
{
    internal class GpsDistanceCalculator : IGpsDistanceCalculator
    {
        private const double METERS_PER_MILES = 1609.344;

        /// <summary>
        /// Get the mileage distance between two lat/lon points
        /// </summary>
        /// <param name="point1Lat">First point's lat</param>
        /// <param name="point1Lon">First point's lon</param>
        /// <param name="point2Lat">Second point's lat</param>
        /// <param name="point2Lon">Second point's lon</param>
        /// <returns>The miles between the points</returns>
        public double CalculateDistance(double point1Lat, double point1Lon, double point2Lat, double point2Lon)
        {
            var point1 = new GeoCoordinate(point1Lat, point1Lon);
            var point2 = new GeoCoordinate(point2Lat, point2Lon);

            var metersBetweenPoints = point1.GetDistanceTo(point2);
            return metersBetweenPoints / METERS_PER_MILES;
        }
    }
}
