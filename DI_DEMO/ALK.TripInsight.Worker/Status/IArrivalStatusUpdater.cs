using ALK.TripInsight.Model;

namespace ALK.TripInsight.Worker.Status
{
    internal interface IArrivalStatusUpdater
    {
        ArrivalStatus UpdateArrivalStatus(TripStopInfo nextStop);
    }
}
