using ALK.TripInsight.Model;

namespace ALK.TripInsight.Worker.Trip.StopInfo
{
	public interface ITripStopInfoProvider
	{
        TripStopInfo GetNextOpenOrArrivedStop(long tripId, int ownerId);
    }
}
