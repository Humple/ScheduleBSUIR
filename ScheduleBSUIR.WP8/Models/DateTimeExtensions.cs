using System;
using System.Globalization;

namespace ScheduleBSUIR.Models
{
    public static class DateTimeExtensions
    {
        public static int ToBsuirWeek(this DateTime date)
        {
            // Расчет номера текущей недели БГУИР
            DateTime startdate;
            int dayDelta, weeknum = 0;

            startdate = date.Month <= 8
                ? new DateTime(date.Year - 1, 9, 1)
                : new DateTime(date.Year, 9, 1);

            if (startdate.DayOfWeek == 0) weeknum++;

            startdate = startdate.AddDays(-1 * (int)startdate.DayOfWeek);

            dayDelta = date.Month <= 8
                ? Math.Abs(date.DayOfYear + (new DateTime(startdate.Year, 12, 31).DayOfYear - startdate.DayOfYear))
                : Math.Abs(date.DayOfYear - startdate.DayOfYear);

            while (dayDelta > 7)
            {
                dayDelta -= 7;
                weeknum++;
            }
            weeknum++;
            while (weeknum > 4)
            {
                weeknum -= 4;
            }

            return weeknum;
        }

        public static string ToDayNameRu(this DateTime date)
        {
            return date.ToString("dddd", new CultureInfo("ru-ru"));
        }

    }
}
