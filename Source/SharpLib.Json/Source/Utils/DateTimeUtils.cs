using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace SharpLib.Json
{
    internal static class DateTimeUtils
    {
        internal static readonly long InitialJavaScriptDateTicks = 621355968000000000;

        private const int DaysPer100Years = 36524;

        private const int DaysPer400Years = 146097;

        private const int DaysPer4Years = 1461;

        private const int DaysPerYear = 365;

        private const long TicksPerDay = 864000000000L;

        private static readonly int[] DaysToMonth365;

        private static readonly int[] DaysToMonth366;

        static DateTimeUtils()
        {
            DaysToMonth365 = new[] { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
            DaysToMonth366 = new[] { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 };
        }

        public static TimeSpan GetUtcOffset(this DateTime d)
        {
            return TimeZoneInfo.Local.GetUtcOffset(d);
        }

        public static XmlDateTimeSerializationMode ToSerializationMode(DateTimeKind kind)
        {
            switch (kind)
            {
                case DateTimeKind.Local:
                    return XmlDateTimeSerializationMode.Local;
                case DateTimeKind.Unspecified:
                    return XmlDateTimeSerializationMode.Unspecified;
                case DateTimeKind.Utc:
                    return XmlDateTimeSerializationMode.Utc;
                default:
                    throw MiscellaneousUtils.CreateArgumentOutOfRangeException("kind", kind, "Unexpected DateTimeKind value.");
            }
        }

        internal static DateTime EnsureDateTime(DateTime value, DateTimeZoneHandling timeZone)
        {
            switch (timeZone)
            {
                case DateTimeZoneHandling.Local:
                    value = SwitchToLocalTime(value);
                    break;
                case DateTimeZoneHandling.Utc:
                    value = SwitchToUtcTime(value);
                    break;
                case DateTimeZoneHandling.Unspecified:
                    value = new DateTime(value.Ticks, DateTimeKind.Unspecified);
                    break;
                case DateTimeZoneHandling.RoundtripKind:
                    break;
                default:
                    throw new ArgumentException("Invalid date time handling value.");
            }

            return value;
        }

        private static DateTime SwitchToLocalTime(DateTime value)
        {
            switch (value.Kind)
            {
                case DateTimeKind.Unspecified:
                    return new DateTime(value.Ticks, DateTimeKind.Local);

                case DateTimeKind.Utc:
                    return value.ToLocalTime();

                case DateTimeKind.Local:
                    return value;
            }
            return value;
        }

        private static DateTime SwitchToUtcTime(DateTime value)
        {
            switch (value.Kind)
            {
                case DateTimeKind.Unspecified:
                    return new DateTime(value.Ticks, DateTimeKind.Utc);

                case DateTimeKind.Utc:
                    return value;

                case DateTimeKind.Local:
                    return value.ToUniversalTime();
            }
            return value;
        }

        private static long ToUniversalTicks(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return dateTime.Ticks;
            }

            return ToUniversalTicks(dateTime, dateTime.GetUtcOffset());
        }

        private static long ToUniversalTicks(DateTime dateTime, TimeSpan offset)
        {
            if (dateTime.Kind == DateTimeKind.Utc || dateTime == DateTime.MaxValue || dateTime == DateTime.MinValue)
            {
                return dateTime.Ticks;
            }

            long ticks = dateTime.Ticks - offset.Ticks;
            if (ticks > 3155378975999999999L)
            {
                return 3155378975999999999L;
            }

            if (ticks < 0L)
            {
                return 0L;
            }

            return ticks;
        }

        internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, TimeSpan offset)
        {
            long universialTicks = ToUniversalTicks(dateTime, offset);

            return UniversialTicksToJavaScriptTicks(universialTicks);
        }

        internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime)
        {
            return ConvertDateTimeToJavaScriptTicks(dateTime, true);
        }

        internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, bool convertToUtc)
        {
            long ticks = (convertToUtc) ? ToUniversalTicks(dateTime) : dateTime.Ticks;

            return UniversialTicksToJavaScriptTicks(ticks);
        }

        private static long UniversialTicksToJavaScriptTicks(long universialTicks)
        {
            long javaScriptTicks = (universialTicks - InitialJavaScriptDateTicks) / 10000;

            return javaScriptTicks;
        }

        internal static DateTime ConvertJavaScriptTicksToDateTime(long javaScriptTicks)
        {
            DateTime dateTime = new DateTime((javaScriptTicks * 10000) + InitialJavaScriptDateTicks, DateTimeKind.Utc);

            return dateTime;
        }

        #region Parse

        internal static bool TryParseDateIso(string text, DateParseHandling dateParseHandling, DateTimeZoneHandling dateTimeZoneHandling, out object dt)
        {
            DateTimeParser dateTimeParser = new DateTimeParser();
            if (!dateTimeParser.Parse(text))
            {
                dt = null;
                return false;
            }

            DateTime d = new DateTime(dateTimeParser.Year, dateTimeParser.Month, dateTimeParser.Day, dateTimeParser.Hour, dateTimeParser.Minute, dateTimeParser.Second);
            d = d.AddTicks(dateTimeParser.Fraction);

            if (dateParseHandling == DateParseHandling.DateTimeOffset)
            {
                TimeSpan offset;

                switch (dateTimeParser.Zone)
                {
                    case ParserTimeZone.Utc:
                        offset = new TimeSpan(0L);
                        break;
                    case ParserTimeZone.LocalWestOfUtc:
                        offset = new TimeSpan(-dateTimeParser.ZoneHour, -dateTimeParser.ZoneMinute, 0);
                        break;
                    case ParserTimeZone.LocalEastOfUtc:
                        offset = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
                        break;
                    default:
                        offset = TimeZoneInfo.Local.GetUtcOffset(d);
                        break;
                }

                long ticks = d.Ticks - offset.Ticks;
                if (ticks < 0 || ticks > 3155378975999999999)
                {
                    dt = null;
                    return false;
                }

                dt = new DateTimeOffset(d, offset);
                return true;
            }
            else
            {
                long ticks;

                switch (dateTimeParser.Zone)
                {
                    case ParserTimeZone.Utc:
                        d = new DateTime(d.Ticks, DateTimeKind.Utc);
                        break;

                    case ParserTimeZone.LocalWestOfUtc:
                        {
                            TimeSpan offset = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
                            ticks = d.Ticks + offset.Ticks;
                            if (ticks <= DateTime.MaxValue.Ticks)
                            {
                                d = new DateTime(ticks, DateTimeKind.Utc).ToLocalTime();
                            }
                            else
                            {
                                ticks += d.GetUtcOffset().Ticks;
                                if (ticks > DateTime.MaxValue.Ticks)
                                {
                                    ticks = DateTime.MaxValue.Ticks;
                                }

                                d = new DateTime(ticks, DateTimeKind.Local);
                            }
                            break;
                        }
                    case ParserTimeZone.LocalEastOfUtc:
                        {
                            TimeSpan offset = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
                            ticks = d.Ticks - offset.Ticks;
                            if (ticks >= DateTime.MinValue.Ticks)
                            {
                                d = new DateTime(ticks, DateTimeKind.Utc).ToLocalTime();
                            }
                            else
                            {
                                ticks += d.GetUtcOffset().Ticks;
                                if (ticks < DateTime.MinValue.Ticks)
                                {
                                    ticks = DateTime.MinValue.Ticks;
                                }

                                d = new DateTime(ticks, DateTimeKind.Local);
                            }
                            break;
                        }
                }

                dt = EnsureDateTime(d, dateTimeZoneHandling);
                return true;
            }
        }

        internal static bool TryParseDateTime(string s, DateParseHandling dateParseHandling, DateTimeZoneHandling dateTimeZoneHandling, string dateFormatString, CultureInfo culture, out object dt)
        {
            if (s.Length > 0)
            {
                if (s[0] == '/')
                {
                    if (s.StartsWith("/Date(", StringComparison.Ordinal) && s.EndsWith(")/", StringComparison.Ordinal))
                    {
                        if (TryParseDateMicrosoft(s, dateParseHandling, dateTimeZoneHandling, out dt))
                        {
                            return true;
                        }
                    }
                }
                else if (s.Length >= 19 && s.Length <= 40 && char.IsDigit(s[0]) && s[10] == 'T')
                {
                    if (TryParseDateIso(s, dateParseHandling, dateTimeZoneHandling, out dt))
                    {
                        return true;
                    }
                }

                if (!string.IsNullOrEmpty(dateFormatString))
                {
                    if (TryParseDateExact(s, dateParseHandling, dateTimeZoneHandling, dateFormatString, culture, out dt))
                    {
                        return true;
                    }
                }
            }

            dt = null;
            return false;
        }

        private static bool TryParseDateMicrosoft(string text, DateParseHandling dateParseHandling, DateTimeZoneHandling dateTimeZoneHandling, out object dt)
        {
            string value = text.Substring(6, text.Length - 8);
            DateTimeKind kind = DateTimeKind.Utc;

            int index = value.IndexOf('+', 1);

            if (index == -1)
            {
                index = value.IndexOf('-', 1);
            }

            TimeSpan offset = TimeSpan.Zero;

            if (index != -1)
            {
                kind = DateTimeKind.Local;
                offset = ReadOffset(value.Substring(index));
                value = value.Substring(0, index);
            }

            long javaScriptTicks;
            if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out javaScriptTicks))
            {
                dt = null;
                return false;
            }

            DateTime utcDateTime = ConvertJavaScriptTicksToDateTime(javaScriptTicks);

            if (dateParseHandling == DateParseHandling.DateTimeOffset)
            {
                dt = new DateTimeOffset(utcDateTime.Add(offset).Ticks, offset);
                return true;
            }

            DateTime dateTime;
            switch (kind)
            {
                case DateTimeKind.Unspecified:
                    dateTime = DateTime.SpecifyKind(utcDateTime.ToLocalTime(), DateTimeKind.Unspecified);
                    break;
                case DateTimeKind.Local:
                    dateTime = utcDateTime.ToLocalTime();
                    break;
                default:
                    dateTime = utcDateTime;
                    break;
            }

            dt = EnsureDateTime(dateTime, dateTimeZoneHandling);
            return true;
        }

        private static bool TryParseDateExact(string text, DateParseHandling dateParseHandling, DateTimeZoneHandling dateTimeZoneHandling, string dateFormatString, CultureInfo culture, out object dt)
        {
            if (dateParseHandling == DateParseHandling.DateTimeOffset)
            {
                DateTimeOffset temp;
                if (DateTimeOffset.TryParseExact(text, dateFormatString, culture, DateTimeStyles.RoundtripKind, out temp))
                {
                    dt = temp;
                    return true;
                }
            }
            else
            {
                DateTime temp;
                if (DateTime.TryParseExact(text, dateFormatString, culture, DateTimeStyles.RoundtripKind, out temp))
                {
                    temp = EnsureDateTime(temp, dateTimeZoneHandling);
                    dt = temp;
                    return true;
                }
            }

            dt = null;
            return false;
        }

        private static TimeSpan ReadOffset(string offsetText)
        {
            bool negative = (offsetText[0] == '-');

            int hours = int.Parse(offsetText.Substring(1, 2), NumberStyles.Integer, CultureInfo.InvariantCulture);
            int minutes = 0;
            if (offsetText.Length >= 5)
            {
                minutes = int.Parse(offsetText.Substring(3, 2), NumberStyles.Integer, CultureInfo.InvariantCulture);
            }

            TimeSpan offset = TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes);
            if (negative)
            {
                offset = offset.Negate();
            }

            return offset;
        }

        #endregion

        #region Write

        internal static void WriteDateTimeString(TextWriter writer, DateTime value, DateFormatHandling format, string formatString, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(formatString))
            {
                char[] chars = new char[64];
                int pos = WriteDateTimeString(chars, 0, value, null, value.Kind, format);
                writer.Write(chars, 0, pos);
            }
            else
            {
                writer.Write(value.ToString(formatString, culture));
            }
        }

        internal static int WriteDateTimeString(char[] chars, int start, DateTime value, TimeSpan? offset, DateTimeKind kind, DateFormatHandling format)
        {
            int pos = start;

            if (format == DateFormatHandling.MicrosoftDateFormat)
            {
                TimeSpan o = offset ?? value.GetUtcOffset();

                long javaScriptTicks = ConvertDateTimeToJavaScriptTicks(value, o);

                @"\/Date(".CopyTo(0, chars, pos, 7);
                pos += 7;

                string ticksText = javaScriptTicks.ToString(CultureInfo.InvariantCulture);
                ticksText.CopyTo(0, chars, pos, ticksText.Length);
                pos += ticksText.Length;

                switch (kind)
                {
                    case DateTimeKind.Unspecified:
                        if (value != DateTime.MaxValue && value != DateTime.MinValue)
                        {
                            pos = WriteDateTimeOffset(chars, pos, o, format);
                        }
                        break;
                    case DateTimeKind.Local:
                        pos = WriteDateTimeOffset(chars, pos, o, format);
                        break;
                }

                @")\/".CopyTo(0, chars, pos, 3);
                pos += 3;
            }
            else
            {
                pos = WriteDefaultIsoDate(chars, pos, value);

                switch (kind)
                {
                    case DateTimeKind.Local:
                        pos = WriteDateTimeOffset(chars, pos, offset ?? value.GetUtcOffset(), format);
                        break;
                    case DateTimeKind.Utc:
                        chars[pos++] = 'Z';
                        break;
                }
            }

            return pos;
        }

        internal static int WriteDefaultIsoDate(char[] chars, int start, DateTime dt)
        {
            int length = 19;

            int year;
            int month;
            int day;
            GetDateValues(dt, out year, out month, out day);

            CopyIntToCharArray(chars, start, year, 4);
            chars[start + 4] = '-';
            CopyIntToCharArray(chars, start + 5, month, 2);
            chars[start + 7] = '-';
            CopyIntToCharArray(chars, start + 8, day, 2);
            chars[start + 10] = 'T';
            CopyIntToCharArray(chars, start + 11, dt.Hour, 2);
            chars[start + 13] = ':';
            CopyIntToCharArray(chars, start + 14, dt.Minute, 2);
            chars[start + 16] = ':';
            CopyIntToCharArray(chars, start + 17, dt.Second, 2);

            int fraction = (int)(dt.Ticks % 10000000L);

            if (fraction != 0)
            {
                int digits = 7;
                while ((fraction % 10) == 0)
                {
                    digits--;
                    fraction /= 10;
                }

                chars[start + 19] = '.';
                CopyIntToCharArray(chars, start + 20, fraction, digits);

                length += digits + 1;
            }

            return start + length;
        }

        private static void CopyIntToCharArray(char[] chars, int start, int value, int digits)
        {
            while (digits-- != 0)
            {
                chars[start + digits] = (char)((value % 10) + 48);
                value /= 10;
            }
        }

        internal static int WriteDateTimeOffset(char[] chars, int start, TimeSpan offset, DateFormatHandling format)
        {
            chars[start++] = (offset.Ticks >= 0L) ? '+' : '-';

            int absHours = Math.Abs(offset.Hours);
            CopyIntToCharArray(chars, start, absHours, 2);
            start += 2;

            if (format == DateFormatHandling.IsoDateFormat)
            {
                chars[start++] = ':';
            }

            int absMinutes = Math.Abs(offset.Minutes);
            CopyIntToCharArray(chars, start, absMinutes, 2);
            start += 2;

            return start;
        }

#if !NET20
        internal static void WriteDateTimeOffsetString(TextWriter writer, DateTimeOffset value, DateFormatHandling format, string formatString, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(formatString))
            {
                char[] chars = new char[64];
                int pos = WriteDateTimeString(chars, 0, (format == DateFormatHandling.IsoDateFormat) ? value.DateTime : value.UtcDateTime, value.Offset, DateTimeKind.Local, format);

                writer.Write(chars, 0, pos);
            }
            else
            {
                writer.Write(value.ToString(formatString, culture));
            }
        }
#endif

        #endregion

        private static void GetDateValues(DateTime td, out int year, out int month, out int day)
        {
            long ticks = td.Ticks;

            int n = (int)(ticks / TicksPerDay);

            int y400 = n / DaysPer400Years;

            n -= y400 * DaysPer400Years;

            int y100 = n / DaysPer100Years;

            if (y100 == 4)
            {
                y100 = 3;
            }

            n -= y100 * DaysPer100Years;

            int y4 = n / DaysPer4Years;

            n -= y4 * DaysPer4Years;

            int y1 = n / DaysPerYear;

            if (y1 == 4)
            {
                y1 = 3;
            }

            year = y400 * 400 + y100 * 100 + y4 * 4 + y1 + 1;

            n -= y1 * DaysPerYear;

            bool leapYear = y1 == 3 && (y4 != 24 || y100 == 3);
            int[] days = leapYear ? DaysToMonth366 : DaysToMonth365;

            int m = n >> 5 + 1;

            while (n >= days[m])
            {
                m++;
            }

            month = m;

            day = n - days[m - 1] + 1;
        }
    }
}