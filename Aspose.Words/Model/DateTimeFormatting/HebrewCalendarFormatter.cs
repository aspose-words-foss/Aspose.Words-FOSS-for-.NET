// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/08/2016 by Edward Voronov

using System;
using System.Globalization;
using Aspose.Common;

namespace Aspose.Words
{
    internal class HebrewCalendarFormatter : CalendarFormatter
    {
        public HebrewCalendarFormatter()
            : base(new HebrewCalendar())
        {
        }

        protected override bool UseInvariantCulture(char token, int tokenLength)
        {
            switch (token)
            {
                case 'd':
                case 'M':
                case 'y':
                    return false;
                default:
                    return true;
            }
        }

        protected override string FormatCore(DateTime value, char token, int tokenLength)
        {
            Debug.Assert(!UseInvariantCulture(token, tokenLength));

            switch (token)
            {
                case 'd':
                    return FormatDay(value, tokenLength);
                case 'M':
                    return FormatMonth(value);
                case 'y':
                    return FormatYear(value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override CultureInfo InvariantCulture
        {
            get { return gInvariantCulture; }
        }

        private string FormatDay(DateTime value, int tokenLength)
        {
            return (tokenLength > 2)
                ? FormatDayOfWeek(value)
                : FormatDayOfMonth(value);
        }

        private string FormatDayOfWeek(DateTime value)
        {
            DayOfWeek dayOfWeek = Calendar.GetDayOfWeek(value);
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday: return "Yom Rishon";
                case DayOfWeek.Monday: return "Yom Sheni";
                case DayOfWeek.Tuesday: return "Yom Shlishi";
                case DayOfWeek.Wednesday: return "Yom Revi'i";
                case DayOfWeek.Thursday: return "Yom Chamishi";
                case DayOfWeek.Friday: return "Yom Shishi";
                case DayOfWeek.Saturday: return "Shabat";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string FormatDayOfMonth(DateTime value)
        {
            int dayOfMonth = Calendar.GetDayOfMonth(value);
            return dayOfMonth.ToString();
        }

        private string FormatMonth(DateTime value)
        {
            int month = Calendar.GetMonth(value);

            // Adjust month index aware leap year.
            // Based on https://msdn.microsoft.com/en-us/library/system.globalization.hebrewcalendar(v=vs.110).aspx#Anchor_6
            if (Calendar.IsLeapYear(Calendar.GetYear(value)))
            {
                if (month > 5)
                    month++;
            }
            else
            {
                if (month > 6)
                    month += 2;
            }

            switch (month)
            {
                case 1: return "Tishrei";
                case 2: return "Cheshvan";
                case 3: return "Kislev";
                case 4: return "Tevet";
                case 5: return "Shevat";
                case 6: return "Adar";
                case 7: return "Adar Alef";
                case 8: return "Adar Beit";
                case 9: return "Nissan";
                case 10: return "Iyar";
                case 11: return "Sivan";
                case 12: return "Tamuz";
                case 13: return "Av";
                case 14: return "Elul";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string FormatYear(DateTime value)
        {
            int year = Calendar.GetYear(value);
            return year.ToString();
        }

        private static readonly CultureInfo gInvariantCulture = SystemPal.GetCulture("he-IL");
    }
}
