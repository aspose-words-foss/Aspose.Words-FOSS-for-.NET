// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/08/2011 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the CREATEDATE field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves the date and time at which the document was created. By default, the Gregorian calendar is used.
    /// </remarks>
    public class FieldCreateDate : Field, IFieldCodeTokenInfoProvider, IFieldWithCalendar
    {
        internal override FieldUpdateAction UpdateCore()
        {
            Document document = FetchDocument();
            CalendarType calendarType = FieldDateUtils.ResolveCalendarType(this);
            DateTimeConstant value = GetResultInternal(document, calendarType);
            return new FieldUpdateActionApplyResult(this, value);
        }

        private static DateTimeConstant GetResultInternal(Document document, CalendarType calendarType)
        {
            return new DateTimeConstant(
                DateTimeUtil.ToLocalTime(document.BuiltInDocumentProperties.CreatedTime),
                calendarType);
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
        private const string UseSakaEraCalendarSwitch = "\\s";
        private const string UseUmAlQuraCalendarSwitch = "\\u";

        internal static IFieldInfoResultProvider FieldInfoResultProvider = new CreateDateInfoResultProvider();

        private class CreateDateInfoResultProvider : IFieldInfoResultProvider
        {
            Constant IFieldInfoResultProvider.GetResult(Document document, IFieldCode fieldCode)
            {
                return GetResultInternal(document, CalendarType.Gregorian);
            }
        }
    }
}
