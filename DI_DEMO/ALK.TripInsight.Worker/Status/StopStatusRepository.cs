using Dapper;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Globalization;
using ALK.TripInsight.Model;

namespace ALK.TripInsight.Worker.Status
{
    internal class StopStatusRepository : IStopStatusRepository
    {
        private readonly string _connectionString;
        private readonly string UPDATE_METHOD = "t";

        public StopStatusRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool UpdateStopStatus(long tripId, short stopNumber, StopStatus status)
        {
            return true;
        }

        public bool UpdateStopStatus(long tripId, short stopNumber, byte stopStatus, byte arrivalStatus, 
                                    string actualArrival, string actualDeparture, int? waitTime)
        {
            bool updateSuccess = false;

            using (var connection = new SqlConnection(_connectionString))
            {
                var parameter = new DynamicParameters();
                parameter.Add("routeId", tripId);
                parameter.Add("stopNumber", stopNumber);
                parameter.Add("stopStatus", stopStatus);
                parameter.Add("arrivalStatus", arrivalStatus);
                parameter.Add("triggerType", UPDATE_METHOD);

                if (!string.IsNullOrWhiteSpace(actualArrival))
                {
                    if (DateTimeOffset.TryParse(actualArrival, CultureInfo.InvariantCulture,
                                                DateTimeStyles.AssumeUniversal, out DateTimeOffset dateTime))
                        parameter.Add("actualArrival", dateTime);
                }

                if (!string.IsNullOrWhiteSpace(actualDeparture))
                {
                    if (DateTimeOffset.TryParse(actualDeparture, CultureInfo.InvariantCulture,
                                                DateTimeStyles.AssumeUniversal, out DateTimeOffset dateTime))
                        parameter.Add("actualDeparture", dateTime);
                }

                if (waitTime.HasValue)
                    parameter.Add("waitTime", waitTime);

                updateSuccess = connection.Execute("dbo.SPR_TripInsightStop_UpdateStatus", param: parameter, commandType: CommandType.StoredProcedure) > 0;
            }

            return updateSuccess;
        }
    }
}
