using ALK.TripInsight.Model;

namespace ALK.TripInsight.Worker.GPS
{
    internal interface IGpsPositionRepository
    {
        bool SaveGpsPosition(Position[] positions, int ownerId, long? tripId);
    }
}