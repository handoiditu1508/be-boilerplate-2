using NodaTime;
using System;
using System.Linq;

namespace Hamburger.Helpers
{
    public static class TimeZoneHelper
    {
        /// <summary>
        /// Convert UTC DateTime to specific time zone.
        /// </summary>
        /// <param name="utcDateTime">DateTime in UTC.</param>
        /// <param name="timeZoneId">Time zone id to convert to.</param>
        /// <returns>Converted DateTime.</returns>
        public static DateTime ToTimeZone(DateTime utcDateTime, string timeZoneId)
        {
            IDateTimeZoneProvider timeZoneProvider = DateTimeZoneProviders.Tzdb;

            if (timeZoneProvider.Ids.Any(s => s != timeZoneId))
                throw CustomException.Validation.InvalidTimeZoneId(timeZoneId);

            DateTimeZone tzUtc = timeZoneProvider["UTC"];
            DateTimeZone tzDestination = timeZoneProvider[timeZoneId];

            LocalDateTime localNodaDate = LocalDateTime.FromDateTime(utcDateTime);
            ZonedDateTime utcZonedDate = tzUtc.AtStrictly(localNodaDate);
            ZonedDateTime zonedDate = utcZonedDate.WithZone(tzDestination);

            return zonedDate.ToDateTimeUnspecified();
        }

        /// <summary>
        /// Convert UTC DateTime to Ho Chi Minh time zone.
        /// </summary>
        /// <param name="utcDateTime">DateTime in UTC.</param>
        /// <returns></returns>
        public static DateTime ToHoChiMinhTimeZone(DateTime utcDateTime)
        {
            return ToTimeZone(utcDateTime, "Asia/Ho_Chi_Minh");
        }
    }
}
