// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System;
using System.Collections.Generic;
using Aspose.Common;
using Aspose.Drawing.Fonts;
using Aspose.Words.Drawing;
using Aspose.Words.Lists;
using Aspose.Words.RW.Html.Css;
using Aspose.Words.RW.Html.HtmlList;
using Aspose.Words.RW.Html.Parser;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Responsible for importing HTML lists.
    /// </summary>
    internal class HtmlListReader
    {
        internal HtmlListReader(
            DocumentBuilder builder,
            DocumentFormatter documentFormatter,
            HtmlResourceLoader htmlResourceLoader,
            string baseUri,
            double defaultWholeListLeftIndent,
            bool adjustMarkerPositions)
        {
            Debug.Assert(builder != null);
            Debug.Assert(documentFormatter != null);
            Debug.Assert(htmlResourceLoader != null);

            mBuilder = builder;
            mDocumentFormatter = documentFormatter;
            mHtmlResourceLoader = htmlResourceLoader;
            mBaseUri = baseUri;

            mPictureBulletLoader = new HtmlPictureBulletLoader(mBuilder.Document, mHtmlResourceLoader, mBaseUri);
            mListLevelInfos = new Stack<HtmlListLevelInfo>();
            mAdjustMarkerPositions = adjustMarkerPositions;
            mDefaultWholeListLeftIndent = defaultWholeListLeftIndent;
            mOpenedListItemCount = 0;

            mModelListManager = new HtmlModelListManager(mBuilder.Document);
            mIsLastListItemParagraphListItem = false;
        }

        internal void BeginLevel(string tag)
        {
            BeginLevel(tag, false);
        }

        internal void EndLevel()
        {
            if (mListLevelInfos.Count > 0)
            {
                mListLevelInfos.Pop();
            }

            if (mListLevelInfos.Count == 0)
            {
                // Reset the value so it cannot accidentally be used from outside the list.
                mWholeListLeftIndent = 0;

                if (!mIsLastListItemParagraphListItem)
                {
                    mModelListManager.DeleteAllLists();
                }
            }
        }

        internal void BeginItem(HtmlElementNode node)
        {
            Debug.Assert(mOpenedListItemCount >= 0);

            // WORDSNET-23569, WORDSNET-10611 Start a new list for an orphan list item.
            if (mListLevelInfos.Count == 0)
            {
                BeginLevel("ul", true);
            }

            HandleListItemElement(node);
            mOpenedListItemCount++;
        }

        internal void EndItem()
        {
            Debug.Assert(mOpenedListItemCount > 0);

            // Note that the item may already be closed here. For example, if it belongs to a nested level.
            mItem = null;
            mOpenedListItemCount--;

            // WORDSNET-14529 List items can be nested if they are set via nested elements
            // (for example, &lt;span&gt;) with 'display: list-item' style.
            // So we should close implicit lists only after last opened list item is closed.
            if ((mOpenedListItemCount == 0) && CurrentListLevelInfo.IsImplicit)
            {
                EndLevel();
            }
        }

        internal void IndentCurrentParagraphAsListItemIfNeeded()
        {
            if ((mItem == null) || (mBuilder.CurrentParagraph != mItem.Paragraph))
            {
                // The current paragraph is not a list item.
                return;
            }

            HtmlListLevelInfo currentListLevelInfo = CurrentListLevelInfo;
            ListLevel listLevel = currentListLevelInfo.CurrentListLevel;

            CssLength leftIndent = GetCurrentElementLeftIndent();
            CssLength textIndent = mDocumentFormatter.ElementDeclarations.GetCssLength("text-indent");

            CssLength firstLineIndent = mItem.GetFirstLineIndent(leftIndent, textIndent);

            if (mAdjustMarkerPositions ||
                leftIndent.IsCustom ||
                firstLineIndent.IsCustom ||
                (!MathUtil.IsZero(mWholeListLeftIndent)))
            {
                mBuilder.ParagraphFormat.LeftIndent = mItem.GetLeftIndent(leftIndent, textIndent).Value;
                mBuilder.ParagraphFormat.FirstLineIndent = firstLineIndent.Value;

                CssLength tabStop = mItem.GetTabStop(leftIndent, textIndent, listLevel);
                mBuilder.ParagraphFormat.TabStops.Clear();
                if (tabStop != null)
                {
                    TabAlignment tabAlignment = mDocumentFormatter.IsBlockRtl()
                        ? TabAlignment.Right
                        : TabAlignment.Left;
                    mBuilder.ParagraphFormat.TabStops.Add(tabStop.Value, tabAlignment, TabLeader.None);
                }

                if (listLevel != null)
                {
                    listLevel.ParaPr.TabStops.Clear();
                }
            }
        }

        internal bool IsAtStartOfListItem
        {
            get
            {
                return (mItem != null) &&
                    (mBuilder.CurrentParagraph == mItem.Paragraph) &&
                    mBuilder.IsAtStartOfParagraph;
            }
        }

        internal bool IsListItemNoneListStyle
        {
            get { return (mItem != null) && mItem.IsListItemNoneListStyle; }
        }

        internal int CurrentLevelNumber
        {
            get
            {
                HtmlListLevelInfo listLevelInfo = CurrentListLevelInfo;
                return (listLevelInfo != null) ? listLevelInfo.ListLevelNumber : -1;
            }
        }

        internal ListLevel CurrentListLevel
        {
            get
            {
                HtmlListLevelInfo listLevelInfo = CurrentListLevelInfo;
                return (listLevelInfo != null) ? listLevelInfo.CurrentListLevel : null;
            }
        }

        internal bool CurrentListHasListItems
        {
            get
            {
                if (CurrentListLevelInfo == null)
                    return false;

                return CurrentListLevelInfo.ListItemCount > 0;
            }
        }

        private CssLength GetCurrentElementLeftIndent()
        {
            return mDocumentFormatter.IsBlockRtl()
                ? mDocumentFormatter.BoxModel.Right
                : mDocumentFormatter.BoxModel.Left;
        }

        private void BeginLevel(string tag, bool isImplicit)
        {
            // A new level ends the current list item.
            mItem = null;

            int levelNumber = CurrentLevelNumber + 1;
            HtmlListLevelInfo newListLevelInfo = new HtmlListLevelInfo(levelNumber, isImplicit);

            newListLevelInfo.ListTemplate = GetListTemplate(tag, levelNumber);
            newListLevelInfo.DefaultListItemMarker = HtmlListItemMarker.GetFromListTag(tag, levelNumber);

            newListLevelInfo.IsNoneListStyle =
                mDocumentFormatter.ElementDeclarations.ContainsIdentifier("list-style-type", "none");

            int startValue = 1;
            string startAttribute = mDocumentFormatter.CurrentElement.GetAttributeValue("start");
            if (startAttribute != null)
            {
                int parsedStartValue = FormatterPal.TryParseInt(startAttribute);
                if (parsedStartValue >= 0)
                {
                    startValue = parsedStartValue;
                }
            }
            newListLevelInfo.StartValue = startValue;

            // A left indent value from the document builder is applied at the top-level of a list so all its sublevels
            // get indented as well.
            if (mListLevelInfos.Count == 0)
            {
                CalculateWholeListLeftIndent();
            }

            mListLevelInfos.Push(newListLevelInfo);
        }

        private void CalculateWholeListLeftIndent()
        {
            // We only apply left indent value from the document builder to list elements only if elements don't have
            // custom left position.

            // We cannot just use the current 'left' value returned by our CSS box model, because it also includes the padding
            // of the list element. But in HTML left padding on list elements is used to offset list items from other
            // paragraphs and is more like a characteristic of the list itself, other than a means of positioning list items
            // relative to the left side of the document. As a result, left padding on list elements shouldn't be taken
            // into account when deciding whether a list has custom (non-default) left indent.

            // The following code actually subtracts left padding of the list element from the 'left' value returned
            // by the CSS box model.

            bool isRtl = mDocumentFormatter.IsBlockRtl();

            bool parentBoxHasCustomLeft = isRtl
                ? mDocumentFormatter.BoxModel.HasCustomParentRight()
                : mDocumentFormatter.BoxModel.HasCustomParentLeft();

            string leftMarginProperty = isRtl
                ? "margin-right"
                : "margin-left";
            bool listHasCustomLeftMargin = mDocumentFormatter.ElementDeclarations[leftMarginProperty] != null;

            mWholeListLeftIndent = (parentBoxHasCustomLeft || listHasCustomLeftMargin)
                ? 0
                : mDefaultWholeListLeftIndent;
        }

        private void HandleListItemElement(HtmlElementNode node)
        {
            HtmlListLevelInfo listLevelInfo = CurrentListLevelInfo;

            HtmlListItemMarker listItemMarker = GetListItemMarker();
            if (listItemMarker == null)
                listItemMarker = listLevelInfo.DefaultListItemMarker;

            string imageUrl = mDocumentFormatter.ElementDeclarations.GetUri("list-style-image");
            bool isListItemNoneListStyle = mDocumentFormatter.ElementDeclarations.ContainsIdentifier(
                "list-style-type", "none");

            string listItemValueAttribute = mDocumentFormatter.CurrentElement.GetAttributeValue("value", "-1");
            int listItemValue = FormatterPal.TryParseInt(listItemValueAttribute);
            // WORDSNET-20283 Ignore "start at" values that are not accepted by MS Word. This is what MS Word does when
            // it loads HTML.
            if (ListLevel.IsStartAtValid(listItemValue))
            {
                listLevelInfo.StartValue = listItemValue;
                listLevelInfo.ListItemCount = 0;
            }

            bool isParagraphListItem = HtmlUtil.IsParagraphOrHeadingElement(mDocumentFormatter.CurrentElement.ElementName);
            mIsLastListItemParagraphListItem = isParagraphListItem;

            mDocumentFormatter.SwitchToPart(HtmlElementPart.Before, false);
            string generatedContent = mDocumentFormatter.GetGeneratedContent();
            mDocumentFormatter.SwitchToPart(HtmlElementPart.Element, false);

            // WORDSNET-11999 Import list item with 'list-style-type:none' style as simple paragraph.
            // WORDSNET-17520 Import list item with empty generated content as simple paragraph.
            bool isImportListItemAsParagraph =
                isListItemNoneListStyle &&
                !isParagraphListItem &&
                !StringUtil.HasChars(imageUrl) &&
                !StringUtil.HasChars(generatedContent);

            bool isPseudoElementListItem = false;

            HtmlListItemBase htmlListItem = null;
            if (!isImportListItemAsParagraph)
            {
                htmlListItem = GetHtmlListItem(
                    listLevelInfo,
                    listItemMarker,
                    imageUrl,
                    generatedContent,
                    isListItemNoneListStyle,
                    isParagraphListItem);
                HtmlModelList htmlModelList = mModelListManager.AddListItem(listLevelInfo.ListTemplate, htmlListItem);

                isPseudoElementListItem = htmlListItem.IsPseudoElement;

                int listLevelNumber = htmlListItem.ListLevelId.ListLevelNumber;

                mBuilder.ListFormat.List = htmlModelList.List;
                mBuilder.ListFormat.ListLevelNumber = listLevelNumber;

                ListLevel listLevel = htmlModelList.GetListLevel(listLevelNumber).ListLevel;
                listLevelInfo.CurrentListLevel = listLevel;

                // Implicit lists that contain orphan list items (<li> elements that have no parent <ol>/<ul> elements) have
                // zero default indentation.
                if (listLevelInfo.IsImplicit)
                {
                    listLevel.TextPosition = 0;
                    listLevel.NumberPosition = 0;
                }

                if (isParagraphListItem)
                    ApplyListLabelFormatting();
            }

            listLevelInfo.ListItemMarker = listItemMarker;
            listLevelInfo.ListItemCount++;

            if (!isImportListItemAsParagraph && mAdjustMarkerPositions)
            {
                // WORDSNET-12958 List numbers are not aligned in output Html/Doc/Docx.
                listLevelInfo.CurrentListLevel.Alignment = ListLevelAlignment.Left;
            }

            double markerTextWidth = !isImportListItemAsParagraph
                ? GetMarkerTextWidth(htmlListItem, imageUrl)
                : 0;

            // WORDSNET-12668 List padding simulation spans writed by HTML Export aren't recognized during importing.
            // We write the padding simulation in HtmlListWriter.WritePaddingSimulation() method.
            double paddingSimulationWidth = mDocumentFormatter.ElementDeclarations.GetLength(
                HtmlConstants.ListPaddingSimulation);
            if (MathUtil.IsMinValue(paddingSimulationWidth))
            {
                paddingSimulationWidth = 0;
            }

            if (!isParagraphListItem && isPseudoElementListItem)
            {
                string text = GetTextOfFirstTextNode(node);
                text = HtmlUtil.RemoveControlCharsAndWhitespaces(text, false);
                if (text.StartsWith(" ", StringComparison.Ordinal))
                {
                    paddingSimulationWidth += HtmlConstants.FirstSpaceWidth;
                }
            }

            string displayStr = mDocumentFormatter.ElementDeclarations.GetIdentifier("display");
            bool isDisplayListItem = (displayStr == string.Empty) || StringUtil.EqualsIgnoreCase(displayStr, "list-item");

            CssLength textIndent = mDocumentFormatter.ElementDeclarations.GetCssLength("text-indent");

            // Markers are inside if 'li' elements aren't located inside 'ul' or 'ol' elements. Chrome browser uses
            // this behavior and we do the same. Also markers are inside if no native list marker is shown.
            // For bulleted items imported from pseudo-elements marker is located inside if text-indent is greater than
            // or equals 0.
            bool isInsideListStylePosition =
                !isPseudoElementListItem &&
                mDocumentFormatter.ElementDeclarations.ContainsIdentifier("list-style-position", "inside");

            bool isMarkerPositionInside =
                isParagraphListItem ||
                isImportListItemAsParagraph ||
                listLevelInfo.IsImplicit ||
                (listLevelInfo.IsNoneListStyle && !isPseudoElementListItem) ||
                !isDisplayListItem ||
                isInsideListStylePosition ||
                (isPseudoElementListItem && ((textIndent.Value >= 0) || (System.Math.Abs(textIndent.Value) <= markerTextWidth)));

            if (!isImportListItemAsParagraph && isMarkerPositionInside)
            {
                listLevelInfo.CurrentListLevel.Alignment = ListLevelAlignment.Left;
                if (!isPseudoElementListItem &&
                    !isParagraphListItem &&
                    (listLevelInfo.IsNoneListStyle || !isDisplayListItem))
                {
                    listLevelInfo.CurrentListLevel.TrailingCharacter = ListTrailingCharacter.Nothing;
                }
            }

            double markerToTextDistance = 0;
            if (mAdjustMarkerPositions || isMarkerPositionInside)
            {
                markerToTextDistance = markerTextWidth + paddingSimulationWidth;

                if (!isParagraphListItem && !isPseudoElementListItem)
                {
                    markerToTextDistance += HtmlConstants.ListLabelToTextWidth;
                }
            }
            else if (!isImportListItemAsParagraph)
            {
                markerToTextDistance =
                    listLevelInfo.CurrentListLevel.TextPosition - listLevelInfo.CurrentListLevel.NumberPosition;
            }

            if (isPseudoElementListItem && !isParagraphListItem)
            {
                // Depending on the direction we should import marker info not only from ::before but also
                // ::after pseudo-element, but markers do not work properly in browser for it.
                // For ::after pseudo-element we also must read 'padding-left' and we may implement it in the future.
                CssLength bulletPaddingFromPseudoElement =
                    mDocumentFormatter.BeforePseudoElementDeclarations.GetCssLength("padding-right");

                // WORDSNET-24702 List items with markers specified by ::before or ::after pseudo-elements
                // and without padding shouldn't have trailing character.
                if ((bulletPaddingFromPseudoElement.Value == 0) && (paddingSimulationWidth == 0))
                {
                    listLevelInfo.CurrentListLevel.TrailingCharacter = ListTrailingCharacter.Nothing;
                }

                markerToTextDistance = markerTextWidth + paddingSimulationWidth;

                mItem = new HtmlPseudoElementListItemInfo(
                    mBuilder.CurrentParagraph,
                    new CssLength(mWholeListLeftIndent, true),
                    GetCurrentElementLeftIndent(),
                    bulletPaddingFromPseudoElement,
                    new CssLength(markerToTextDistance, true),
                    isMarkerPositionInside,
                    isListItemNoneListStyle);
            }
            else
            {
                string paddingProperty = mDocumentFormatter.IsBlockRtl()
                    ? "padding-right"
                    : "padding-left";
                CssLength padding = mDocumentFormatter.ElementDeclarations.GetCssLength(paddingProperty);

                mItem = new HtmlRealListItemInfo(
                    mBuilder.CurrentParagraph,
                    new CssLength(mWholeListLeftIndent, true),
                    GetCurrentElementLeftIndent(),
                    padding,
                    new CssLength(markerToTextDistance, true),
                    isMarkerPositionInside,
                    isListItemNoneListStyle);
            }
        }

        private HtmlListItemBase GetHtmlListItem(
            HtmlListLevelInfo listLevelInfo,
            HtmlListItemMarker listItemMarker,
            string imageUrl,
            string generatedContent,
            bool isListItemNoneListStyle,
            bool isParagraphListItem)
        {
            double listLevelNumber = mDocumentFormatter.ElementDeclarations.GetNumber(HtmlConstants.ListLevelNumber);
            int htmlListLevelNumber = !MathUtil.IsMinValue(listLevelNumber)
                ? (int)listLevelNumber
                : listLevelInfo.ListLevelNumber;

            int pictureBulletId = mPictureBulletLoader.GetPictureBulletId(imageUrl);
            if (pictureBulletId >= 0)
            {
                Shape pictureBullet = mBuilder.Document.Lists.GetPictureBullet(pictureBulletId);
                double correctedHeightInPoints = GetCorrectedPictureBulletHeightInPoints(pictureBullet);
                int pictureBulletHeight = ConvertUtilCore.PointToHalfPoint(correctedHeightInPoints);

                return new HtmlPictureBulletListItem(
                    new HtmlListLevelId(htmlListLevelNumber),
                    pictureBulletId,
                    pictureBulletHeight);
            }

            if (isParagraphListItem)
            {
                string listNumberFormatValue =
                    mDocumentFormatter.ElementDeclarations.GetString(HtmlConstants.ListNumberFormat);
                string listNumberValuesValue =
                    mDocumentFormatter.ElementDeclarations.GetString(HtmlConstants.ListNumberValues);
                string listNumberStylesValue =
                    mDocumentFormatter.ElementDeclarations.GetString(HtmlConstants.ListNumberStyles);

                HtmlNumberListItem paragraphListItem = HtmlNumberListItem.Create(
                    listNumberFormatValue,
                    listNumberValuesValue,
                    listNumberStylesValue,
                    htmlListLevelNumber);

                if (paragraphListItem != null)
                {
                    return paragraphListItem;
                }
            }
            else if (StringUtil.HasChars(generatedContent))
            {
                HtmlNumberListItem generatedContentListItem = HtmlNumberListItem.Create(
                    mDocumentFormatter,
                    generatedContent,
                    htmlListLevelNumber);

                if (generatedContentListItem != null)
                {
                    return generatedContentListItem;
                }
            }

            if (listItemMarker.IsBullet)
            {
                string numberFormat = mDocumentFormatter.ElementDeclarations.GetString(HtmlConstants.ListLabelNumberFormat);
                string fontFamily = mDocumentFormatter.ElementDeclarations.GetString(HtmlConstants.OriginalFontFamily);
                if (IsValidNumberFormatForBullet(numberFormat) && StringUtil.HasChars(fontFamily))
                {
                    return new HtmlBulletListItem(
                        new HtmlListLevelId(htmlListLevelNumber),
                        HtmlMarkerType.Aw,
                        mBuilder,
                        mDocumentFormatter,
                        numberFormat,
                        listItemMarker.NumberStyle,
                        fontFamily);
                }

                string contentAsString = mDocumentFormatter.BeforePseudoElementDeclarations.GetString("content");

                // WORDSNET-14766 Import custom list markers from pseudo-elements.
                // If li element has no roundtrip information about marker, we can import it from
                // pseudo-element. Depending on the direction we should import marker info
                // not only from ::before but also ::after pseudo-element, but markers do not work properly in browser for it.
                if (!mDocumentFormatter.IsBlockRtl() &&
                    isListItemNoneListStyle &&
                    IsValidNumberFormatForBullet(contentAsString))
                {
                    return new HtmlBulletListItem(
                        new HtmlListLevelId(listLevelInfo.ListLevelNumber),
                        HtmlMarkerType.PseudoElement,
                        mBuilder,
                        mDocumentFormatter,
                        contentAsString,
                        listItemMarker.NumberStyle,
                        string.Empty);
                }

                return new HtmlBulletListItem(
                    new HtmlListLevelId(htmlListLevelNumber),
                    HtmlMarkerType.Html,
                    mBuilder,
                    mDocumentFormatter,
                    listItemMarker.BulletFormat,
                    listItemMarker.NumberStyle,
                    listItemMarker.BulletFontName);
            }

            return HtmlNumberListItem.Create(
                htmlListLevelNumber,
                listLevelInfo.ListLevelNumberValue + 1,
                listItemMarker.NumberStyle);
        }

        private void ApplyListLabelFormatting()
        {
            int pushedElementsCount = 0;

            IElementProvider firstChildElement = mDocumentFormatter.CurrentElement;
            while ((firstChildElement.GetFirstChildElement() != null) &&
                (firstChildElement.GetFirstChildElement().ElementName == "span"))
            {
                firstChildElement = firstChildElement.GetFirstChildElement();

                mDocumentFormatter.PushElement((IHtmlElementProvider)firstChildElement, false);

                pushedElementsCount++;
            }

            // WORDSNET-26508 Apply formatting of list label to paragraph break font instead of list label font.
            mDocumentFormatter.ToFont(mBuilder.CurrentParagraph.ParagraphBreakFont, mBuilder.CurrentParagraph.ParagraphStyle);

            for (int i = 0; i < pushedElementsCount; i++)
            {
                mDocumentFormatter.PopElement();
            }

            IHtmlElementProvider currentElement = mDocumentFormatter.CurrentElement;

            // We should re-push current element. See Text18318Margins.
            mDocumentFormatter.PopElement();
            mDocumentFormatter.PushElement(currentElement, false);
        }

        private static string GetTextOfFirstTextNode(HtmlElementNode node)
        {
            foreach (HtmlNode childNode in node.Children)
            {
                // WORDSNET-21142 Check if it's a text node that doesn't contain only new line and space characters.
                // We ignore such whitespace text nodes, because they are there only to indent elements of HTML code and
                // contain no meaningful text.
                HtmlTextNode textNode = childNode as HtmlTextNode;
                if ((textNode != null) && HtmlUtil.ContainsAnythingButWhitespaces(textNode.Text, true))
                {
                    return textNode.Text;
                }

                HtmlElementNode htmlElementNode = childNode as HtmlElementNode;
                if (htmlElementNode != null)
                {
                    string text = GetTextOfFirstTextNode(htmlElementNode);
                    if (text != string.Empty)
                        return text;
                }
            }

            return string.Empty;
        }

        private double GetMarkerTextWidth(HtmlListItemBase htmlListItem, string imageUrl)
        {
            int pictureBulletId = mPictureBulletLoader.GetPictureBulletId(imageUrl);
            if (pictureBulletId >= 0)
            {
                Shape pictureBullet = mBuilder.Document.Lists.GetPictureBullet(pictureBulletId);
                double originalHeightInPoints = pictureBullet.ImageData.ImageSize.HeightPoints;
                double correctedHeightInPoints = GetCorrectedPictureBulletHeightInPoints(pictureBullet);

                return pictureBullet.ImageData.ImageSize.WidthPoints * correctedHeightInPoints / originalHeightInPoints;
            }

            Font listLabelFont = mBuilder.CurrentParagraph.ListLabel.Font;
            DrFont drawingFont = mBuilder.Document.FontProvider.FetchDrFont(
                listLabelFont.Name,
                (float)listLabelFont.Size,
                listLabelFont.FontStyle);

            return drawingFont.GetTextWidthPoints(htmlListItem.ListLabelString);
        }

        private static bool IsValidNumberFormatForBullet(string numberFormat)
        {
            return StringUtil.HasChars(numberFormat) &&
                (numberFormat.Length == 1) &&
                !ListLevel.IsPlaceholder(numberFormat[0]);
        }

        internal static double GetCorrectedPictureBulletHeightInPoints(Shape pictureBullet)
        {
            // Seems like we should always add this value to image's height to make it have same size as in browsers.
            const double pictureBulletHeightCorrectionValueInPoints = 2;
            double originalHeightInPoints = pictureBullet.ImageData.ImageSize.HeightPoints;
            return originalHeightInPoints + pictureBulletHeightCorrectionValueInPoints;
        }

        /// <summary>
        /// Returns a list-item marker of the current HTML element.
        /// </summary>
        /// <returns>List-item marker corresponding to the current HTML element.</returns>
        private HtmlListItemMarker GetListItemMarker()
        {
            string listStyleType = mDocumentFormatter.ElementDeclarations.GetIdentifier("list-style-type");
            return HtmlListItemMarker.GetFromListStyleType(listStyleType);
        }

        /// <summary>
        /// Gets a model list template for current list level.
        /// </summary>
        private ListTemplate GetListTemplate(string listTag, int listLevel)
        {
            HtmlListItemMarker listItemMarker = GetListItemMarker();
            if (listItemMarker == null)
                listItemMarker = HtmlListItemMarker.GetFromListTag(listTag, listLevel);
            return listItemMarker.ListTemplate;
        }

        private HtmlListLevelInfo CurrentListLevelInfo
        {
            get { return mListLevelInfos.Top(); }
        }

        private readonly DocumentBuilder mBuilder;
        private readonly DocumentFormatter mDocumentFormatter;
        private readonly HtmlResourceLoader mHtmlResourceLoader;
        private readonly string mBaseUri;

        /// <summary>
        /// Currently opened list levels.
        /// </summary>
        private readonly Stack<HtmlListLevelInfo> mListLevelInfos;

        private readonly HtmlPictureBulletLoader mPictureBulletLoader;

        /// <summary>
        /// If this option is on, positions of imported list item markers are additionally adjusted so that the resulting list
        /// looks closer to a list rendered in browsers.
        /// </summary>
        private readonly bool mAdjustMarkerPositions;

        /// <summary>
        /// Indentation used for all lists that have no custom indentation (no custom margins or padding).
        /// </summary>
        private readonly double mDefaultWholeListLeftIndent;

        /// <summary>
        /// Indentation used for the current list as a whole. Items at all levels of this list are additionally indented by this
        /// value.
        /// </summary>
        /// <remarks>
        /// WORDSNET-13239 When inserting an HTML fragment with builder formatting, lists that have no custom
        /// indentation should use target paragraph's indentation.
        /// </remarks>
        private double mWholeListLeftIndent;

        /// <summary>
        /// The list item currently being processed. Can be <c>null</c>.
        /// </summary>
        private HtmlListItemInfo mItem;

        /// <summary>
        /// This field counts opened list items across all list levels.
        /// </summary>
        private int mOpenedListItemCount;

        private readonly HtmlModelListManager mModelListManager;

        /// <summary>
        /// This property is used to prevent HtmlModelListManager from deleting all lists,
        /// when we're reading list items which are specified as "p" elements.
        /// </summary>
        private bool mIsLastListItemParagraphListItem;
    }
}
