using ALK.TripInsight.Model;

namespace ALK.TripInsight.Worker.Status
{
    internal interface IStopStatusRepository
    {
        bool UpdateStopStatus(long tripId, short stopNumber, byte stopStatus, byte arrivalStatus,
                            string actualArrival, string actualDeparture, int? waitTime);

       bool UpdateStopStatus(long tripId, short stopNumber, StopStatus status);
    }
}