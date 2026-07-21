// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2010 by Dmitry Vorobyev

using Aspose.Common;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a constant whose value is a floating point number.
    /// </summary>
    internal class DoubleConstant : Constant
    {
        internal DoubleConstant(double value)
        {
            mValue = value;
            string number = FormatterPal.DoubleToStr(value);
            int index = number.IndexOf('.');
            NumberOfDigitsAfterDecimalPoint = index != -1 ? number.Substring(index + 1).Length : 0;
        }

        private DoubleConstant(double value, bool isCurrency, bool isUseGroupSeparator, int numberOfDigitsAfterDecimalPoint, bool hasEndNonDigits)
        {
            mValue = value;
            IsCurrency = isCurrency;
            IsUsesGroupSeparator = isUseGroupSeparator;
            NumberOfDigitsAfterDecimalPoint = numberOfDigitsAfterDecimalPoint;
            HasEndNonDigits = hasEndNonDigits;
        }

        /// <summary>
        /// Creates a floating number constant from the specified value and attempts to copy style from value source.
        /// I need this because any constant in the expression that has currency format or decimal grouping causes
        /// the resulting value (if it is DoubleConstant as well) to be the same.
        /// </summary>
        internal static DoubleConstant CreateFrom(Constant source, double value)
        {
            // JAVA: manual collection initialization for autoportability.
            ConstantCollection col = new ConstantCollection();
            col.Add(source);
            return CreateFromInternal(col, value);
        }

        /// <summary>
        /// Creates a floating number constant from the specified value and attempts to copy style from value source.
        /// I need this because any constant in the expression that has currency format or decimal grouping causes
        /// the resulting value (if it is DoubleConstant as well) to be the same.
        /// </summary>
        internal static DoubleConstant CreateFrom(Constant source, double value, int numberOfDigitsAfterDecimalPoint)
        {
            // JAVA: manual collection initialization for autoportability.
            ConstantCollection col = new ConstantCollection();
            col.Add(source);
            return CreateFromInternal(col, value, numberOfDigitsAfterDecimalPoint);
        }

        /// <summary>
        /// Creates a floating number constant from the specified value and attempts to copy style from value source.
        /// I need this because any constant in the expression that has currency format or decimal grouping causes
        /// the resulting value (if it is DoubleConstant as well) to be the same.
        /// </summary>
        internal static DoubleConstant CreateFrom(Constant source1, Constant source2, double value)
        {
            // JAVA: manual collection initialization for autoportability.
            ConstantCollection col = new ConstantCollection();
            col.Add(source1);
            col.Add(source2);
            return CreateFromInternal(col, value);
        }

        /// <summary>
        /// Creates a floating number constant from the specified value and attempts to copy style from value source.
        /// I need this because any constant in the expression that has currency format or decimal grouping causes
        /// the resulting value (if it is DoubleConstant as well) to be the same.
        /// </summary>
        internal static DoubleConstant CreateFrom(Constant source1, Constant source2, double value, int numberOfDigitsAfterDecimalPoint)
        {
            // JAVA: manual collection initialization for autoportability.
            ConstantCollection col = new ConstantCollection();
            col.Add(source1);
            col.Add(source2);
            return CreateFromInternal(col, value, numberOfDigitsAfterDecimalPoint);
        }

        /// <summary>
        /// Creates a floating number constant from the specified value and attempts to copy style from value source.
        /// I need this because any constant in the expression that has currency format or decimal grouping causes
        /// the resulting value (if it is DoubleConstant as well) to be the same.
        /// </summary>
        internal static DoubleConstant CreateFrom(ConstantCollection sources, double value)
        {
            return CreateFromInternal(sources, value);
        }

        /// <summary>
        /// Creates a floating number constant from the specified value and attempts to copy style from value source.
        /// I need this because any constant in the expression that has currency format or decimal grouping causes
        /// the resulting value (if it is DoubleConstant as well) to be the same.
        /// </summary>
        internal static DoubleConstant CreateFrom(ConstantCollection sources, double value, int numberOfDigitsAfterDecimalPoint)
        {
            return CreateFromInternal(sources, value, numberOfDigitsAfterDecimalPoint);
        }

        private static DoubleConstant CreateFromInternal(ConstantCollection sources, double value)
        {
            return CreateFromInternal(sources, value, -1);
        }

        private static DoubleConstant CreateFromInternal(ConstantCollection sources, double value, int numberOfDigitsAfterDecimalPoint)
        {
            DoubleConstant constant = new DoubleConstant(value);
            if (numberOfDigitsAfterDecimalPoint > -1)
                constant.NumberOfDigitsAfterDecimalPoint = numberOfDigitsAfterDecimalPoint;

            foreach (Constant source in sources)
                constant.TryCopyStyleFrom(source);

            return constant;
        }

        private void TryCopyStyleFrom(Constant source)
        {
            if (source.ConstantType != ConstantType.Double)
                return;

            DoubleConstant sourceConstant = (DoubleConstant)source;

            if (sourceConstant.IsCurrency)
                IsCurrency = true;

            if (sourceConstant.IsUsesGroupSeparator)
                IsUsesGroupSeparator = true;

            if (sourceConstant.IsUsesNegativeParentheses)
                IsUsesNegativeParentheses = true;
        }

        internal static DoubleConstant TryParse(string s)
        {
            if (!StringUtil.HasChars(s))
                return null;

            bool isSignPresence = false;
            if (s.Length > 1 && (s[0] == '-'))
            {
                s = s.Substring(1, s.Length - 1);
                isSignPresence = true;
            }

            bool hasNumberGroupSeparator = s.IndexOf(FormatterPal.GetNumberGroupSeparatorCurrent()) != -1;

            bool isCurrency;
            s = RemoveCurrencySymbol(s, out isCurrency);

            int percentPosition = s.IndexOf('%');
            bool isPercentage = percentPosition != -1;
            double multiplier = isPercentage ? 0.01 : 1;

            bool hasEndNonDigits = CheckHasEndNonDigits(s);
            s = StringUtil.TrimEndNonDigits(s);

            if (isPercentage && s.Length != percentPosition)
                return null;

            int numberOfDigitsAfterDecimalPoint = GetNumberOfDigitsAfterDecimalPoint(s);
            if (isPercentage)
                numberOfDigitsAfterDecimalPoint += 2;

            if (!IsValidGroupSeparators(s))
                return null;

            double value = FormatterPal.TryParseCurrencyCurrent(s);

            // WORDSNET-5321 The numeric string can be in exponential notation. Need to check this out.
            if (double.IsNaN(value))
                value = FormatterPal.TryParseDoubleCurrent(s);

            bool isNegative = isSignPresence;
            if (!double.IsNaN(value))
                return new DoubleConstant(multiplier * (isNegative ? -value : value), isCurrency, hasNumberGroupSeparator, numberOfDigitsAfterDecimalPoint, hasEndNonDigits);

            return null;
        }

        private static bool CheckHasEndNonDigits(string input)
        {
            for (int i = input.Length - 1; i >= 0; i--)
            {
                char c = input[i];

                if (char.IsDigit(c))
                    return false;

                if (!char.IsWhiteSpace(c) && c != '%')
                    return true;
            }

            return false;
        }

        private static bool IsValidGroupSeparators(string value)
        {
            int groupSize = FormatterPal.GetNumberGroupSizeCurrent();
            if (groupSize == 0)
                return true;

            char groupSeparator = FormatterPal.GetNumberGroupSeparatorCurrent();
            char decimalSeparator = FormatterPal.GetDecimalSeparatorCurrent();

            int lastGroupSeparatorIndex = value.IndexOf(groupSeparator);
            if (lastGroupSeparatorIndex == -1)
                return true;

            while (true)
            {
                int currentGroupSeparatorIndex = value.IndexOf(groupSeparator, lastGroupSeparatorIndex + 1);
                if (currentGroupSeparatorIndex == -1)
                    break;

                if (currentGroupSeparatorIndex - lastGroupSeparatorIndex != groupSize + 1)
                    return false;

                lastGroupSeparatorIndex = currentGroupSeparatorIndex;
            }


            int decimalSeparatorIndex = value.IndexOf(decimalSeparator);
            if (decimalSeparatorIndex == -1)
                return true;

            if (decimalSeparatorIndex < lastGroupSeparatorIndex)
                return false;

            if (decimalSeparatorIndex - lastGroupSeparatorIndex != groupSize + 1)
                return false;

            return true;
        }

        private static int GetNumberOfDigitsAfterDecimalPoint(string s)
        {
            int decimalPointPosition = s.IndexOf(FormatterPal.GetDecimalSeparatorCurrent());
            if (decimalPointPosition == -1)
                return 0;

            return s.Length - decimalPointPosition - 1;
        }

        private static string RemoveCurrencySymbol(string s, out bool isCurrency)
        {
            // WORDSNET-3679 For "fr-CH" culture might be set either of the two currency symbols - "SFr." or (optional) "fr.".
            // WORDSNET-17864 For "fr-CH" culture the currency symbol is "CHF" in .NET 4.0 and above.
            s = FormatterPal.NormalizeCurrencySymbols(s);

            // Remove currency symbol of current culture
            string currencySymbolCurrent = FormatterPal.GetCurrencySymbolCurrent();
            string afterReplace = StringUtil.RemoveSubstring(s, currencySymbolCurrent);
            if (afterReplace != s)
            {
                isCurrency = true;
                return afterReplace;
            }

            // Remove currency symbols of known cultures
            isCurrency = false;
            foreach (string currency in gCurrencies)
            {
                afterReplace = StringUtil.RemoveSubstring(s, currency);
                if (afterReplace != s)
                {
                    return afterReplace;
                }
            }

            return s;
        }

        internal static FieldFormattingResult TryFormatNumber(double value, RichString format, Field field)
        {
            Debug.Assert(!RichStringBehaviour.IsNullOrEmptyInternal(format));

            FieldOptions fieldOptions = field.FetchDocument().FieldOptions;
            NumberFormattingOptions options = fieldOptions.GetNumberFormattingOptions();
            IFieldResultFormatter resultFormatter = fieldOptions.ResultFormatter;

            if (resultFormatter != null)
            {
                string plainTextResult = resultFormatter.FormatNumeric(value, format.ToSystemString());
                if (plainTextResult != null)
                {
                    return new FieldFormattingResult(
                        RichString.CreateFromString(plainTextResult),
                        false);
                }
            }

            IString richResult = FormatterPal.NumberToStrMSWord(
                value,
                format,
                options,
                RichStringBehaviour.Instance);

            bool legacyNumberFormat = NumberFormattingOptionsUtil.HasLegacyNumberFormat(options);
            return new FieldFormattingResult((RichString)richResult, !legacyNumberFormat);
        }

        internal override FieldFormattingResult TryFormatNumber(RichString format, Field field)
        {
            return TryFormatNumber(mValue, format, field);
        }

        internal override bool TryConvertToDouble(out double value)
        {
            value = mValue;
            return true;
        }

        internal override double ValueDouble
        {
            get { return mValue; }
        }

        internal override bool ValueBoolean
        {
            get { return mValue != 0d; }
        }

        internal override string ValueString
        {
            get
            {
                return FormatterPal.NumberToStrMSWordWithNoFormat(
                    mValue,
                    IsCurrency,
                    IsUsesGroupSeparator,
                    NumberOfDigitsAfterDecimalPoint,
                    IsUsesNegativeParentheses);
            }
        }

        internal override ConstantType ConstantType
        {
            get { return ConstantType.Double; }
        }

        internal bool IsCurrency { get; private set; }

        internal bool IsUsesGroupSeparator { get; private set; }

        internal bool IsUsesNegativeParentheses { get; set; }

        internal int NumberOfDigitsAfterDecimalPoint { get; private set; }

        internal bool HasEndNonDigits { get; }

        private readonly double mValue;

        private static readonly string[] gCurrencies = { "$", "€", "¥", "£", "CHF", "C$", "kr", "HK$", "р", "₽" };
    }
}
