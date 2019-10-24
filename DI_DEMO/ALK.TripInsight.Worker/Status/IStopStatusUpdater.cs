using System;
using ALK.TripInsight.Model;
using NetTopologySuite.Geometries;

namespace ALK.TripInsight.Worker.Status
{
    internal interface IStopStatusUpdater
    {
       bool UpdateStopStatusForPositions(TripStopInfo nextStop,
                                         Tuple<double, Position> positionWithDistance,
                                         int ownerId,
                                         long tripID,
                                         Polygon sitePolygon);
    }
}
