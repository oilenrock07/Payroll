using System;

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
    }
}
