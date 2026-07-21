// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/09/2022 by Anton Savko

using System;
using System.Text;
using Aspose.Collections;

namespace Aspose.Words.RW.Html.Css
{
    internal class GeneratedContentListLabelInfoCreator : IPseudoElementContentPartVisitor
    {
        internal GeneratedContentListLabelInfoCreator(
            CssCounters counters,
            IHtmlElementProvider element,
            int skipListLevelCount,
            int startListLevelNumber)
        {
            Debug.Assert(counters != null);
            Debug.Assert(element != null);
            mCounters = counters;
            mElement = element;

            mSkipListLevelCount = skipListLevelCount;
            mCurrentListLevelNumber = startListLevelNumber;

            mNumberFormatBuilder = new StringBuilder();
            mLevelNumberToNumberValueMap = new IntToIntDictionary();
            mLevelNumberToNumberStyleMap = new IntToIntDictionary();
        }

        internal static GeneratedContentListLabelInfo GetListLabelInfo(
            PseudoElementContent content,
            CssCounters counters,
            IHtmlElementProvider element,
            int skipListLevelCount,
            int currentListLevelNumber)
        {
            GeneratedContentListLabelInfoCreator creator = new GeneratedContentListLabelInfoCreator(
                counters,
                element,
                skipListLevelCount,
                currentListLevelNumber);

            content.Accept(creator);

            return new GeneratedContentListLabelInfo(
                creator.mNumberFormatBuilder.ToString(),
                creator.mLevelNumberToNumberValueMap,
                creator.mLevelNumberToNumberStyleMap);
        }

        void IPseudoElementContentPartVisitor.VisitAttr(string attributeName)
        {
            string attributeValue = mElement.GetAttributeValue(attributeName, string.Empty);
            HandleText(attributeValue);
        }

        void IPseudoElementContentPartVisitor.VisitCounter(string counterName, NumberStyle counterStyle)
        {
            int numberValue = mCounters.GetValue(counterName);

            if (mSkipListLevelCount > 0)
            {
                HandleAsText(numberValue, counterStyle);
            }
            else
            {
                HandleAsListLevelNumber(numberValue, counterStyle);
            }
        }

        void IPseudoElementContentPartVisitor.VisitCounters(string counterName, string separator, NumberStyle counterStyle)
        {
            int[] numberValues = mCounters.GetAllValues(counterName);

            int skipListLevelCount = System.Math.Min(mSkipListLevelCount, numberValues.Length);

            for (int i = 0; i < skipListLevelCount; i++)
            {
                HandleAsText(numberValues[i], counterStyle);

                if (i != numberValues.Length - 1)
                {
                    mNumberFormatBuilder.Append(separator);
                }
            }

            for (int i = skipListLevelCount; i < numberValues.Length; i++)
            {
                HandleAsListLevelNumber(numberValues[i], counterStyle);

                if (i != numberValues.Length - 1)
                {
                    mNumberFormatBuilder.Append(separator);
                }
            }
        }

        void IPseudoElementContentPartVisitor.VisitText(string text)
        {
            HandleText(text);
        }

        private void HandleText(string text)
        {
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
                    // String is guaranteed to have only digits.
                    string numberValueString = text.Substring(numberOffset, numberLength);
                    int numberValue = Convert.ToInt32(numberValueString);

                    if (mSkipListLevelCount > 0)
                    {
                        HandleAsText(numberValue, NumberStyle.Arabic);
                    }
                    else
                    {
                        HandleAsListLevelNumber(numberValue, NumberStyle.Arabic);
                    }
                }

                while ((offset < text.Length) && !StringUtil.IsDigit(text[offset]))
                {
                    mNumberFormatBuilder.Append(text[offset]);
                    offset++;
                }
            }
        }

        private void HandleAsText(int numberValue, NumberStyle counterStyle)
        {
            string valueString = NumberConverter.NumberToString(numberValue, counterStyle, true);

            mNumberFormatBuilder.Append(valueString);

            mSkipListLevelCount--;
        }

        private void HandleAsListLevelNumber(int numberValue, NumberStyle counterStyle)
        {
            mNumberFormatBuilder.Append((char)mCurrentListLevelNumber);

            mLevelNumberToNumberValueMap.Add(mCurrentListLevelNumber, numberValue);
            mLevelNumberToNumberStyleMap.Add(mCurrentListLevelNumber, (int)counterStyle);

            mCurrentListLevelNumber++;
        }

        private readonly CssCounters mCounters;
        private readonly IHtmlElementProvider mElement;

        private int mSkipListLevelCount;
        private int mCurrentListLevelNumber;

        private readonly StringBuilder mNumberFormatBuilder;
        private readonly IntToIntDictionary mLevelNumberToNumberValueMap;
        private readonly IntToIntDictionary mLevelNumberToNumberStyleMap;
    }
}
