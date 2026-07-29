// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/09/2022 by Anton Savko

namespace Aspose.Words.RW.Html.Css
{
    internal class GeneratedContentListLevelCounter : IPseudoElementContentPartVisitor
    {
        private GeneratedContentListLevelCounter(CssCounters counters, IHtmlElementProvider element)
        {
            Debug.Assert(counters != null);
            Debug.Assert(element != null);

            mCounters = counters;
            mElement = element;
            mListLevelCount = 0;
        }

        internal static int GetListLevelCount(
            PseudoElementContent content,
            CssCounters counters,
            IHtmlElementProvider element)
        {
            GeneratedContentListLevelCounter counter = new GeneratedContentListLevelCounter(counters, element);
            content.Accept(counter);
            return counter.mListLevelCount;
        }

        void IPseudoElementContentPartVisitor.VisitAttr(string attributeName)
        {
            string attributeValue = mElement.GetAttributeValue(attributeName, string.Empty);
            HandleText(attributeValue);
        }

        void IPseudoElementContentPartVisitor.VisitCounter(string counterName, NumberStyle counterStyle)
        {
            mListLevelCount++;
        }

        void IPseudoElementContentPartVisitor.VisitCounters(string counterName, string separator, NumberStyle counterStyle)
        {
            int[] numberValues = mCounters.GetAllValues(counterName);
            mListLevelCount += numberValues.Length;
        }

        void IPseudoElementContentPartVisitor.VisitText(string text)
        {
            HandleText(text);
        }

        private void HandleText(string text)
        {
            int listLevelCount = 0;

            int offset = 0;
            while (offset < text.Length)
            {
                int numberOffset = offset;
                while ((offset < text.Length) && StringUtil.IsDigit(text[offset]))
                {
                    offset++;
                }

                int numberLength = offset - numberOffset;
                if (numberLength > 0)
                {
                    listLevelCount++;
                }

                while ((offset < text.Length) && !StringUtil.IsDigit(text[offset]))
                {
                    offset++;
                }
            }

            mListLevelCount += listLevelCount;
        }

        private readonly CssCounters mCounters;
        private readonly IHtmlElementProvider mElement;
        private int mListLevelCount;
    }
}
