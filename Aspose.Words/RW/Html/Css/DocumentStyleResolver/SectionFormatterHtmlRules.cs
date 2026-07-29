// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/07/2023 by Victor Chebotok

using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Applies CSS formatting to document sections.
    /// Uses formatting rules that are different from what MS Word uses but produce better looking results
    /// and are more confomant to the CSS and HTML specifications.
    /// </summary>
    internal class SectionFormatterHtmlRules : SectionFormatter
    {
        internal SectionFormatterHtmlRules(
            Document document,
            CssStyleTracker cssStyleTracker)
            : base(document, cssStyleTracker)
        {
            // Empty constructor.
        }

        internal override void UpdateDefaultSectionProperties(Section section)
        {
            // Nothing to do.
        }

        /// <summary>
        /// Applies CSS size style to a model section's page setup.
        /// </summary>
        /// <remarks>
        /// MS Word applies the following rules during setting page setup:
        /// 1. If "size" has a dimension then page orientation is always portrait (example: "size: 300pt 200pt").
        ///    We don't follow MS Word behavior and instead detect page orientation by aspect ratio, because it is more logical.
        /// 2. If "size" is "portrait" then the page orientation is set to portrait (example: "size: portrait").
        /// 3. If "size" is "landscape" then the page orientation is set to landscape (example: "size: landscape").
        /// 4. If "size" contains a page size name (Letter, A4 and etc) then the resulting page size in MS Word depends
        ///    on Office language preferences. For English (US) the default page size is Letter, for other languages it is A4.
        ///    We don't follow MS Word behavior in that case and instead set the page size as specified in CSS.
        /// </remarks>
        protected override void ApplyPageSetupFormatting(
            Section section,
            CssDeclarationCollection pageDeclarations)
        {
            CssDeclaration sizeDeclaration = pageDeclarations["size"];
            if (sizeDeclaration == null)
                return;

            CssPageSizePropertyValue value = ParsePageSizePropertyValue(sizeDeclaration.Value);
            if (!(sizeDeclaration.Value is CssPageSizePropertyValue))
                return;

            double width = CssUtil.LengthToPoint(value.Width);
            double height = CssUtil.LengthToPoint(value.Height);
            bool pageSizeIsSpecified = !MathUtil.IsMinValue(width) && !MathUtil.IsMinValue(height);

            PageSetup pageSetup = section.PageSetup;

            // Set page orientation first, because the Orientation setter exchanges width and height when page orientation
            // changes.
            switch (value.Orientation)
            {
                case CssPageOrientation.Landscape:
                {
                    pageSetup.Orientation = Orientation.Landscape;
                    break;
                }
                case CssPageOrientation.Portrait:
                case CssPageOrientation.NotSpecified:
                {
                    // WORDSNET-10576 If page size is specified, set page orientation by the longest side of the page.
                    // Otherwise, use the portrait orientation.
                    pageSetup.Orientation = (pageSizeIsSpecified && (width > height))
                        ? Orientation.Landscape
                        : Orientation.Portrait;
                    break;
                }
                default:
                {
                    Debug.Assert(false);
                    break;
                }
            }

            // Set the page size. Here the width value is always less than or equal to the height value. Otherwise, we would've
            // changed the page orientation.
            if (pageSizeIsSpecified)
            {
                if ((value.Orientation == CssPageOrientation.Portrait) ||
                    (value.Orientation == CssPageOrientation.NotSpecified))
                {
                    pageSetup.PageWidth = width;
                    pageSetup.PageHeight = height;
                }
                else
                {
                    pageSetup.PageWidth = height;
                    pageSetup.PageHeight = width;
                }
            }

            // WORDSNET-22945 Preserve page numbering properties. The code below restores these properties from CSS.

            string pageNumberStyle = pageDeclarations.GetIdentifier(HtmlConstants.PageSetupNumberStyle);
            // If the page number style CSS property doesn't exist, the default 'arabic' value is used.
            pageSetup.PageNumberStyle = DocxEnum.DocxToNumberStyle(pageNumberStyle);

            string chapterPageSeparator = pageDeclarations.GetIdentifier(HtmlConstants.PageSetupChapterSeparator);
            // If the chapter separator CSS property doesn't exist, the default 'hyphen' value is used.
            pageSetup.ChapterPageSeparator = NrxSectEnum.XmlToChapterPageSeparator(chapterPageSeparator);

            double headingLevelForChapter = pageDeclarations.GetNumber(HtmlConstants.PageSetupHeadingLevelForChapter);
            pageSetup.HeadingLevelForChapter = (int)MathUtil.FitToRange(headingLevelForChapter, 0, 9);

            double pageStartingNumber = pageDeclarations.GetNumber(HtmlConstants.PageSetupStartingNumber);
            // If the page starting number is omitted, numbering continues from the highest page number in the previous section.
            if (MathUtil.IsMinValue(pageStartingNumber))
            {
                pageSetup.RestartPageNumbering = false;
            }
            else
            {
                pageSetup.PageStartingNumber = (int)MathUtil.FitToRange(pageStartingNumber, 1, int.MaxValue);
                pageSetup.RestartPageNumbering = true;
            }

            double headerDistance = pageDeclarations.GetLength(HtmlConstants.PageSetupHeaderDistance);
            if (!MathUtil.IsMinValue(headerDistance))
            {
                // Limit the header distance value to the range allowed by MS Word.
                pageSetup.HeaderDistance = MathUtil.FitToRange(headerDistance, 0, ConvertUtilCore.MaxSizePoint);
            }

            double footerDistance = pageDeclarations.GetLength(HtmlConstants.PageSetupFooterDistance);
            if (!MathUtil.IsMinValue(footerDistance))
            {
                // Limit the footer distance value to the range allowed by MS Word.
                pageSetup.FooterDistance = MathUtil.FitToRange(footerDistance, 0, ConvertUtilCore.MaxSizePoint);
            }
        }

        protected override void ApplySpecialFormatting(Section section)
        {
            TextFlow textFlow = CssUtil.CssBlockFlowDirectionToTextFlow(CssStyleTracker.GetBlockFlowDirection());
            if (section.SectPr.TextFlow != textFlow)
            {
                section.SectPr.TextFlow = textFlow;
            }

            if (CssStyleTracker.ElementDeclarations.ContainsIdentifier("direction", "rtl"))
            {
                section.SectPr.Bidi = true;
            }
        }

        private static CssPageSizePropertyValue ParsePageSizePropertyValue(CssPropertyValue propertyValue)
        {
            // Value:    <length>{1,2} | auto | [ <page-size> || [ portrait | landscape] ]
            switch (propertyValue.Count)
            {
                case 1:
                {
                    CssValue value = propertyValue[0];
                    if (value.Equals(CssValue.Auto))
                        return CssPageSizePropertyValue.CreateAuto();
                    if (value.ValueType == CssValueType.Identifier)
                        return CssPageSizePropertyValue.CreatePageSize((CssIdentifierValue)value);

                    CssLengthValue valueAsLength = value.ToLength(true);
                    if (valueAsLength != null)
                    {
                        return CssPageSizePropertyValue.CreateLength(valueAsLength);
                    }
                    break;
                }
                case 2:
                {
                    CssValue left = propertyValue[0];
                    CssValue right = propertyValue[1];
                    if (left.ValueType == right.ValueType)
                    {
                        if (left.ValueType == CssValueType.Length)
                            return CssPageSizePropertyValue.CreateLength((CssLengthValue)left, (CssLengthValue)right);
                        if (left.ValueType == CssValueType.Identifier)
                            return CssPageSizePropertyValue.CreatePageSize((CssIdentifierValue)left, (CssIdentifierValue)right);
                    }
                    break;
                }
                default:
                    break;
            }
            return null;
        }
    }
}
