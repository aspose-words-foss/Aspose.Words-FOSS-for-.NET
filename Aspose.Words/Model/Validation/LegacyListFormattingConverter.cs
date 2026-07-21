// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2010 by Roman Korchagin

using System.Drawing;
using Aspose.Drawing.Fonts;
using Aspose.Words.Lists;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Implements legacy tab stop calculation.
    /// </summary>
    /// <remarks>
    /// List labels SHOULD be updated to get proper result.
    /// </remarks>
    internal static class LegacyListFormattingConverter
    {
        /// <summary>
        /// There is an "incompatibility" between legacy and normal list formatting and we must actually have a paragraph
        /// to be able to convert legacy list formatting into normal list formatting.
        /// </summary>
        internal static TabStop CalculateLegacyTab(Paragraph para)
        {
            if (!para.IsListItem)
                return null;

            ListLevel level = para.ListLevel;
            if (!level.Legacy)
                return null;

            // RK The description of list legacy formatting in OOXML and DOC specifications is incorrect.
            //
            // RK Here is the way legacy list formatting is supposed to work:
            //
            // If legacy formatting is present, it overrides the normal formatting.
            // The sole purpose of legacy formatting is to specify the distance between the list label and the paragraph text.
            // When legacy formatting is used, the list label trailing character looks and acts like a tab in MS Word.
            //
            // There are two fields LegacyIndent and LegacySpace that can specify the distance between the list label and the text.
            // LegacyIndent specifies the distance from the left side of the list label to the left side of the first line of text.
            // LegacySpace specifies the distance from the right side of the list label to the left side of the first line of text.
            // If both LegacyIndent and LegacySpace are specified, then both distances are calculated and the biggest is to be used.
            // Normal paragraph indent and first line indent are taken into account as usual.

            // It is easy to calculate the distance if LegacyIndent is specified - we can add this value to the position
            // of the first line of the paragraph and set a tab stop there.
            //
            // It is more difficult to calculate the distance if LegacySpace is specified. We need to know the length
            // of the label text. To calculate the length of the label of text we need to know its formatting and measure the string.
            // This code already calculates the length of the label text and then selects the bigger
            // between LegacyIndent and LegacySpace + LabelStringWidth.

            // IP 14/02/2014: Added IsLegacyTab to TabStop because legacy tab must be special processed. (CR 9339)
            // 1. Legacy tab is taken into account only for list label tab (i.e legacy tab is used only for first row and must be ignored for other lines in the paragraph).
            // 2. All other custom tabs must be ignored for first row if legacy tab is present.
            // IP: I'm not sure that TabStop should be generated for processed legacySpace and legacyIndent. They probably should be the usual paragraph properties.

            double leftOfListLabel = para.ParagraphFormat.LeftIndent + para.ParagraphFormat.FirstLineIndent;
            double leftOfText;
            double calculatedLegacySpace = 0;

            if ((level.LegacyIndent != 0) || (level.LegacySpace != 0))
            {
                if (level.LegacySpace != 0)
                    calculatedLegacySpace = GetLegacySpace(para);

                leftOfText = leftOfListLabel +
                             System.Math.Max(ConvertUtilCore.TwipToPoint(level.LegacyIndent), calculatedLegacySpace);
            }
            else
            {
                // This seems to be what MS Word is doing in TestListNasty5.
                leftOfText = para.ParagraphFormat.LeftIndent;
            }

            // When legacy formatting is used, the list label must act like a tab.
            // WORDSNET-7995 If the left position of the text comes before the left position of the label,
            // the tab stop cannot be used (only tabs that come after the list label can be used).
            // In this case the text should start right next to the label as if it had no trailing character at all.
            ListTrailingCharacter trailingCharacter = leftOfText <= leftOfListLabel
                    ? ListTrailingCharacter.Nothing
                    : ListTrailingCharacter.Tab;

            return TabStop.CreateLegacyTabStop(leftOfText, trailingCharacter);
        }

        /// <summary>
        /// Calculate legacy space for paragraph.
        /// </summary>
        /// <param name="para">Paragraph with list item.</param>
        /// <returns>Space size in points.</returns>
        private static double GetLegacySpace(Paragraph para)
        {
            ListLabel listLabel = para.ListLabel;
            ListLevel level = para.ListLevel;

            // Get DrFont to measure label string.
            DrFont font = para.Document.FontProvider.FetchDrFont(listLabel.Font.Name,
                (float)listLabel.Font.Size, FontStyle.Regular);
            double labelStringWidth = font.GetTextWidthPoints(listLabel.LabelStringOriginal);

            return ConvertUtilCore.TwipToPoint(level.LegacySpace) + labelStringWidth;
        }

    }
}
