// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/05/2004 by Roman Korchagin

using System.Collections.Generic;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Represents all formatting for a table cell.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-tables/">Working with Tables</a> documentation article.</para>
    /// </summary>
    public class CellFormat : IBorderAttrSource, IShadingAttrSource
    {
        internal CellFormat(ICellAttrSource parent)
        {
            mParent = parent;
        }

        /// <summary>
        /// Resets to default cell formatting. Does not change the width of the cell.
        /// </summary>
        public void ClearFormatting()
        {
            // WORDSNET-1277 Clearing cell format should not really change the cell width.
            object savedWidth = mParent.GetDirectCellAttr(CellAttr.Width);
            object savedPreferredWidth = mParent.GetDirectCellAttr(CellAttr.PreferredWidth);

            mParent.ClearCellAttrs();

            if (savedWidth != null)
                mParent.SetCellAttr(CellAttr.Width, savedWidth);
            if (savedPreferredWidth != null)
                mParent.SetCellAttr(CellAttr.PreferredWidth, savedPreferredWidth);
        }

        /// <summary>
        /// Resets to default cell formatting. Note, this includes clearing cell width too.
        /// </summary>
        internal void ClearAllFormatting()
        {
            mParent.ClearCellAttrs();
        }

        /// <summary>
        /// Sets the amount of space (in points) to add to the left/top/right/bottom of the contents of cell.
        /// </summary>
        public void SetPaddings(double leftPadding, double topPadding, double rightPadding, double bottomPadding)
        {
            LeftPadding = leftPadding;
            TopPadding = topPadding;
            RightPadding = rightPadding;
            BottomPadding = bottomPadding;
        }

        /// <summary>
        /// Returns or sets the amount of space (in points) to add to the left of the contents of cell.
        /// </summary>
        public double LeftPadding
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchAttr(CellAttr.LeftPadding)); }
            set { mParent.SetCellAttr(CellAttr.LeftPadding, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Returns or sets the amount of space (in points) to add to the right of the contents of cell.
        /// </summary>
        public double RightPadding
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchAttr(CellAttr.RightPadding)); }
            set { mParent.SetCellAttr(CellAttr.RightPadding, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Returns or sets the amount of space (in points) to add above the contents of cell.
        /// </summary>
        public double TopPadding
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchAttr(CellAttr.TopPadding)); }
            set { mParent.SetCellAttr(CellAttr.TopPadding, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Returns or sets the amount of space (in points) to add below the contents of cell.
        /// </summary>
        public double BottomPadding
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchAttr(CellAttr.BottomPadding)); }
            set { mParent.SetCellAttr(CellAttr.BottomPadding, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Gets collection of borders of the cell.
        /// </summary>
        public BorderCollection Borders
        {
            get
            {
                if (mBorders == null)
                    mBorders = new BorderCollection(this);
                return mBorders;
            }
        }

        /// <summary>
        /// Returns a <see cref="Aspose.Words.Shading"/> object that refers to the shading formatting for the cell.
        /// </summary>
        public Shading Shading
        {
            get
            {
                //<<GetOrCreateComplexAttr>> pattern
                Shading shading = (Shading)mParent.GetDirectCellAttr(CellAttr.Shading);
                if (shading == null)
                {
                    CellFormat parent = this;
                    shading = new Shading(parent, CellAttr.Shading);
                    shading = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(shading, parent);
                    mParent.SetCellAttr(CellAttr.Shading, shading);
                }
                return shading;
            }
        }

        /// <summary>
        /// Returns or sets the vertical alignment of text in the cell.
        /// </summary>
        public CellVerticalAlignment VerticalAlignment
        {
            get { return (CellVerticalAlignment)FetchAttr(CellAttr.VerticalAlignment); }
            set { mParent.SetCellAttr(CellAttr.VerticalAlignment, value); }
        }

        /// <summary>
        /// Returns or sets visibility of cell mark.
        /// </summary>
        /// <remarks>
        /// Specifies that table cell content is rendered with no height if all cells in the row are empty;
        /// however, cells have a visible height if they have nonzero cell borders, cell margins, or cell spacing.
        /// </remarks>
        public bool HideMark
        {
            get { return (bool)FetchAttr(CellAttr.HideMark); }
            set { mParent.SetCellAttr(CellAttr.HideMark, value); }
        }

        /// <summary>
        /// Gets the width of the cell in points.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The width is calculated by Aspose.Words on document loading and saving.
        /// Currently, not every combination of table, cell and document properties is supported.
        /// The returned value may not be accurate for some documents.
        /// It may not exactly match the cell width as calculated by MS Word when the document is opened in MS Word.
        /// </para>
        ///
        /// <para>
        /// Setting this property is not recommended.
        /// There is no guarantee that the cell will actually have the set width.
        /// The width may be adjusted to accommodate cell contents in an auto-fit table layout.
        /// Cells in other rows may have conflicting width settings.
        /// The table may be resized to fit into the container or to meet table width settings.
        /// Consider using <see cref="PreferredWidth"/> for setting the cell width.
        /// Setting this property sets <see cref="PreferredWidth"/> implicitly since version 15.8.
        /// </para>
        ///
        /// <seealso cref="Aspose.Words.Tables.CellFormat.PreferredWidth"/>
        /// </remarks>
        public double Width
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchAttr(CellAttr.Width)); }
            set
            {
                int valueTwips = ConvertUtilCore.PointToTwip(value);
                mParent.SetCellAttr(CellAttr.Width, valueTwips);

                PreferredWidth preferred = PreferredWidth.FromTwipsSafe(valueTwips);
                mParent.SetCellAttr(CellAttr.PreferredWidth, preferred);
            }
        }

        /// <summary>
        /// Returns or sets the preferred width of the cell.
        /// </summary>
        /// <remarks>
        /// <para>The preferred width (along with the table's Auto Fit option) determines how the actual
        /// width of the cell is calculated by the table layout algorithm. Table layout can be performed by
        /// Aspose.Words when it saves the document or by Microsoft Word when it displays the document.</para>
        ///
        /// <para>The preferred width can be specified in points or in percent. The preferred width
        /// can also be specified as "auto", which means no preferred width is specified.</para>
        ///
        /// <para>The default value is <see cref="Aspose.Words.Tables.PreferredWidth.Auto"/>.</para>
        ///
        /// <seealso cref="Width"/>
        /// </remarks>
        public PreferredWidth PreferredWidth
        {
            get { return (PreferredWidth)FetchAttr(CellAttr.PreferredWidth); }
            set { mParent.SetCellAttr(CellAttr.PreferredWidth, value); }
        }

        /// <summary>
        /// Specifies how the cell is merged with other cells vertically.
        /// </summary>
        /// <remarks>
        /// <p>Cells can only be merged vertically if their left and right boundaries are identical.</p>
        /// <p>When cells are vertically merged, the display areas of the merged cells are consolidated.
        /// The consolidated area is used to display the contents of the first vertically merged cell
        /// and all other vertically merged cells must be empty.</p>
        /// <seealso cref="HorizontalMerge"/>
        /// </remarks>
        public CellMerge VerticalMerge
        {
            get { return (CellMerge)FetchAttr(CellAttr.VerticalMerge); }
            set { mParent.SetCellAttr(CellAttr.VerticalMerge, value); }
        }

        /// <summary>
        /// Specifies how the cell is merged horizontally with other cells in the row.
        /// </summary>
        /// <seealso cref="VerticalMerge"/>
        public CellMerge HorizontalMerge
        {
            get { return (CellMerge)FetchAttr(CellAttr.HorizontalMerge); }
            set { mParent.SetCellAttr(CellAttr.HorizontalMerge, value); }
        }

        /// <summary>
        /// Returns or sets the orientation of text in a table cell.
        /// </summary>
        public TextOrientation Orientation
        {
            get { return (TextOrientation)FetchAttr(CellAttr.Orientation); }
            set { mParent.SetCellAttr(CellAttr.Orientation, value); }
        }

        /// <summary>
        /// If <c>true</c>, fits text in the cell, compressing each paragraph to the width of the cell.
        /// </summary>
        public bool FitText
        {
            get { return (bool)FetchAttr(CellAttr.FitText); }
            set { mParent.SetCellAttr(CellAttr.FitText, value); }
        }

        /// <summary>
        /// If <c>true</c>, wrap text for the cell.
        /// </summary>
        public bool WrapText
        {
            get { return (bool)FetchAttr(CellAttr.WrapText); }
            set { mParent.SetCellAttr(CellAttr.WrapText, value); }
        }

        private object FetchAttr(int key)
        {
            object value = mParent.GetDirectCellAttr(key);
            return (value != null) ? value : mParent.FetchInheritedCellAttr(key);
        }

        object IBorderAttrSource.GetDirectBorderAttr(int key)
        {
            return mParent.GetDirectCellAttr(key);
        }

        object IBorderAttrSource.FetchInheritedBorderAttr(int key)
        {
            return mParent.FetchInheritedCellAttr(key);
        }

        void IBorderAttrSource.SetBorderAttr(int key, object value)
        {
            mParent.SetCellAttr(key, value);
        }

        SortedList<BorderType, int> IBorderAttrSource.PossibleBorderKeys
        {
            get { return CellPr.PossibleBorderKeys; }
        }

        object IShadingAttrSource.FetchInheritedShadingAttr(int key)
        {
            return mParent.FetchInheritedCellAttr(key);
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly ICellAttrSource mParent;
        private BorderCollection mBorders;
    }
}
