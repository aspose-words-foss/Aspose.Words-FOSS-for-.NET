// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/07/2009 by Dmitry Vorobyev

using Aspose.Common;
using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the DATE field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Inserts the current date and time. By default, the Gregorian calendar is used.
    /// </remarks>
    public class FieldDate : Field, IFieldCodeTokenInfoProvider, IFieldWithCalendar
    {
        internal override FieldUpdateAction UpdateCore()
        {
            CalendarType calendarType = FieldDateUtils.ResolveCalendarType(this);
            return new FieldUpdateActionApplyResult(this, new DateTimeConstant(FetchDocument().CurrentDateTimeCache, calendarType));
        }

        internal override string GetDefaultDateTimeFormat()
        {
            // Can't use .NET's standard specifier here as Word does not have them.
            return FormatterPal.GetShortDatePatternCurrent();
        }

        /// <summary>
        /// Gets or sets whether to use the Hijri Lunar or Hebrew Lunar calendar.
        /// </summary>
        public bool UseLunarCalendar
        {
            get { return FieldCodeCache.HasSwitch(UseLunarCalendarSwitch); }
            set { FieldCodeCache.SetSwitch(UseLunarCalendarSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to use a format last used by the hosting application when inserting a new DATE field.
        /// </summary>
        public bool UseLastFormat
        {
            get { return FieldCodeCache.HasSwitch(UseLastFormatSwitch); }
            set { FieldCodeCache.SetSwitch(UseLastFormatSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to use the Saka Era calendar.
        /// </summary>
        public bool UseSakaEraCalendar
        {
            get { return FieldCodeCache.HasSwitch(UseSakaEraCalendarSwitch); }
            set { FieldCodeCache.SetSwitch(UseSakaEraCalendarSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to use the Um-al-Qura calendar.
        /// </summary>
        public bool UseUmAlQuraCalendar
        {
            get { return FieldCodeCache.HasSwitch(UseUmAlQuraCalendarSwitch); }
            set { FieldCodeCache.SetSwitch(UseUmAlQuraCalendarSwitch, value); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case UseLunarCalendarSwitch:
                case UseLastFormatSwitch:
                case UseSakaEraCalendarSwitch:
                case UseUmAlQuraCalendarSwitch:
                {
                    return FieldSwitchType.Flag;
                }
                default:
                {
                    return FieldSwitchType.Unknown;
                }
            }
        }

        private const string UseLunarCalendarSwitch = "\\h";
        private const string UseLastFormatSwitch = "\\l";
        private const string UseSakaEraCalendarSwitch = "\\s";
        private const string UseUmAlQuraCalendarSwitch = "\\u";
    }
}
