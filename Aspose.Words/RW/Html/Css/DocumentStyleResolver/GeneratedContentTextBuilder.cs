// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/01/2016 by Anton Savko

using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Builds a textual representation of generated content.
    /// </summary>
    internal class GeneratedContentTextBuilder : IPseudoElementContentPartVisitor
    {
        private GeneratedContentTextBuilder(CssCounters counters, IHtmlElementProvider element)
        {
            Debug.Assert(counters != null);
            Debug.Assert(element != null);

            mCounters = counters;
            mElement = element;
            mText = new StringBuilder();
        }

        /// <summary>
        /// Gets a textual representation of generated content.
        /// </summary>
        /// <returns>
        /// Text that represents this content part. The text may be empty but is never <c>null</c>.
        /// </returns>
        internal static string GetText(PseudoElementContent content, CssCounters counters, IHtmlElementProvider element)
        {
            GeneratedContentTextBuilder builder = new GeneratedContentTextBuilder(counters, element);
            content.Accept(builder);
            return builder.mText.ToString();
        }

        void IPseudoElementContentPartVisitor.VisitAttr(string attributeName)
        {
            string attributeValue = mElement.GetAttributeValue(attributeName, string.Empty);
            mText.Append(attributeValue);
        }

        void IPseudoElementContentPartVisitor.VisitCounter(string counterName, NumberStyle counterStyle)
        {
            int numberValue = mCounters.GetValue(counterName);

            string valueString = NumberConverter.NumberToString(numberValue, counterStyle, true);
            mText.Append(valueString);
        }

        void IPseudoElementContentPartVisitor.VisitCounters(string counterName, string separator, NumberStyle counterStyle)
        {
            int[] numberValues = mCounters.GetAllValues(counterName);

            for (int i = 0; i < numberValues.Length; i++)
            {
                string valueString = NumberConverter.NumberToString(numberValues[i], counterStyle, true);
                mText.Append(valueString);

                if (i != numberValues.Length - 1)
                {
                    mText.Append(separator);
                }
            }
        }

        void IPseudoElementContentPartVisitor.VisitText(string text)
        {
            mText.Append(text);
        }

        private readonly IHtmlElementProvider mElement;
        private readonly CssCounters mCounters;
        private readonly StringBuilder mText;
    }
}
