// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/08/2006 by Roman Korchagin

using System;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Revisions;
using Aspose.Words.Settings;
using Aspose.Words.Tables;

namespace Aspose.Words
{
    /// <summary>
    /// This class provides implementation for inline-content specific functions
    /// that must be available in the classes that are base classes for inline nodes:
    /// <see cref="Inline"/>, <see cref="InlineStory"/> and <see cref="Aspose.Words.Drawing.ShapeBase"/>.
    /// These functions are needed in all of the above classes, yet these classes have no suitable
    /// base class where I can put these functions. Therefore the common functions are here.
    /// </summary>
    internal static class InlineHelper
    {
        /// <summary>
        /// Gets character style for inline.
        /// </summary>
        /// <returns> Returns DefaultParagraphFont style if inline doesn't have character style assigned. </returns>
        internal static Style GetCharacterStyle(IInline inlineNode)
        {
            return GetCharacterStyle(inlineNode, RunPrExpandFlags.Normal, true);
        }

        /// <summary>
        /// Creates a fully expanded version of the RunPr object.
        /// </summary>
        internal static RunPr GetExpandedRunPr(IInline node, RunPrExpandFlags flags)
        {
            RunPr dstRunPr = new RunPr();
            ExpandRunPr(node, dstRunPr, flags);
            return dstRunPr;
        }

        /// <summary>
        /// Fully expands font properties of inline into direct formatting.
        /// </summary>
        internal static void ExpandRunPr(IInline inline, RunPr dstRunPr, RunPrExpandFlags flags)
        {
            ExpandRunPr(inline.Document_IInline, inline.ParentParagraph_IInline, inline.RunPr_IInline, dstRunPr, flags);
        }

        /// <summary>
        /// Fully expands font properties of paragraph break into direct formatting.
        /// </summary>
        internal static void ExpandRunPr(Paragraph para, RunPr dstRunPr, RunPrExpandFlags flags)
        {
            ExpandRunPr(para.Document, para, para.ParagraphBreakRunPr, dstRunPr, flags);
        }

        /// <summary>
        /// Fully expands font properties of this object into direct formatting.
        /// </summary>
        internal static void ExpandRunPr(DocumentBase doc, Paragraph parentPara, RunPr runPr, RunPr dstRunPr, RunPrExpandFlags flags)
        {
            StyleCollection styles = doc.Styles;

            Style charStyle = GetCharacterStyle(doc, runPr, flags, false);

            bool isExpandParaStyle = (flags & RunPrExpandFlags.NoParaStyle) == 0;
            Style paraStyle = isExpandParaStyle ? GetParaStyle(parentPara, flags) : styles.GetByIstd(StyleIndex.Normal, false);

            Style tableStyle = GetTableStyle(parentPara);

            RunPr directRunPr = runPr.GetSourceRunPr(flags);

            // when layout renders revisions it needs to know if run has them
            // however the GetSourceRunPr will accept revision and remove format revision from directRunPr
            // we need to keep this attribute in the result collection so that layout knows this run has one
            if ((flags & RunPrExpandFlags.Revised) != 0 && runPr.HasFormatRevision)
                dstRunPr.FormatRevision = (FormatRevision)((IComplexAttr)runPr.FormatRevision).DeepCloneComplexAttr();

            bool containsFontSizeNotInGlobalDefaults = dstRunPr.ContainsAnyKey(FontAttr.Size, FontAttr.SizeBi);
            bool directFormatting = ((flags & RunPrExpandFlags.NoDirectFormatting) == 0);

            // Reset flag immediately as we need to ignore direct formatting only for this entity.
            flags &= ~RunPrExpandFlags.NoDirectFormatting;

            bool isExpandDocumentDefaults = ((flags & RunPrExpandFlags.DocumentDefaults) != 0);
            RunPr defaults = isExpandDocumentDefaults ? styles.DefaultRunPr : new RunPr();

            containsFontSizeNotInGlobalDefaults |= defaults.ContainsAnyKey(FontAttr.Size, FontAttr.SizeBi);

            bool isExpandGlobalDefaults = ((flags & RunPrExpandFlags.GlobalDefaults) != 0);
            defaults.ExpandTo(dstRunPr, isExpandGlobalDefaults);

            // Expand font formatting from the table style if needed.
            if ((flags & RunPrExpandFlags.NoTableStyle) == 0)
            {
                if (parentPara != null)
                {
                    Cell cell = parentPara.ParentCell;
                    if (cell != null)
                    {
                        Table table = cell.ParentTable;
                        if (table != null)
                        {
                            tableStyle = tableStyle as TableStyle;
                            if (tableStyle != null)
                            {
                                ((TableStyle)tableStyle).ExpandRunPr(cell, dstRunPr);
                                containsFontSizeNotInGlobalDefaults |= tableStyle.RunPr.ContainsAnyKey(FontAttr.Size, FontAttr.SizeBi);
                            }
                        }
                    }
                }
            }

            if (paraStyle != null)
            {
                // WORDSNET-5894 Use Normal style in case Character style is applied to paragraph.
                if (paraStyle.Type == StyleType.Character)
                    paraStyle = styles.GetBySti(StyleIdentifier.Normal, false);

                if (containsFontSizeNotInGlobalDefaults && IsIgnoreNormalFontSize(doc, tableStyle as TableStyle))
                    flags |= RunPrExpandFlags.IgnoreNormalFontSize;

                // IN. The defaults have been expanded already and it might been overridden with table style above.
                // For sure, we do not want to override it again with the defaults during paragraph style expanding.
                // So, clear corresponding flags.
                RunPrExpandFlags paraFlags = flags & (~RunPrExpandFlags.DocumentDefaults & ~RunPrExpandFlags.GlobalDefaults);
                paraStyle.ExpandRunPr(dstRunPr, paraFlags);
            }

            // WORDSNET-6033 Paragraph style (part of linked style) assigned to run. Ignore such style.
            if ((charStyle != null) && (charStyle.Type != StyleType.Paragraph))
            {
                bool isIgnoreHyperlinkCharStyle = ((flags & RunPrExpandFlags.IgnoreHyperlinkCharStyle) != 0);
                bool isIgnoreCharStyle = (isIgnoreHyperlinkCharStyle && (charStyle.StyleIdentifier == StyleIdentifier.Hyperlink));
                if (!isIgnoreCharStyle)
                {
                    // WORDSNET-25424 Expand style revised properties if requested.
                    bool isRevised = (flags & RunPrExpandFlags.Revised) != 0;
                    charStyle.ExpandRunPr(dstRunPr, isRevised ? RunPrExpandFlags.Revised : RunPrExpandFlags.Normal);
                }
                else
                {
                    // WORDSNET-14332 Most likely not all hyperlink char style attributes have to be ignored.
                    // Mimic MSW and set destination AllCaps to true if AllCaps or SmallCaps of hyperlink style are equals true.
                    if (!dstRunPr.Contains(FontAttr.AllCaps) && charStyle.Font.AllCaps ||
                        (!dstRunPr.Contains(FontAttr.SmallCaps) && charStyle.Font.SmallCaps))
                        dstRunPr.SetAttr(FontAttr.AllCaps, AttrBoolEx.True);

                    // WORDSNET-24506 Font family of the Hyperlink style shouldn't be ignored
                    foreach (int attribute in RunPr.FontNameAttributes)
                    {
                        object value = charStyle.GetFontAttr(attribute, false);
                        if (value != null)
                            dstRunPr.SetAttr(attribute, value);
                    }
                }
            }

            // Most likely format revision may reference different style, etc. and thus we must do more magic in the lines above.
            // This code works only for paragraph nodes when we need to know formatting of the paragraph break.
            // For inline nodes caller has already removed format revision (but we already recorded it in dstRunPr)
            //
            // IN. If NoDirectFormatting flag is set, then do not expand direct attributes.
            if (directFormatting)
            {
                if (directRunPr.HasFormatRevision && (flags & RunPrExpandFlags.Revised) != 0)
                {
                    RunPr src = directRunPr.Clone();
                    src.AcceptFormatRevision();
                    src.ExpandTo(dstRunPr);

                    // We must copy insert/delete/format revision attributes to the destination
                    // because code needs to know if run must be rendered accordingly.
                    if (directRunPr.HasDeleteRevision)
                        dstRunPr.SetAttr(RevisionAttr.DeleteRevision, ((IComplexAttr)directRunPr.DeleteRevision).DeepCloneComplexAttr());
                    if (directRunPr.HasInsertRevision)
                        dstRunPr.SetAttr(RevisionAttr.InsertRevision, ((IComplexAttr)directRunPr.InsertRevision).DeepCloneComplexAttr());
                    if (directRunPr.HasFormatRevision)
                        dstRunPr.SetAttr(RevisionAttr.FormatRevision, ((IComplexAttr)directRunPr.FormatRevision).DeepCloneComplexAttr());
                    if (directRunPr.HasMoveFromRevision)
                        dstRunPr.SetAttr(RevisionAttr.MoveFromRevision, ((IComplexAttr)directRunPr.MoveFromRevision).DeepCloneComplexAttr());
                    if (directRunPr.HasMoveToRevision)
                        dstRunPr.SetAttr(RevisionAttr.MoveToRevision, ((IComplexAttr)directRunPr.MoveToRevision).DeepCloneComplexAttr());
                }
                else
                {
                    directRunPr.ExpandTo(dstRunPr);

                    directRunPr.ThemeColorInheritanceHack(dstRunPr);
                }
            }

            ExcludeSolidTextFillInheritance(dstRunPr, directRunPr);

            // Special handling for run in comments.
            bool isChangeCommentsSize = (flags & RunPrExpandFlags.NoChangeCommentSize) == 0;
            if (isChangeCommentsSize && ((parentPara != null) && (parentPara.GetAncestor(NodeType.Comment) != null)))
            {
                Style commentStyle = styles.GetBySti(StyleIdentifier.BalloonText, false);
                if (commentStyle != null)
                {
                    commentStyle.RunPr.MirrorTo(dstRunPr, FontAttr.Size);
                    commentStyle.RunPr.MirrorTo(dstRunPr, FontAttr.SizeBi);
                }
            }
        }

        // WORDSNET-23840
        // It appears that MS Word has some illogical exception regarding attribute inheritance:
        // TextFill effect with solid color should not be inherited if the local color is specified (including auto value).
        // Local TextFill effect still overrides a local color.
        private static void ExcludeSolidTextFillInheritance(RunPr dstRunPr, RunPr directRunPr)
        {
            if (dstRunPr.ContainsKey(FontAttr.EffectFill) &&
                directRunPr.ContainsKey(FontAttr.Color) &&
                !directRunPr.ContainsKey(FontAttr.EffectFill))
            {
                DmlSolidFill solidTextFill = dstRunPr[FontAttr.EffectFill] as DmlSolidFill;
                if (solidTextFill != null)
                    dstRunPr.Remove(FontAttr.EffectFill);
            }
        }

        /// <summary>
        /// Returns either paragraph style or revised paragraph style.
        /// </summary>
        internal static Style GetParaStyle(Paragraph para, RunPrExpandFlags flags)
        {
            if (para == null)
                return null;

            // In order to get revised run properties we must use revised paragraph style for it.
            // WORDSNET-24396 Handle also AfterChanges flag to get proper style while calculating positive difference.
            RevisionsView revisionsView = ((flags & RunPrExpandFlags.Revised) != 0) ||
                                          ((flags & RunPrExpandFlags.AfterChanges) != 0)
                ? RevisionsView.Final
                : RevisionsView.Original;

            return para.GetParagraphStyle(revisionsView);
        }

        /// <summary>
        /// Returns table style.
        /// </summary>
        internal static Style GetTableStyle(Paragraph para)
        {
            if (para == null)
                return null;

            return (para.IsInCell &&
                   // WORDSNET-11447 IsInCell is true, but ParentTable is null when row or cell is cloned.
                   (para.ParentTable != null))
                    ? para.ParentTable.Style
                    : null;
        }

        /// <summary>
        /// Helps to calculate the full value of a single character formatting attribute.
        /// </summary>
        internal static object FetchInheritedAttr(IInline inline, int fontAttr)
        {
            // This method is called when a direct value for this attribute is not specified.

            // Try to find the value specified in the character style.
            Style charStyle = GetCharacterStyle(inline, RunPrExpandFlags.Normal, false);

            // WORDSNET-19994 Do not consider a character style whose type isn't equal to 'StyleType.Character'.
            object value = ((charStyle != null) && (charStyle.Type == StyleType.Character))
                ? charStyle.GetFontAttr(fontAttr, false)
                : null;

            if (value != null)
            {
                if (value is AttrBoolEx)
                {
                    // WORDSNET-27952 Toggle attributes not resolved correctly when defined in Character style and Paragraph style.
                    // DM This is a *partial* fix to resolve between toggle attributes in Character style and Paragraph style
                    // according to ECMA-376 17.7.3. Table styles are ignored.
                    value = ResolveToggleAttribute(inline, fontAttr, (AttrBoolEx)value);
                }
            }
            else
            {
                // Next, get the value from the paragraph style.
                value = GetAttrFromParagraphStyle(inline, fontAttr);

                // WORDSNET-11068 After all get value from table style.
                if ((value == null) && IsInTable(inline))
                    value = GetAttrFromTableStyle(inline, fontAttr);
            }

            // Get the value from document defaults defined in styles collection.
            if (value == null)
                value = inline.Document_IInline.Styles.DefaultRunPr.GetDirectAttr(fontAttr);

            // WORDSNET-25882 Get the value from shape style.
            if ((value == null) && (fontAttr == FontAttr.Color) && IsInShape(inline))
                value = GetFontColorFromShapeStyle(inline);

            // Return the value taken from global application defaults if it's null.
            return (value != null)
                ? value
                : RunPr.GetDefaultAttr(fontAttr);
        }

        /// <summary>
        /// Gets font color defined in shape style.
        /// </summary>
        private static DrColor GetFontColorFromShapeStyle(IInline inline)
        {
            Shape shape = (Shape)inline.ParentParagraph_IInline.FirstNonMarkupParentNode;
            DmlShape dmlShape = shape.DmlNode as DmlShape;
            if ((dmlShape == null) || (dmlShape.Style == null) || dmlShape.Style.FontReference.IsEmpty)
                return null;

            DocumentBase doc = inline.Document_IInline;
            return dmlShape.Style.FontReference.Color.CreateDrColor(doc.GetThemeInternal(), null);
        }

        /// <summary>
        /// Gets character style for inline or paragraph break.
        /// </summary>
        /// <param name="inline">Inline</param>
        /// <param name="flags">Should be used to get either revised or not character style.</param>
        /// <param name="fetchDefault">If this is set true method returns DefaultParagraphFont in case inline doesn't have character style assigned.</param>
        /// <returns></returns>
        private static Style GetCharacterStyle(IInline inline, RunPrExpandFlags flags, bool fetchDefault)
        {
            Style charStyle = null;
            RunPr runPr = inline.RunPr_IInline;
            DocumentBase doc = inline.Document_IInline;

            if (fetchDefault)
            {
                int istd = (int)runPr.FetchAttr(FontAttr.Istd, flags);
                charStyle = doc.Styles.FetchByIstd(istd, StyleIndex.DefaultParagraphFont);
            }
            else
            {
                object istdVal = runPr.GetDirectAttr(FontAttr.Istd, flags);
                if (null != istdVal)
                    charStyle = doc.Styles.GetByIstd((int)istdVal, false);
            }

            return charStyle;
        }

        private static Style GetCharacterStyle(DocumentBase doc, RunPr runPr, RunPrExpandFlags flags, bool fetchDefault)
        {
            Style charStyle = null;

            if (fetchDefault)
            {
                int istd = (int)runPr.FetchAttr(FontAttr.Istd, flags);
                charStyle = doc.Styles.FetchByIstd(istd, StyleIndex.DefaultParagraphFont);
            }
            else
            {
                object istdVal = runPr.GetDirectAttr(FontAttr.Istd, flags);
                if (null != istdVal)
                    charStyle = doc.Styles.GetByIstd((int)istdVal, false);
            }

            return charStyle;
        }

        private static bool IsInTable(IInline inline)
        {
            Paragraph parentPara = inline.ParentParagraph_IInline;
            return (parentPara != null) && (parentPara.ParentTable != null);
        }

        /// <summary>
        /// Returns a value indicating whether the specified inline node is located in a shape.
        /// </summary>
        private static bool IsInShape(IInline inline)
        {
            Paragraph parentPara = inline.ParentParagraph_IInline;
            return (parentPara != null) && parentPara.IsInShape;
        }

        /// <summary>
        /// Resolves toggle attributes defined at multiple levels of styles hierarchy.
        /// </summary>
        /// <remarks>Currently, table styles are not supported.</remarks>
        /// <param name="inline"></param>
        /// <param name="fontAttr"></param>
        /// <param name="charStyleAttr"></param>
        /// <returns></returns>
        private static AttrBoolEx ResolveToggleAttribute(IInline inline, int fontAttr, AttrBoolEx charStyleAttr)
        {
            Debug.Assert(charStyleAttr != null);

            AttrBoolEx resolvedAttr = charStyleAttr;
            AttrBoolEx paraStyleAttr = (AttrBoolEx)GetAttrFromParagraphStyle(inline, fontAttr);

            // ECMA-376 17.7.3: If the value of the toggle property appears at multiple levels of the style hierarchy...
            // This means attribute is defined both in char style and in paragraph style.
            // Note: We do not support attributes from table styles.
            if ((paraStyleAttr != null) && (charStyleAttr != null))
            {
                AttrBoolEx defaultAttr = (AttrBoolEx)GetDocumentDefaultAttr(inline, fontAttr);
                if (defaultAttr == AttrBoolEx.True)
                {
                    // ECMA-376 17.7.3: ...If the value specified by the document defaults is true, the effective value is true...
                    resolvedAttr = defaultAttr;
                }
                else
                {
                    // ECMA-376 17.7.3: ...Otherwise, the values are combined by a Boolean XOR...
                    // Note: We do not support attributes from table styles, so XOR char and para styles only.
                    resolvedAttr = (charStyleAttr == paraStyleAttr)
                        ? AttrBoolEx.False
                        : AttrBoolEx.True;
                }
            }

            return resolvedAttr;
        }

        /// <summary>
        /// Gets the attribute from the parent paragraph style inheritance tree.
        /// </summary>
        /// <param name="inline"></param>
        /// <param name="fontAttr"></param>
        /// <returns>Returns null if the attribute is not defined at paragraph style level.</returns>
        private static object GetAttrFromParagraphStyle(IInline inline, int fontAttr)
        {
            Paragraph parentPara = inline.ParentParagraph_IInline;

            if (parentPara == null)
                return null;

            Style style = parentPara.ParagraphStyle;
            if ((style.StyleIdentifier == StyleIdentifier.Normal) && IsSizeKey(fontAttr))
            {
                object value = GetTableOrDefaultFontSize(parentPara, fontAttr);
                if (value != null)
                    return value;
            }

            return style.GetFontAttr(fontAttr, false);
        }

        private static bool IsSizeKey(int key)
        {
            return (key == FontAttr.Size) || (key == FontAttr.SizeBi);
        }

        /// <summary>
        /// Returns font size considering 'ignore default font size' feature or null if option should not be applied.
        /// </summary>
        // See https://msdn.microsoft.com/en-us/library/dd944611%28v=office.12%29.aspx?f=255&MSPPError=-2147217396
        private static object GetTableOrDefaultFontSize(Paragraph para, int key)
        {
            DocumentBase doc = para.Document;

            Table table = para.ParentTable;
            if (table == null)
                return null;

            Style paraStyle = para.ParagraphStyle;

            object value = null;
            TableStyle tableStyle = doc.Styles.GetByIstd(table.Istd, false) as TableStyle;
            if (tableStyle == null)
                return null;

            // if Normal font size should be ignored,
            // try to get it from table style then from document defaults and finally from paragraph style.
            if (IsIgnoreNormalFontSize(doc, tableStyle) && (paraStyle.RunPr.Contains(key) && ((int)paraStyle.RunPr[key] == 24)))
            {
                value = ((IRunAttrSource)tableStyle).GetDirectRunAttr(key);

                if (value == null)
                    value = tableStyle.GetInheritedFontAttr(key, false);

                if (value == null)
                    value = doc.Styles.DefaultRunPr[key];

                if (IsSizeKey(key) && (value != null) && ((int)value == 20))
                    value = null;
            }

            return value;
        }

        /// <summary>
        /// Gets attribute value from table style.
        /// </summary>
        private static object GetAttrFromTableStyle(IInline inline, int key)
        {
            Table parentTable = inline.ParentParagraph_IInline.ParentTable;

            object value = null;
            if (inline.Document_IInline.Styles.GetByIstd(parentTable.Istd, false) != null)
            {
                TableStyle tableStyle = parentTable.Style as TableStyle;
                Cell cell = inline.ParentParagraph_IInline.ParentCell;

                if((tableStyle != null) && (cell != null))
                    value = tableStyle.GetRunAttr(key, cell);
            }

            return value;
        }

        /// <summary>
        /// Gets the default attribute value.
        /// The value is taken from the document defaults defined in the styles collection, or from global defaults.
        /// </summary>
        private static object GetDocumentDefaultAttr(IInline inline, int fontAttr)
        {
            // First, get from document defaults defined in styles collection.
            object attrValue = inline.Document_IInline.Styles.DefaultRunPr.GetDirectAttr(fontAttr);

            if (attrValue == null)
            {
                // Get from the global application defaults.
                attrValue = RunPr.GetDefaultAttr(fontAttr);
            }

            return attrValue;
        }

        /// <summary>
        /// Checks if font size specified on Normal paragraph style should be ignored.
        /// </summary>
        internal static bool IsIgnoreNormalFontSize(DocumentBase doc, TableStyle tableStyle)
        {
            if (tableStyle == null)
                return false;

            if (tableStyle.Istd == StyleIndex.TableNormal)
                return false;

            // Accordingly to the spec for 'overrideTableStyleFontSizeAndJustification' option
            // in https://msdn.microsoft.com/en-us/library/dd944611%28v=office.12%29.aspx?f=255&MSPPError=-2147217396
            // if the default paragraph style (as specified in [ISO/IEC29500-1:2012] section 17.7.4.17)
            // specifies a font size of 11pt or 12pt, then that setting will not override
            // the font size specified by the table style for paragraphs in tables.
            // Although spec says about the 11pt font size, it seems that Word applies this rule only for the font size 12pt.
            CustomCompatibilitySetting overrideTableStyle = doc.DocPr.CompatibilityOptions.CustomCompatibilitySettings["overrideTableStyleFontSizeAndJustification"];
            // By default this setting is false.
            return ((overrideTableStyle == null) || (overrideTableStyle.Value == "0"));
        }

        internal static bool IsInsertRevision(IInline inline)
        {
            return (inline.RunPr_IInline.HasInsertRevision);
        }

        internal static bool IsDeleteRevision(IInline inline)
        {
            return (inline.RunPr_IInline.HasDeleteRevision);
        }

        internal static bool IsMoveFromRevision(IInline inline)
        {
            return (inline.RunPr_IInline.HasMoveFromRevision);
        }

        internal static bool IsMoveToRevision(IInline inline)
        {
            return (inline.RunPr_IInline.HasMoveToRevision);
        }

        internal static bool IsFormatRevision(IInline inline)
        {
            return (inline.RunPr_IInline.HasFormatRevision);
        }

        /// <summary>
        /// Returns resolved value of the specified attribute. This is used by AE to get Hidden attr value.
        /// </summary>
        internal static object FetchAttr(IRunAttrSource src, int key, RevisionsView revisionsView)
        {
            // This happens if we try to get value from the node which is not IRunAttrSource.
            if (null == src)
                return null;

            object value = src.GetDirectRunAttr(key, revisionsView);

            // The value is not specified directly, obtain it from the parent.
            if (value == null)
                return src.FetchInheritedRunAttr(key);

            // The value is specified directly, but might need to be resolved using parent values.
            if (value is AttrBoolEx)
                return ((AttrBoolEx)value).ResolveFetchInheritedRunAttr(src, key);

            //The value is specified and does not need any special handling.
            return value;
        }

        /// <summary>
        /// Returns resolved value of the specified attribute. This is used by AE to get Hidden attr value.
        /// </summary>
        internal static object FetchAttr(IRunAttrSource src, int key)
        {
            return FetchAttr(src, key, RevisionsView.Original);
        }

        /// <summary>
        /// Returns a resolved value of the specified AttrBoolEx attribute.
        /// </summary>
        /// <remarks>
        /// It will throw on invalid cast if the attribute value is not AttrBoolEx.
        /// </remarks>
        internal static bool GetBool(IRunAttrSource src, int key)
        {
            return ((AttrBoolEx)FetchAttr(src, key)).ToBool();
        }

        /// <summary>
        /// Indicates that inline has character category hint set.
        /// </summary>
        internal static bool HasCharacterCategoryHint(IRunAttrSource src)
        {
            return src.GetDirectRunAttr(FontAttr.CharacterCategoryHint) != null;
        }

        /// <summary>
        /// Returns top visible shading in hierarchy of properties.
        /// </summary>
        internal static Shading TopVisibleShading(IRunAttrSource src)
        {
            // 1. Check shading for the run text.
            Shading shading = (Shading)FetchAttr(src, FontAttr.Shading);
            if (shading.IsVisible)
                return shading;

            // This can be either inline or style.
            if (src is IParaAttrSource)
            {
                // For style we should process it's IParaAttrSource.
                IParaAttrSource paraAttrs = (IParaAttrSource)src;

                shading = (Shading)paraAttrs.FetchParaAttr(ParaAttr.Shading);
                if (shading.IsVisible)
                    return shading;
            }
            else if (src is IInline)
            {
                // For inline continue with parent paragraph.
                Paragraph parentPara = ((IInline)src).ParentParagraph_IInline;

                // 2. If text has no shading, then try to get it from the parent paragraph.
                if (parentPara != null)
                {
                    shading = (Shading)((IParaAttrSource)parentPara).FetchParaAttr(ParaAttr.Shading);
                    if (shading.IsVisible)
                        return shading;

                    // WORDSNET-27432 Try to get shading from the parent Shape, if any.
                    Shape parentShape = parentPara.FirstNonMarkupParentNode as Shape;
                    if ((parentShape != null) && (parentShape.MarkupLanguage == ShapeMarkupLanguage.Dml))
                    {
                        throw new NotSupportedException("FOSS");
                    }

                    // 3. If parent paragraph has no shading, then try to get it from the parent cell, if any.
                    if (parentPara.IsInCell)
                    {
                        shading = (Shading)((ICellAttrSource)parentPara.ParentCell).FetchCellAttr(CellAttr.Shading);
                        if (shading.IsVisible)
                            return shading;
                    }

                    // 4. At last if document has page background then try to get shading from it.
                    if (parentPara.Document.BackgroundShape != null)
                    {
                        throw new NotSupportedException("FOSS");
                    }
                }
            }

            return null;
        }
    }
}
