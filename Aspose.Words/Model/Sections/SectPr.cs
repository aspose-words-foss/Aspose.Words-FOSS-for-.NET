// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/05/2005 by Roman Korchagin

using System;
using System.Drawing;
using System.Globalization;
using System.Threading;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Notes;
using Aspose.Words.Nrx;

namespace Aspose.Words
{
    /// <summary>
    /// Provides typed access to section attributes.
    /// </summary>
    internal class SectPr : WordAttrCollection, ISectionAttrSource
    {
        internal void ValidateColumns(IWarningCallback warningCallback)
        {
            // Resiliency for a document where custom columns are specified, but not correctly defined.
            if (!ColumnsEvenlySpaced)
            {
                // WORDSNET-5054 Document contains definition (both incomplete) for two columns but column count is set to 1.
                // First step is to remove extra columns.
                while((ColumnsCount > Columns.Count) && (Columns.Count > 0))
                {
                    // Remove extra columns.
                    Columns.RemoveAt(Columns.Count - 1);
                    WarnUnexpected(warningCallback, WarningStrings.ExtraColumnDefinitions);
                }

                // WORDSNET-5054 Width for first column is missed.
                // AM. Word renders this file OK and shows that column width is 17.09cm (approx 9690 as RawWidth). I tried to understand how this value can be calculated but couldn't get it.
                // I could skip writing width for such columns while saving binary DOC but in this case layout engine is failed to produce good output so I decided to write this value explicitly.
                // We have three defects J5054, J2885 and J5964 with the same problem and all of them is good after this fix is applied.
                for (int i = 0; i < Columns.Count; i++)
                    if (Columns[i].RawWidth == 0)
                    {
                        // Write "magic" column width.
                        Columns[i].RawWidth = 9690;
                        WarnUnexpected(warningCallback, WarningStrings.ColumnWidthMissed);
                    }

                int customColumnsDefinedCount = Columns.Count;

                if (customColumnsDefinedCount == 0)
                    ColumnsEvenlySpaced = true;

                // WORDSNET-23855 It appears the number of defined columns should never be less than the number of columns specified,
                // but the other way around seems to occur from time to time in MS Word.
                if (ColumnsCount > customColumnsDefinedCount)
                    ColumnsCount = customColumnsDefinedCount;
            }
        }

        /// <summary>
        /// Updates the cached default values as well as the current culture-specific attribute values in accordance with the system LCID.
        /// </summary>
        internal void UpdateDefaultsByLcid()
        {
            if (!PlatformUtilPal.IsWindows())
                return;

            mSystemLcid = SystemPal.GetSystemDefaultLcid();
            SectPr sectPr = AddCultureDefaults(mSystemLcid);

            UpdateAttr(SectAttr.LeftMargin, sectPr.FetchAttr(SectAttr.LeftMargin));
            UpdateAttr(SectAttr.RightMargin, sectPr.FetchAttr(SectAttr.RightMargin));
            UpdateAttr(SectAttr.TopMargin, sectPr.FetchAttr(SectAttr.TopMargin));
            UpdateAttr(SectAttr.BottomMargin, sectPr.FetchAttr(SectAttr.BottomMargin));
            UpdateAttr(SectAttr.HeaderDistance, sectPr.FetchAttr(SectAttr.HeaderDistance));
            UpdateAttr(SectAttr.FooterDistance, sectPr.FetchAttr(SectAttr.FooterDistance));
            UpdateAttr(SectAttr.ColumnsSpacing, sectPr.FetchAttr(SectAttr.ColumnsSpacing));
        }

        internal NumberStyle PageNumberStyle
        {
            get { return (NumberStyle)FetchAttr(SectAttr.PageNumberStyle); }
            set { SetAttr(SectAttr.PageNumberStyle, value); }
        }

        internal NumberStyle PageNumberStyleFinal
        {
            get { return (NumberStyle)FetchAttr(SectAttr.PageNumberStyle, RevisionsView.Final); }
        }

        internal ChapterPageSeparator ChapterPageSeparator
        {
            get { return (ChapterPageSeparator)FetchAttr(SectAttr.ChapterPageSeparator); }
            set { SetAttr(SectAttr.ChapterPageSeparator, value); }
        }

        internal ChapterPageSeparator ChapterPageSeparatorFinal
        {
            get { return (ChapterPageSeparator)FetchAttr(SectAttr.ChapterPageSeparator, RevisionsView.Final); }
        }

        internal SectionStart SectionStart
        {
            get { return (SectionStart)FetchAttr(SectAttr.SectionStart); }
            set { SetAttr(SectAttr.SectionStart, value); }
        }

        internal bool DifferentFirstPageHeaderFooter
        {
            get { return (bool)FetchAttr(SectAttr.DifferentFirstPageHeaderFooter); }
            set { SetAttr(SectAttr.DifferentFirstPageHeaderFooter, value); }
        }

        internal bool RestartPageNumbering
        {
            get { return (bool)FetchAttr(SectAttr.RestartPageNumbering); }
            set { SetAttr(SectAttr.RestartPageNumbering, value); }
        }

        internal bool ColumnsLineBetween
        {
            get { return (bool)FetchAttr(SectAttr.ColumnsLineBetween); }
            set { SetAttr(SectAttr.ColumnsLineBetween, value); }
        }

        internal int FirstPageTray
        {
            get { return (int)FetchAttr(SectAttr.FirstPageTray); }
            set { SetAttr(SectAttr.FirstPageTray, value); }
        }

        internal int OtherPagesTray
        {
            get { return (int)FetchAttr(SectAttr.OtherPagesTray); }
            set { SetAttr(SectAttr.OtherPagesTray, value); }
        }

        internal bool SuppressEndnotes
        {
            get { return (bool)FetchAttr(SectAttr.SuppressEndnotes); }
            set { SetAttr(SectAttr.SuppressEndnotes, value); }
        }

        internal LineNumberRestartMode LineNumberRestartMode
        {
            get { return (LineNumberRestartMode)FetchAttr(SectAttr.LineNumberRestartMode); }
            set { SetAttr(SectAttr.LineNumberRestartMode, value); }
        }

        internal int LineNumberCountBy
        {
            get { return (int)FetchAttr(SectAttr.LineNumberCountBy); }
            set { SetAttr(SectAttr.LineNumberCountBy, value); }
        }

        internal Border BorderTop
        {
            get { return (Border)FetchAttr(SectAttr.BorderTop); }
            set { SetAttr(SectAttr.BorderTop, value); }
        }

        internal Border BorderLeft
        {
            get { return (Border)FetchAttr(SectAttr.BorderLeft); }
            set { SetAttr(SectAttr.BorderLeft, value); }
        }

        internal Border BorderBottom
        {
            get { return (Border)FetchAttr(SectAttr.BorderBottom); }
            set { SetAttr(SectAttr.BorderBottom, value); }
        }

        internal Border BorderRight
        {
            get { return (Border)FetchAttr(SectAttr.BorderRight); }
            set { SetAttr(SectAttr.BorderRight, value); }
        }

        internal int CharSpace
        {
            get { return (int)FetchAttr(SectAttr.CharSpace); }
            set { SetAttr(SectAttr.CharSpace, value); }
        }

        internal int LinePitch
        {
            get { return (int)FetchAttr(SectAttr.LinePitch); }
            set { SetAttr(SectAttr.LinePitch, value); }
        }

        internal int LineStartingNumber
        {
            get { return (int)FetchAttr(SectAttr.LineStartingNumber); }
            set { SetAttr(SectAttr.LineStartingNumber, value); }
        }

        internal int HeadingLevelForChapter
        {
            get { return (int)FetchAttr(SectAttr.HeadingLevelForChapter); }
            set { SetAttr(SectAttr.HeadingLevelForChapter, value); }
        }

        internal int HeadingLevelForChapterFinal
        {
            get { return (int)FetchAttr(SectAttr.HeadingLevelForChapter, RevisionsView.Final); }
        }

        internal int PageStartingNumber
        {
            get { return (int)FetchAttr(SectAttr.PageStartingNumber); }
            set { SetAttr(SectAttr.PageStartingNumber, value); }
        }

        internal int PageStartingNumberFinal
        {
            get { return (int)FetchAttr(SectAttr.PageStartingNumber, RevisionsView.Final); }
        }

        internal Orientation Orientation
        {
            get { return (Orientation)FetchAttr(SectAttr.Orientation); }
            set { SetAttr(SectAttr.Orientation, value); }
        }

        internal PageBorderAppliesTo BorderAppliesTo
        {
            get { return (PageBorderAppliesTo)FetchAttr(SectAttr.BorderAppliesTo); }
            set { SetAttr(SectAttr.BorderAppliesTo, value); }
        }

        internal bool BorderAlwaysInFront
        {
            get { return (bool)FetchAttr(SectAttr.BorderAlwaysInFront); }
            set { SetAttr(SectAttr.BorderAlwaysInFront, value); }
        }

        internal PageBorderDistanceFrom BorderDistanceFrom
        {
            get { return (PageBorderDistanceFrom)FetchAttr(SectAttr.BorderDistanceFrom); }
            set { SetAttr(SectAttr.BorderDistanceFrom, value); }
        }

        internal int PageWidth
        {
            get { return (int)FetchAttr(SectAttr.PageWidth); }
            set { SetAttr(SectAttr.PageWidth, value); }
        }

        internal int PageHeight
        {
            get { return (int)FetchAttr(SectAttr.PageHeight); }
            set { SetAttr(SectAttr.PageHeight, value); }
        }

        internal int LeftMargin
        {
            get { return (int)FetchAttr(SectAttr.LeftMargin); }
            set { SetAttr(SectAttr.LeftMargin, value); }
        }

        internal int RightMargin
        {
            get { return (int)FetchAttr(SectAttr.RightMargin); }
            set { SetAttr(SectAttr.RightMargin, value); }
        }

        internal int TopMargin
        {
            get { return (int)FetchAttr(SectAttr.TopMargin); }
            set { SetAttr(SectAttr.TopMargin, value); }
        }

        internal int BottomMargin
        {
            get { return (int)FetchAttr(SectAttr.BottomMargin); }
            set { SetAttr(SectAttr.BottomMargin, value); }
        }

        internal int Gutter
        {
            get { return (int)FetchAttr(SectAttr.Gutter); }
            set { SetAttr(SectAttr.Gutter, value); }
        }

        internal int HeaderDistance
        {
            get { return (int)FetchAttr(SectAttr.HeaderDistance); }
            set { SetAttr(SectAttr.HeaderDistance, value); }
        }

        internal int FooterDistance
        {
            get { return (int)FetchAttr(SectAttr.FooterDistance); }
            set { SetAttr(SectAttr.FooterDistance, value); }
        }

        internal PageVerticalAlignment VerticalAlignment
        {
            get { return (PageVerticalAlignment)FetchAttr(SectAttr.VerticalAlignment); }
            set { SetAttr(SectAttr.VerticalAlignment, value); }
        }

        internal int ColumnsCount
        {
            get { return (int)FetchAttr(SectAttr.ColumnsCount); }
            set { SetAttr(SectAttr.ColumnsCount, value); }
        }

        internal bool ColumnsEvenlySpaced
        {
            get { return (bool)FetchAttr(SectAttr.ColumnsEvenlySpaced); }
            set { SetAttr(SectAttr.ColumnsEvenlySpaced, value); }
        }

        /// <summary>
        /// Gets total size of the evenly spaced column in section, in twips.
        /// </summary>
        /// <remarks>
        /// The total size is used to determine the size of an individual column.
        /// The columns may not be actually equal because of rounding.
        /// </remarks>
        internal int GetTotalEvenlySpacedColumnSizeTwips(bool hasSideGutter)
        {
            Debug.Assert(ColumnsEvenlySpaced);

            // Width available for columns on the page.
            int availableWidth = GetContentWidthTwips(hasSideGutter);

            // In TestDefect23855 ColumnsCount is 0 for some reason.
            int spacingCount = System.Math.Max(ColumnsCount - 1, 0);
            availableWidth -= spacingCount * ColumnsSpacing;

            return availableWidth;
        }

        internal int GetContentWidthTwips (bool countGutter)
        {
            int gutter = countGutter ? Gutter : 0;

            // Width available for columns on the page.
            return PageWidth - LeftMargin - RightMargin - gutter;
        }

        internal int ColumnsSpacing
        {
            get { return (int)FetchAttr(SectAttr.ColumnsSpacing); }
            set { SetAttr(SectAttr.ColumnsSpacing, value); }
        }

        internal TextColumnCollectionInternal Columns
        {
            get { return (TextColumnCollectionInternal)FetchAttr(SectAttr.Columns); }
            set { SetAttr(SectAttr.Columns, value); }
        }

        internal bool Unlocked
        {
            get { return (bool)FetchAttr(SectAttr.Unlocked); }
            set { SetAttr(SectAttr.Unlocked, value); }
        }

        internal int LineNumberDistanceFromText
        {
            get { return (int)FetchAttr(SectAttr.LineNumberDistanceFromText); }
            set { SetAttr(SectAttr.LineNumberDistanceFromText, value); }
        }

        internal bool RtlGutter
        {
            get { return (bool)FetchAttr(SectAttr.RtlGutter); }
            set { SetAttr(SectAttr.RtlGutter, value); }
        }

        internal SectionLayoutMode GridType
        {
            get { return (SectionLayoutMode)FetchAttr(SectAttr.GridType); }
            set { SetAttr(SectAttr.GridType, value); }
        }

        internal TextFlow TextFlow
        {
            get { return (TextFlow)FetchAttr(SectAttr.TextFlow); }
            set { SetAttr(SectAttr.TextFlow, value); }
        }

        internal bool Bidi
        {
            get { return (bool)FetchAttr(SectAttr.Bidi); }
            set { SetAttr(SectAttr.Bidi, value); }
        }

        internal FootnotePosition FootnotePosition
        {
            get { return (FootnotePosition)FetchAttr(SectAttr.FootnoteLocation); }
            set { SetAttr(SectAttr.FootnoteLocation, value); }
        }

        internal FootnoteNumberingRule FootnoteNumberingRule
        {
            get { return (FootnoteNumberingRule)FetchAttr(SectAttr.FootnoteNumberingRule); }
            set { SetAttr(SectAttr.FootnoteNumberingRule, value); }
        }

        internal NumberStyle FootnoteNumberStyle
        {
            get { return (NumberStyle)FetchAttr(SectAttr.FootnoteNumberStyle); }
            set { SetAttr(SectAttr.FootnoteNumberStyle, value); }
        }

        internal int FootnoteStartNumber
        {
            get { return (int)FetchAttr(SectAttr.FootnoteStartNumber); }
            set { SetAttr(SectAttr.FootnoteStartNumber, value); }
        }

        /// <summary>
        /// Number of footnote columns. If it is zero, then a number of columns on a displayed page is used.
        /// </summary>
        internal int FootnoteColumns
        {
            get { return (int)FetchAttr(SectAttr.FootnoteColumns); }
            set { SetAttr(SectAttr.FootnoteColumns, value); }
        }

        internal EndnotePosition EndnotePosition
        {
            get { return (EndnotePosition)FetchAttr(SectAttr.FootnoteLocation + SectAttr.EndnoteKeyDelta); }
            set { SetAttr(SectAttr.FootnoteLocation + SectAttr.EndnoteKeyDelta, value); }
        }

        internal FootnoteNumberingRule EndnoteNumberingRule
        {
            get { return (FootnoteNumberingRule)FetchAttr(SectAttr.FootnoteNumberingRule + SectAttr.EndnoteKeyDelta); }
            set { SetAttr(SectAttr.FootnoteNumberingRule + SectAttr.EndnoteKeyDelta, value); }
        }

        internal NumberStyle EndnoteNumberStyle
        {
            get { return (NumberStyle)FetchAttr(SectAttr.FootnoteNumberStyle + SectAttr.EndnoteKeyDelta); }
            set { SetAttr(SectAttr.FootnoteNumberStyle + SectAttr.EndnoteKeyDelta, value); }
        }

        internal int EndnoteStartNumber
        {
            get { return (int)FetchAttr(SectAttr.FootnoteStartNumber + SectAttr.EndnoteKeyDelta); }
            set { SetAttr(SectAttr.FootnoteStartNumber + SectAttr.EndnoteKeyDelta, value); }
        }

        /// <summary>
        /// Gets or sets Test flag. This flag is needed for large number of old tests.
        /// </summary>
        /// <remarks>
        /// You should mark tests that don't use test mode as NonParallelizable.
        /// </remarks>
        internal static bool TestMode
        {
            get { return gTestMode; }
            set
            {
                gTestMode = value;
            }
        }

        /// <summary>
        /// ISectionAttrSource.
        /// </summary>
        public object GetDirectSectionAttr(int key)
        {
            return GetDirectAttr(key);
        }

        /// <summary>
        /// ISectionAttrSource.
        /// </summary>
        public object GetDirectSectionAttr(int key, RevisionsView revisionsView)
        {
            return GetDirectAttr(key, revisionsView);
        }

        /// <summary>
        /// ISectionAttrSource.
        /// </summary>
        public object FetchInheritedSectionAttr(int key)
        {
            return FetchInheritedAttr(key);
        }

        /// <summary>
        /// ISectionAttrSource.
        /// </summary>
        public object FetchSectionAttr(int key)
        {
            return FetchAttr(key);
        }

        /// <summary>
        /// ISectionAttrSource.
        /// </summary>
        public object FetchSectionAttr(int key, RevisionsView revisionsView)
        {
            return FetchAttr(key, revisionsView);
        }

        /// <summary>
        /// ISectionAttrSource.
        /// </summary>
        public void SetSectionAttr(int key, object value)
        {
            // The PaperCode attribute specifies the producing application’s unique identifier value for current paper size.
            // If Height/Width was changed this code has no meaning.
            if ((key == SectAttr.PageWidth) || (key == SectAttr.PageHeight))
                Remove(SectAttr.PaperCode);

            SetAttr(key, value);
        }

        /// <summary>
        /// ISectionAttrSource.
        /// </summary>
        public void SetSectionAttr(int key, object value, RevisionsView revisionsView)
        {
            // The PaperCode attribute specifies the producing application’s unique identifier value for current paper size.
            // If Height/Width was changed this code has no meaning.
            if ((key == SectAttr.PageWidth) || (key == SectAttr.PageHeight))
                Remove(SectAttr.PaperCode, revisionsView);

            SetAttr(key, value, revisionsView);
        }


        /// <summary>
        /// ISectionAttrSource.
        /// </summary>
        public void ClearSectionAttrs()
        {
            Clear();
        }

        protected override AttrCollection GetDefaults()
        {
            int lcid = (mSystemLcid != 0)
                ? mSystemLcid
                : GetCurrentLcid();

            return AddCultureDefaults(lcid);
        }

        /// <summary>
        /// Updates the specified value of an attribute if its value has been previously set explicitly.
        /// </summary>
        private void UpdateAttr(int key, object value)
        {
            if (GetDirectAttr(key) != null)
                SetAttr(key, value);
        }

        internal static object FetchDefaultAttr(int key)
        {
            SectPr defaults = AddCultureDefaults(GetCurrentLcid());

            return defaults.GetDirectAttr(key);
        }

        /// <summary>
        /// Checks collection of the attributes existence in the cache and
        /// add it, if it does not exist.
        /// WORDSNET-13826 Every thread with custom culture has own default collection
        /// of the attributes.
        /// </summary>
        /// <returns>Collection with default values of the attributes.</returns>
        private static SectPr AddCultureDefaults(int lcid)
        {
            SectPr attrs;

            lock (gDefaultsSync)
                attrs = DefaultsCache[lcid];

            // Collection for current culture already exist, so return it.
            if (attrs != null)
                return attrs;

            attrs = new SectPr();
            FillSectPr(attrs, lcid);

            lock (gDefaultsSync)
                DefaultsCache[lcid] = attrs;

            return attrs;
        }

        /// <summary>
        /// Fills attribute collection with default values.
        /// </summary>
        /// <param name="attrs">Destination collection of the attributes to add values.</param>
        /// <param name="lcid">Culture identifier.</param>
        private static void FillSectPr(SectPr attrs, int lcid)
        {
            // Do not clone to avoid duplicating of the object with reference type.
            for (int i = 0; i < gDefaults.Count; i++)
              attrs.Add(gDefaults.GetKey(i), gDefaults.GetByIndex(i));

            // Fill culture depended attributes.
            LocaleDefaults.Margins margins = GetDefaultPageMargins(lcid);

            // MS Word uses different page margin defaults depending on the current culture.
            // See X:\awspecs\DOC[MS-DOC].pdf, page 151 sprmSDxaColumns for example.
            // AW mimics this behavior.
            // There might be other values dependent on locale.
            attrs.Add(SectAttr.LeftMargin, margins.Left);
            attrs.Add(SectAttr.RightMargin, margins.Right);
            attrs.Add(SectAttr.TopMargin, margins.Top);
            attrs.Add(SectAttr.BottomMargin, margins.Bottom);

            attrs.Add(SectAttr.HeaderDistance, GetDefaultHeaderFooterDistance(lcid, false));
            attrs.Add(SectAttr.FooterDistance, GetDefaultHeaderFooterDistance(lcid, false));

            attrs.Remove(SectAttr.ColumnsSpacing);

            attrs.Add(SectAttr.ColumnsSpacing, GetDefaultSpaceBetweenColumns(lcid, false));
        }

        private static void WarnUnexpected(IWarningCallback warningCallback, string description)
        {
            if (warningCallback != null)
                warningCallback.Warning(new WarningInfo(WarningType.UnexpectedContent, WarningSource.Unknown, description));
        }

        static SectPr()
        {
            gDefaults = new SectPr();

            gDefaults.Add(SectAttr.SectionStart, SectionStart.NewPage);
            gDefaults.Add(SectAttr.DifferentFirstPageHeaderFooter, false);
            gDefaults.Add(SectAttr.SuppressEndnotes, false);
            gDefaults.Add(SectAttr.VerticalAlignment, PageVerticalAlignment.Top);
            gDefaults.Add(SectAttr.TextFlow, TextFlow.Horizontal);
            gDefaults.Add(SectAttr.Bidi, false);

            gDefaults.Add(SectAttr.PageWidth, 12240);
            gDefaults.Add(SectAttr.PageHeight, 15840);
            gDefaults.Add(SectAttr.Orientation, Orientation.Portrait);
            gDefaults.Add(SectAttr.PaperCode, 0);

            gDefaults.Add(SectAttr.Gutter, 0);

            gDefaults.Add(SectAttr.FirstPageTray, 0);
            gDefaults.Add(SectAttr.OtherPagesTray, 0);

            gDefaults.Add(SectAttr.ChapterPageSeparator, ChapterPageSeparator.Hyphen);
            gDefaults.Add(SectAttr.HeadingLevelForChapter, 0);
            gDefaults.Add(SectAttr.PageNumberStyle, NumberStyle.Arabic);
            gDefaults.Add(SectAttr.RestartPageNumbering, false);
            gDefaults.Add(SectAttr.PageStartingNumber, 1);

            gDefaults.Add(SectAttr.LineNumberRestartMode, LineNumberRestartMode.RestartPage);
            gDefaults.Add(SectAttr.LineNumberCountBy, 0);
            gDefaults.Add(SectAttr.LineNumberDistanceFromText, 0);
            gDefaults.Add(SectAttr.LineStartingNumber, 1);

            gDefaults.Add(SectAttr.ColumnsEvenlySpaced, true);
            gDefaults.Add(SectAttr.ColumnsCount, 1);
            gDefaults.Add(SectAttr.ColumnsLineBetween, false);
            // This is a complex attribute. Be careful not to modify the actual values in it.
            gDefaults.Add(SectAttr.Columns, new TextColumnCollectionInternal());

            gDefaults.Add(SectAttr.RtlGutter, false);
            gDefaults.Add(SectAttr.Unlocked, false);

            // These are complex attributes. Be careful not to modify the actual values in them.
            gDefaults.Add(SectAttr.BorderLeft, new Border());
            gDefaults.Add(SectAttr.BorderRight, new Border());
            gDefaults.Add(SectAttr.BorderTop, new Border());
            gDefaults.Add(SectAttr.BorderBottom, new Border());

            gDefaults.Add(SectAttr.BorderAppliesTo, PageBorderAppliesTo.AllPages);
            gDefaults.Add(SectAttr.BorderDistanceFrom, PageBorderDistanceFrom.Text);
            gDefaults.Add(SectAttr.BorderAlwaysInFront, true);

            gDefaults.Add(SectAttr.CharSpace, 0);
            gDefaults.Add(SectAttr.LinePitch, 0);
            gDefaults.Add(SectAttr.GridType, 0);

            gDefaults.Add(SectAttr.FootnoteLocation, FootnotePosition.BottomOfPage);
            gDefaults.Add(SectAttr.FootnoteNumberStyle, NumberStyle.Arabic);
            gDefaults.Add(SectAttr.FootnoteStartNumber, 1);
            gDefaults.Add(SectAttr.FootnoteNumberingRule, FootnoteNumberingRule.Continuous);
            gDefaults.Add(SectAttr.FootnoteColumns, 0);

            gDefaults.Add(SectAttr.EndnoteLocation, EndnotePosition.EndOfDocument);
            gDefaults.Add(SectAttr.EndnoteNumberStyle, NumberStyle.LowercaseRoman);
            gDefaults.Add(SectAttr.EndnoteStartNumber, 1);
            gDefaults.Add(SectAttr.EndnoteNumberingRule, FootnoteNumberingRule.Continuous);

            gDefaults.Add(SectAttr.Sys_GprfIhdt, GrpfIhdt.None);

            gDefaults.Add(SectAttr.Rsid, 0);
        }

        /// <summary>
        /// Sets defaults that depend on locale. Changes only properties that are already set.
        /// </summary>
        internal void SetLocaleDefaultsForNewDocument()
        {
            int currentLcid = GetCurrentLcid();

            LocaleDefaults.Margins margins = GetDefaultPageMargins(currentLcid);

            if (ContainsKey(SectAttr.LeftMargin))
                SetAttr(SectAttr.LeftMargin, margins.Left);
            if (ContainsKey(SectAttr.RightMargin))
                SetAttr(SectAttr.RightMargin, margins.Right);
            if (ContainsKey(SectAttr.TopMargin))
                SetAttr(SectAttr.TopMargin, margins.Top);
            if (ContainsKey(SectAttr.BottomMargin))
                SetAttr(SectAttr.BottomMargin, margins.Bottom);

            if (ContainsKey(SectAttr.HeaderDistance))
                SetAttr(SectAttr.HeaderDistance, GetDefaultHeaderFooterDistance(currentLcid, true));
            if (ContainsKey(SectAttr.FooterDistance))
                SetAttr(SectAttr.FooterDistance, GetDefaultHeaderFooterDistance(currentLcid, true));

            if (ContainsKey(SectAttr.ColumnsSpacing))
                SetAttr(SectAttr.ColumnsSpacing, GetDefaultSpaceBetweenColumns(currentLcid, true));
        }

        /// <summary>
        /// Sets default page size that depends on locale.
        /// </summary>
        internal void SetDefaultPageSize()
        {
            int currentLcid = GetCurrentLcid();
            Size defaultPageSize = LocaleDefaults.GetPageSize(currentLcid);
            SetAttr(SectAttr.PageWidth, defaultPageSize.Width);
            SetAttr(SectAttr.PageHeight, defaultPageSize.Height);
        }

        /// <summary>
        /// Gets default page margins dependent on the current locale. Returns predefined value in test mode.
        /// </summary>
        private static LocaleDefaults.Margins GetDefaultPageMargins(int currentLcid)
        {
            if (currentLcid != TestIndefiniteLocale)
                return LocaleDefaults.GetPageMargins(currentLcid);
            else
                return gTestDefaultMargins;
        }

        /// <summary>
        /// Gets default header/footer distance dependent on the current locale. Returns predefined value in test mode.
        /// </summary>
        private static int GetDefaultHeaderFooterDistance(int currentLcid, bool forNewDoc)
        {
            if (currentLcid != TestIndefiniteLocale)
                return LocaleDefaults.GetHeaderFooterDistance(currentLcid);
            else
                return forNewDoc ? TestHeaderFooterDistanceForNew : TestHeaderFooterDistanceForLoad;
        }

        /// <summary>
        /// Gets default column spacing dependent on the current locale. Returns predefined value in test mode.
        /// </summary>
        private static int GetDefaultSpaceBetweenColumns(int currentLcid, bool forNewDoc)
        {
            if (currentLcid != TestIndefiniteLocale)
                return LocaleDefaults.GetSpaceBetweenColumns(currentLcid);
            else
                return forNewDoc ? TestColumnSpacingForNew : TestColumnSpacingForLoad;
        }

        /// <summary>
        /// Returns current locale ID or TestIndefiniteLocale value in test mode.
        /// </summary>
        internal static int GetCurrentLcid()
        {
            if (!gTestMode)
            {
                CultureInfo culture = Thread.CurrentThread.CurrentCulture;
                return culture.LCID;
            }
            else
            {
                return TestIndefiniteLocale;
            }
        }

        /// <summary>
        /// Stores collections of the default values for attributes.
        /// Appropriate collection can be obtained by culture identifier.
        /// </summary>
        /// <remarks>Access level is internal for test purposes.</remarks>
        internal static readonly IntToObjDictionary<SectPr> DefaultsCache = new IntToObjDictionary<SectPr>();

        /// <summary>
        /// Enumerates all section border key.
        /// </summary>
        internal static readonly int[] BorderAttrs = new int[]
        {
            SectAttr.BorderTop,
            SectAttr.BorderLeft,
            SectAttr.BorderBottom,
            SectAttr.BorderRight,
        };

        /// <summary>
        /// Object to synchronize access to cache with default values of the attributes.
        /// </summary>
        private static readonly object gDefaultsSync = new object();

        /// <summary>
        /// Collection of the culture independent attributes with default values.
        /// </summary>
        private static readonly SectPr gDefaults;

        /// <summary>
        /// This static member is used to set defaults of page margin, header/footer distance and column spacing
        /// to old values that is needed for large number of old tests.
        /// </summary>
#if !JAVA
        [ThreadStatic]
#endif
        private static bool gTestMode;

        private int mSystemLcid;

        private static readonly LocaleDefaults.Margins gTestDefaultMargins =
            new LocaleDefaults.Margins(1800, 1800, 1440, 1440);

        private const int TestIndefiniteLocale = -1;
        private const int TestHeaderFooterDistanceForLoad = 720;
        private const int TestHeaderFooterDistanceForNew = 708;
        private const int TestColumnSpacingForLoad = 720;
        private const int TestColumnSpacingForNew = 708;
    }
}
