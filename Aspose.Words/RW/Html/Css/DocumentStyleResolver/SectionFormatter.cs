// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/07/2023 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Applies CSS formatting to document sections.
    /// </summary>
    internal abstract class SectionFormatter
    {
        protected SectionFormatter(
            Document document,
            CssStyleTracker cssStyleTracker)
        {
            Document = document;
            CssStyleTracker = cssStyleTracker;
        }

        internal abstract void UpdateDefaultSectionProperties(Section section);

        internal void Format(Section section)
        {
            if (CssStyleTracker.IsEmpty)
            {
                return;
            }

            if (CssStyleTracker.ElementDeclarations["page"] != null)
            {
                CssDeclarationCollection pageDeclarations = CssStyleTracker.GetAllPageDeclarations();
                ApplyPageSetupFormatting(section, pageDeclarations);
                // Note that we must apply page size first, because page margin values may be relative.
                ApplyPageMargins(section, pageDeclarations);
            }

            ApplySpecialFormatting(section);
        }

        protected abstract void ApplyPageSetupFormatting(
            Section section,
            CssDeclarationCollection pageDeclarations);

        protected abstract void ApplySpecialFormatting(Section section);

        private static void ApplyPageMargins(
            Section section,
            CssDeclarationCollection pageDeclarations)
        {
            SectPr sectPr = section.SectPr;
            int pageWidth = sectPr.PageWidth;
            int pageHeight = sectPr.PageHeight;

            int margin = GetPageMargin(pageDeclarations, "margin-top", pageHeight);
            if (margin >= 0)
                sectPr.TopMargin = margin;

            margin = GetPageMargin(pageDeclarations, "margin-right", pageWidth);
            if (margin >= 0)
                sectPr.RightMargin = margin;

            margin = GetPageMargin(pageDeclarations, "margin-bottom", pageHeight);
            if (margin >= 0)
                sectPr.BottomMargin = margin;

            margin = GetPageMargin(pageDeclarations, "margin-left", pageWidth);
            if (margin >= 0)
                sectPr.LeftMargin = margin;
        }

        private static int GetPageMargin(
            CssDeclarationCollection declarations,
            string propertyName,
            int pageSize)
        {
            int margin = int.MinValue;

            CssDeclaration declaration = declarations[propertyName];
            if ((declaration == null) || (declaration.Value.Count > 1))
            {
                return margin;
            }

            CssValue cssValue = declaration.Value.FirstValue;

            switch (cssValue.ValueType)
            {
                case CssValueType.Percentage:
                {
                    double ratio = ((CssPercentageValue)cssValue).DoubleValue / 100.0;
                    margin = DoublePal.RoundToIntUp(ratio * pageSize);
                    break;
                }
                default:
                {
                    double length = CssUtil.LengthToPoint(cssValue);
                    if (!MathUtil.AreEqual(length, double.MinValue))
                    {
                        margin = ConvertUtilCore.PointToTwip(length);
                    }
                    break;
                }
            }

            return margin;
        }

        protected Document Document { get; }

        protected CssStyleTracker CssStyleTracker { get; }
    }
}
