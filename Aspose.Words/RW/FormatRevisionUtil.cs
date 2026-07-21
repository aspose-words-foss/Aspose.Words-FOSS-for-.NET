// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/07/2013 by Alexey Morozov

namespace Aspose.Words.RW
{
    /// <summary>
    /// Implements methods to convert revision markup to positive-difference form.
    /// </summary>
    /// <remarks>
    /// AM. Docx/Wml formats use BeforeChanges/AfterChanges format revision model and Doc and Aspose.Words
    /// use BeforeChanges/PositiveDifference format revision model. For most cases these models are similar,
    /// that's why in common revisions are read/written correctly without any additional processing.
    ///
    /// But consider the example to see actual difference:
    ///
    /// DocumentDefaults has SpacingAfter 10pt.
    /// Paragraph had SpacingAfter 20pt and was changed to 10pt under change tracking.
    ///
    /// For Docx/Wml Word writes such format revision as:
    ///   BeforeChanges: SpacingAfter = 20pt.
    ///   AfterChanges: None (because 10pt value is inherited from DocumentDefaults and Word never writes inherited attributes).
    ///
    /// For Doc Word writes:
    ///   BeforeChanges: SpacingAfter = 20pt.
    ///   PositiveDifference: SpacingAfter = 10pt.
    ///
    /// BeforeChanges/AfterChanges model is simpler and even for Doc format style format revision uses this model.
    /// To get final markup we should do nothing, just select AfterChanges.
    ///
    /// For PositiveDifference model to get final markup we should calculate BeforeChanges + PositiveDifference.
    /// So we need to calculate PositiveDifference upon Docx/Wml reading otherwise final markup will be wrong.
    ///
    /// I have strong feeling that we should change AW revision model and start to use BeforeChanges/AfterChanges model.
    /// Going to discuss it later.
    /// </remarks>
    internal static class FormatRevisionUtil
    {
        /// <summary>
        /// Converts paragraph revision markup to PositiveDifference form. Should be called after all styles and lists are imported.
        /// </summary>
        internal static void ConvertToPositiveDifference(Paragraph para)
        {
            if (para.ParagraphBreakRunPr.FormatRevision != null)
            {
                const RunPrExpandFlags runPrExpandFlags = RunPrExpandFlags.DocumentDefaults;
                const RunPrExpandFlags revPrExpandFlags = RunPrExpandFlags.AfterChanges | RunPrExpandFlags.DocumentDefaults;

                RunPr expandedRunPr = para.GetExpandedParagraphBreakRunPr(runPrExpandFlags);
                RunPr expandedRevPr = para.GetExpandedParagraphBreakRunPr(revPrExpandFlags);

                expandedRevPr.Collapse(expandedRunPr);
                para.ParagraphBreakRunPr.FormatRevision.RevPr = expandedRevPr;
            }

            if (para.ParaPr.FormatRevision != null)
            {
                ParaPrExpandFlags paraPrExpandFlags = ParaPrExpandFlags.Normal;
                ParaPrExpandFlags revPrExpandFlags = ParaPrExpandFlags.AfterChanges;

                // In table paragraphs should not use Document Defaults.
                if(!para.IsInCell)
                {
                    paraPrExpandFlags |= ParaPrExpandFlags.DocumentDefaults;
                    revPrExpandFlags |= ParaPrExpandFlags.DocumentDefaults;
                }

                ParaPr expandedParaPr = para.GetExpandedParaPr(paraPrExpandFlags);
                ParaPr expandedRevPr = para.GetExpandedParaPr(revPrExpandFlags);

                // Istd is not inherited in revisions so absence of Istd
                // in "normal" revised properties means that styled paragraph goes to Normal,
                // ensure that Normal (0) Istd is implicitly set in revision.
                ParaPr revPr = (ParaPr)para.ParaPr.FormatRevision.RevPr;
                if (para.ParaPr.Contains(ParaAttr.Istd) && !revPr.Contains(ParaAttr.Istd))
                    revPr.SetAttr(ParaAttr.Istd, 0);

                if (para.ParaPr.Istd == revPr.Istd)
                {
                    int originalListId = (int)expandedParaPr.FetchAttr(ParaAttr.ListId);
                    int finalListId = (int)expandedRevPr.FetchAttr(ParaAttr.ListId);

                    ParaPr temp = new ParaPr();
                    expandedRevPr.MoveTo(temp, ParaAttr.ListId);
                    expandedRevPr.MoveTo(temp, ParaAttr.ListLevel);

                    if (originalListId != finalListId)
                    {
                        // WORDSNET-14997 Since we remove left indent when list revision accepted we need to preserve
                        // this indent in revised properties.
                        // I believe that we should also preserve all list related attributes:
                        // LeftIndent, FirstLineIndent and TabStops but lets postpone for a while.
                        expandedRevPr.MoveTo(temp, ParaAttr.LeftIndent);
                        // WORDSNET-19608 Preserve "hanging" attribute in revised properties.
                        expandedRevPr.MoveTo(temp, ParaAttr.FirstLineIndent);
                    }

                    expandedRevPr.Collapse(expandedParaPr);

                    // Restore saved attributes.
                    temp.CopyTo(expandedRevPr);

                    para.ParaPr.FormatRevision.RevPr = expandedRevPr;
                }
            }
        }

        /// <summary>
        /// Converts style revision markup to PositiveDifference form. Should be called after all styles and lists are imported.
        /// </summary>
        internal static void ConvertToPositiveDifference(Style style)
        {
            if (style.ParaPr.FormatRevision != null)
            {
                ParaPr normalParaPr = style.GetExpandedParaPr(ParaPrExpandFlags.DocumentDefaults);
                ParaPr revisedParaPr = style.GetExpandedParaPr(ParaPrExpandFlags.AfterChanges | ParaPrExpandFlags.DocumentDefaults);

                normalParaPr.Remove(ParaAttr.Istd);
                revisedParaPr.Remove(ParaAttr.Istd);

                revisedParaPr.Collapse(normalParaPr);
                style.ParaPr.FormatRevision.RevPr = revisedParaPr;
            }

            if (style.RunPr.FormatRevision != null)
            {
                RunPr normalRunPr = style.GetExpandedRunPr(RunPrExpandFlags.DocumentDefaults);
                RunPr revisedRunPr = style.GetExpandedRunPr(RunPrExpandFlags.AfterChanges | RunPrExpandFlags.DocumentDefaults);

                normalRunPr.Remove(FontAttr.Istd);
                revisedRunPr.Remove(FontAttr.Istd);

                revisedRunPr.Collapse(normalRunPr);
                style.RunPr.FormatRevision.RevPr = revisedRunPr;
            }
        }

        /// <summary>
        /// Converts run revision markup to PositiveDifference form. Should be called after all styles and lists are imported.
        /// </summary>
        internal static void ConvertToPositiveDifference(IInline inlineNode)
        {
            if (inlineNode.RunPr_IInline.FormatRevision == null)
                return;


            // WORDSNET-15583 Paragraph style has to be skipped on expanding when applying of style was canceled.
            // WORDSNET-24550 Paragraph style should be ignored only in "before" changes.
            RunPrExpandFlags beforeExpandFlags = RunPrExpandFlags.DocumentDefaults;
            RunPrExpandFlags afterExpandFlags = RunPrExpandFlags.DocumentDefaults | RunPrExpandFlags.AfterChanges;
            if (inlineNode.ParentParagraph_IInline.ParaPr.HasIstdRevision)
                beforeExpandFlags |= RunPrExpandFlags.NoParaStyle;


            RunPr beforeRunPr = inlineNode.GetExpandedRunPr_IInline(beforeExpandFlags);
            RunPr afterRunPr = inlineNode.GetExpandedRunPr_IInline(afterExpandFlags);

            // WORDSNET-9784 If the style was applied during a formatting revision we should not perform collapsing.
            if (afterRunPr.ContainsKey(FontAttr.Istd))
                return;

            // Convert "after" changes to positive difference.
            afterRunPr.Collapse(beforeRunPr);

            // WORDSNET-24550 When there is a parent paragraph style revision, then only attributes
            // that exist in direct "before" properties collection should be added as positive difference
            // to "after" changes RevPr collection.
            if (inlineNode.ParentParagraph_IInline.ParaPr.HasIstdRevision)
            {
                for (int i = 0; i < inlineNode.RunPr_IInline.Count; i++)
                {
                    int key = inlineNode.RunPr_IInline.GetKey(i);
                    // Skip ignorable attributes.
                    if (ArrayUtil.BinarySearch(gIgnoreAttrs, 0, gIgnoreAttrs.Length, key) == 0)
                        continue;

                    // Add or remove collapsed value.
                    if (afterRunPr[key] != null)
                        inlineNode.RunPr_IInline.FormatRevision.RevPr[key] = afterRunPr[key];
                    else
                        inlineNode.RunPr_IInline.FormatRevision.RevPr.Remove(key);
                }
            }
            else
            {
                inlineNode.RunPr_IInline.FormatRevision.RevPr = afterRunPr;
            }
        }

        // The set of ignorable attributes.
        private static readonly int[] gIgnoreAttrs = new int[]
        {
            RevisionAttr.DeleteRevision /*12*/,
            RevisionAttr.MoveFromRevision /*13*/,
            RevisionAttr.InsertRevision /*14*/,
            RevisionAttr.MoveToRevision /*15*/,
            FontAttr.RsidRPr /* 30 */,
            FontAttr.RsidR /* 40 */,
            RevisionAttr.FormatRevision /* 10010 */
        };
    }
}
