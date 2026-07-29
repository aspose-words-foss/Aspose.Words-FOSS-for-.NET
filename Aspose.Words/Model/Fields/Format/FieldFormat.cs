// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/08/2009 by Dmitry Vorobyev

using System;
using Aspose.Numbering;
using Aspose.Words.Fields.Expressions;
using Aspose.Words.Lists;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides typed access to field's numeric, date and time, and general formatting.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    public class FieldFormat
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldFormat(Field field)
        {
            mField = field;
        }

        /// <summary>
        /// Formats the calculated result based on the field format switches. Returns true if any formatting was applied.
        /// </summary>
        internal bool FormatResult(Constant value, out FieldFormattingResult formattingResult)
        {
            RichString result;
            bool preserveRichFormatting = false;
            bool isFormattedAsNumber = TryConvertNumber(value, out result) ||
                                       TryFormatNumber(value, out result, out preserveRichFormatting) ||
                                       TryFormatPageNumber(value, out result);

            bool isFormattedAsDateTime = false;
            if (!isFormattedAsNumber)
            {
                isFormattedAsDateTime = TryFormatDateTime(value, out result);

                // WORDSNET-7807 Try to applay numeric format to DateTime value.
                if (isFormattedAsDateTime)
                    result = TryApplyNumericFormatToDateTime(result);
            }

            bool isFormattedAsString = TryFormatString(result, out result);

            formattingResult = result != null
                ? new FieldFormattingResult(result, preserveRichFormatting)
                : null;
            return isFormattedAsNumber || isFormattedAsDateTime || isFormattedAsString;
        }

        /// <summary>
        /// Attempts to apply date/time formatting to the specified constant based on the current update context.
        /// Returns true if the formatting was applied.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool TryFormatDateTime(Constant value, out RichString result)
        {
            result = RichString.CreateFromString(value.ValueString);

            string format = DateTimeFormat;
            bool isDateTimeFormatSpecified = format != null;

            if (!isDateTimeFormatSpecified)
                format = mField.GetDefaultDateTimeFormat();

            if (format == null)
                return false;

            IFieldResultFormatter fieldResultFormatter = isDateTimeFormatSpecified ? mField.FetchDocument().FieldOptions.ResultFormatter : null;
            string formattedResult = value.TryFormatDateTime(format, mField.FieldCodeCache.LanguageIdFarEast, fieldResultFormatter);
            if (formattedResult != null)
            {
                result = RichString.CreateFromString(formattedResult);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to apply numeric formatting to the DateTime value.
        /// Returns formatted string or original string if Numeric format wasn't applied.
        /// </summary>
        /// <param name="value">Converted DateTime value.</param>
        /// <returns></returns>
        private RichString TryApplyNumericFormatToDateTime(RichString value)
        {
            if (RichStringBehaviour.IsNullOrEmptyInternal(value))
                return value;

            string numericFormat = GeneralFormats.GetNumericFormatString();
            if (StringUtil.HasChars(numericFormat))
            {
                RichString numericFormattedResult;
                bool isConverted = TryConvertNumber(new StringConstant(value.ToSystemString()), out numericFormattedResult);

                if (isConverted)
                    return numericFormattedResult;

            }

            return value;
        }

        /// <summary>
        /// Attempts to apply numeric formatting to the specified string based on the current update context.
        /// Returns true if the formatting was applied.
        /// </summary>
        private bool TryFormatNumber(Constant value, out RichString result, out bool preserveRichFormatting)
        {
            result = RichString.CreateFromString(value.ValueString);
            preserveRichFormatting = false;

            RichString format = RichNumericFormat;

            if (RichStringBehaviour.IsNullOrEmptyInternal(format))
                return false;

            format = format.TrimFormat(FontAttr.RsidR, FontAttr.RsidRPr);

            FieldFormattingResult formattedResult = value.TryFormatNumber(format, mField);
            if (formattedResult != null)
            {
                result = formattedResult.Text;
                preserveRichFormatting = formattedResult.PreserveRichFormatting;
                return true;
            }

            return false;
        }

        private bool TryConvertNumber(Constant value, out RichString result)
        {
            result = null;

            double doubleValue;
            if (!value.TryConvertToDouble(out doubleValue))
                return false;

            long intValue = (long)doubleValue;

            // Simply ignore formatting switch if the number is invalid. Word displays
            // 'Error! Number cannot be represented in specified format.' message in this case.
            // WORDSNET-21047 MS Word does not allow numbers over 999999, but we DO.
            if (intValue < 0)
                return false;

            GeneralFormat format = GeneralFormats.GetNumericFormat();
            if (format != GeneralFormat.None)
            {
                IFieldResultFormatter resultFormatter = mField.FetchDocument().FieldOptions.ResultFormatter;
                if (resultFormatter != null)
                {
                    string formatted = resultFormatter.Format(doubleValue, format);
                    result = RichString.CreateFromString(formatted);
                    if (result != null)
                        return true;
                }

                switch (format)
                {
                    case GeneralFormat.DBChar:
                    case GeneralFormat.SBChar:
                        return false;
                    case GeneralFormat.DollarText:
                        result = RichString.CreateFromString(TextualNumber.ToDollarText(doubleValue));
                        break;
                    default:
                        result = RichString.CreateFromString(NumberConverter.NumberToLocalizedString(intValue, GeneralFormatUtil.GeneralFormatToNumberStyle(format), false));
                        break;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to format a number using a page number format taken from a corresponding section.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool TryFormatPageNumber(Constant value, out RichString result)
        {
            result = null;

            double doubleValue;
            if (!value.TryConvertToDouble(out doubleValue))
                return false;

            Section pageNumberFormatSection = mField.GetPageNumberFormatSection();
            if (pageNumberFormatSection != null)
            {
                result = RichString.CreateFromString(NumberConverter.NumberToString(
                    (int)doubleValue,
                    pageNumberFormatSection.SectPr.PageNumberStyleFinal,
                    false));

                // Get a chapter number and include it to result, if it is required. Note, that MS Word does not include
                // an empty chapter number (even if the corresponding chapter title paragraph is found).
                string chapterNumber = GetChapterNumber(pageNumberFormatSection);
                if (StringUtil.HasChars(chapterNumber))
                {
                    char separator = GetChapterPageSeparatorChar(pageNumberFormatSection.SectPr.ChapterPageSeparatorFinal);

                    result = new RichStringBuilder(chapterNumber.Length + 1 + result.Length)
                        .Append(chapterNumber, new RunPr())
                        .Append(separator, new RunPr())
                        .AppendInternal(result)
                        .ToRichString();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a chapter number string for the given field in the context of the specified format section.
        /// </summary>
        private string GetChapterNumber(Section formatSection)
        {
            // If a chapter number should not be added to a page number, simply return null.
            if (formatSection.SectPr.HeadingLevelForChapterFinal == 0)
                return null;

            // Find the corresponding chapter title paragraph and if it is found, use it to generate a chapter number to return.
            Paragraph chapterTitleParagraph = ChapterTitleParagraphFinder.FindChapterTitleParagraph(mField, formatSection);

            return (chapterTitleParagraph != null)
                ? ListLabelUtil.BuildListLabel(chapterTitleParagraph.ListLabel.NumberStateFinal, new FieldFormatListLabelBuildBehaviour())
                : null;
        }

        /// <summary>
        /// Returns a character corresponding to the specified <see cref="ChapterPageSeparator"/> value.
        /// </summary>
        private static char GetChapterPageSeparatorChar(ChapterPageSeparator separator)
        {
            switch (separator)
            {
                case ChapterPageSeparator.Hyphen:
                    return '-';
                case ChapterPageSeparator.Period:
                    return '.';
                case ChapterPageSeparator.Colon:
                    return ':';
                case ChapterPageSeparator.EmDash:
                    return '—';
                case ChapterPageSeparator.EnDash:
                    return '–';
                default:
                    throw new ArgumentOutOfRangeException("separator");
            }
        }

        /// <summary>
        /// Attempts to apply general formatting to the specified string based on the current update context.
        /// Returns true if the formatting was applied.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool TryFormatString(RichString value, out RichString result)
        {
            result = value;
            bool isFormatted = false;

            IFieldResultFormatter resultFormatter = mField.FetchDocument().FieldOptions.ResultFormatter;
            foreach (GeneralFormat format in GeneralFormats)
            {
                CharCase charCase = GeneralFormatUtil.GeneralFormatToCharCase(format);
                if (charCase != CharCase.Default)
                {
                    result = FormatString(result, format, charCase, resultFormatter);
                    isFormatted = true;
                }
            }

            return isFormatted;
        }

        private static RichString FormatString(RichString value, GeneralFormat format, CharCase charCase, IFieldResultFormatter resultFormatter)
        {
            if (resultFormatter != null)
            {
                string result = resultFormatter.Format(value.ToSystemString(), format);
                if (result != null)
                    return RichString.CreateFromString(result);
            }

            return RichStringUtils.FormatCharCase(value, charCase);
        }

        /// <summary>
        /// Returns an implementation of a field result format provider.
        /// </summary>
        /// <returns></returns>
        internal IFieldResultFormatProvider GetFieldResultFormatProvider()
        {
            CompositeFieldResultFormatProvider provider = new CompositeFieldResultFormatProvider();

            // WORDSNET-14698 MS Word applies result formats in order they are specified in the field code.
            // Applying particular format twice is redundant. That is why we iterate formats in backward direction
            // and use only latest instance of particular format.
            for (int i = GeneralFormats.Count - 1; i >= 0; i--)
            {
                switch (GeneralFormats[i])
                {
                    case GeneralFormat.CharFormat:
                        // CHARFORMAT - take formatting from the first meaningful run of field code.
                        provider.InsertProvider(new CharFormatProvider(mField));
                        break;
                    case GeneralFormat.MergeFormat:
                        // MERGEFORMAT - at the moment we simply take formatting from the first meaningful run of
                        // field result, but it seems like there is a more sophisticated algorithm. To be investigated.
                        provider.InsertProvider(new MergeFormatProvider(mField));
                        break;
                    default:
                        break;
                }
            }

            if (provider.IsEmpty)
            {
                IFieldResultFormatProvider fieldProvider = mField as IFieldResultFormatProvider;
                if (fieldProvider != null)
                {
                    provider.AppendProvider(fieldProvider);
                }
                else if (!FieldUtil.RetainOriginalFormatting(mField) && string.IsNullOrEmpty(NumericFormat))
                {
                    provider.AppendProvider(new CharFormatProvider(mField));
                }
            }

            // WORDSNET-1586 Apply noproof attribute to mergefield field result.
            if (FieldUtil.DisableResultSpellChecking(mField.Type))
                provider.AppendProvider(NoProofingFormatProvider.Instance);

            return provider;
        }

        private FieldCode FieldCode
        {
            get { return mField.FieldCodeCache; }
        }

        /// <summary>
        /// Gets or sets a formatting that is applied to a numeric field result. Corresponds to the \# switch.
        /// </summary>
        public string NumericFormat
        {
            get { return FieldCode.GetSwitchArgumentAsString(NumericFormatSwitch); }
            set { FieldCode.SetSwitch(NumericFormatSwitch, value); }
        }

        internal RichString RichNumericFormat
        {
            get { return FieldCode.GetSwitchArgumentAsRichString(NumericFormatSwitch); }
        }

        /// <summary>
        /// Gets or sets a formatting that is applied to a date and time field result. Corresponds to the \@ switch.
        /// </summary>
        public string DateTimeFormat
        {
            get { return FieldCode.GetSwitchArgumentAsString(DateTimeFormatSwitch); }
            set { FieldCode.SetSwitch(DateTimeFormatSwitch, value); }
        }

        /// <summary>
        /// Gets a collection of general formats that are applied to a numeric, text or any field result.
        /// Corresponds to the \* switches.
        /// </summary>
        public GeneralFormatCollection GeneralFormats
        {
            get { return mGeneralFormats ?? (mGeneralFormats = new GeneralFormatCollection(FieldCode)); }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Field mField;
        private GeneralFormatCollection mGeneralFormats;

        internal const string NumericFormatSwitch = "\\#";
        internal const string DateTimeFormatSwitch = "\\@";
        internal const string GeneralFormatSwitch = "\\*";

        private class FieldFormatListLabelBuildBehaviour : IListLabelBuildBehaviour
        {
            public void NotifyListNumberAppended(int listLabelLength)
            {
                mListLabelLengthAtLastListNumber = listLabelLength;
            }

            public bool ShouldAppendNotListNumberChar(char c)
            {
                return mListLabelLengthAtLastListNumber != 0;
            }

            public int FinalizeListLabelLength(int listLabelLength)
            {
                return mListLabelLengthAtLastListNumber;
            }

            public NumberStyle NormalizeNumberStyle(NumberStyle numberStyle)
            {
                switch (numberStyle)
                {
                    case NumberStyle.Number:
                    case NumberStyle.Ordinal:
                    case NumberStyle.OrdinalText:
                        return NumberStyle.Arabic;
                    default:
                        return numberStyle;
                }
            }

            private int mListLabelLengthAtLastListNumber;
        }
    }
}
