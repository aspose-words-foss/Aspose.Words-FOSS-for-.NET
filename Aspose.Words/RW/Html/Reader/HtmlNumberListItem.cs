// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/06/2019 by Anton Savko

using System;
using System.Text;
using Aspose.Collections;
using Aspose.Words.Lists;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Html.Css;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Represents list item with a number.
    /// </summary>
    internal class HtmlNumberListItem : HtmlListItemBase
    {
        private HtmlNumberListItem(
            HtmlListLevelId listLevelId,
            HtmlMarkerType markerType,
            string listLabelString,
            string numberFormat,
            IntToIntDictionary levelNumberToNumberStyleMap,
            IntToIntDictionary levelNumberToNumberValueMap)
            : base(listLevelId, markerType, listLabelString)
        {
            SetListLevelType(HtmlModelListLevelType.Number);
            SetNumberFormat(numberFormat);

            IntToIntDictionary.Enumerator numberStyleEnumerator = levelNumberToNumberStyleMap.GetEnumerator();
            while (numberStyleEnumerator.MoveNext())
            {
                NumberStyle numberStyle = (NumberStyle)numberStyleEnumerator.CurrentValue;
                SetNumberStyle(numberStyleEnumerator.CurrentKey, numberStyle);
            }

            IntToIntDictionary.Enumerator numberValueEnumerator = levelNumberToNumberValueMap.GetEnumerator();
            while (numberValueEnumerator.MoveNext())
            {
                SetNumberValue(numberValueEnumerator.CurrentKey, numberValueEnumerator.CurrentValue);
            }
        }

        /// <summary>
        /// Creates <see cref="HtmlNumberListItem"/> object from CSS generated content.
        /// </summary>
        internal static HtmlNumberListItem Create(
            DocumentFormatter documentFormatter,
            string generatedContent,
            int htmlListLevelNumber)
        {
            documentFormatter.SwitchToPart(HtmlElementPart.Before, false);
            int listLevelsCount = documentFormatter.GetListLevelCount();
            documentFormatter.SwitchToPart(HtmlElementPart.Element, false);

            if (listLevelsCount == 0)
            {
                return null;
            }

            HtmlListLevelId listLevelId = new HtmlListLevelId(htmlListLevelNumber);

            int nextListLevelNumber = listLevelId.ListLevelNumber + 1;
            int skipListLevelsCount = System.Math.Max(0, listLevelsCount - nextListLevelNumber);
            int startListLevelNumber = System.Math.Max(0, nextListLevelNumber - listLevelsCount);

            documentFormatter.SwitchToPart(HtmlElementPart.Before, false);

            GeneratedContentListLabelInfo listLabelInfo =
                documentFormatter.GetListLabelInfo(skipListLevelsCount, startListLevelNumber);

            documentFormatter.SwitchToPart(HtmlElementPart.Element, false);

            HtmlNumberListItem numberListItem = new HtmlNumberListItem(
                listLevelId,
                HtmlMarkerType.PseudoElement,
                generatedContent,
                listLabelInfo.NumberFormat,
                listLabelInfo.LevelNumberToNumberStyleMap,
                listLabelInfo.LevelNumberToNumberValueMap);

            return numberListItem;
        }

        /// <summary>
        /// Creates <see cref="HtmlNumberListItem"/> object from &lt;ol&gt; HTML element.
        /// </summary>
        internal static HtmlNumberListItem Create(
            int htmlListLevelNumber,
            int listLevelNumberValue,
            NumberStyle listLevelNumberStyle)
        {
            HtmlListLevelId listLevelId = new HtmlListLevelId(htmlListLevelNumber);

            string listLevelNumberString = NumberConverter.NumberToString(listLevelNumberValue, listLevelNumberStyle, true);
            string listLabelString = listLevelNumberString + ".";

            string numberFormat = new string((char)listLevelId.ListLevelNumber, 1) + ".";

            IntToIntDictionary levelNumberToNumberStyleMap = new IntToIntDictionary();
            levelNumberToNumberStyleMap.Add(listLevelId.ListLevelNumber, (int)listLevelNumberStyle);

            IntToIntDictionary levelNumberToNumberValueMap = new IntToIntDictionary();
            levelNumberToNumberValueMap.Add(listLevelId.ListLevelNumber, listLevelNumberValue);

            HtmlNumberListItem numberListItem = new HtmlNumberListItem(
                listLevelId,
                HtmlMarkerType.Html,
                listLabelString,
                numberFormat,
                levelNumberToNumberStyleMap,
                levelNumberToNumberValueMap);

            return numberListItem;
        }

        /// <summary>
        /// Creates <see cref="HtmlNumberListItem"/> object from -aw-* CSS properties.
        /// </summary>
        internal static HtmlNumberListItem Create(
            string listNumberFormatValue,
            string listNumberValuesValue,
            string listNumberStylesValue,
            int htmlListLevelNumber)
        {
            HtmlListLevelId listLevelId = new HtmlListLevelId(htmlListLevelNumber);
            string numberFormat = GetNumberFormat(listNumberFormatValue);

            string[] numberStyles = ParseValues(listNumberStylesValue);
            IntToIntDictionary levelNumberToNumberStyleMap = GetLevelNumberToNumberStyleMap(
                numberStyles,
                numberFormat,
                listLevelId.ListLevelNumber);

            string[] numberValues = ParseValues(listNumberValuesValue);
            IntToIntDictionary levelNumberToNumberValueMap = GetLevelNumberToNumberValueMap(
                numberValues,
                numberFormat,
                levelNumberToNumberStyleMap);

            string listLabelString =
                GetListLabelString(numberFormat, levelNumberToNumberStyleMap, levelNumberToNumberValueMap);

            HtmlNumberListItem numberListItem = new HtmlNumberListItem(
                listLevelId,
                HtmlMarkerType.Aw,
                listLabelString,
                numberFormat,
                levelNumberToNumberStyleMap,
                levelNumberToNumberValueMap);

            return numberListItem;
        }

        internal override void PostModifyList(HtmlModelList modelList)
        {
            HtmlModelListLevel modelListLevel = modelList.GetListLevel(ListLevelId.ListLevelNumber);
            RemoveFontAttributesForOrderedList(modelListLevel.ListLevel.RunPr);
        }

        private static string GetNumberFormat(string numberFormatValue)
        {
            StringBuilder builder = new StringBuilder(numberFormatValue);
            for (int i = ListLevel.MinLevel; i < ListLevel.MaxLevels; i++)
            {
                builder.Replace(HtmlUtil.GetListLevelNumberString(i), new string((char)i, 1));
            }

            return builder.ToString();
        }

        private static IntToIntDictionary GetLevelNumberToNumberStyleMap(
            string[] numberStyles,
            string numberFormat,
            int currentListLevelNumber)
        {
            IntToIntDictionary levelNumberToNumberStyleMap = new IntToIntDictionary();

            int numberIndex = 0;

            for (int i = ListLevel.MinLevel; i < ListLevel.MaxLevels; i++)
            {
                // Number style for current list level should be always specified.
                char levelNumberChar = (char)i;
                if ((numberFormat.IndexOf(levelNumberChar) != -1) || (i == currentListLevelNumber))
                {
                    if (numberIndex < numberStyles.Length)
                    {
                        levelNumberToNumberStyleMap.Add(i, (int)GetNumberStyle(numberStyles[numberIndex]));
                    }
                    else
                    {
                        levelNumberToNumberStyleMap.Add(i, (int)ListLevelNumberStyleDefault);
                    }

                    numberIndex++;
                }
            }

            return levelNumberToNumberStyleMap;
        }

        private static IntToIntDictionary GetLevelNumberToNumberValueMap(
            string[] numberValues,
            string numberFormat,
            IntToIntDictionary levelNumberToNumberStyleMap)
        {
            IntToIntDictionary levelNumberToNumberValueMap = new IntToIntDictionary();

            int numberIndex = 0;

            for (int i = ListLevel.MinLevel; i < ListLevel.MaxLevels; i++)
            {
                char levelNumberChar = (char)i;
                if ((numberFormat.IndexOf(levelNumberChar) != -1) &&
                    ListLabelUtil.HasCounter((NumberStyle)levelNumberToNumberStyleMap[i]))
                {
                    if (numberIndex < numberValues.Length)
                    {
                        levelNumberToNumberValueMap.Add(i, GetNumberValue(numberValues[numberIndex]));
                    }
                    else
                    {
                        levelNumberToNumberValueMap.Add(i, ListLevelNumberValueDefault);
                    }

                    numberIndex++;
                }
            }

            return levelNumberToNumberValueMap;
        }

        private static string[] ParseValues(string values)
        {
            return values.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static NumberStyle GetNumberStyle(string numberStyleValue)
        {
            return DocxEnum.DocxToNumberStyle(numberStyleValue);
        }

        private static int GetNumberValue(string numberValueValue)
        {
            int numberValue;
            try
            {
                numberValue = Convert.ToInt32(numberValueValue);
            }
            catch (FormatException)
            {
                numberValue = ListLevelNumberValueDefault;
            }
            catch (OverflowException)
            {
                numberValue = ListLevelNumberValueDefault;
            }

            return numberValue;
        }

        private static string GetListLabelString(
            string numberFormat,
            IntToIntDictionary levelNumberToNumberStyleMap,
            IntToIntDictionary levelNumberToNumberValueMap)
        {
            StringBuilder listLabelStringBuilder = new StringBuilder(numberFormat);

            for (int i = ListLevel.MinLevel; i < ListLevel.MaxLevels; i++)
            {
                char levelNumberChar = (char)i;
                if (numberFormat.IndexOf(levelNumberChar) != -1)
                {
                    NumberStyle numberStyle = (NumberStyle)levelNumberToNumberStyleMap[i];
                    int numberValue = levelNumberToNumberValueMap[i];

                    string numberString = NumberConverter.NumberToString(numberValue, numberStyle, true);

                    listLabelStringBuilder.Replace(levelNumberChar.ToString(), numberString);
                }
            }

            return listLabelStringBuilder.ToString();
        }

        private const int ListLevelNumberValueDefault = 1;
        private const NumberStyle ListLevelNumberStyleDefault = NumberStyle.Arabic;
    }
}
