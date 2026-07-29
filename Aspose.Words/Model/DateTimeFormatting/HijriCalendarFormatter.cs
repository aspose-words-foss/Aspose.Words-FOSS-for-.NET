// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/08/2016 by Edward Voronov

using System;
using System.Globalization;
using Aspose.Common;

namespace Aspose.Words
{
    internal class HijriCalendarFormatter : CalendarFormatter
    {
        public HijriCalendarFormatter()
            : base(new HijriCalendar())
        {
        }

        protected override bool UseInvariantCulture(char token, int tokenLength)
        {
            switch (token)
            {
                case 'd':
                    return tokenLength <= 2;
                case 'M':
                    return tokenLength <= 2;
                default:
                    return true;
            }
        }

        protected override CultureInfo InvariantCulture
        {
            get { return gInvariantCulture; }
        }

        protected override string FormatCore(DateTime value, char token, int tokenLength)
        {
            Debug.Assert(!UseInvariantCulture(token, tokenLength));

            switch (token)
            {
                case 'd':
                    Debug.Assert(tokenLength > 2);
                    return FormatDay(value);
                case 'M':
                    Debug.Assert(tokenLength > 2);
                    return FormatMonth(value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string FormatDay(DateTime value)
        {
            DayOfWeek dayOfWeek = Calendar.GetDayOfWeek(value);
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday: return "AlAhad";
                case DayOfWeek.Monday: return "AlEthnien";
                case DayOfWeek.Tuesday: return "AthThulatha";
                case DayOfWeek.Wednesday: return "AlArbia'a";
                case DayOfWeek.Thursday: return "AlKhamis";
                case DayOfWeek.Friday: return "AlJumaa";
                case DayOfWeek.Saturday: return "AsSabt";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string FormatMonth(DateTime value)
        {
            int month = Calendar.GetMonth(value);
            switch (month)
            {
                case 1: return "Muharram";
                case 2: return "Safar";
                case 3: return "Rabi' Awwal";
                case 4: return "Rabi' Thani";
                case 5: return "Jamada El Oula";
                case 6: return "Jamada El Thaniah";
                case 7: return "Rajab";
                case 8: return "Sha'ban";
                case 9: return "Ramadan";
                case 10: return "Shawwal";
                case 11: return "Thoul Ki'dah";
                case 12: return "Thoul Hijjah";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static readonly CultureInfo gInvariantCulture = SystemPal.GetCulture("ar-SA");
    }
}
