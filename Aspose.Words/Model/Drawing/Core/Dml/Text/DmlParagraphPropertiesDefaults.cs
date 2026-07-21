// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    internal class DmlParagraphPropertiesDefaults : DmlHierarchicalPropertyBag
    {
        private DmlParagraphPropertiesDefaults()
        {
            // WORDSNET-8939 Left alignment should be default for paragraphs in drawing parts.
            // Looks like in charts it is ParagraphAlignment.Center by default. In DmlChartTxPr, default
            // value is changed on creation of DML paragraphs.
            SetProperty((int)DmlParagraphPropertiesIds.Level, 0);
            SetProperty((int)DmlParagraphPropertiesIds.Alignment, ParagraphAlignment.Left);
            SetProperty((int)DmlParagraphPropertiesIds.DefaultTabSize, 914400);
                // Office uses a default value of 914400 EMUs.
            SetProperty((int)DmlParagraphPropertiesIds.IsEastAsianLineBreakAllowed, true);
            SetProperty((int)DmlParagraphPropertiesIds.FontAlignment, DmlFontAlignment.Baseline);
            SetProperty((int)DmlParagraphPropertiesIds.IsHangingPunctuationAllowed, false);
                // Office uses a default value of 1, or true, for the hangingPunct attribute.
            SetProperty((int)DmlParagraphPropertiesIds.TextIdentation, 0);
                // Office uses a default value of 0 for the indent attribute.
            SetProperty((int)DmlParagraphPropertiesIds.IsLatinLineBreakAllowed, false);
                //  Office uses a default value of 0, or false, for the latinLnBrk attribute.
            SetProperty((int)DmlParagraphPropertiesIds.LeftMargin, 0);
                // Office uses a default value of 0 for the marL attribute.
            SetProperty((int)DmlParagraphPropertiesIds.RightMargin, 0);
            SetProperty((int)DmlParagraphPropertiesIds.RightToLeftFlowDirection, false);
            SetProperty((int)DmlParagraphPropertiesIds.LineSpacing, new DmlPercentageTextSpacing()); //  Default is 100% 
            SetProperty((int)DmlParagraphPropertiesIds.SpaceAfter, new DmlPointsTextSpacing()); // No after text spacing
            SetProperty((int)DmlParagraphPropertiesIds.SpaceBefore, new DmlPointsTextSpacing()); // No before text spacing

            SetProperty((int)DmlParagraphPropertiesIds.Extensions, null);
        }

        internal static DmlParagraphPropertiesDefaults Instance
        {
            get { return mInstance; }
        }

        private static readonly DmlParagraphPropertiesDefaults mInstance = new DmlParagraphPropertiesDefaults();
    }
}
