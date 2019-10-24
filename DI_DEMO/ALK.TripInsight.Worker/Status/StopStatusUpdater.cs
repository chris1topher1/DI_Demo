using System;
using System.Globalization;
using ALK.TripInsight.Worker.Date;
using ALK.TripInsight.Model;
using ALK.TripInsight.Worker.Trip.StopInfo;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace ALK.TripInsight.Worker.Status
{
    internal class StopStatusUpdater : IStopStatusUpdater
    {
        private const double MAX_AIR_DISTANCE_FROM_STOP = 0.5;

        private readonly ITripStopInfoProvider _tripStopInfoProvider;
        private readonly IStopStatusRepository _stopStatusRepository;
        private readonly IDateTimeParser _dateTimeParser;
        private readonly IArrivalStatusUpdater _arrivalStatusUpdater;
        private readonly ILogger<StopStatusUpdater> _logger;

        /// <summary>
        /// Initializes the <see cref="StopStatusProcessor"/> class.
        /// </summary>
        public StopStatusUpdater(ILogger<StopStatusUpdater> logger, 
                                 ITripStopInfoProvider tripStopInfoProvider, 
                                 IStopStatusRepository stopStatusRepository,
                                 IDateTimeParser dateTimeParser,
                                 IArrivalStatusUpdater arrivalStatusUpdater)
        {
            _tripStopInfoProvider = tripStopInfoProvider;
            _logger = logger;
            _stopStatusRepository = stopStatusRepository;
            _dateTimeParser = dateTimeParser;
            _arrivalStatusUpdater = arrivalStatusUpdater;
        }

        /// <summary>
        /// Update the stop status (if needed) based on position information
        /// </summary>
        /// <param name="nextStop">The next Open or Arrived stop in the trip</param>
        /// <param name="positionsWithDistances">The GPS Position with the distance to the next stop</param>
        /// <param name="ownerId">The owner of the trip</param>
        /// <returns>True if updated the stop status, false otherwise</returns>
        public bool UpdateStopStatusForPositions(TripStopInfo nextStop, Tuple<double, Position> positionWithDistance,
                                                 int ownerId, long tripID, Polygon sitePolygon)
        {
            bool stopStatusUpdated = false;

            if (nextStop == null || positionWithDistance == null)
                return false;

            var point = new Point(Convert.ToDouble(positionWithDistance.Item2.GPSCoordinate?.Latitude), 
                                  Convert.ToDouble(positionWithDistance.Item2.GPSCoordinate?.Longitude));

            if (_needsStopStatusUpdate(nextStop, point, sitePolygon, positionWithDistance.Item1))
            {
                if (nextStop.StopStatus == StopStatus.Open)
                {
                    _logger.LogTrace($"Message: Stop Status was Open and GPS Point is inside stop geometry for trip id {tripID}. Updating stop status to Arrived");

                    var arrivalStatus = _arrivalStatusUpdater.UpdateArrivalStatus(nextStop);
                    nextStop.StopStatus = StopStatus.Arrived;

                    stopStatusUpdated = _stopStatusRepository.UpdateStopStatus(tripID, nextStop.StopSequence,
                        (byte)nextStop.StopStatus, (byte)arrivalStatus, positionWithDistance.Item2.DeviceTime, string.Empty, null);
                }
                else if (nextStop.StopStatus == StopStatus.Arrived)
                {
                    _logger.LogTrace($"Message: Stop Status was Arrived and GPS Point is outside stop geometry for trip id {tripID}. Updating stop status to Completed.");

                    nextStop.ActualDeparture = positionWithDistance.Item2.DeviceTime;
                    var waitTime = _calculateWaitTime(nextStop.ActualArrival, nextStop.ActualDeparture);
                    var arrivalStatus = _arrivalStatusUpdater.UpdateArrivalStatus(nextStop);
                    nextStop.StopStatus = StopStatus.Completed;

                    stopStatusUpdated = _stopStatusRepository.UpdateStopStatus(tripID, nextStop.StopSequence,
                        (byte)nextStop.StopStatus, (byte)arrivalStatus, string.Empty, positionWithDistance.Item2.DeviceTime, waitTime);
                }
            }

            return stopStatusUpdated;
        }

        private bool _needsStopStatusUpdate(TripStopInfo nextStop, Point point, Polygon polygon, double milesToGPSCoord)
        {
            bool needsStopStatusUpdate = false;

            // If we have an actual polygon, use that. Otherwise, use air distance.
            if (polygon != null)
            {
                // For an Open stop, we want to check if the driver has entered the stop Polygon, so use Contains
                if (nextStop.StopStatus == StopStatus.Open && polygon.Contains(point))
                    needsStopStatusUpdate = true;
                // For an Arrived stop, we want to check if the driver has left the stop Polygon, so use !Contains
                else if (nextStop.StopStatus == StopStatus.Arrived && !polygon.Contains(point))
                    needsStopStatusUpdate = true;
            }
            else
            {
                // For an Open stop, we want to check if the driver is near the stop
                if (nextStop.StopStatus == StopStatus.Open && milesToGPSCoord < MAX_AIR_DISTANCE_FROM_STOP)
                    needsStopStatusUpdate = true;
                // For an Arrived stop, we want to check if the driver has moved away from the stop
                else if (nextStop.StopStatus == StopStatus.Arrived && milesToGPSCoord >= MAX_AIR_DISTANCE_FROM_STOP)
                    needsStopStatusUpdate = true;
            }

            return needsStopStatusUpdate;
        }

        private int? _calculateWaitTime(string actualArrival, string actualDeparture)
        {
            if (!string.IsNullOrWhiteSpace(actualArrival))
            {
                if (DateTimeOffset.TryParse(actualArrival, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset arrivedTime) &&
                    DateTimeOffset.TryParse(actualDeparture, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset departedTime))
                {
                    var timeSpentAtStop = (int)departedTime.Subtract(arrivedTime).TotalMinutes;
                    return timeSpentAtStop >= 0 ? timeSpentAtStop : 0;
                }
            }

            return null;
        }
    }
}
