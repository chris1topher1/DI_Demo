using ALK.TripInsight.Model;
using ALK.TripInsight.Worker.Date;
using System;

namespace ALK.TripInsight.Worker.Status
{
    internal class ArrivalStatusUpdater : IArrivalStatusUpdater
    {
        private readonly IDateTimeParser _dateTimeParser;

        public ArrivalStatusUpdater(IDateTimeParser dateTimeParser)
        {
            _dateTimeParser = dateTimeParser;
        }

        public ArrivalStatus UpdateArrivalStatus(TripStopInfo nextStop)
        {
            ArrivalStatus status = ArrivalStatus.OnTime;

            string windowStartTime = nextStop.EarliestArrival;
            string windowEndTime = nextStop.LatestArrival;

            // No current ETA or ActualArrival means we can't do any of this validation; bail out
            if (string.IsNullOrWhiteSpace(nextStop.CurrentETA) && string.IsNullOrWhiteSpace(nextStop.ActualArrival))
                return status;

            // If we have an actual arrival time, use that. Otherwise, use CurrenETA as a fallback.
            DateTimeOffset actualETA = _dateTimeParser.Convert(string.IsNullOrWhiteSpace(nextStop.ActualArrival) ? nextStop.CurrentETA : nextStop.ActualArrival, true);

            bool bCompareToPlannedETA = false;

            // For non-Planned trips, if we weren't given the start and ending arrival times, just compare currentETA/ActualArrival to PlannedETA
            if (string.IsNullOrWhiteSpace(windowEndTime) && string.IsNullOrWhiteSpace(windowStartTime))
            {
                windowStartTime = nextStop.PlannedETA;
                windowEndTime = nextStop.PlannedETA;
                // Don't want to show the stop as "At Risk" if we're just comparing planned/actual ETA
                bCompareToPlannedETA = _dateTimeParser.Convert(nextStop.PlannedETA, true) == actualETA;
            }

            // if we have an end of time window, see if we're at risk
            if (!string.IsNullOrWhiteSpace(windowEndTime))
            {
                DateTimeOffset endOfTimeWindow = _dateTimeParser.Convert(windowEndTime, true);

                if (actualETA > endOfTimeWindow)
                {
                    status = ArrivalStatus.Late;
                }
                else
                {
                    TimeSpan etaDifference = endOfTimeWindow - actualETA;
                    if (!bCompareToPlannedETA && nextStop.AtRiskThreshold.HasValue && nextStop.AtRiskThreshold.Value > 0 && etaDifference.TotalMinutes <= nextStop.AtRiskThreshold.Value)
                    {
                        status = ArrivalStatus.AtRisk;
                    }
                }
            }

            // if the end of time window is OK and we have a beginning window, check if we're going to be early
            if (!string.IsNullOrWhiteSpace(windowStartTime) && status == ArrivalStatus.OnTime)
            {
                // see if they are going to be early
                DateTimeOffset startOfTimeWindow = _dateTimeParser.Convert(windowStartTime, true);

                if (startOfTimeWindow > actualETA)
                {
                    TimeSpan etaDifference = startOfTimeWindow - actualETA;
                    if (nextStop.TooEarlyThreshold.HasValue && etaDifference.TotalMinutes >= nextStop.TooEarlyThreshold.Value)
                        status = ArrivalStatus.TooEarly;
                    else
                        status = ArrivalStatus.Early;
                }
            }

            return status;
        }
    }
}
