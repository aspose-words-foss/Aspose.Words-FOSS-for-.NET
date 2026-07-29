// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System.Collections.Generic;
using System.Drawing;
using Aspose.Words.Notes;
using Aspose.Words.Sections;
using Aspose.Words.Settings;
using AsposePaperSize = Aspose.Words.PaperSize;

namespace Aspose.Words
{
    /// <summary>
    /// Represents the page setup properties of a section.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-sections/">Working with Sections</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="PageSetup"/> object contains all the page setup attributes of a section
    /// (left margin, bottom margin, paper size, and so on) as properties.</p>
    /// </remarks>
    /// <dev>
    /// This is a presentation object only, does not store any model data.
    /// </dev>
    public class PageSetup : IBorderAttrSource
    {
        /// <summary>
        /// Ctor for tests only: styles are not specified.
        /// </summary>
        /// <param name="parent">Source of page setup properties.</param>
        /// <param name="docPr">Needed to access some global page setup properties.</param>
        internal PageSetup(ISectionAttrSource parent, DocPr docPr)
            : this(parent, docPr, null, null)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="parent">Source of page setup properties.</param>
        /// <param name="docPr">Needed to access some global page setup properties.</param>
        /// <param name="styles">Needed to access Normal style to calculate <see cref="CharactersPerLine"/>.</param>
        /// <param name="firstSectionAttrSource">Needed to calculate content width/height and margins.</param>
        internal PageSetup(ISectionAttrSource parent, DocPr docPr, StyleCollection styles,
            ISectionAttrSource firstSectionAttrSource)
        {
            Debug.Assert(parent != null);
            Debug.Assert(docPr != null);

            mDocument = (styles != null) && (styles.Document is Document)
                ? styles.Document.FetchDocument()
                : null;
            mParent = parent;
            mDocPr = docPr;
            mStyles = styles;
            // If the first section attribute source is not specified, expect that 'parent' is attribute
            // source of the first/only section.
            mFirstSectionAttrSource = (firstSectionAttrSource != null) ? firstSectionAttrSource : parent;
        }

        /// <summary>
        /// Resets page setup to default paper size, margins and orientation.
        /// </summary>
        public void ClearFormatting()
        {
            mParent.ClearSectionAttrs();
        }

        /// <summary>
        /// True if the document has different headers and footers for odd-numbered and even-numbered pages.
        /// </summary>
        /// <remarks>
        /// Note, changing this property affects all sections in the document.
        /// </remarks>
        public bool OddAndEvenPagesHeaderFooter
        {
            get { return mDocPr.EvenAndOddHeaders; }
            set { mDocPr.EvenAndOddHeaders = value; }
        }

        /// <summary>
        /// True if a different header or footer is used on the first page.
        /// </summary>
        public bool DifferentFirstPageHeaderFooter
        {
            get { return (bool)FetchAttr(SectAttr.DifferentFirstPageHeaderFooter); }
            set { SetAttr(SectAttr.DifferentFirstPageHeaderFooter, value); }
        }

        /// <summary>
        /// For multiple page documents, gets or sets how a document is printed or rendered so that it can be bound as a booklet.
        /// </summary>
        public MultiplePagesType MultiplePages
        {
            get { return mDocPr.MultiplePages; }
            set { mDocPr.MultiplePages = value; }
        }

        /// <summary>
        /// Returns or sets the number of pages to be included in each booklet.
        /// </summary>
        public int SheetsPerBooklet
        {
            get { return mDocPr.BookFoldPrintingSheets; }
            set { mDocPr.BookFoldPrintingSheets = value; }
        }

        /// <summary>
        /// Returns or sets the type of section break for the specified object.
        /// </summary>
        public SectionStart SectionStart
        {
            get { return (SectionStart)FetchAttr(SectAttr.SectionStart); }
            set { SetAttr(SectAttr.SectionStart, value); }
        }

        /// <summary>
        /// True if endnotes are printed at the end of the next section that doesn't suppress endnotes.
        /// Suppressed endnotes are printed before the endnotes in that section.
        /// </summary>
        public bool SuppressEndnotes
        {
            get { return (bool)FetchAttr(SectAttr.SuppressEndnotes); }
            set { SetAttr(SectAttr.SuppressEndnotes, value); }
        }

        /// <summary>
        /// Returns or sets the vertical alignment of text on each page in a document or section.
        /// </summary>
        public PageVerticalAlignment VerticalAlignment
        {
            get { return (PageVerticalAlignment)FetchAttr(SectAttr.VerticalAlignment); }
            set { SetAttr(SectAttr.VerticalAlignment, value); }
        }

        /// <summary>
        /// Specifies that this section contains bidirectional (complex scripts) text.
        /// </summary>
        /// <remarks>
        /// <p>When <c>true</c>, the columns in this section are laid out from right to left.</p>
        /// </remarks>
        public bool Bidi
        {
            get { return (bool)FetchAttr(SectAttr.Bidi); }
            set { SetAttr(SectAttr.Bidi, value); }
        }

        /// <summary>
        /// Gets or sets the layout mode of this section.
        /// </summary>
        public SectionLayoutMode LayoutMode
        {
            get { return (SectionLayoutMode)FetchAttr(SectAttr.GridType); }
            set { SetAttr(SectAttr.GridType, (int)value); }
        }

        /// <summary>
        /// Gets or sets the number of characters per line in the document grid.
        /// </summary>
        /// <remarks>
        /// <para>Minimum value of the property is 1. Maximum value depends on page width and font size of the Normal
        /// style. Minimum character pitch is 90 percent of the font size. For example, maximum number of characters
        /// per line of a Letter page with one-inch margins is 43.</para>
        /// <para>By default, the property has a value, on which character pitch equals to font size of the Normal
        /// style.</para>
        /// </remarks>
        public int CharactersPerLine
        {
            get
            {
                Debug.Assert(mStyles != null);
                Style normalStyle = mStyles[StyleIdentifier.Normal];

                double pitch = normalStyle.Font.Size + CharSpace / 4096d;

                // MS Word calculates average column width with using space that is set in the 'cols' element, it does
                // not calculate sum of all column spaces: do the same.
                return (int)System.Math.Truncate(TextColumns.Width / pitch);
            }
            set
            {
                if (CharactersPerLine != value)
                {
                    Debug.Assert(mStyles != null);
                    Style normalStyle = mStyles[StyleIdentifier.Normal];

                    // Looks like MS Word uses the 0.9 factor without any depending on font face.
                    int maxValue = (int)System.Math.Truncate(TextColumns.Width / (normalStyle.Font.Size * 0.9));

                    ArgumentUtil.CheckRangeInclusive(value, 1, maxValue, "CharsPerLine");

                    double pitch = TextColumns.Width / value;
                    // We need to round to less integer to make the getter return the same value.
                    CharSpace = (int)System.Math.Truncate((pitch - normalStyle.Font.Size) * 4096);
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of lines per page in the document grid.
        /// </summary>
        /// <remarks>
        /// <para>Minimum value of the property is 1. Maximum value depends on page height and font size of the Normal
        /// style. Minimum line pitch is 136 percent of the font size. For example, maximum number of lines per page of
        /// a Letter page with one-inch margins is 39.</para>
        /// <para>By default, the property has a value, on which line pitch is in 1.5 times greater than font size of
        /// the Normal style.</para>
        /// </remarks>
        public int LinesPerPage
        {
            get { return (int)System.Math.Truncate(ContentHeightInTextDirection / LinePitch); }
            set
            {
                if (LinesPerPage != value)
                {
                    Debug.Assert(mStyles != null);
                    Style normalStyle = mStyles[StyleIdentifier.Normal];

                    // Looks like MS Word uses the 1.36 factor without any depending on font face.
                    int maxValue = (int)System.Math.Truncate(ContentHeightInTextDirection / normalStyle.Font.Size / 1.36);
                    ArgumentUtil.CheckRangeInclusive(value, 1, maxValue, "LinesPerPage");

                    // We need to round to less integer to make the getter return the same value.
                    SetAttr(SectAttr.LinePitch,
                        (int)System.Math.Truncate(ContentHeightInTextDirection / value * ConvertUtilCore.TwipsPerPoint));
                }
            }
        }

        /// <summary>
        /// Returns or sets the width of the page in points.
        /// </summary>
        public double PageWidth
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchAttr(SectAttr.PageWidth)); }
            set { SetAttr(SectAttr.PageWidth, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Returns or sets the height of the page in points.
        /// </summary>
        public double PageHeight
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchAttr(SectAttr.PageHeight)); }
            set { SetAttr(SectAttr.PageHeight, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Returns or sets preset <see cref="Aspose.Words.Margins"/> of the page.
        /// </summary>
        public Margins Margins
        {
            get
            {
                Margins[] marginTypes = new Margins[]
                {
                    Margins.Normal, Margins.Narrow, Margins.Moderate, Margins.Wide, Margins.Mirrored
                };

                foreach (Margins margins in marginTypes)
                {
                    LocaleDefaults.Margins marginValues = GetMarginValues(margins);

                    if ((marginValues.Left == (int)FetchAttr(SectAttr.LeftMargin)) &&
                        (marginValues.Right == (int)FetchAttr(SectAttr.RightMargin)) &&
                        (marginValues.Top == (int)FetchAttr(SectAttr.TopMargin)) &&
                        (marginValues.Bottom == (int)FetchAttr(SectAttr.BottomMargin)) &&
                        ((margins != Margins.Mirrored) || (mDocPr.MultiplePages == MultiplePagesType.MirrorMargins)))
                        return margins;
                }

                return Margins.Custom;
            }

            set
            {
                LocaleDefaults.Margins marginValues = GetMarginValues(value);
                SetAttr(SectAttr.LeftMargin, marginValues.Left);
                SetAttr(SectAttr.RightMargin, marginValues.Right);
                SetAttr(SectAttr.TopMargin, marginValues.Top);
                SetAttr(SectAttr.BottomMargin, marginValues.Bottom);

                if (value == Margins.Mirrored)
                    mDocPr.MultiplePages = MultiplePagesType.MirrorMargins;
            }
        }

        private static LocaleDefaults.Margins GetMarginValues(Margins margins)
        {
            if (margins == Margins.Normal)
                return LocaleDefaults.GetPageMargins(SectPr.GetCurrentLcid());

            return gPageMarginsPresets[margins];
        }

        internal SizeF PageSize
        {
            get { return new SizeF((float)PageWidth, (float)PageHeight); }
        }

        /// <summary>
        /// True if the gutter setting is applicable and it is a left gutter.
        /// </summary>
        internal bool IsLeftGutter
        {
            get { return (mDocPr.IsGutterSide && !RtlGutter); }
        }

        /// <summary>
        /// True if the gutter setting is applicable and it is a right gutter.
        /// </summary>
        internal bool IsRightGutter
        {
            get { return (mDocPr.IsGutterSide && RtlGutter); }
        }

        /// <summary>
        /// Gets distance of the left side of content area of page from the left page bound.
        /// </summary>
        /// <remarks>
        /// Note that the returned value may be not correct for even pages of the section since margins and gutter
        /// position may be different on odd and even pages depending on value of the <see cref="MultiplePages"/>
        /// property. Also MS Word may calculate margin size including gutter and page content size differently
        /// for different features/aspects (e.g. for column dimension, shape positioning, header/footer distance).
        /// </remarks>
        internal float ContentLeft
        {
            get
            {
                PageMarginCalculator calculator = CreatePageMarginCalculator();
                return (float)ConvertUtilCore.TwipToPoint(calculator.GetMarginWithGutter(PageSide.Left, true));
            }
        }

        /// <summary>
        /// Gets distance of the right side of content area of page from the left page bound.
        /// </summary>
        /// <remarks>
        /// Note that the returned value may be not correct for even pages of the section since margins and gutter
        /// position may be different on odd and even pages depending on value of the <see cref="MultiplePages"/>
        /// property. Also MS Word may calculate margin size including gutter and page content size differently
        /// for different features/aspects (e.g. for column dimension, shape positioning, header/footer distance).
        /// </remarks>
        internal float ContentRight
        {
            get { return ContentLeft + ContentWidth; }
        }

        /// <summary>
        /// Gets distance of the top side of content area of page from the top page bound.
        /// </summary>
        /// <remarks>
        /// Note that the returned value may be not correct for even pages of the section since margins and gutter
        /// position may be different on odd and even pages depending on value of the <see cref="MultiplePages"/>
        /// property. Also MS Word may calculate margin size including gutter and page content size differently
        /// for different features/aspects (e.g. for column dimension, shape positioning, header/footer distance).
        /// </remarks>
        internal float ContentTop
        {
            get
            {
                PageMarginCalculator calculator = CreatePageMarginCalculator();
                return (float)ConvertUtilCore.TwipToPoint(calculator.GetMarginWithGutter(PageSide.Top, true));
            }
        }

        /// <summary>
        /// Gets distance of the bottom side of content area of page from the top page bound.
        /// </summary>
        /// <remarks>
        /// Note that the returned value may be not correct for even pages of the section since margins and gutter
        /// position may be different on odd and even pages depending on value of the <see cref="MultiplePages"/>
        /// property. Also MS Word may calculate margin size including gutter and page content size differently
        /// for different features/aspects (e.g. for column dimension, shape positioning, header/footer distance).
        /// </remarks>
        internal float ContentBottom
        {
            get { return ContentTop + ContentHeight; }
        }

        /// <summary>
        /// Returns page width minus left and right margins and minus gutter (if it is on the left or right side).
        /// </summary>
        /// <remarks>
        /// Note that MS Word may calculate margin size including gutter and page content size differently for
        /// different features/aspects (e.g. for column dimension, shape positioning, header/footer distance).
        /// So, the returned value may be not entirely correct in some cases.
        /// </remarks>
        internal float ContentWidth
        {
            get
            {
                PageMarginCalculator calculator = CreatePageMarginCalculator();
                int leftMarginTwips = calculator.GetMarginWithGutter(PageSide.Left, true);
                int rightMarginTwips = calculator.GetMarginWithGutter(PageSide.Right, true);

                return (float)(PageWidth - ConvertUtilCore.TwipToPoint(leftMarginTwips + rightMarginTwips));
            }
        }

        /// <summary>
        /// Returns page height minus top and bottom margins and minus gutter (if it is on the top or bottom side).
        /// </summary>
        /// <remarks>
        /// Note that MS Word may calculate margin size including gutter and page content size differently for
        /// different features/aspects (e.g. for column dimension, shape positioning, header/footer distance).
        /// So, the returned value may be not entirely correct in some cases. Using <see cref="PageMarginCalculator"/>
        /// may solve such a problem.
        /// </remarks>
        internal float ContentHeight
        {
            get
            {
                PageMarginCalculator calculator = CreatePageMarginCalculator();
                int topMarginTwips = calculator.GetMarginWithGutter(PageSide.Top, true);
                int bottomMarginTwips = calculator.GetMarginWithGutter(PageSide.Bottom, true);

                return (float)(PageHeight - ConvertUtilCore.TwipToPoint(topMarginTwips + bottomMarginTwips));
            }
        }

        /// <summary>
        /// Returns content width depending on text direction.
        /// </summary>
        internal double ContentWidthInTextDirection
        {
            get { return IsHorizontalTextLine ? ContentWidth : ContentHeight; }
        }

        /// <summary>
        /// Returns content height depending on text direction.
        /// </summary>
        internal double ContentHeightInTextDirection
        {
            get { return IsHorizontalTextLine ? ContentHeight : ContentWidth; }
        }

        /// <summary>
        /// Returns or sets the paper size.
        /// </summary>
        /// <remarks>
        /// <para>Setting this property updates <see cref="PageWidth"/> and <see cref="PageHeight"/> values.
        /// Setting this value to <see cref="AsposePaperSize.Custom"/> does not change existing values.</para>
        /// </remarks>
        public PaperSize PaperSize
        {
            get
            {
                return WordUtil.SizeToPaperSize((int)FetchAttr(SectAttr.PageWidth), (int)FetchAttr(SectAttr.PageHeight), IsLandscape);
            }
            set
            {
                // WORDSNET-1474 DV Setting PaperSize to Custom should not change the existing page width and height.
                if (value == PaperSize.Custom)
                    return;

                Size size = WordUtil.PaperSizeToSize(value);
                if (IsLandscape)
                {
                    // Flip width and height when in landscape mode.
                    SetAttr(SectAttr.PageWidth, size.Height);
                    SetAttr(SectAttr.PageHeight, size.Width);
                }
                else
                {
                    SetAttr(SectAttr.PageWidth, size.Width);
                    SetAttr(SectAttr.PageHeight, size.Height);
                }
            }
        }

        private bool IsLandscape
        {
            get { return (Orientation == Orientation.Landscape); }
        }

        /// <summary>
        /// Returns or sets the orientation of the page.
        /// </summary>
        /// <remarks>
        /// <p>Changing <see cref="Orientation"/> swaps <see cref="PageWidth"/> and <see cref="PageHeight"/>.</p>
        /// </remarks>
        public Orientation Orientation
        {
            get { return (Orientation)FetchAttr(SectAttr.Orientation); }
            set
            {
                if (Orientation != value)
                {
                    SetAttr(SectAttr.Orientation, value);
                    double temp = PageWidth;
                    PageWidth = PageHeight;
                    PageHeight = temp;

                    // AM. Should we flip margins as well?
                }
            }
        }

        /// <summary>
        /// Returns or sets the distance (in points) between the left edge of the page and the left boundary of the body text.
        /// </summary>
        public double LeftMargin
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchAttr(SectAttr.LeftMargin)); }
            set { SetAttr(SectAttr.LeftMargin, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Returns or sets the distance (in points) between the right edge of the page and the right boundary of the body text.
        /// </summary>
        public double RightMargin
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchAttr(SectAttr.RightMargin)); }
            set { SetAttr(SectAttr.RightMargin, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Returns or sets the distance (in points) between the top edge of the page and the top boundary of the body text.
        /// </summary>
        public double TopMargin
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchAttr(SectAttr.TopMargin)); }
            set { SetAttr(SectAttr.TopMargin, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Returns or sets the distance (in points) between the bottom edge of the page and the bottom boundary of the body text.
        /// </summary>
        public double BottomMargin
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchAttr(SectAttr.BottomMargin)); }
            set { SetAttr(SectAttr.BottomMargin, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Returns or sets the distance (in points) between the header and the top of the page.
        /// </summary>
        public double HeaderDistance
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchAttr(SectAttr.HeaderDistance)); }
            set { SetAttr(SectAttr.HeaderDistance, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Returns or sets the distance (in points) between the footer and the bottom of the page.
        /// </summary>
        public double FooterDistance
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchAttr(SectAttr.FooterDistance)); }
            set { SetAttr(SectAttr.FooterDistance, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Gets or sets the amount of extra space added to the margin for document binding.
        /// </summary>
        public double Gutter
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchAttr(SectAttr.Gutter)); }
            set { SetAttr(SectAttr.Gutter, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Gets or sets the paper tray (bin) to use for the first page of a section.
        /// The value is implementation (printer) specific.
        /// </summary>
        public int FirstPageTray
        {
            get { return (int)FetchAttr(SectAttr.FirstPageTray); }
            set { SetAttr(SectAttr.FirstPageTray, value); }
        }

        /// <summary>
        /// Gets or sets the paper tray (bin) to be used for all but the first page of a section.
        /// The value is implementation (printer) specific.
        /// </summary>
        public int OtherPagesTray
        {
            get { return (int)FetchAttr(SectAttr.OtherPagesTray); }
            set { SetAttr(SectAttr.OtherPagesTray, value); }
        }

        /// <summary>
        /// Gets or sets the heading level style that is applied to the chapter titles in the document.
        /// </summary>
        /// <remarks>
        /// <p>Can be a number from 0 through 9. 0 means no chapter number if applied to page number.</p>
        /// <p>Before you can create page numbers that include chapter numbers, the document headings must have a numbered outline format applied.</p>
        /// </remarks>
        public int HeadingLevelForChapter
        {
            // pgNumType/@chap-style in WordML.
            get { return (int)FetchAttr(SectAttr.HeadingLevelForChapter); }
            set { SetAttr(SectAttr.HeadingLevelForChapter, value); }
        }

        /// <summary>
        /// Gets or sets the separator character that appears between the chapter number and the page number.
        /// </summary>
        /// <remarks>
        /// <p>Before you can create page numbers that include chapter numbers, the document headings must have a numbered outline format applied.</p>
        /// </remarks>
        public ChapterPageSeparator ChapterPageSeparator
        {
            // pgNumType/@chap-sep in WordML.
            get { return (ChapterPageSeparator)FetchAttr(SectAttr.ChapterPageSeparator); }
            set { SetAttr(SectAttr.ChapterPageSeparator, value); }
        }

        /// <summary>
        /// Gets or sets the page number format.
        /// </summary>
        public NumberStyle PageNumberStyle
        {
            // pgNumType/@fmt in WordML
            get { return (NumberStyle)FetchAttr(SectAttr.PageNumberStyle); }
            set { SetAttr(SectAttr.PageNumberStyle, value); }
        }

        /// <summary>
        /// True if page numbering restarts at the beginning of the section.
        /// </summary>
        /// <remarks>
        /// If set to <c>false</c>, the <see cref="RestartPageNumbering"/> property will override the
        /// <see cref="PageStartingNumber"/> property so that page numbering can continue from the previous section.
        /// </remarks>
        public bool RestartPageNumbering
        {
            get { return (bool)FetchAttr(SectAttr.RestartPageNumbering); }
            set { SetAttr(SectAttr.RestartPageNumbering, value); }
        }

        /// <summary>
        /// Gets or sets the starting page number of the section.
        /// </summary>
        /// <remarks>
        /// The <see cref="RestartPageNumbering"/> property, if set to <c>false</c>, will override the
        /// <see cref="PageStartingNumber"/> property so that page numbering can continue from the previous section.
        /// </remarks>
        public int PageStartingNumber
        {
            get { return (int)FetchAttr(SectAttr.PageStartingNumber); }
            set { SetAttr(SectAttr.PageStartingNumber, value); }
        }

        /// <summary>
        /// Gets or sets the way line numbering runs  that is, whether it starts over at the beginning of a new
        /// page or section or runs continuously.
        /// </summary>
        public LineNumberRestartMode LineNumberRestartMode
        {
            get { return (LineNumberRestartMode)FetchAttr(SectAttr.LineNumberRestartMode); }
            set { SetAttr(SectAttr.LineNumberRestartMode, value); }
        }

        /// <summary>
        /// Returns or sets the numeric increment for line numbers.
        /// </summary>
        public int LineNumberCountBy
        {
            get { return (int)FetchAttr(SectAttr.LineNumberCountBy); }
            set { SetAttr(SectAttr.LineNumberCountBy, value); }
        }

        /// <summary>
        /// Gets or sets distance between the right edge of line numbers and the left edge of the document.
        /// </summary>
        /// <remarks>
        /// Set this property to zero for automatic distance between the line numbers and text of the document.
        /// </remarks>
        public double LineNumberDistanceFromText
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchAttr(SectAttr.LineNumberDistanceFromText)); }
            set { SetAttr(SectAttr.LineNumberDistanceFromText, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Gets or sets the starting line number.
        /// </summary>
        public int LineStartingNumber
        {
            //lnNumType/@start in WordML, note it must be stored (value - 1).
            get { return (int)FetchAttr(SectAttr.LineStartingNumber); }
            set { SetAttr(SectAttr.LineStartingNumber, value); }
        }

        /// <summary>
        /// Returns a collection that represents the set of text columns.
        /// </summary>
        public TextColumnCollection TextColumns
        {
            get
            {
                if (mTextColumnsCache == null)
                    mTextColumnsCache = new TextColumnCollection(this);
                return mTextColumnsCache;
            }
        }

        /// <summary>
        /// Gets or sets whether Microsoft Word uses gutters for the section based on a right-to-left language or a left-to-right language.
        /// </summary>
        public bool RtlGutter
        {
            get { return (bool)FetchAttr(SectAttr.RtlGutter); }
            set { SetAttr(SectAttr.RtlGutter, value); }
        }

        /// <summary>
        /// Specifies where the page border is positioned relative to intersecting texts and objects.
        /// </summary>
        public bool BorderAlwaysInFront
        {
            get { return (bool)FetchAttr(SectAttr.BorderAlwaysInFront); }
            set { SetAttr(SectAttr.BorderAlwaysInFront, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the specified page border is measured from the edge of the page or from the text it surrounds.
        /// </summary>
        public PageBorderDistanceFrom BorderDistanceFrom
        {
            get { return (PageBorderDistanceFrom)FetchAttr(SectAttr.BorderDistanceFrom); }
            set { SetAttr(SectAttr.BorderDistanceFrom, value); }
        }

        /// <summary>
        /// Specifies which pages the page border is printed on.
        /// </summary>
        public PageBorderAppliesTo BorderAppliesTo
        {
            get { return (PageBorderAppliesTo)FetchAttr(SectAttr.BorderAppliesTo); }
            set { SetAttr(SectAttr.BorderAppliesTo, value); }
        }

        /// <summary>
        /// Specifies whether the page border includes or excludes the header.
        /// </summary>
        /// <remarks>
        /// Note, changing this property affects all sections in the document.
        /// </remarks>
        public bool BorderSurroundsHeader
        {
            get { return !mDocPr.BordersDoNotSurroundHeader; }
            set { mDocPr.BordersDoNotSurroundHeader = !value; }
        }

        /// <summary>
        /// Specifies whether the page border includes or excludes the footer.
        /// </summary>
        /// <remarks>
        /// Note, changing this property affects all sections in the document.
        /// </remarks>
        public bool BorderSurroundsFooter
        {
            get { return !mDocPr.BordersDoNotSurroundFooter; }
            set { mDocPr.BordersDoNotSurroundFooter = !value; }
        }

        /// <summary>
        /// Gets a collection of the page borders.
        /// </summary>
        public BorderCollection Borders
        {
            get
            {
                if (mBordersCache == null)
                    mBordersCache = new BorderCollection(this);
                return mBordersCache;
            }
        }

        /// <summary>
        /// Provides options that control numbering and positioning of footnotes in this section.
        /// </summary>
        public FootnoteOptions FootnoteOptions
        {
            get
            {
                if (mFootnoteOptionsCache == null)
                    mFootnoteOptionsCache = new FootnoteOptions(mParent);
                return mFootnoteOptionsCache;
            }
        }

        /// <summary>
        /// Provides options that control numbering and positioning of endnotes in this section.
        /// </summary>
        public EndnoteOptions EndnoteOptions
        {
            get
            {
                if (mEndnoteOptionsCache == null)
                    mEndnoteOptionsCache = new EndnoteOptions(mParent);
                return mEndnoteOptionsCache;
            }
        }

        /// <summary>
        /// Allows to specify <see cref="TextOrientation"/> for the whole page.
        /// Default value is <see cref="Aspose.Words.TextOrientation.Horizontal"/>
        /// </summary>
        /// <remarks>This property is only supported for MS Word native formats DOCX, WML, RTF and DOC.</remarks>
        public TextOrientation TextOrientation
        {
            get { return TextFlowOrientationConverter.FromTextFlow(TextFlow); }
            set { TextFlow = TextFlowOrientationConverter.FromTextOrientation(value); }
        }

        /// <summary>
        /// Gets or sets space between characters that allows defining the number of characters per line for a document.
        /// docGrid/@char-space
        /// </summary>
        /// <remarks>
        /// This attribute's value shall be specified by multiplying the difference between the desired character
        /// pitch and the character pitch for that character in the font size of the Normal font by 4096. If this
        /// attribute is omitted, the default value is zero.
        /// </remarks>
        internal int CharSpace
        {
            get { return (int)FetchAttr(SectAttr.CharSpace); }
            set { SetAttr(SectAttr.CharSpace, value); }
        }

        /// <summary>
        /// Gets or sets the line pitch and space between lines. The number of lines
        /// per page will automatically be adjusted to fit the space between the lines.
        /// docGrid/@line-pitch
        /// </summary>
        internal double LinePitch
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchAttr(SectAttr.LinePitch)); }
            set { SetAttr(SectAttr.LinePitch, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Specifies the text direction within this section.
        /// </summary>
        internal TextFlow TextFlow
        {
            get { return (TextFlow)FetchAttr(SectAttr.TextFlow); }
            set { SetAttr(SectAttr.TextFlow, value); }
        }

        /// <summary>
        /// Gets or sets internal paper code to ensure the proper type is chosen if size matches size
        /// of multiple paper types supported by your printer.
        ///
        /// Somewhat obscure field, does not look like PaperSize anyway.
        /// pgSz/@code in WordML.
        /// </summary>
        internal int PaperCode
        {
            get { return (int)FetchAttr(SectAttr.PaperCode); }
            set { SetAttr(SectAttr.PaperCode, value); }
        }

        /// <summary>
        /// True when this section is unprotected in a protected document.
        /// </summary>
        internal bool Unlocked
        {
            get { return (bool)FetchAttr(SectAttr.Unlocked); }
            set { SetAttr(SectAttr.Unlocked, value); }
        }

        internal ISectionAttrSource Parent
        {
            get { return mParent; }
        }

        /// <summary>
        /// Gets a flag indicating that text line is oriented horizontally.
        /// </summary>
        private bool IsHorizontalTextLine
        {
            get
            {
                return TextOrientation == TextOrientation.Horizontal ||
                    TextOrientation == TextOrientation.HorizontalRotatedFarEast;
            }
        }

        private object FetchAttr(int key)
        {
            if ((mDocument != null) && (mDocument.RevisionsView == RevisionsView.Final))
                return FetchAttr(key, RevisionsView.Final);

            return mParent.FetchSectionAttr(key);
        }

        private object FetchAttr(int key, RevisionsView revisionsView)
        {
            return mParent.FetchSectionAttr(key, revisionsView);
        }

        private void SetAttr(int key, object value)
        {
            if ((mDocument != null) && (mDocument.RevisionsView == RevisionsView.Final))
                SetAttr(key, value, RevisionsView.Final);
            else
                mParent.SetSectionAttr(key, value);
        }

        private void SetAttr(int key, object value, RevisionsView revisionsView)
        {
            mParent.SetSectionAttr(key, value, revisionsView);
        }

        /// <summary>
        /// Creates an instance of <see cref="PageMarginCalculator"/> that allows calculation of page margins and
        /// other metrics for different multiple page setups.
        /// </summary>
        private PageMarginCalculator CreatePageMarginCalculator()
        {
            return new PageMarginCalculator(mParent, mDocPr,
                (Orientation)mFirstSectionAttrSource.FetchSectionAttr(SectAttr.Orientation));
        }

        object IBorderAttrSource.GetDirectBorderAttr(int key)
        {
            return mParent.GetDirectSectionAttr(key);
        }

        object IBorderAttrSource.FetchInheritedBorderAttr(int key)
        {
            return mParent.FetchInheritedSectionAttr(key);
        }

        void IBorderAttrSource.SetBorderAttr(int key, object value)
        {
            mParent.SetSectionAttr(key, value);
        }

        SortedList<BorderType, int> IBorderAttrSource.PossibleBorderKeys
        {
            get { return gPageBorders; }
        }

        /// <summary>
        /// Static ctor.
        /// </summary>
        static PageSetup()
        {
            gPageBorders.Add(BorderType.Top, SectAttr.BorderTop);
            gPageBorders.Add(BorderType.Left, SectAttr.BorderLeft);
            gPageBorders.Add(BorderType.Bottom, SectAttr.BorderBottom);
            gPageBorders.Add(BorderType.Right, SectAttr.BorderRight);

            gPageMarginsPresets.Add(Margins.Narrow, new LocaleDefaults.Margins(720, 720, 720, 720));
            gPageMarginsPresets.Add(Margins.Moderate, new LocaleDefaults.Margins(1080, 1080, 1440, 1440));
            gPageMarginsPresets.Add(Margins.Wide, new LocaleDefaults.Margins(2880, 2880, 1440, 1440));
            gPageMarginsPresets.Add(Margins.Mirrored, new LocaleDefaults.Margins(1800, 1440, 1440, 1440));
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Document mDocument;
        private readonly ISectionAttrSource mParent;
        private readonly DocPr mDocPr;
        private readonly StyleCollection mStyles;
        private readonly ISectionAttrSource mFirstSectionAttrSource;
        private TextColumnCollection mTextColumnsCache;
        private BorderCollection mBordersCache;
        private FootnoteOptions mFootnoteOptionsCache;
        private EndnoteOptions mEndnoteOptionsCache;

        /// <summary>
        /// Map of possible page border types to attribute names.
        /// </summary>
        private static readonly SortedList<BorderType, int> gPageBorders = new SortedList<BorderType, int>();

        /// <summary>
        /// Map of page margins presets.
        /// </summary>
        private static readonly Dictionary<Margins, LocaleDefaults.Margins> gPageMarginsPresets = new Dictionary<Margins, LocaleDefaults.Margins>();
    }
}
