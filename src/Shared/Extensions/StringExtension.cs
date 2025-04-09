using SharedLibrary.Enums;
using SharedLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
    public static class StringExtension
    {
        public static DateTime ToDateTime(this string? date, ConvertingDate type)
        {
            if (date != null)
            {
                if (DateTimeOffset.TryParseExact(date, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset dateWithOffset))
                {
                    return dateWithOffset.DateTime;
                }
            }

            switch (type)
            {
                case ConvertingDate.FromStartMonth:
                    return DateRange.StartDate(2);
                case ConvertingDate.FromLast30Days:
                    return DateRange.StartDate(3);
                case ConvertingDate.Now:
                    return DateTime.Now;
                default:
                    return DateRange.EndDate(2);
            }
        }
        public static DateTime? ToNullableDateTime(this string date, string format = "yyyy-MM-dd")
        {
            if (DateTime.TryParseExact(date, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }

            return null;
        }
    }
}
