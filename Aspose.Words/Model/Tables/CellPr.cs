// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/05/2005 by Roman Korchagin

using System;
using System.Collections.Generic;
using Aspose.Words.Revisions;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Collection of cell attributes. Provides typed access to the values stored in the collection.
    /// </summary>
    internal class CellPr : WordAttrCollection, ICellAttrSource
    {
        /// <summary>
        /// Removes all borders.
        /// </summary>
        internal void RemoveBorders()
        {
            foreach (int key in PossibleBorderKeys.Values)
                Remove(key);
        }

        /// <summary>
        /// Copies all borders to the specified CellPr collection.
        /// If there is no border in this collection,
        /// then removes it from the destination collection too.
        /// </summary>
        internal void CopyBordersTo(CellPr dstCellPr)
        {
            foreach (int key in PossibleBorderKeys.Values)
            {
                if (Contains(key))
                {
                    Border border = new Border();
                    border.CopyFrom((Border)this[key]);
                    dstCellPr[key] = border;
                }
                else
                {
                    dstCellPr.Remove(key);
                }
            }
        }
        
        internal void RemoveShading()
        {
            Remove(CellAttr.Shading);
        }

        /// <summary>
        /// Makes preferred width and width valid.
        /// Returns true if the width was modified by this method.
        /// </summary>
        internal bool ValidateWidth()
        {
            PreferredWidth prefWidth = (PreferredWidth)GetDirectAttr(CellAttr.PreferredWidth); 

            // Validate width. If the cell is not merged to previous, it is invalid for it to have zero width.
            // It is also invalid to have a negative width. Correct this situation by storing preferred width 
            // or the default cell width as the cell width.
            if ((Width <= 0) && (!IsMergedToPrevious))
            {
                if ((prefWidth != null) && prefWidth.IsFixed && prefWidth.IsPositive)
                {
                    Width = prefWidth.ValueTwips;
                }
                else
                {
                    // RK We have to set non-zero cell width for a document to be valid.
                    Width = DefaultCellWidthTwips;
                    // Dmatv: There are some test documents indicating that MS Word does not set the above default width in such cases.
                    // I've made some experiments with the document from TestDefect14170, trying to use minimum cell widths.
                    // I was not able to imitate the layout MS Word shows on opening the document.
                    // It appears that MS Word actually constructs the grid using a negative cell width,
                    // so that the cell with the negative width becomes completely invisible, and the cells after it
                    // appear shifted left for because of the negative width before them.
                    // I believe AW should not try to imitate that behavior.
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// RK This method is useful when reading DOCX and WML. In these documents we read preferred width first.
        /// Then we read table grid and set width from table grid. But table grid is not always in sync.
        /// So we improve our chances of getting the correct width by setting width from the preferred width (when it is fixed).
        /// It seems that the value in the preferred width is correct more often than in the table grid.
        /// </summary>
        internal void SetPreferredWidthAndWidth(PreferredWidth preferredWidth)
        {
            this.PreferredWidth = preferredWidth;
            if (preferredWidth.IsFixed)
                this.Width = preferredWidth.ValueTwips;
        }

        /// <summary>
        /// Sets the specified value in the desired collection depending on RevisionsView parameter, meanwhile the old value is being reserved in RevPr.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is currently used exclusively for setting a vertical merge value depending on the revision view.
        /// The second part of the method, when RevisionsView is equal to Final, is currently not being tested and is not being used anywhere in the code.
        /// </para>
        /// <para>
        /// AM. I don't fully understand how to set final properties. 
        /// Should we make new format revision if it absent? Where we get format revision author/date then?
        /// Or should we just set original property in this case? 
        /// That's why not going to put this into WordAttrCollection for a while.
        /// </para>
        /// </remarks>
        internal void SetAttrRevisionAware(int key, object value, RevisionsView view)
        {
            if (view == RevisionsView.Original)
            {
                if (FormatRevision != null)
                {
                    if (!FormatRevision.RevPr.Contains(key))
                    {
                        // first we need to preserve attribute in final collection.
                        MoveTo(FormatRevision.RevPr, key);
                    }
                }

                SetAttr(key, value);
            }
            else
            {
                // WORDSNET-21986 Do not create format revision if new value equals to original.
                object originalValue = FetchAttr(key, view);
                if (object.Equals(originalValue, value))
                    return;

                if (FormatRevision == null)
                    FormatRevision = new FormatRevision(new CellPr(), "", DateTime.Now);

                FormatRevision.RevPr.SetAttr(key, value);
            }
        }

        internal int Width
        {
            get { return (int)FetchAttr(CellAttr.Width); }
            set { SetAttr(CellAttr.Width, value); }
        }

        internal PreferredWidth PreferredWidth
        {
            get { return (PreferredWidth)FetchAttr(CellAttr.PreferredWidth); }
            set { SetAttr(CellAttr.PreferredWidth, value); }
        }

        internal CellMerge HorizontalMerge
        {
            get { return (CellMerge)FetchAttr(CellAttr.HorizontalMerge); }
            set { SetAttr(CellAttr.HorizontalMerge, value); }
        }

        internal CellMerge VerticalMerge
        {
            get { return (CellMerge)FetchAttr(CellAttr.VerticalMerge); }
            set { SetAttr(CellAttr.VerticalMerge, value); }
        }

        internal bool IsMergedToPrevious
        {
            get { return ((HorizontalMerge == CellMerge.Previous) || (VerticalMerge == CellMerge.Previous)); }
        }

        internal TextOrientation Orientation
        {
            get { return (TextOrientation)FetchAttr(CellAttr.Orientation); }
        }

        internal CellVerticalAlignment VerticalAlignment
        {
            get { return (CellVerticalAlignment)FetchAttr(CellAttr.VerticalAlignment); }
            set { SetAttr(CellAttr.VerticalAlignment, value); }
        }

        internal bool WrapText
        {
            get { return (bool)FetchAttr(CellAttr.WrapText); }
        }

        internal bool HideMark
        {
            get { return (bool) FetchAttr(CellAttr.HideMark); }
            set { SetAttr(CellAttr.HideMark, value); }
        }

        internal bool FitText
        {
            get { return (bool)FetchAttr(CellAttr.FitText); }
        }

        internal int TopPadding
        {
            get { return (int)FetchAttr(CellAttr.TopPadding); }
            set { SetAttr(CellAttr.TopPadding, value); }
        }

        internal int BottomPadding
        {
            get { return (int)FetchAttr(CellAttr.BottomPadding); }
            set { SetAttr(CellAttr.BottomPadding, value); }
        }

        internal int LeftPadding
        {
            get { return (int)FetchAttr(CellAttr.LeftPadding); }
            set { SetAttr(CellAttr.LeftPadding, value); }
        }

        internal int RightPadding
        {
            get { return (int)FetchAttr(CellAttr.RightPadding); }
            set { SetAttr(CellAttr.RightPadding, value); }
        }

        internal Border BorderTop
        {
            get { return (Border)GetDirectAttr(CellAttr.BorderTop); }
            set { SetAttr(CellAttr.BorderTop, value); }
        }

        internal Border BorderBottom
        {
            get { return (Border)GetDirectAttr(CellAttr.BorderBottom); }
            set { SetAttr(CellAttr.BorderBottom, value); }
        }

        internal Border BorderLeft
        {
            get { return (Border)GetDirectAttr(CellAttr.BorderLeft); }
            set { SetAttr(CellAttr.BorderLeft, value); }
        }

        internal Border BorderRight
        {
            get { return (Border)GetDirectAttr(CellAttr.BorderRight); }
            set { SetAttr(CellAttr.BorderRight, value); }
        }

        internal Border BorderDiagonalDown
        {
            get { return (Border)GetDirectAttr(CellAttr.BorderDiagonalDown); }
            set { SetAttr(CellAttr.BorderDiagonalDown, value); }
        }

        internal Border BorderDiagonalUp
        {
            get { return (Border)GetDirectAttr(CellAttr.BorderDiagonalUp); }
            set { SetAttr(CellAttr.BorderDiagonalUp, value); }
        }

        internal Border BorderHorizontal
        {
            get { return (Border)GetDirectAttr(CellAttr.BorderHorizontal); }
            set { SetAttr(CellAttr.BorderHorizontal, value); }
        }

        internal Border BorderVertical
        {
            get { return (Border)GetDirectAttr(CellAttr.BorderVertical); }
            set { SetAttr(CellAttr.BorderVertical, value); }
        }

        internal Shading Shading
        {
            get { return (Shading)GetDirectAttr(CellAttr.Shading); }
            set { SetAttr(CellAttr.Shading, value); }
        }

        /// <summary>
        /// Returns either revised or original properties depending on CellPr has format revision.
        /// Note that revised properties is NOT accepted properties and needs to be accepted to get full final property set.
        /// </summary>
        internal CellPr RevisedPr
        {
            get { return HasFormatRevision ? (CellPr)FormatRevision.RevPr : this; }
        }

        /// <summary>
        /// Returns final cell properties. If there is a formatting revision applied to the cell, accepted cell properties
        /// are returned. Otherwise, this cell properties are returned as is.
        /// </summary>
        internal CellPr FinalPr
        {
            get
            {
                if (!HasFormatRevision)
                {
                    return this;
                }

                CellPr finalPr = this.Clone();
                finalPr.AcceptFormatRevision();
                return finalPr;
            }
        }

        internal int GridSpan
        {
            get
            {
                // DM originally I treated 0 as invalid value of the attribute.
                // However when working on adjacent table merge it turned out that in doc/rtf merged cells can have zero width,
                // and the whole width of the merged cell is allocated to the first cell in the merge.
                // So that cell gets the whole grid span, and the rest of the cells have grid span 0.
                // I tried removing such cells from the model right away, but eventually decided that
                // it is better to do it as a separate change later. 
                // It affects about ten tests that check the model.

                return (int)FetchAttr(CellAttr.Sys_CellSpan);
                // FetchAttr() will return the default, that is, 1, if the attribute is not specified.
            }
            set
            {
                Debug.Assert(value >= 0);
                SetAttr(CellAttr.Sys_CellSpan, value);
            }
        }

        protected override AttrCollection GetDefaults()
        {
            return gDefaults;
        }

        internal static object FetchDefaultAttr(int key)
        {
            return gDefaults.FetchAttr(key);
        }

        #region ICellAttrSource

        object ICellAttrSource.GetDirectCellAttr(int key)
        {
            return base.GetDirectAttr(key);
        }

        object ICellAttrSource.FetchCellAttr(int key)
        {
            return base.FetchAttr(key);
        }

        object ICellAttrSource.FetchInheritedCellAttr(int key)
        {
            return base.FetchInheritedAttr(key);
        }

        void ICellAttrSource.SetCellAttr(int key, object value)
        {
            base.SetAttr(key, value);
        }

        void ICellAttrSource.ClearCellAttrs()
        {
            base.Clear();
        }

        #endregion

        static CellPr()
        {
            gDefaults = new CellPr();

            gDefaults.Add(CellAttr.Width, 0);
            gDefaults.Add(CellAttr.PreferredWidth, PreferredWidth.Auto);
            gDefaults.Add(CellAttr.VerticalMerge, CellMerge.None);
            gDefaults.Add(CellAttr.HorizontalMerge, CellMerge.None);
            gDefaults.Add(CellAttr.Orientation, TextOrientation.Horizontal);
            gDefaults.Add(CellAttr.VerticalAlignment, CellVerticalAlignment.Top);
            gDefaults.Add(CellAttr.TopPadding, 0);
            gDefaults.Add(CellAttr.BottomPadding, 0);
            gDefaults.Add(CellAttr.LeftPadding, 0);
            gDefaults.Add(CellAttr.RightPadding, 0);
            gDefaults.Add(CellAttr.WrapText, true);
            gDefaults.Add(CellAttr.HideMark, false);
            gDefaults.Add(CellAttr.FitText, false);

            // I'm of two minds whether cell borders and shading should have defaults.
            // On one hand, they should not because they must be resolved to the table defaults,
            // but on the other hand resolving to table defaults is quite hard, so I have these here for now.
            // These are complex attributes. Be careful not to modify the actual values in them.
            gDefaults.Add(CellAttr.BorderTop, new Border());
            gDefaults.Add(CellAttr.BorderLeft, new Border());
            gDefaults.Add(CellAttr.BorderBottom, new Border());
            gDefaults.Add(CellAttr.BorderRight, new Border());
            gDefaults.Add(CellAttr.BorderDiagonalDown, new Border());
            gDefaults.Add(CellAttr.BorderDiagonalUp, new Border());
            gDefaults.Add(CellAttr.BorderHorizontal, new Border());
            gDefaults.Add(CellAttr.BorderVertical, new Border());

            gDefaults.Add(CellAttr.Shading, new Shading());

            gDefaults.Add(CellAttr.Sys_CellSpan, 1);

            PossibleBorderKeys.Add(BorderType.Top, CellAttr.BorderTop);
            PossibleBorderKeys.Add(BorderType.Left, CellAttr.BorderLeft);
            PossibleBorderKeys.Add(BorderType.Bottom, CellAttr.BorderBottom);
            PossibleBorderKeys.Add(BorderType.Right, CellAttr.BorderRight);
            PossibleBorderKeys.Add(BorderType.DiagonalDown, CellAttr.BorderDiagonalDown);
            PossibleBorderKeys.Add(BorderType.DiagonalUp, CellAttr.BorderDiagonalUp);
        }

        private static readonly CellPr gDefaults;

        /// <summary>
        /// The key is <see cref="BorderType"/> that represents a border that can occur in a <see cref="TablePr"/> collection.
        /// The value is an <see cref="TableAttr"/> integer that represents the attribute key of that border.
        /// </summary>
        internal static readonly SortedList<BorderType, int> PossibleBorderKeys = 
            new SortedList<BorderType, int>();

        /// <summary>
        /// A default cell width to be used in case of zero cell width.
        /// </summary>
        /// <remarks>
        /// Zero cell width is invalid.
        /// </remarks>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int DefaultCellWidthTwips = 72 * 20;    //1"

        /// <summary>
        /// All possible cell border keys.
        /// </summary>
        internal static readonly int[] AllBorderKeys = new int[]
        {
            CellAttr.BorderLeft,
            CellAttr.BorderRight,
            CellAttr.BorderTop,
            CellAttr.BorderBottom,
            CellAttr.BorderVertical,
            CellAttr.BorderHorizontal,
            CellAttr.BorderDiagonalDown,
            CellAttr.BorderDiagonalUp
        };
    }
}
