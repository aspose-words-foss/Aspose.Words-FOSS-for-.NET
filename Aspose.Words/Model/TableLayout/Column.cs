// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2006 by Dmitry Vorobyev

using Aspose.Words.Tables;

namespace Aspose.Words.TableLayout
{
    /// <summary>
    /// Represents a table column. Holds necessary width values that are required for layout calculations.
    /// </summary>
    internal class Column
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal Column()
        {
        }

        internal void InitializeEffectiveWidth()
        {
            mEffectivePreferredWidth = mPreferredWidth;
            mEffectiveLayoutWidth = mLayoutWidth.Clone();
        }

        internal PreferredWidth PreferredWidth
        {
            get { return mPreferredWidth; }
            set { mPreferredWidth = value; }
        }

        internal LayoutWidth LayoutWidth
        {
            get { return mLayoutWidth; }
            set { mLayoutWidth = value; }
        }

        /// <summary>
        /// Gets or sets the preferred width that is calculated based on preferred widths of the cells which compose the column.
        /// </summary>
        internal PreferredWidth EffectivePreferredWidth
        {
            get { return mEffectivePreferredWidth; }
            set { mEffectivePreferredWidth = value; }
        }

        /// <summary>
        /// Gets or sets the layout (min and max) width of the column.
        /// </summary>
        internal LayoutWidth EffectiveLayoutWidth
        {
            get { return mEffectiveLayoutWidth; }
            set { mEffectiveLayoutWidth = value; }
        }

        /// <summary>
        /// Gets or sets the resulting width used for setting cell widths.
        /// </summary>
        internal double CalculatedWidth
        {
            get { return mCalculatedWidth; }
            set { mCalculatedWidth = value; }
        }

        /// <summary>
        /// Ensures that the column has a minimum layout width.
        /// </summary>
        internal void EnsureColumnHasLayoutWidth()
        {
            LayoutWidth.Min = System.Math.Max(LayoutWidth.Min, ConvertUtilCore.MinSizePoint);   // 1 px.
            LayoutWidth.Max = System.Math.Max(LayoutWidth.Max, ConvertUtilCore.MinSizePoint);   // 1 px.
        }

        private PreferredWidth mPreferredWidth = PreferredWidth.Auto;
        private LayoutWidth mLayoutWidth = new LayoutWidth();
        private PreferredWidth mEffectivePreferredWidth;
        private LayoutWidth mEffectiveLayoutWidth;
        private double mCalculatedWidth;
    }
}
