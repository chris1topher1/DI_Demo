using ALK.TripInsight.Model;

namespace ALK.TripInsight.Worker.GPS
{
    internal interface IGpsPositionProvider
    {
        Position GetLatestPosition(long? tripId, int ownerId);
    }
}