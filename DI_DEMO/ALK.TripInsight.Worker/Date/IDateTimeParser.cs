using System;

namespace ALK.TripInsight.Worker.Date
{
    internal interface IDateTimeParser
    {
        string HandleOptionalDates(object date, bool bToUtc = false);

        DateTimeOffset Convert(string dt, bool ignoreSeconds = true);
    }
}
