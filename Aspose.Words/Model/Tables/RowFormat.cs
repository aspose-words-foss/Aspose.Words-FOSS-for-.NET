// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/05/2004 by Roman Korchagin
using System.Collections.Generic;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Represents all formatting for a table row.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-tables/">Working with Tables</a> documentation article.</para>
    /// </summary>
    /// <dev>
    /// RK I don't expose Shading here because default shading in MS Word works very strange,
    /// cells do not seem to inherit the shading specified for the table so it will be pretty
    /// confusing to work with. Setting row shading should actually go and set shading for
    /// all of the cells in the row.
    /// </dev>
    public class RowFormat : IBorderAttrSource
    {
        internal RowFormat(IRowAttrSource parent)
        {
            mParent = parent;
        }

        /// <summary>
        /// Resets to default row formatting.
        /// </summary>
        public void ClearFormatting()
        {
            mParent.ResetToDefaultAttrs();
        }

        /// <summary>
        /// Gets the collection of default cell borders for the row.
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
        /// Gets or sets the height of the table row in points.
        /// </summary>
        public double Height
        {
            get
            {
                Height height = (Height)mParent.FetchRowAttr(TableAttr.RowHeight);
                return ConvertUtilCore.TwipToPoint(height.Value);
            }
            set
            {
                Height height = (Height)FetchOrCreateComplexRowAttr(TableAttr.RowHeight);
                height.Value = ConvertUtilCore.PointToTwip(value);
            }
        }

        /// <summary>
        /// Gets or sets the rule for determining the height of the table row.
        /// </summary>
        public HeightRule HeightRule
        {
            get
            {
                Height height = (Height)mParent.FetchRowAttr(TableAttr.RowHeight);
                return height.Rule;
            }
            set
            {
                Height height = (Height)FetchOrCreateComplexRowAttr(TableAttr.RowHeight);
                height.Rule = value;
            }
        }

        /// <summary>
        /// True if the text in a table row is allowed to split across a page break.
        /// </summary>
        public bool AllowBreakAcrossPages
        {
            get { return (bool)FetchAttr(TableAttr.AllowBreakAcrossPages); }
            set { mParent.SetRowAttr(TableAttr.AllowBreakAcrossPages, value); }
        }

        /// <summary>
        /// True if the row is repeated as a table heading on every page when the table spans more than one page.
        /// </summary>
        public bool HeadingFormat
        {
            get { return (bool)FetchAttr(TableAttr.HeadingFormat); }
            set { mParent.SetRowAttr(TableAttr.HeadingFormat, value); }
        }

        private object FetchAttr(int key)
        {
            return mParent.FetchRowAttr(key);
        }

        object IBorderAttrSource.GetDirectBorderAttr(int key)
        {
            return mParent.GetDirectRowAttr(key);
        }

        object IBorderAttrSource.FetchInheritedBorderAttr(int key)
        {
            return mParent.FetchInheritedRowAttr(key);
        }

        void IBorderAttrSource.SetBorderAttr(int key, object value)
        {
            mParent.SetRowAttr(key, value);
        }

        SortedList<BorderType, int> IBorderAttrSource.PossibleBorderKeys
        {
            get { return TablePr.PossibleBorderKeys; }
        }

        private object FetchOrCreateComplexRowAttr(int key)
        {
            object directValue = mParent.GetDirectRowAttr(key);

            if (directValue != null)
                return directValue;

            object defaultValue = TablePr.FetchDefaultAttr(key);

            object clonedValue = ((IComplexAttr)defaultValue).DeepCloneComplexAttr();
            mParent.SetRowAttr(key, clonedValue);

            return clonedValue;
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IRowAttrSource mParent;
        private BorderCollection mBorders;
    }
}
