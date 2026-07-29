// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/06/2005 by Roman Korchagin

using System;
using System.Text.RegularExpressions;
using Aspose.Common;
using Aspose.Words.Fields.Expressions;
using Aspose.Words.Properties;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the DOCPROPERTY field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves the indicated document information.
    /// </remarks>
    public class FieldDocProperty : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            if (!StringUtil.HasChars(PropertyName))
                return new FieldUpdateActionInsertErrorMessage(this, "Error! No property name supplied.");

            Document doc = FetchDocument();

            // WORDSNET-10809 First try to find the property in custom, then in built in document properties.
            DocumentProperty property = doc.CustomDocumentProperties[PropertyName];
            bool isCustomProperty = (property != null);
            if (!isCustomProperty)
                property = doc.BuiltInDocumentProperties[PropertyName];

            if (property == null)
                return new FieldUpdateActionInsertErrorMessage(this, "Error! Unknown document property name.");

            return new FieldUpdateActionApplyResult(this, GetUnformattedResult(property, isCustomProperty));
        }

        private Constant GetUnformattedResult(DocumentProperty property, bool isCustomProperty)
        {
            switch (property.Type)
            {
                case PropertyType.DateTime:
                    return new StringConstant(GetUnformattedDateTimeResult(property, isCustomProperty));
                case PropertyType.Double:
                    return new DoubleConstant(property.ToDouble());
                default:
                    return new StringConstant(property.ToString());
            }
        }

        /// <summary>
        /// Returns field result without formatting to be applied.
        /// </summary>
        /// <param name="property">document property</param>
        /// <param name="isCustomProperty">true if <paramref name="property"/> is a custom document property</param>
        /// <returns>field result without formatting to be applied</returns>
        private string GetUnformattedDateTimeResult(DocumentProperty property, bool isCustomProperty)
        {
            // WORDSNET-4290 Value of date/time document property is truncated to date during DocumentProperty.ToString call.
            // Also it is not converted to local time value. So we need to handle it separately.
            Debug.Assert(property.Type == PropertyType.DateTime);

            System.Globalization.CultureInfo preProcessCulture = FetchDocument().FieldOptions.PreProcessCulture;

            // MS Word always stores date/time document property value in UTC (truncated to minutes)
            // and converts it to local time at this point.
            DateTime value = DateTimeUtil.ToLocalTime(property.ToDateTime());

            // MS Word uses current locale short date pattern to represent date part of date/time
            // document property value in field result by default (when no picture switch is provided).
            // But it converts year format in its own fasion. As it is too complex to take in account all
            // cases of this convertation, we will do it only for most popular formats as follows:
            // "yyyy" -> "yyyy", "yy" -> "yyyy", "y" -> "yy".
            string dateFormat = FormatterPal.GetShortDatePatternForCulture(preProcessCulture);
            dateFormat = gYearFormatRegex.Replace(dateFormat, YearFormatReplacePattern);

            // MS Word truncates custom date/time document property value to date and displays only date part by default.
            if (isCustomProperty)
                return value.ToString(dateFormat);

            // MS Word uses first occurance of hour format in current locale long time pattern.
            // If it's not present then "H" is the default.
            string timeFormat = FormatterPal.GetLongTimePatternForCulture(preProcessCulture);
            Match match = gHourFormatRegex.Match(timeFormat);
            string hourFormat = match.Success ? match.Groups[HourFormatGroupIndex].Value : DefaultHourFormat;

            // MS Word uses first occurance of time separator in current locale long time pattern.
            // But if it's not present or its length is not equal to one then ":" is used instead.
            string timeSeparator = FormatterPal.GetTimeSeparatorForCulture(preProcessCulture);
            if ((timeSeparator == null) || (timeSeparator.Length != DefaultTimeSeparator.Length))
                timeSeparator = DefaultTimeSeparator;

            // MS Word uses pattern "<ShortDate> <Hour><TimeSeparator>mm" relative to current locale to represent
            // built-in date/time document property value by default (when no picture switch is provided).
            string dateTimeFormat = string.Format(DateTimeFormatFormat, dateFormat, hourFormat, timeSeparator);

            return value.ToString(dateTimeFormat);
        }

        /// <summary>
        /// Gets or sets the name of the document property to retrieve.
        /// </summary>
        internal string PropertyName
        {
            get { return FieldCodeCache.GetArgumentAsString(PropertyNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(PropertyNameArgumentIndex, value); }
        }

        /// <summary>
        /// Format of built-in date/time document property value default format. Single quotes are used to escape
        /// <see cref="DefaultTimeSeparator"/> which value has special meaning in .Net date/time formatting routines.
        /// Note that "\" can't be escaped that way, but we have no other options because of Java compatibility.
        /// MS Word always formats minutes with "mm" format in that case.
        /// </summary>
        private const string DateTimeFormatFormat = @"{0} {1}'{2}'mm";

        /// <summary>
        /// Default MS Word time separator.
        /// </summary>
        private const string DefaultTimeSeparator = ":";

        /// <summary>
        /// Default MS Word hour format.
        /// </summary>
        private const string DefaultHourFormat = "H";

        /// <summary>
        /// Index of hour format group in <see cref="gHourFormatRegex"/>.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int HourFormatGroupIndex = 2;

        /// <summary>
        /// Regular expression to extract hour format from time pattern.
        /// It bypasses hour format characters enclosed by single quotes which means they are not relative to hour format.
        /// </summary>
        private static readonly Regex gHourFormatRegex = new Regex(@"\A([^']*'[^']*')*?[^']*?((h|H)\3?)", RegexOptions.Compiled);

        /// <summary>
        /// Regular expression to convert year format according to MS Word.
        /// It bypasses year format characters enclosed by single quotes which means they are not relative to year format.
        /// </summary>
        private static readonly Regex gYearFormatRegex = new Regex(@"\G(([^']*'[^']*')*?[^']*?)y(y?)y*", RegexOptions.Compiled);

        /// <summary>
        /// Replace pattern for <see cref="gYearFormatRegex"/> regex.
        /// If year format is "y" then it is replaced by "yy", otherwise it is replaced by "yyyy".
        /// </summary>
        private const string YearFormatReplacePattern = "$1yy$3$3";

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int PropertyNameArgumentIndex = 0;
    }
}
