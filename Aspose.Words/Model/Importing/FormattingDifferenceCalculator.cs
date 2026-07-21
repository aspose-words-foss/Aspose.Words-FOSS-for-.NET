// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/10/2017 by Ilya Navrotskiy

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Fields;
using Aspose.Words.Lists;
using Aspose.Words.RW;
using Aspose.Words.Themes;

namespace Aspose.Words
{
    /// <summary>
    /// Implements calculation formatting difference between source and destination documents.
    /// </summary>
    internal class FormattingDifferenceCalculator
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FormattingDifferenceCalculator(ImportContext context)
        {
            mContext = context;

            // Prepare frequently used fully expanded attributes.
            Style styleNormal = context.DstStyles.GetBySti(StyleIdentifier.Normal, false);
            if (styleNormal != null)
            {
                RunPrExpandFlags runPrExpandFlags = RunPrExpandFlags.Revised;
                // WORDSNET-25741 As we do not collapse attributes in Merger mode,
                // we don't need to expand global defaults.
                if (!mContext.ImportFormatOptions.IsMerger)
                    runPrExpandFlags |= RunPrExpandFlags.GlobalDefaults;

                const ParaPrExpandFlags paraPrExpandFlags = ParaPrExpandFlags.Revised;

                mExpNormal.RunPr = styleNormal.GetExpandedRunPr(runPrExpandFlags);
                mExpNormal.ParaPr = styleNormal.GetExpandedParaPr(paraPrExpandFlags);

                mExpNormalWithDefaults.RunPr =
                    styleNormal.GetExpandedRunPr(runPrExpandFlags | RunPrExpandFlags.DocumentDefaults);
                mExpNormalWithDefaults.ParaPr =
                    styleNormal.GetExpandedParaPr(paraPrExpandFlags | ParaPrExpandFlags.DocumentDefaults);
            }
            else
            {
                // Get expanded global defaults.
                mExpNormal.RunPr.ExpandTo(mExpNormal.RunPr, true);
                // Paragraph properties in any case used with document defaults. So, there is not a reason to expand
                // paragraph properties for the case which does not consider document defaults.

                // Get expanded global + document defaults. Note, the source document style defaults should be used.
                // WORDSNET-26869 As we do not collapse attributes in Merger mode, we don't need to expand global defaults.
                context.SrcStyles.DefaultRunPr.ExpandTo(mExpNormalWithDefaults.RunPr, !mContext.ImportFormatOptions.IsMerger);

                // WORDSNET-23865 Do not expand global defaults to avoid appearing text frame attributes in the direct formatting.
                context.SrcStyles.DefaultParaPr.ExpandTo(mExpNormalWithDefaults.ParaPr, false);

                // The Normal style does not exist in a destination document, so it will be either imported from the source
                // document or it will be created from the BuiltIn styles collection. We need to determine this to get
                // correct Normal style properties for expand cache. The most correct way to do this would be to check whether
                // there is any paragraph node for importing. But when this constructor is invoked,
                // mContext.TopmostNode property may not be set yet, so it is not so easy to determine here which types
                // of nodes will be imported. As this is a very rare case, lets postpone this for a while and always
                // use Normal style from a source document.
                styleNormal = context.SrcStyles.GetBySti(StyleIdentifier.Normal, false);
                if (styleNormal != null)
                {
                    styleNormal.RunPr.ExpandTo(mExpNormal.RunPr, false);

                    styleNormal.RunPr.ExpandTo(mExpNormalWithDefaults.RunPr, false);
                    styleNormal.ParaPr.ExpandTo(mExpNormalWithDefaults.ParaPr, false);
                }
            }

            mNormal = styleNormal;
        }

        /// <summary>
        /// Calculates attributes difference in source and destination documents for the <paramref name="srcInline"/>
        /// and applies it to the <paramref name="dstInline"/>.
        /// </summary>
        internal void Apply(IInline srcInline, IInline dstInline)
        {
            RunPr runPrDiff = GetRunPrDiff(srcInline, false);

            // Preserve edit revisions in case when revision tracking is enabled
            // for destination document (see Document.StartTrackRevisions()).
            if (dstInline.RunPr_IInline.HasRevisions)
                dstInline.RunPr_IInline.MirrorTo(runPrDiff, gRevisionAttrs);

            dstInline.RunPr_IInline = runPrDiff;

            if (srcInline.RunPr_IInline.HasFormatRevision)
            {
                WordAttrCollection revPr = dstInline.RunPr_IInline.FormatRevision.RevPr;
                revPr.Clear();
                GetRunPrDiff(srcInline, true).ExpandTo(revPr);

                // We store in model difference between original and revised attributes.
                // IN. I don't use FormatRevisionUtil.ConvertToPositiveDifference() here because we
                // have already expanded attributes here and there is no any additional processing
                // in that method is performed. In addition, there is no parent paragraph is set yet,
                // so expanding these attributes is pointless.
                revPr.Collapse(dstInline.RunPr_IInline);
            }
        }

        /// <summary>
        /// Calculates attributes difference in source and destination documents for the <paramref name="srcPara"/>
        /// and applies it to the <paramref name="dstPara"/>.
        /// </summary>
        internal void Apply(Paragraph srcPara, Paragraph dstPara)
        {
            // Word does not expand style difference in original revision.
            // It just creates style if it is a builtIn style or imports it,
            // if it is a user defined style.
            if (srcPara.ParaPr.HasFormatRevision)
            {
                if (srcPara.ParaPr.Istd != 0)
                    dstPara.ParaPr.Istd = ImportOrCreateStyle(srcPara.ParagraphStyle).Istd;

                // Calculate revised ParaPr.
                WordAttrCollection revPr = dstPara.ParaPr.FormatRevision.RevPr;
                revPr.Clear();
                GetParaPrDiff(srcPara, true).ExpandTo(revPr);
                // We store in model difference between original and revised attributes.
                FormatRevisionUtil.ConvertToPositiveDifference(dstPara);
            }
            else
            {
                ParaPr paraPrDiff = GetParaPrDiff(srcPara, false);
                // IN. If revision tracking is enabled for the destination document
                // (see Document.StartTrackRevisions()), then revision could be created
                // for the only destination paragraph and we should preserve it in that case.
                // Also, I think that only edit revisions can be tracked by AW for a moment,
                // so I don't care here about Format revisions.
                if (dstPara.ParaPr.HasRevisions)
                    dstPara.ParaPr.MirrorTo(paraPrDiff, gRevisionAttrs);

                dstPara.ParaPr = paraPrDiff;
            }

            if (srcPara.ParagraphBreakRunPr.HasFormatRevision)
            {
                // Calculate revised ParagraphBreakRunPr.
                WordAttrCollection revPr = dstPara.ParagraphBreakRunPr.FormatRevision.RevPr;
                revPr.Clear();
                GetParagraphBreakRunPrDiff(srcPara, true).ExpandTo(revPr);
                // We store in model difference between original and revised attributes.
                // IN. I don't use FormatRevisionUtil.ConvertToPositiveDifference() here because we
                // have already expanded attributes here and there is no any additional processing
                // in that method is performed.
                revPr.Collapse(dstPara.ParagraphBreakRunPr);
            }
            else
            {
                // Calculate ParagraphBreakRunPr difference between source and destination documents.
                RunPr breakRunPrDiff = GetParagraphBreakRunPrDiff(srcPara, false);

                // Preserve edit revisions in case when revision tracking is enabled
                // for destination document (see Document.StartTrackRevisions()).
                if (dstPara.ParagraphBreakRunPr.HasRevisions)
                    dstPara.ParagraphBreakRunPr.MirrorTo(breakRunPrDiff, gRevisionAttrs);

                dstPara.ParagraphBreakRunPr = breakRunPrDiff;
            }
        }

        /// <summary>
        /// Notifies a specified Node is visiting.
        /// </summary>
        internal void VisitNode(Node node)
        {
            switch (node.NodeType)
            {
                case NodeType.FieldStart:
                {
                    int count;
                    mFieldsCount.TryGetValue(((FieldChar)node).FieldType, out count);
                    mFieldsCount[((FieldChar)node).FieldType] = ++count;
                    break;
                }

                case NodeType.FieldEnd:
                {
                    int count;
                    mFieldsCount.TryGetValue(((FieldChar)node).FieldType, out count);
                    mFieldsCount[((FieldChar)node).FieldType] = --count;
                    break;
                }
            }
        }

        /// <summary>
        /// Returns RunPr that contains attributes difference of the specified
        /// <paramref name="srcInline"/> in source and destination documents.
        /// </summary>
        /// <remarks>
        /// It almost completely duplicates code in <see cref="GetParagraphBreakRunPrDiff"/>,
        /// but at the moment I do not see a simple way to get rid of it.
        /// </remarks>
        private RunPr GetRunPrDiff(IInline srcInline, bool isRevised)
        {
            RunPrExpandFlags expandFlags =  RunPrExpandFlags.NoChangeCommentSize;
            // Expand with global defaults to get rid of extra global default properties
            // on collapsing with destination Normal style.
            // WORDSNET-25741 We do not collapse attributes in Merger mode.
            if (!mContext.ImportFormatOptions.IsMerger)
                expandFlags |= RunPrExpandFlags.GlobalDefaults;

            // We treat attributes as 'final' when there is no formatting revision,
            // so we use 'revised' flag, because revision can be in a some parent style.
            if (!srcInline.RunPr_IInline.HasFormatRevision || isRevised)
                expandFlags |= RunPrExpandFlags.Revised;

            // When we import, for example, just some inline node, without its parent paragraph, we should not take into
            // account properties of that paragraph. When we copy in Word some text into clipboard without its paragraph mark,
            // and then paste this text into another place, Word does not take into account properties of that paragraph,
            // just because there are no any paragraphs in the clipboard.
            Paragraph parentPara = srcInline.ParentParagraph_IInline;
            bool isImportParentPara = (parentPara != null) && NodeUtil.IsAncestorOrSelf(parentPara, mContext.TopmostNode);
            expandFlags |= (isImportParentPara) ? RunPrExpandFlags.DocumentDefaults : RunPrExpandFlags.NoParaStyle;

            bool isImportParentTable = isImportParentPara && (parentPara.ParentTable != null)
                && NodeUtil.IsAncestorOrSelf(parentPara.ParentTable, mContext.TopmostNode);
            if (!isImportParentTable)
                expandFlags |= RunPrExpandFlags.NoTableStyle;

            // WORDSNET-28135 Ignore Hyperlink style in TOC for Merger feature.
            if (mContext.ImportFormatOptions.IsMerger && IsInToc && IsInHyperlink)
                expandFlags |= RunPrExpandFlags.IgnoreHyperlinkCharStyle;

            RunPr diffRunPr = srcInline.GetExpandedRunPr_IInline(expandFlags);

            mContext.ApplyThemeDifference(diffRunPr);

            // WORDSNET-17186 We should not apply Themes to a destination Normal attributes
            // even Themes are different to mimic Word behavior.
            RunPr dstNormalRunPr = GetNormalRunPr(expandFlags);

            // Mimic Word behavior and calculate difference with the destination Normal style.
            // It always will be Normal style, because after expanding source attributes,
            // Word removes style.
            // WORDSNET-25741 If destination document has default value in some direct Normal attribute
            // and the same value is in corresponding direct attribute of the source Node and this Node
            // is inside a Table with some TableStyle that also has this attribute set, then this
            // attribute will be lost in destination document due to collapsing with Normal style,
            // if same TableStyle in destination document has different value in corresponding attribute.
            // So, to preserve original formatting of the source document in Merger, we should not do collapse,
            // because in this mode we have to preserve exactly source formatting, but not to mimic Word.
            if (mContext.ImportFormatOptions.IsMerger)
            {
                // WORDSNET-26603 We preserve some styles in Merger, so destination
                // style will not be 'Normal' as usual and we have to check this case.
                int srcParaIstd = (int)srcInline.ParentParagraph_IInline.FetchParaAttr(ParaAttr.Istd, RevisionsView.Final);
                if (IsNonRemovableMergerIstd(srcParaIstd))
                {
                    int dstParaIstd = GetDstIstd(srcInline.ParentParagraph_IInline);
                    Style dstStyle = mContext.DstStyles.GetByIstd(dstParaIstd, false);
                    if (dstStyle != null)
                        dstNormalRunPr = dstStyle.GetExpandedRunPr(expandFlags);
                }

                foreach (int key in dstNormalRunPr.GetKeys())
                    diffRunPr.SetAttrNoOverride(key, RunPr.GetDefaultAttr(key));
            }
            else
            {
                // WORDSNET-27678 Don't collapse revised attributes that also exist
                // in before-changes attributes over a destination Normal style,
                // as we keep in model only difference between before and after changes.
                int[] keysToIgnore = (isRevised) ? srcInline.RunPr_IInline.GetKeys() : new int[] {};
                diffRunPr.Collapse(dstNormalRunPr, keysToIgnore);
            }

            diffRunPr.Remove(FontAttr.Istd);

            // Restore non-collapsible attributes. See StyleCollection.CopyRunPr() for details.
            RunPr srcRunPr = isRevised ? (RunPr)srcInline.RunPr_IInline.FormatRevision.RevPr : srcInline.RunPr_IInline;
            srcRunPr.MirrorTo(diffRunPr, RunPr.NonCollapsibleFontKeys);

            if (isImportParentTable)
            {
                // Mimic Word behavior: remove attributes, which are equal to the attributes of the imported
                // parent table style when it is not overriden by Normal style attributes.
                // See TestJira14587ParentTableStyleRemoveEqualAttrs() and Test23366() for example.
                Style dstTableStyle = ImportParentTableStyle(parentPara);
                RemoveEqualAttrs(diffRunPr, dstTableStyle, isRevised);
            }

            return diffRunPr;
        }

        /// <summary>
        /// Returns ParaPr that contains attributes difference of the specified
        /// <paramref name="srcPara"/> in source and destination documents.
        /// </summary>
        private ParaPr GetParaPrDiff(Paragraph srcPara, bool isRevised)
        {
            // WORDSNET-20066 Global defaults are excluded from the expanding.
            // The global default properties remain in the model, but so far there is no problem.
            ParaPrExpandFlags expandFlags = ParaPrExpandFlags.Normal;

            if (srcPara.ParentTable != null)
            {
                // If we import only paragraph itself, without parent table,
                // we should ignore parent table properties on expand.
                if (NodeUtil.IsAncestorOrSelf(srcPara.ParentTable, mContext.TopmostNode))
                    expandFlags |= ParaPrExpandFlags.ExpandTableStyle;
            }

            // Defaults values of documents have to be taken in attention at any case (no matter
            // where paragraph placed - inside table or not). There is issue, when two documents have
            // different defaults  and table style in the source document does not override default
            // attributes. So, AW loses formatting, when defaults values of source document are not
            // used. For example, see TestJira16660.
            expandFlags |= ParaPrExpandFlags.DocumentDefaults;

            // We treat attributes as 'final' when there is no formatting revision,
            // so we use 'revised' flag, because revision can be in a some parent style.
            if (!srcPara.ParaPr.HasFormatRevision || isRevised)
                expandFlags |= ParaPrExpandFlags.Revised;

            ParaPr diffParaPr = srcPara.GetExpandedParaPr(expandFlags);

            // Calculate difference with the destination Normal style.
            // It always will be Normal style, because after expanding source attributes,
            // Word removes paragraph style.
            ParaPr dstNormalParaPr = GetNormalParaPr(expandFlags);
            // WORDSNET-25741 We do not collapse in Merger, see details in GetRunPrDiff().
            if (mContext.ImportFormatOptions.IsMerger)
            {
                // WORDSNET-26603 We preserve some styles in Merger, so destination
                // style will not be 'Normal' as usual and we have to check this case.
                if (IsNonRemovableMergerIstd(srcPara.ParaPr.Istd))
                {
                    // WORDSNET-26667. As we here because the style of the paragraph will not be reset to 'Normal' below,
                    // we have to replace it with the corresponding correct style from the destination document.
                    diffParaPr.Istd = GetDstIstd(srcPara);
                    Style dstStyle = mContext.DstStyles.GetByIstd(diffParaPr.Istd, false);
                    if (dstStyle != null)
                        dstNormalParaPr = dstStyle.GetExpandedParaPr(expandFlags);
                }

                foreach (int key in dstNormalParaPr.GetKeys())
                    diffParaPr.SetAttrNoOverride(key, ParaPr.FetchDefaultAttr(key));

                // WORDSNET-25832 Despite we expand Style into direct attributes, Word displays differently
                // paragraphs depending on either the following styles are set explicitly, or not.
                if (!IsNonRemovableMergerIstd(srcPara.ParaPr.Istd))
                {
                    diffParaPr.Remove(ParaAttr.Istd);
                }
                else if (StyleIndex.IsHeadingIstd(diffParaPr.Istd))
                {
                    // Actually, this does not affect output result, but we forced to do this to avoid
                    // failing Test25832. See DocumentPostLoader.FixUpParagraphOutlineLevel() for details.
                    diffParaPr.Remove(ParaAttr.OutlineLevel);
                }
            }
            else
            {
                diffParaPr.Collapse(dstNormalParaPr);
                diffParaPr.Remove(ParaAttr.Istd);
            }

            // Remove floating attributes in TextBoxes. See for details Paragraph.GetExpandedParaPr().
            if (srcPara.IsInShape)
                diffParaPr.RemoveFloatingAttrs();

            // Restore non-collapsible attribute. See StyleCollection.CopyParaPr() for details.
            ParaPr srcParaPr = isRevised ? (ParaPr)srcPara.ParaPr.FormatRevision.RevPr : srcPara.ParaPr;
            srcParaPr.MirrorTo(diffParaPr, gNonCollapsibleParaKeys);

            // Import list.
            ImportList(srcPara, diffParaPr);

            if ((expandFlags & ParaPrExpandFlags.ExpandTableStyle) != 0)
            {
                // Mimic Word behavior, see comments in GetRunPrDiff() for details.
                Style dstTableStyle = ImportParentTableStyle(srcPara);
                RemoveEqualAttrs(diffParaPr, dstTableStyle, isRevised);
            }

            return diffParaPr;
        }

        /// <summary>
        /// Returns RunPr that contains attributes difference for <see cref="Paragraph.ParagraphBreakRunPr"/>
        /// of the specified <paramref name="srcPara"/> in source and destination documents.
        /// </summary>
        private RunPr GetParagraphBreakRunPrDiff(Paragraph srcPara, bool isRevised)
        {
            // Expand with global defaults to get rid of extra global default properties
            // on collapsing with destination Normal style.
            RunPrExpandFlags expandFlags = RunPrExpandFlags.DocumentDefaults | RunPrExpandFlags.NoChangeCommentSize;
            // WORDSNET-25741 We do not collapse attributes in Merger mode.
            if (!mContext.ImportFormatOptions.IsMerger)
                expandFlags |= RunPrExpandFlags.GlobalDefaults;


            bool isImportParentTable = (srcPara.ParentTable != null) && NodeUtil.IsAncestorOrSelf(srcPara.ParentTable, mContext.TopmostNode);
            if (!isImportParentTable)
                expandFlags |= RunPrExpandFlags.NoTableStyle;

            // We treat attributes as 'final' when there is no formatting revision,
            // so we use 'revised' flag, because revision can be in a some parent style.
            if (!srcPara.ParagraphBreakRunPr.HasFormatRevision || isRevised)
                expandFlags |= RunPrExpandFlags.Revised;

            RunPr diffRunPr = srcPara.GetExpandedParagraphBreakRunPr(expandFlags);

            if (mContext.IsThemeFontsDifferent)
                Theme.Apply(mContext.SrcDoc.GetThemeInternal(), diffRunPr);

            // WORDSNET-17186 We should not apply Themes to a destination Normal attributes
            // even Themes are different to mimic Word behavior.
            RunPr dstNormalRunPr = GetNormalRunPr(expandFlags);

            // Mimic Word behavior and calculate difference with the destination Normal style.
            // It always will be Normal style, because after expanding source attributes,
            // Word removes style at the paragraph.
            // WORDSNET-25741 We do not collapse in Merger, see details in GetRunPrDiff().
            if (mContext.ImportFormatOptions.IsMerger)
            {
                // WORDSNET-26603 We preserve some styles in Merger, so destination
                // style will not be 'Normal' as usual and we have to check this case.
                int paraIstd = (int)srcPara.FetchParaAttr(ParaAttr.Istd, RevisionsView.Final);
                if (IsNonRemovableMergerIstd(paraIstd))
                {
                    Style dstStyle = mContext.DstStyles.GetByIstd(paraIstd, false);
                    if (dstStyle != null)
                        dstNormalRunPr = dstStyle.GetExpandedRunPr(expandFlags);
                }

                foreach (int key in dstNormalRunPr.GetKeys())
                    diffRunPr.SetAttrNoOverride(key, RunPr.GetDefaultAttr(key));
            }
            else
            {
                diffRunPr.Collapse(dstNormalRunPr);
            }

            diffRunPr.Remove(FontAttr.Istd);

            // Restore non-collapsible attributes. See StyleCollection.CopyRunPr() for details.
            RunPr srcBreakRunPr = isRevised ? (RunPr)srcPara.ParagraphBreakRunPr.FormatRevision.RevPr : srcPara.ParagraphBreakRunPr;
            srcBreakRunPr.MirrorTo(diffRunPr, RunPr.NonCollapsibleFontKeys);

            if (isImportParentTable)
            {
                // Mimic Word behavior, see comments in GetRunPrDiff() for details.
                Style dstTableStyle = ImportParentTableStyle(srcPara);
                RemoveEqualAttrs(diffRunPr, dstTableStyle, isRevised);
            }

            return diffRunPr;
        }

        /// <summary>
        /// Returns style from the <see cref="ImportContext.DstStyles"/> with the same name as <paramref name="srcStyle"/>.
        /// If there is no such style and it is <see cref="Style.BuiltIn"/> and it does not
        /// exist in destination collection, then creates new one from the scratch.
        /// Otherwise, imports <paramref name="srcStyle"/> into destination collection.
        /// </summary>
        private Style ImportOrCreateStyle(Style srcStyle)
        {
            Style dstStyle = mContext.DstStyles.GetByName(srcStyle.Name, false);
            if (dstStyle != null)
                return dstStyle;

            if (srcStyle.BuiltIn)
            {
                StyleCollection dstStyles = mContext.DstStyles;

                Style existingStyle = dstStyles.FindLocaleIndependentMatch(srcStyle);
                if (existingStyle != null)
                    return existingStyle;

                Style builtInStyle = dstStyles.BuiltInStyles.GetBySti(srcStyle.StyleIdentifier, false);
                return dstStyles.AddCopy(builtInStyle);
            }

            return mContext.DstStyles.ImportStyle(srcStyle);
        }

        /// <summary>
        /// Imports parent table style.
        /// </summary>
        private Style ImportParentTableStyle(Paragraph srcPara)
        {
            Style srcTableStyle = InlineHelper.GetTableStyle(srcPara);
            return (srcTableStyle == null) ? null : mContext.DstStyles.ImportStyle(mContext, srcTableStyle);
        }

        /// <summary>
        /// Imports list from a source paragraph to a specified destination paragraph properties.
        /// </summary>
        private void ImportList(Paragraph srcPara, ParaPr dstParaPr)
        {
            if (dstParaPr.Contains(ParaAttr.ListId))
            {
                // WORDSNET - 26044 We clone ParaPr containing the source difference attributes to avoid the
                // source ParaPr mutation during ImportList() process.
                ParaPr paraPrClone = dstParaPr.Clone();
                NodeImporter.ImportList(paraPrClone, dstParaPr, mContext);
            }
            else
            {
                // WORDSNET-27734 When there is no ListId set in direct attributes of a destination paragraph,
                // Word also checks if there is the same destination style of the imported paragraph and
                // if it is numbered, then collapses its numbering properties over a Normal style,
                // i.e., it sets listId = 1 and listLevel = 0 in this case.
                const int defaultListId = 1;
                const int defaultListLevel = 0;

                int dstIstd = GetDstIstd(srcPara);
                Style dstStyle = mContext.DstStyles.GetByIstd(dstIstd, false);
                if (dstStyle == null)
                    return;

                if (dstStyle.ParaPr.ListId == 0)
                    return;

                dstParaPr.ListId = (mContext.DstLists.GetListByListId(defaultListId) == null)
                    ? mContext.DstLists.Add(ListTemplate.NumberDefault).ListId
                    : defaultListId;

                dstParaPr.ListLevel = defaultListLevel;
            }
        }

        /// <summary>
        /// Removes attributes from <paramref name="attrs"/> that are equal to the attributes in <paramref name="tableStyle"/>
        /// and missing in <see cref="mNormal"/> style at the same time.
        /// </summary>
        private void RemoveEqualAttrs(AttrCollection attrs, Style tableStyle, bool isRevised)
        {
            if (tableStyle == null)
                return;

            bool isIgnoreNormalSizeAndJustification =
                InlineHelper.IsIgnoreNormalFontSize(mContext.DstDoc, (TableStyle)tableStyle);

            bool isRunAttrs = (attrs is RunPr);
            WordAttrCollection tableStyleAttrs = isRunAttrs ? tableStyle.RunPr : (WordAttrCollection)tableStyle.ParaPr;
            if (isRevised && tableStyleAttrs.HasFormatRevision)
                tableStyleAttrs = tableStyleAttrs.FormatRevision.RevPr;

            AttrCollection normalAttrs = null;
            if (mNormal != null)
                normalAttrs = isRunAttrs ? mNormal.RunPr : (AttrCollection)mNormal.ParaPr;

            for (int i = attrs.Count - 1; i >= 0; i--)
            {
                int key = attrs.GetKey(i);
                object tableVal = tableStyleAttrs[key];
                if (tableVal == null)
                    continue;

                object normalVal = (normalAttrs != null) ? normalAttrs[key] : null;
                // WORDSNET-19927 Process of removing equal attributes must consider compatibility
                // setting "overrideTableStyleFontSizeAndJustification".
                // WORDSNET-23366 Remove equal to table style attribute only when this attribute is not overriden by
                // the Normal style (because paragraph style has obviously precedence over a table style in hierarchy).
                if ((normalVal == null) ||
                    (isIgnoreNormalSizeAndJustification && IsIgnorableSizeAndJustification(key, normalVal)))
                {
                    if (tableVal.Equals(attrs[key]))
                        attrs.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Returns true, if specified key and value should be ignored in accordance with <see cref="InlineHelper.IsIgnoreNormalFontSize"/>.
        /// </summary>
        private static bool IsIgnorableSizeAndJustification(int key, object value)
        {
            switch (key)
            {
                case FontAttr.SizeBi:
                case FontAttr.Size:
                    return ((int)value == 24);

                case ParaAttr.Alignment:
                    return ((ParagraphAlignment)value == ParagraphAlignment.Left);

                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns RunPr from the cached Normal style with expanded attributes.
        /// </summary>
        private RunPr GetNormalRunPr(RunPrExpandFlags flags)
        {
            return ((flags & RunPrExpandFlags.DocumentDefaults) != 0) ? mExpNormalWithDefaults.RunPr : mExpNormal.RunPr;
        }

        /// <summary>
        /// Returns ParaPr from the cached Normal style with expanded attributes.
        /// </summary>
        private ParaPr GetNormalParaPr(ParaPrExpandFlags flags)
        {
            return ((flags & ParaPrExpandFlags.DocumentDefaults) != 0) ? mExpNormalWithDefaults.ParaPr : mExpNormal.ParaPr;
        }

        /// <summary>
        /// Returns true, if a specified istd should not be removed after style
        /// is expanded when calculating formatting difference for Merger.
        /// </summary>
        private bool IsNonRemovableMergerIstd(int istd)
        {
            if (StyleIndex.IsHeadingIstd(istd))
                return true;

            Style style = mContext.SrcStyles.GetBySti(StyleIdentifier.ListParagraph, false);
            if ((style) != null && (style.Istd == istd))
                return true;

            return false;
        }

        /// <summary>
        /// Returns istd of the specified source paragraph that it will have in a destination document after the import.
        /// </summary>
        private int GetDstIstd(Paragraph srcPara)
        {
            Debug.Assert(srcPara != null);

            // First check already imported styles.
            int dstIstd = mContext.ImportedIstds[srcPara.ParaPr.Istd];
            if (!IntToIntDictionary.IsNullSubstitute(dstIstd))
                return dstIstd;

            // Then check originally existed in destination document styles.
            Style dstStyleMatch = mContext.DstStyles.FindLocaleIndependentMatch(srcPara.ParagraphStyle);
            if (dstStyleMatch != null)
                return dstStyleMatch.Istd;

            // Actually, when we calculate formatting difference, the style should be either
            // imported already, or exists in a destination document originally.
            // But let's handle the almost impossible case as well by just returning 0.
            return 0;
        }

        /// <summary>
        /// Gets a boolean value indicating whether the currently processing Node
        /// in corresponding <see cref="NodeImporter"/> is inside a TOC field.
        /// </summary>
        private bool IsInToc
        {
            get
            {
                int count;
                mFieldsCount.TryGetValue(FieldType.FieldTOC, out count);

                return count > 0;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether the currently processing Node
        /// in corresponding <see cref="NodeImporter"/> is inside a HYPERLINK field.
        /// </summary>
        private bool IsInHyperlink
        {
            get
            {
                int count;
                mFieldsCount.TryGetValue(FieldType.FieldHyperlink, out count);

                return count > 0;
            }
        }

        /// <summary>
        /// Keeps number of a currently started in corresponding <see cref="NodeImporter"/> types of fields.
        /// Key: field type.
        /// Value: count.
        /// </summary>
        private readonly Dictionary<FieldType, int> mFieldsCount = new Dictionary<FieldType, int>();

        /// <summary>
        /// Importing context.
        /// </summary>
        private readonly ImportContext mContext;

        /// <summary>
        /// Non-collapsible paragraph attributes, <see cref="AttrCollection.IsIgnoredOnCollapse"/>.
        /// </summary>
        private static readonly int[] gNonCollapsibleParaKeys = { ParaAttr.RsidP, ParaAttr.Sys_Alignment97, ParaAttr.Sys_LeftIndent97, ParaAttr.Sys_RightIndent97, ParaAttr.Sys_FirstLineIndent97 };

        /// <summary>
        /// Revision attributes.
        /// </summary>
        private static readonly int[] gRevisionAttrs =
        {
            RevisionAttr.DeleteRevision,
            RevisionAttr.InsertRevision,
            RevisionAttr.MoveFromRevision,
            RevisionAttr.MoveToRevision,
            RevisionAttr.FormatRevision
        };

        /// <summary>
        /// Normal style of the destination document.
        /// </summary>
        private readonly Style mNormal;

        /// <summary>
        /// Normal style with expanded attributes.
        /// </summary>
        /// <remarks>This is a cached version of Normal style with expanded attributes that is very frequently used
        /// in formatting difference calculation.</remarks>
        private readonly Style mExpNormal = Style.Create(StyleType.Paragraph);

        /// <summary>
        /// Normal style with expanded attributes considering document Defaults.
        /// </summary>
        /// <remarks>This is a cached version of Normal style with expanded attributes that is very frequently used
        /// in formatting difference calculation.</remarks>
        private readonly Style mExpNormalWithDefaults = Style.Create(StyleType.Paragraph);
    }
}
