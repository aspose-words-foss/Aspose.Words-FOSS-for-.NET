// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/08/2016 by Edward Voronov

using System;
using System.Globalization;
using System.Text;

namespace Aspose.Words
{
    internal class GregorianCalendarFormatter : CalendarFormatter
    {
        public GregorianCalendarFormatter()
            : base(new GregorianCalendar())
        {
        }

        protected override void AdjustPattern(DateTime value, StringBuilder pattern, char token, int tokenLength, CultureInfo culture, string formatString, int tokenPosition)
        {
            if (!LanguageOnly.Compare(culture.LCID, LanguageOnly.French))
                return;

            if (token != 'd' || tokenLength != 1)
                return;

            if (value.Day != 1)
                return;

            if (!StringUtil.StartsWithOrdinalIgnoreCase(formatString.Substring(tokenPosition), "d MMMM"))
                return;

            pattern.Append("er");
        }

        protected override bool UseInvariantCulture(char token, int tokenLength)
        {
            return false;
        }

        protected override string FormatCore(DateTime value, char token, int tokenLength)
        {
            throw new InvalidOperationException();
        }

        protected override CultureInfo InvariantCulture
        {
            get { throw new InvalidOperationException(); }
        }
    }
}