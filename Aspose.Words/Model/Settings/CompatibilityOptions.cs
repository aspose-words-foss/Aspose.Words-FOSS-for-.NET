// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/05/2006 by Roman Korchagin

using System;
using System.Collections.Generic;
using Aspose.Words.Saving;

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Contains compatibility options (that is, the user preferences entered on the <b>Compatibility</b>
    /// tab of the <b>Options</b> dialog in Microsoft Word).
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/detect-file-format-and-check-format-compatibility/">Detect File Format and Check Format Compatibility</a> documentation article.</para>
    /// </summary>
    /// <seealso cref="Document"/>
    /// <seealso cref="Document.CompatibilityOptions"/>
    public class CompatibilityOptions
    {
        /// <summary>
        /// <para>Allows to optimize the document contents as well as default Aspose.Words behavior to a particular versions of MS Word.</para>
        /// <para>Use this method to prevent MS Word from displaying "Compatibility mode" ribbon upon document loading.
        /// (Note that you may also need to set the <see cref="OoxmlSaveOptions.Compliance"/> property to
        /// <see cref="OoxmlCompliance.Iso29500_2008_Transitional"/> or higher.)
        /// </para>
        /// </summary>
        public void OptimizeFor(MsWordVersion version)
        {
            Clear();
            switch (version)
            {
                case MsWordVersion.Word2000:
                    mMsWordVersion = MsWordVersionCore.Word2000;
                    InitWord2000Options(this);
                    break;
                case MsWordVersion.Word2002:
                    mMsWordVersion = MsWordVersionCore.Word2002;
                    InitWord2002Options(this);
                    break;
                case MsWordVersion.Word2003:
                    mMsWordVersion = MsWordVersionCore.Word2003;
                    InitWord2003Options(this);
                    break;
                case MsWordVersion.Word2007:
                    mMsWordVersion = MsWordVersionCore.Word2007;
                    // 2007 options already set by default.
                    break;
                case MsWordVersion.Word2010:
                    mMsWordVersion = MsWordVersionCore.Word2010;
                    InitWord2010Options(this);
                    break;
                case MsWordVersion.Word2013:
                    mMsWordVersion = MsWordVersionCore.Word2013;
                    InitWord2013Options(this);
                    break;
                case MsWordVersion.Word2016:
                    mMsWordVersion = MsWordVersionCore.Word2016;
                    InitWord2016Options(this);
                    break;
                case MsWordVersion.Word2019:
                    mMsWordVersion = MsWordVersionCore.Word2019;
                    InitWord2019Options(this);
                    break;
                default:
                    // This should never happen, so report it if it does.
                    throw new NotImplementedException("Please report exception.");
            }

            mIsOptimized = true;
        }

        /// <summary>
        /// Ctor. The options are initialized to Word 2007 default values.
        /// </summary>
        internal CompatibilityOptions()
        {
            InitInheritedCompatibilityOptions();
        }

        private void SetProperty(Compatibility key, bool value)
        {
            mCompatibilityOptions[key] = value;
        }

        private bool GetProperty(Compatibility key)
        {
            bool value;

            if (mCompatibilityOptions.TryGetValue(key, out value))
                return value;

            if ((mCompatibilityOptions.Count == 0) &&
                (mInheritedCompatibilityOptions.TryGetValue(key, out value)))
                return value;

            return false;
        }

        internal CompatibilityOptions Clone()
        {
            CompatibilityOptions lhs = (CompatibilityOptions)MemberwiseClone();
            lhs.mCustomSettings = mCustomSettings.Clone();
            return lhs;
        }

        /// <summary>
        /// Do Not Create Custom Tab Stop for Hanging Indent.
        /// </summary>
        public bool NoTabHangInd
        {
            get { return GetProperty(Compatibility.NoTabHangInd); }
            set { SetProperty(Compatibility.NoTabHangInd, value); }
        }

        /// <summary>
        /// Do Not Increase Line Height for Raised/Lowered Text.
        /// </summary>
        public bool NoSpaceRaiseLower
        {
            get { return GetProperty(Compatibility.NoSpaceRaiseLower); }
            set { SetProperty(Compatibility.NoSpaceRaiseLower, value); }
        }

        /// <summary>
        /// Do Not Use Space Before On First Line After a Page Break.
        /// </summary>
        public bool SuppressSpBfAfterPgBrk
        {
            get { return GetProperty(Compatibility.SuppressSpBfAfterPgBrk); }
            set { SetProperty(Compatibility.SuppressSpBfAfterPgBrk, value); }
        }

        /// <summary>
        /// Line Wrap Trailing Spaces.
        /// </summary>
        public bool WrapTrailSpaces
        {
            get { return GetProperty(Compatibility.WrapTrailSpaces); }
            set { SetProperty(Compatibility.WrapTrailSpaces, value); }
        }

        /// <summary>
        /// Print Colors as Black And White without Dithering.
        /// </summary>
        public bool PrintColBlack
        {
            get { return GetProperty(Compatibility.PrintColBlack); }
            set { SetProperty(Compatibility.PrintColBlack, value); }
        }

        /// <summary>
        /// Do Not Balance Text Columns within a Section.
        /// </summary>
        public bool NoColumnBalance
        {
            get { return GetProperty(Compatibility.NoColumnBalance); }
            set { SetProperty(Compatibility.NoColumnBalance, value); }
        }

        /// <summary>
        /// Treat Backslash Quotation Delimiter as Two Quotation Marks.
        /// </summary>
        public bool ConvMailMergeEsc
        {
            get { return GetProperty(Compatibility.ConvMailMergeEsc); }
            set { SetProperty(Compatibility.ConvMailMergeEsc, value); }
        }

        /// <summary>
        /// Ignore Minimum and Exact Line Height for First Line on Page.
        /// </summary>
        public bool SuppressTopSpacing
        {
            get { return GetProperty(Compatibility.SuppressTopSpacing); }
            set { SetProperty(Compatibility.SuppressTopSpacing, value); }
        }

        /// <summary>
        /// Use Simplified Rules For Table Border Conflicts.
        /// </summary>
        public bool UseSingleBorderforContiguousCells
        {
            get { return GetProperty(Compatibility.UseSingleBorderforContiguousCells); }
            set { SetProperty(Compatibility.UseSingleBorderforContiguousCells, value); }
        }

        /// <summary>
        /// Specifies not to blank the area behind metafile pictures.
        /// </summary>
        public bool TransparentMetafiles
        {
            get { return GetProperty(Compatibility.TransparentMetafiles); }
            set { SetProperty(Compatibility.TransparentMetafiles, value); }
        }

        /// <summary>
        /// Display Page/Column Breaks Present in Frames.
        /// </summary>
        public bool ShowBreaksInFrames
        {
            get { return GetProperty(Compatibility.ShowBreaksInFrames); }
            set { SetProperty(Compatibility.ShowBreaksInFrames, value); }
        }

        /// <summary>
        /// Swap Paragraph Borders on Odd Numbered Pages.
        /// </summary>
        public bool SwapBordersFacingPgs
        {
            get { return GetProperty(Compatibility.SwapBordersOddFacingPgs); }
            set { SetProperty(Compatibility.SwapBordersOddFacingPgs, value); }
        }

        /// <summary>
        /// Convert Backslash To Yen Sign When Entered.
        /// </summary>
        public bool DoNotLeaveBackslashAlone
        {
            get { return GetProperty(Compatibility.DoNotLeaveBackslashAlone); }
            set { SetProperty(Compatibility.DoNotLeaveBackslashAlone, value); }
        }

        /// <summary>
        /// Don't Justify Lines Ending in Soft Line Break.
        /// </summary>
        public bool DoNotExpandShiftReturn
        {
            get { return GetProperty(Compatibility.DoNotExpandOnShiftReturn); }
            set { SetProperty(Compatibility.DoNotExpandOnShiftReturn, value); }
        }

        /// <summary>
        /// Underline All Trailing Spaces.
        /// </summary>
        public bool UlTrailSpace
        {
            get { return GetProperty(Compatibility.UlTrailSpace); }
            set { SetProperty(Compatibility.UlTrailSpace, value); }
        }

        /// <summary>
        /// Balance Single Byte and Double Byte Characters.
        /// </summary>
        public bool BalanceSingleByteDoubleByteWidth
        {
            get { return GetProperty(Compatibility.BalanceSingleByteDoubleByteWidth); }
            set { SetProperty(Compatibility.BalanceSingleByteDoubleByteWidth, value); }
        }

        /// <summary>
        /// Ignore Minimum Line Height for First Line on Page.
        /// </summary>
        public bool SuppressSpacingAtTopOfPage
        {
            get { return GetProperty(Compatibility.SuppressTopSpacingAtTopOfPage); }
            set { SetProperty(Compatibility.SuppressTopSpacingAtTopOfPage, value); }
        }

        /// <summary>
        /// Only Expand/Condense Text By Whole Points.
        /// </summary>
        public bool SpacingInWholePoints
        {
            get { return GetProperty(Compatibility.SpacingInWholePoints); }
            set { SetProperty(Compatibility.SpacingInWholePoints, value); }
        }

        /// <summary>
        /// Print Body Text before Header/Footer Contents.
        /// </summary>
        public bool PrintBodyTextBeforeHeader
        {
            get { return GetProperty(Compatibility.PrintBodyTextBeforeHeader); }
            set { SetProperty(Compatibility.PrintBodyTextBeforeHeader, value); }
        }

        /// <summary>
        /// Do Not Add Leading Between Lines of Text.
        /// </summary>
        public bool NoLeading
        {
            get { return GetProperty(Compatibility.NoLeading); }
            set { SetProperty(Compatibility.NoLeading, value); }
        }

        /// <summary>
        /// Add Additional Space Below Baseline For Underlined East Asian Text.
        /// </summary>
        public bool SpaceForUL
        {
            get { return GetProperty(Compatibility.SpaceForUL); }
            set { SetProperty(Compatibility.SpaceForUL, value); }
        }

        /// <summary>
        /// Emulate Word 5.x for the Macintosh Small Caps Formatting.
        /// </summary>
        public bool MWSmallCaps
        {
            get { return GetProperty(Compatibility.MWSmallCaps); }
            set { SetProperty(Compatibility.MWSmallCaps, value); }
        }

        /// <summary>
        /// Emulate WordPerfect 5.x Line Spacing.
        /// </summary>
        public bool SuppressTopSpacingWP
        {
            get { return GetProperty(Compatibility.SuppressTopLineSpacingWP); }
            set { SetProperty(Compatibility.SuppressTopLineSpacingWP, value); }
        }

        /// <summary>
        /// Emulate WordPerfect 6.x Font Height Calculation.
        /// </summary>
        public bool TruncateFontHeightsLikeWP6
        {
            get { return GetProperty(Compatibility.TruncateFontHeightLikeWP6); }
            set { SetProperty(Compatibility.TruncateFontHeightLikeWP6, value); }
        }

        /// <summary>
        /// Increase Priority Of Font Size During Font Substitution.
        /// </summary>
        public bool SubFontBySize
        {
            get { return GetProperty(Compatibility.SubFontBySize); }
            set { SetProperty(Compatibility.SubFontBySize, value); }
        }

        /// <summary>
        /// Emulate Word 6.0 Line Wrapping for East Asian Text.
        /// </summary>
        public bool LineWrapLikeWord6
        {
            get { return GetProperty(Compatibility.LineWrapLikeWord6); }
            set { SetProperty(Compatibility.LineWrapLikeWord6, value); }
        }

        /// <summary>
        /// Do Not Suppress Paragraph Borders Next To Frames.
        /// </summary>
        public bool DoNotSuppressParagraphBorders
        {
            get { return GetProperty(Compatibility.DoNotSuppressParagraphBorder); }
            set { SetProperty(Compatibility.DoNotSuppressParagraphBorder, value); }
        }

        /// <summary>
        /// Do Not Center Content on Lines With Exact Line Height.
        /// </summary>
        public bool NoExtraLineSpacing
        {
            get { return GetProperty(Compatibility.NoExtraLineSpacing); }
            set { SetProperty(Compatibility.NoExtraLineSpacing, value); }
        }

        /// <summary>
        /// Ignore Exact Line Height for Last Line on Page.
        /// </summary>
        public bool SuppressBottomSpacing
        {
            get { return GetProperty(Compatibility.SuppressBottomSpacing); }
            set { SetProperty(Compatibility.SuppressBottomSpacing, value); }
        }

        /// <summary>
        /// Specifies whether to set the width of a space as is done in WordPerfect 5.x.
        /// </summary>
        public bool WPSpaceWidth
        {
            get { return GetProperty(Compatibility.WPSpaceWidth); }
            set { SetProperty(Compatibility.WPSpaceWidth, value); }
        }

        /// <summary>
        /// Emulate WordPerfect 6.x Paragraph Justification.
        /// </summary>
        public bool WPJustification
        {
            get { return GetProperty(Compatibility.WPJustification); }
            set { SetProperty(Compatibility.WPJustification, value); }
        }

        /// <summary>
        /// Use Printer Metrics To Display Documents.
        /// </summary>
        /// <remarks>
        /// Printer Metrics may differ depending on drivers used.
        /// For instance, Windows "Microsoft OpenXPS Class Driver 2" and "Microsoft Print to PDF" provide slightly different metrics.
        /// Therefore, the final document's layout may change if this option is enabled.
        /// </remarks>
        public bool UsePrinterMetrics
        {
            get { return GetProperty(Compatibility.UsePrinterMetrics); }
            set { SetProperty(Compatibility.UsePrinterMetrics, value); }
        }

        /// <summary>
        /// Emulate Word 97 Text Wrapping Around Floating Objects.
        /// </summary>
        public bool ShapeLayoutLikeWW8
        {
            get { return GetProperty(Compatibility.ShapeLayoutLikeWW8); }
            set { SetProperty(Compatibility.ShapeLayoutLikeWW8, value); }
        }

        /// <summary>
        /// Emulate Word 6.x/95/97 Footnote Placement.
        /// </summary>
        public bool FootnoteLayoutLikeWW8
        {
            get { return GetProperty(Compatibility.FootnoteLayoutLikeWW8); }
            set { SetProperty(Compatibility.FootnoteLayoutLikeWW8, value); }
        }

        /// <summary>
        /// Use Fixed Paragraph Spacing for HTML Auto Setting.
        /// </summary>
        public bool DoNotUseHTMLParagraphAutoSpacing
        {
            get { return GetProperty(Compatibility.DoNotUseHtmlParagraphAutoSpacing); }
            set { SetProperty(Compatibility.DoNotUseHtmlParagraphAutoSpacing, value); }
        }

        /// <summary>
        /// Add Document Grid Line Pitch To Lines in Table Cells.
        /// </summary>
        public bool AdjustLineHeightInTable
        {
            get { return GetProperty(Compatibility.AdjustLineHeightInTable); }
            set { SetProperty(Compatibility.AdjustLineHeightInTable, value); }
        }

        /// <summary>
        /// Ignore Width of Last Tab Stop When Aligning Paragraph If It Is Not Left Aligned.
        /// </summary>
        public bool ForgetLastTabAlignment
        {
            get { return GetProperty(Compatibility.ForgetLastTabAlignment); }
            set { SetProperty(Compatibility.ForgetLastTabAlignment, value); }
        }

        /// <summary>
        /// Emulate Word 95 Full-Width Character Spacing.
        /// </summary>
        public bool AutoSpaceLikeWord95
        {
            get { return GetProperty(Compatibility.AutoSpaceLikeWord95); }
            set { SetProperty(Compatibility.AutoSpaceLikeWord95, value); }
        }

        /// <summary>
        /// Align Table Rows Independently.
        /// </summary>
        public bool AlignTablesRowByRow
        {
            get { return GetProperty(Compatibility.AlignTableRowByRow); }
            set { SetProperty(Compatibility.AlignTableRowByRow, value); }
        }

        /// <summary>
        /// Ignore Space Before Table When Deciding If Table Should Wrap Floating Object.
        /// </summary>
        public bool LayoutRawTableWidth
        {
            get { return GetProperty(Compatibility.LayoutRawTableWidth); }
            set { SetProperty(Compatibility.LayoutRawTableWidth, value); }
        }

        /// <summary>
        /// Allow Table Rows to Wrap Inline Objects Independently.
        /// </summary>
        public bool LayoutTableRowsApart
        {
            get { return GetProperty(Compatibility.LayoutTableRowsApart); }
            set { SetProperty(Compatibility.LayoutTableRowsApart, value); }
        }

        /// <summary>
        /// Emulate Word 97 East Asian Line Breaking.
        /// </summary>
        public bool UseWord97LineBreakRules
        {
            get { return GetProperty(Compatibility.UseWord97LineBreakRules); }
            set { SetProperty(Compatibility.UseWord97LineBreakRules, value); }
        }

        /// <summary>
        /// Do Not Allow Floating Tables To Break Across Pages.
        /// </summary>
        public bool DoNotBreakWrappedTables
        {
            get { return GetProperty(Compatibility.DoNotBreakWrappedTables); }
            set { SetProperty(Compatibility.DoNotBreakWrappedTables, value); }
        }

        /// <summary>
        /// Do Not Snap to Document Grid in Table Cells with Objects.
        /// </summary>
        public bool DoNotSnapToGridInCell
        {
            get { return GetProperty(Compatibility.doNotSnapToGridInCell); }
            set { SetProperty(Compatibility.doNotSnapToGridInCell, value); }
        }

        /// <summary>
        /// Select Field When First or Last Character Is Selected.
        /// </summary>
        public bool SelectFldWithFirstOrLastChar
        {
            get { return GetProperty(Compatibility.SelectFldWithFirstOrLastChar); }
            set { SetProperty(Compatibility.SelectFldWithFirstOrLastChar, value); }
        }

        /// <summary>
        /// Use Legacy Ethiopic and Amharic Line Breaking Rules.
        /// </summary>
        public bool ApplyBreakingRules
        {
            get { return GetProperty(Compatibility.ApplyBreakingRules); }
            set { SetProperty(Compatibility.ApplyBreakingRules, value); }
        }

        /// <summary>
        /// Do Not Allow Hanging Punctuation With Character Grid.
        /// </summary>
        public bool DoNotWrapTextWithPunct
        {
            get { return GetProperty(Compatibility.DoNotWrapTextWithPunct); }
            set { SetProperty(Compatibility.DoNotWrapTextWithPunct, value); }
        }

        /// <summary>
        /// Do Not Compress Compressible Characters When Using Document Grid.
        /// </summary>
        public bool DoNotUseEastAsianBreakRules
        {
            get { return GetProperty(Compatibility.DoNotUseEastAsianBreakRules); }
            set { SetProperty(Compatibility.DoNotUseEastAsianBreakRules, value); }
        }

        /// <summary>
        /// Emulate Word 2002 Table Style Rules.
        /// </summary>
        public bool UseWord2002TableStyleRules
        {
            get { return GetProperty(Compatibility.UseWord2002TableStyleRules); }
            set { SetProperty(Compatibility.UseWord2002TableStyleRules, value); }
        }

        /// <summary>
        /// Allow Tables to AutoFit Into Page Margins.
        /// </summary>
        public bool GrowAutofit
        {
            get { return GetProperty(Compatibility.GrowAutofit); }
            set { SetProperty(Compatibility.GrowAutofit, value); }
        }

        /// <summary>
        /// Do Not Automatically Apply List Paragraph Style To Bulleted/Numbered Text.
        /// </summary>
        public bool UseNormalStyleForList
        {
            get { return GetProperty(Compatibility.UseNormalStyleForList); }
            set { SetProperty(Compatibility.UseNormalStyleForList, value); }
        }

        /// <summary>
        /// Ignore Hanging Indent When Creating Tab Stop After Numbering.
        /// </summary>
        public bool DoNotUseIndentAsNumberingTabStop
        {
            get { return GetProperty(Compatibility.DoNotUseIndentAsNumberingTabStop); }
            set { SetProperty(Compatibility.DoNotUseIndentAsNumberingTabStop, value); }
        }

        /// <summary>
        /// Use Alternate Set of East Asian Line Breaking Rules.
        /// </summary>
        public bool UseAltKinsokuLineBreakRules
        {
            get { return GetProperty(Compatibility.UseAltKinsokuLineBreakRules); }
            set { SetProperty(Compatibility.UseAltKinsokuLineBreakRules, value); }
        }

        /// <summary>
        /// Allow Contextual Spacing of Paragraphs in Tables.
        /// </summary>
        public bool AllowSpaceOfSameStyleInTable
        {
            get { return GetProperty(Compatibility.AllowSpaceOfSameStyleInTable); }
            set { SetProperty(Compatibility.AllowSpaceOfSameStyleInTable, value); }
        }

        /// <summary>
        /// Do Not Ignore Floating Objects When Calculating Paragraph Indentation.
        /// </summary>
        public bool DoNotSuppressIndentation
        {
            get { return GetProperty(Compatibility.DoNotSuppressIndentation); }
            set { SetProperty(Compatibility.DoNotSuppressIndentation, value); }
        }

        /// <summary>
        /// Do Not AutoFit Tables To Fit Next To Wrapped Objects.
        /// </summary>
        public bool DoNotAutofitConstrainedTables
        {
            get { return GetProperty(Compatibility.DoNotAutofitConstrainedTables); }
            set { SetProperty(Compatibility.DoNotAutofitConstrainedTables, value); }
        }

        /// <summary>
        /// Allow Table Columns To Exceed Preferred Widths of Constituent Cells.
        /// </summary>
        /// <remarks>
        /// The option is called "Use Word 2003 table autofit rules" in MS Word 2013 user interface.
        /// It actually affects how the grid is calculated for fixed layout tables, too (for some cases).
        /// </remarks>
        public bool AutofitToFirstFixedWidthCell
        {
            get { return GetProperty(Compatibility.AutofitToFirstFixedWidthCell); }
            set { SetProperty(Compatibility.AutofitToFirstFixedWidthCell, value); }
        }

        /// <summary>
        /// Underline Following Character Following Numbering.
        /// </summary>
        public bool UnderlineTabInNumList
        {
            get { return GetProperty(Compatibility.UnderlineTabInNumList); }
            set { SetProperty(Compatibility.UnderlineTabInNumList, value); }
        }

        /// <summary>
        /// Always Use Fixed Width for Hangul Characters.
        /// </summary>
        public bool DisplayHangulFixedWidth
        {
            get { return GetProperty(Compatibility.DisplayHangulFixedWidth); }
            set { SetProperty(Compatibility.DisplayHangulFixedWidth, value); }
        }

        /// <summary>
        /// Always Move Paragraph Mark to Page after a Page Break.
        /// </summary>
        public bool SplitPgBreakAndParaMark
        {
            get { return GetProperty(Compatibility.SplitPgBreakAndParaMark); }
            set { SetProperty(Compatibility.SplitPgBreakAndParaMark, value); }
        }

        /// <summary>
        /// Don't Vertically Align Cells Containing Floating Objects.
        /// </summary>
        public bool DoNotVertAlignCellWithSp
        {
            get { return GetProperty(Compatibility.DoNotVertAlignCellWithSp); }
            set { SetProperty(Compatibility.DoNotVertAlignCellWithSp, value); }
        }

        /// <summary>
        /// Don't Break Table Rows Around Floating Tables.
        /// </summary>
        public bool DoNotBreakConstrainedForcedTable
        {
            get { return GetProperty(Compatibility.DoNotBreakConstrainedForcedTable); }
            set { SetProperty(Compatibility.DoNotBreakConstrainedForcedTable, value); }
        }

        /// <summary>
        /// Ignore Vertical Alignment in Textboxes.
        /// </summary>
        public bool DoNotVertAlignInTxbx
        {
            get { return GetProperty(Compatibility.DoNotVertAlignInTxbx); }
            set { SetProperty(Compatibility.DoNotVertAlignInTxbx, value); }
        }

        /// <summary>
        /// Use ANSI Kerning Pairs from Fonts.
        /// </summary>
        public bool UseAnsiKerningPairs
        {
            get { return GetProperty(Compatibility.UseAnsiKerningPairs); }
            set { SetProperty(Compatibility.UseAnsiKerningPairs, value); }
        }

        /// <summary>
        /// Use Cached Paragraph Information for Column Balancing.
        /// </summary>
        public bool CachedColBalance
        {
            get { return GetProperty(Compatibility.CachedColBalance); }
            set { SetProperty(Compatibility.CachedColBalance, value); }
        }

        /// <summary>
        /// Do Not Bypass East Asian/Complex Script Layout Code.
        /// </summary>
        public bool UseFELayout
        {
            get { return GetProperty(Compatibility.UseFELayout); }
            set { SetProperty(Compatibility.UseFELayout, value); }
        }

        /// <summary>
        /// Specifies how the style hierarchy of the document is evaluated.
        /// </summary>
        public bool OverrideTableStyleFontSizeAndJustification
        {
            get { return GetProperty(Compatibility.OverrideTableStyleFontSizeAndJustification); }
            set { SetProperty(Compatibility.OverrideTableStyleFontSizeAndJustification, value); }
        }

        /// <summary>
        /// Specifies to disable OpenType font formatting features.
        /// </summary>
        public bool DisableOpenTypeFontFormattingFeatures
        {
            get { return GetProperty(Compatibility.DisableOpenTypeFontFormattingFeatures); }
            set { SetProperty(Compatibility.DisableOpenTypeFontFormattingFeatures, value); }
        }

        /// <summary>
        /// Specifies to swap inside and outside for mirror indents and relative positioning.
        /// </summary>
        public bool SwapInsideAndOutsideForMirrorIndentsAndRelativePositioning
        {
            get { return GetProperty(Compatibility.SwapInsideAndOutsideForMirrorIndentsAndRelativePositioning); }
            set { SetProperty(Compatibility.SwapInsideAndOutsideForMirrorIndentsAndRelativePositioning, value); }
        }

        /// <summary>
        /// Specifies to use Word2010 table style rules.
        /// </summary>
        public bool UseWord2010TableStyleRules
        {
            get { return GetProperty(Compatibility.UseWord2010TableStyleRules); }
            set { SetProperty(Compatibility.UseWord2010TableStyleRules, value); }
        }

        /// <summary>
        /// True to disable UI functionality which is not compatible with Word97-2003.
        /// Default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Controls the Word97-2003 compatibility setting that disables UI functionality which
        /// is not compatible with Word97-2003.
        /// When <c>true</c>, 'w:uiCompat97To2003' XML element is written to '\word\settings.xml'
        /// document package part.
        /// Default value is <c>false</c>. When set to <c>false</c>, this element is not written.
        ///
        /// Technically this property is not part of compatibility options, but we have put it here for API convenience.
        /// </remarks>
        public bool UICompat97To2003
        {
            // RK This option is really supposed to be in DocPr, but because DocPr is not public,
            // but this property had to be made public, I've moved it here to compat options.
            get { return GetProperty(Compatibility.UICompat97To2003); }
            set { SetProperty(Compatibility.UICompat97To2003, value); }
        }

        /// <summary>
        /// Provides access to custom defined compatibility settings.
        /// </summary>
        internal CustomCompatibilitySettingCollection CustomCompatibilitySettings
        {
            get { return mCustomSettings; }
        }

        internal static CompatibilityOptions CreateWord97Options()
        {
            CompatibilityOptions result = new CompatibilityOptions();
            InitWord97Options(result);
            return result;
        }

        internal static CompatibilityOptions CreateWord2000Options()
        {
            CompatibilityOptions result = new CompatibilityOptions();
            InitWord2000Options(result);
            return result;
        }

        internal static CompatibilityOptions CreateWord2002Options()
        {
            CompatibilityOptions result = new CompatibilityOptions();
            InitWord2002Options(result);
            return result;
        }

        internal static CompatibilityOptions CreateWord2003Options()
        {
            CompatibilityOptions result = new CompatibilityOptions();
            InitWord2003Options(result);
            return result;
        }

        internal static CompatibilityOptions CreateWord2007Options()
        {
            return new CompatibilityOptions();
        }

        /// <summary>
        /// Specifies which version of MS Word should be used as an etalon model for all default behavior which is not controlled by other settings or options.
        /// Can be set by user through the public API by using <see cref="OptimizeFor"/>.
        /// Also read by AW from w:compatSetting w:name="compatibilityMode" element of DOCX settings package if present.
        /// All AW behavior including model, layout, conversions etc. should consider this option to meet the user expectations.
        /// </summary>
        /// <remarks>The default value for this shall be <see cref="MsWordVersionCore.Unspecified"/></remarks>
        internal MsWordVersionCore MswVersion
        {
            get { return mMsWordVersion; }
            set { mMsWordVersion = value; }
        }

        /// <summary>
        /// Indicates that the <see cref="OptimizeFor(MsWordVersion)"/> method was called by user explicitly.
        /// </summary>
        internal bool IsOptimized
        {
            get { return mIsOptimized; }
            set { mIsOptimized = value; }
        }

        /// <summary>
        /// Gets a boolean value indicating if MS Word would display the document in compatibility mode.
        /// </summary>
        /// <remarks>
        /// When MswVersion is not specified, MS Word still displays the document in compatibility mode.
        /// </remarks>
        internal bool IsWord2013OrLaterCompatible
        {
            get { return mMsWordVersion >= MsWordVersionCore.Word2013; }
        }

        /// <summary>
        /// Indicates whether <see cref="OptimizeFor(MsWordVersion)"/> method was
        /// called by user explicitly and therefore we should convert some nodes to newer
        /// format starting from <see cref="MsWordVersion.Word2007"/> upon saving document.
        /// </summary>
        /// <remarks>
        /// This is analogue to MS Word option "Maintain compatibility with previous version of Word"
        /// in 'SaveAs' dialog. For example, if this option is set,
        /// MS Word converts shape images from VML to DML format.
        /// </remarks>
        internal bool IsNeedConvertToNewerVersion
        {
            get { return mIsOptimized && (mMsWordVersion >= MsWordVersionCore.Word2007); }
        }

        /// <summary>
        /// Clear all compatibility options and compatibility custom settings.
        /// </summary>
        private void Clear()
        {
             // WORDSNET-27205 Word does not clear all options while resaving to different version,
             // but rather only limited set of options. We do not preserve absolutely all options
             // for the moment, but just only a single one 'ulTrailSpace' for which customer has
             // complained about for a while.
            Dictionary<Compatibility, bool> preservedCompatibilityOptions = new Dictionary<Compatibility, bool>();
            foreach (Compatibility compatibilityOption in gPreserveOnClearCompatibilityOptions)
            {
                bool value;
                if (mCompatibilityOptions.TryGetValue(compatibilityOption, out value))
                    preservedCompatibilityOptions[compatibilityOption] = value;
            }

            mCompatibilityOptions.Clear();
            foreach (KeyValuePair<Compatibility,bool> preservedCompatibilityOption in preservedCompatibilityOptions)
                mCompatibilityOptions[preservedCompatibilityOption.Key] = preservedCompatibilityOption.Value;

            mCustomSettings.Clear();
        }

        private static void InitWord97Options(CompatibilityOptions co)
        {
            InitWord2000Options(co);
            co.AlignTablesRowByRow = true;
            co.LayoutTableRowsApart = true;
            co.DoNotUseHTMLParagraphAutoSpacing = true;
            co.ForgetLastTabAlignment = true;
            co.ShapeLayoutLikeWW8 = true;
            co.FootnoteLayoutLikeWW8 = true;
            co.LayoutRawTableWidth = true;
            co.UseWord97LineBreakRules = true;
        }

        private static void InitWord2000Options(CompatibilityOptions co)
        {
            InitWord2002Options(co);
            co.DoNotWrapTextWithPunct = true;
            co.DoNotBreakWrappedTables = true;
            co.DoNotSnapToGridInCell = true;
            co.DoNotUseEastAsianBreakRules = true;
            co.SelectFldWithFirstOrLastChar = true;
            co.UnderlineTabInNumList = true;
        }

        private static void InitWord2002Options(CompatibilityOptions co)
        {
            InitWord2003Options(co);
            co.GrowAutofit = true;
            co.UseWord2002TableStyleRules = true;
        }

        private static void InitWord2003Options(CompatibilityOptions co)
        {
            co.AllowSpaceOfSameStyleInTable = true;
            co.DoNotAutofitConstrainedTables = true;
            co.DoNotBreakConstrainedForcedTable = true;
            co.DoNotUseIndentAsNumberingTabStop = true;
            co.DisplayHangulFixedWidth = true;
            co.DoNotVertAlignInTxbx = true;
            co.DoNotVertAlignCellWithSp = true;
            co.SplitPgBreakAndParaMark = true;
            co.CachedColBalance = true;
            co.UseNormalStyleForList = true;
            co.UseAltKinsokuLineBreakRules = true;
            co.DoNotSuppressIndentation = true;
            co.UseAnsiKerningPairs = true;
            co.AutofitToFirstFixedWidthCell = true;
        }

        /// <summary>
        /// MS Word 2010 specifies the following custom compatibility settings upon creating new document.
        /// </summary>
        private static void InitWord2010Options(CompatibilityOptions co)
        {
            co.CustomCompatibilitySettings.Add(new CustomCompatibilitySetting("compatibilityMode", SchemaUri, "14"));
            co.CustomCompatibilitySettings.Add(new CustomCompatibilitySetting("overrideTableStyleFontSizeAndJustification", SchemaUri, Value));
            co.CustomCompatibilitySettings.Add(new CustomCompatibilitySetting("enableOpenTypeFeatures", SchemaUri, Value));
            co.CustomCompatibilitySettings.Add(new CustomCompatibilitySetting("doNotFlipMirrorIndents", SchemaUri, Value));
        }

        /// <summary>
        /// MS Word 2013 specifies the following custom compatibility settings upon creating new document.
        /// </summary>
        private static void InitWord2013Options(CompatibilityOptions co)
        {
            InitWord2010Options(co);
            co.CustomCompatibilitySettings["compatibilityMode"].Value = "15";
            co.CustomCompatibilitySettings.Add(new CustomCompatibilitySetting("differentiateMultirowTableHeaders", SchemaUri, Value));
        }

        /// <summary>
        /// MS Word 2016 specifies the following custom compatibility settings upon creating new document.
        /// </summary>
        private static void InitWord2016Options(CompatibilityOptions co)
        {
            // No changes in comparison with MS Word 2013.
            InitWord2013Options(co);
        }

        /// <summary>
        /// MS Word 2019 specifies the following custom compatibility settings upon creating new document.
        /// </summary>
        private static void InitWord2019Options(CompatibilityOptions co)
        {
            // No changes in comparison with MS Word 2013.
            InitWord2013Options(co);
        }

        /// <summary>
        /// If CompatibilityOptions are empty, MS Word 2013 uses MSWord2007 CompatibilityOptions with some flags set.
        /// This method inits second level of CompatibilityOptions with these flags set.
        /// They are inherited by document's CompatibilityOptions but not actually present there,
        /// so upon writing we do not write them.
        /// </summary>
        private void InitInheritedCompatibilityOptions()
        {
            mInheritedCompatibilityOptions = new Dictionary<Compatibility, bool>();
            mInheritedCompatibilityOptions[Compatibility.OverrideTableStyleFontSizeAndJustification] = true;
            mInheritedCompatibilityOptions[Compatibility.DisableOpenTypeFontFormattingFeatures] = true;
            mInheritedCompatibilityOptions[Compatibility.SwapInsideAndOutsideForMirrorIndentsAndRelativePositioning] = true;
            mInheritedCompatibilityOptions[Compatibility.UseWord2010TableStyleRules] = true;
        }

        private MsWordVersionCore mMsWordVersion = MsWordVersionCore.Unspecified;

        private CustomCompatibilitySettingCollection mCustomSettings = new CustomCompatibilitySettingCollection();

        private readonly Dictionary<Compatibility, bool> mCompatibilityOptions = new Dictionary<Compatibility, bool>();
        private Dictionary<Compatibility, bool> mInheritedCompatibilityOptions;
        private const string SchemaUri = "http://schemas.microsoft.com/office/word";
        private const string Value = "1";

        /// <summary>
        /// Indicates whether <see cref="mMsWordVersion"/> was loaded from file,
        /// or was explicitly set by user with <see cref="OptimizeFor"/> method.
        /// </summary>
        private bool mIsOptimized;

        /// <summary>
        /// The collection of Compatibility options that we should preserve in <see cref="Clear"/> method to mimic Word.
        /// </summary>
        private static readonly Compatibility[] gPreserveOnClearCompatibilityOptions = new Compatibility[] { Compatibility.UlTrailSpace };
    }
}
