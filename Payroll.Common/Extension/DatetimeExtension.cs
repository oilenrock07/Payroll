using System;
using System.Collections.Generic;

namespace Payroll.Common.Extension
{
    public static class DatetimeExtension
    {
        //yearmonthdayhourminutesecond
        public static string Serialize(this DateTime date)
        {
            string serializedDate = String.Format("{0}{1}{2}{3}{4}{5}", date.Year, date.Month.ToString("00"), date.Day.ToString("00"), date.Hour.ToString("00"),date.Minute.ToString("00"), date.Second.ToString("00"));
            return serializedDate;
        }

        public static DateTime DeserializeDate(this string date)
        {
            //20160208112700
            int year = Convert.ToInt16(date.Substring(0, 4));
            int month = Convert.ToInt16(date.Substring(4, 2));
            int day = Convert.ToInt16(date.Substring(6, 2));
            int hour = Convert.ToInt16(date.Substring(8, 2));
            int minute = Convert.ToInt16(date.Substring(10, 2));
            int second = Convert.ToInt16(date.Substring(12, 2));

            return new DateTime(year, month, day, hour, minute, second);
        }

        public static DateTime TruncateTime(this DateTime date)
        {
            return Convert.ToDateTime(date.ToShortDateString());
        }

        public static bool IsValidBirthDate(this DateTime birthdate)
        {
            return !((birthdate > DateTime.Now) || birthdate < new DateTime(1900, 1, 1));
        }

        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public static DateTime ChangeTime(this DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                hours,
                minutes,
                seconds,
                milliseconds,
                dateTime.Kind);
        }

        public static bool IsRestDay(this DateTime date, int startDay, int endDay)
        {
            DayOfWeek day = date.DayOfWeek;
            DayOfWeek start_day = (DayOfWeek)startDay;
            DayOfWeek end_day = (DayOfWeek)endDay;

            if (end_day < start_day)
                end_day += 7;

            if (day < start_day)
                day += 7;

            if (day >= start_day && day <= end_day)
            {
                return false;
            }
            return true;
        }

        public static DateTime StartOfWeek(this DateTime date, DayOfWeek startOfWeek)
        {
            int diff = date.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return date.AddDays(-1 * diff).Date;
        }

        ///<summary>Gets the first week day following a date.</summary>
        ///<param name="date">The date.</param>
        ///<param name="dayOfWeek">The day of week to return.</param>
        ///<returns>The first dayOfWeek day following date, or date if it is on dayOfWeek.</returns>
        public static DateTime Next(this DateTime date, DayOfWeek dayOfWeek)
        {
            return date.AddDays((dayOfWeek < date.DayOfWeek ? 7 : 0) + dayOfWeek - date.DayOfWeek);
        }

        public static DateTime GetNthWeekofMonth(DateTime date, int nthWeek, DayOfWeek dayOfWeek)
        {
            DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);

            return firstDayOfMonth.Next(dayOfWeek).AddDays((nthWeek - 1) * 7);
        }
    }
}
