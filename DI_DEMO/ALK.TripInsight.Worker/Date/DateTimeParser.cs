using System;
using System.Globalization;

namespace ALK.TripInsight.Worker.Date
{
    internal class DateTimeParser
    {
        private const string SORTABLE_DATE_TIME_PATTERN = "yyyy-MM-ddTHH:mm:sszzz";

        public string HandleOptionalDates(object date, bool bToUtc = false)
        {
            if (date == DBNull.Value || date == null)
                return null;

            var dateTime = DateTimeOffset.Parse(date.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                if (bToUtc)
                    dateTime = dateTime.ToUniversalTime();

                return dateTime.ToString(SORTABLE_DATE_TIME_PATTERN);
        }

        public DateTimeOffset Convert(string dt, bool ignoreSeconds = true)
        {
            DateTimeOffset dtOffset = DateTimeOffset.Parse(dt, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            if (ignoreSeconds == false)
                return dtOffset;

            DateTimeOffset uTime = dtOffset.ToUniversalTime();
            return new DateTimeOffset(uTime.Year, uTime.Month, uTime.Day, uTime.Hour, uTime.Minute, 0, uTime.Offset);
        }
    }
}
