using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ALK.TripInsight.Worker.Date;
using ALK.TripInsight.Model;
using Dapper;

namespace ALK.TripInsight.Worker.Trip.StopInfo
{
    internal class TripStopInfoProvider
    {
        private readonly string _connectionString;
        private readonly DateTimeParser _dateTimeParser;

        public TripStopInfoProvider(string connectionString)
        {
            _connectionString = connectionString;
            _dateTimeParser = new DateTimeParser();
        }

        public TripStopInfo GetNextOpenOrArrivedStop(long tripId, int ownerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ownerId", ownerId);
                parameters.Add("@routeId", tripId);
                var data = connection.Query("dbo.SPR_TripInsight_GetNextOpenOrArrivedStop", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                var stopInfo = data == null
                    ? null
                    : new TripStopInfo
                    {
                        Location = new Location
                        {
                            Address = new Address
                            {
                                City = data.City,
                                Country = data.Country,
                                County = data.County,
                                State = data.State,
                                StreetAddress = data.Address,
                                Zip = data.PostalCode
                            },
                            Coords = new GPSCoordinate
                            {
                                Latitude = data.StopLatitude,
                                Longitude = data.StopLongitude
                            }
                        },
                        StopId = data.StopId,
                        StopName = data.StopName,
                        StopSequence = data.StopSequence,
                        StopStatus = (StopStatus)data.StopStatusId,
                        AssociatedSites = null,
                        TooEarlyThreshold = data.TooEarlyThreshold,
                        AtRiskThreshold = data.AtRiskThreshold,
                        EarliestArrival = _dateTimeParser.HandleOptionalDates(data.EarliestArrival),
                        LatestArrival = _dateTimeParser.HandleOptionalDates(data.LatestArrival),
                        ActualArrival = _dateTimeParser.HandleOptionalDates(data.ActualArrival),
                        ActualDeparture = _dateTimeParser.HandleOptionalDates(data.ActualDeparture),
                        PlannedETA = _dateTimeParser.HandleOptionalDates(data.PlannedETA),
                        CurrentETA = _dateTimeParser.HandleOptionalDates(data.CurrentETA)
                    };

                return stopInfo;
            }
        }
    }
}
