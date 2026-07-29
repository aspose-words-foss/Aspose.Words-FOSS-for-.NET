// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/06/2009 by Roman Korchagin

using System;
using System.Collections.Generic;
using Aspose.Words.Tables;

namespace Aspose.Words
{
    /// <summary>
    /// Represents special formatting applied to some area of a table with assigned table style.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-tables/">Working with Tables</a> documentation article.</para>
    /// </summary>
    public sealed class ConditionalStyle : IBorderAttrSource, IShadingAttrSource, IParaAttrSource, IRunAttrSource
    {
        internal ConditionalStyle(TableStyleOverrideType overrideType)
            : this (overrideType, null)
        {
        }

        internal ConditionalStyle(TableStyleOverrideType overrideType, TableStyle parentStyle)
        {
            mOverrideType = overrideType;
            mParentStyle = parentStyle;
        }

        /// <summary>
        /// Clears formatting of this conditional style.
        /// </summary>
        public void ClearFormatting()
        {
            if (mParaPr != null)
                mParaPr.Clear();
            if (mRunPr != null)
                mRunPr.Clear();
            if (mTablePr != null)
                mTablePr.Clear();
            if (mRowPr != null)
                mRowPr.Clear();
            if (mCellPr != null)
                mCellPr.Clear();
        }

        /// <summary>
        /// Makes a deep copy of the object.
        /// </summary>
        internal ConditionalStyle Clone()
        {
            ConditionalStyle lhs = (ConditionalStyle)MemberwiseClone();

            if (mParaPr != null)
                lhs.mParaPr = mParaPr.Clone();

            if (mRunPr != null)
                lhs.mRunPr = mRunPr.Clone();

            if (mTablePr != null)
                lhs.mTablePr = mTablePr.Clone();

            if (mRowPr != null)
                lhs.mRowPr = mRowPr.Clone();

            if (mCellPr != null)
                lhs.mCellPr = mCellPr.Clone();

            return lhs;
        }

        /// <summary>
        /// Compares this conditional style with the specified object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (typeof(ConditionalStyle) != obj.GetType())
                return false;

            return Equals((ConditionalStyle)obj);
        }

        /// <summary>
        /// Compares this conditional style with the specified one.
        /// </summary>
        internal bool Equals(ConditionalStyle style)
        {
            if (style == null)
                return false;

            return
                (OverrideType == style.OverrideType) &&
                (RunPr.Equals(style.RunPr, Style.ComparisonIgnorableKeys)) &&
                (ParaPr.Equals(style.ParaPr, Style.ComparisonIgnorableKeys)) &&
                (CellPr.Equals(style.CellPr, Style.ComparisonIgnorableKeys)) &&
                (RowPr.Equals(style.RowPr, Style.ComparisonIgnorableKeys)) &&
                (TablePr.Equals(style.TablePr, Style.ComparisonIgnorableKeys));
        }

        /// <summary>
        /// Calculates hash code for this object.
        /// </summary>
        /// <javaName>int hashCode()</javaName>
        public override int GetHashCode()
        {
            int hashCode = OverrideType.GetHashCode();
            hashCode = (hashCode * 397) ^ (HasRunFormatting ? mRunPr.GetHashCode(Style.ComparisonIgnorableKeys) : 0);
            hashCode = (hashCode * 397) ^ (HasParagraphFormatting ? mParaPr.GetHashCode(Style.ComparisonIgnorableKeys) : 0);
            hashCode = (hashCode * 397) ^ (HasCellFormatting ? mCellPr.GetHashCode(Style.ComparisonIgnorableKeys) : 0);
            hashCode = (hashCode * 397) ^ (HasRowFormatting ? mRowPr.GetHashCode(Style.ComparisonIgnorableKeys) : 0);
            hashCode = (hashCode * 397) ^ (HasTableFormatting ? mTablePr.GetHashCode(Style.ComparisonIgnorableKeys) : 0);
            return hashCode;
        }

        /// <summary>
        /// Sets parent table style for this conditional style.
        /// </summary>
        internal void SetParentStyle(TableStyle value)
        {
            mParentStyle = value;
        }

        #region IBorderAttrSource

        object IBorderAttrSource.GetDirectBorderAttr(int key)
        {
            return (mCellPr != null) ? mCellPr[key] : null;
        }

        object IBorderAttrSource.FetchInheritedBorderAttr(int key)
        {
            object value = GetInheritedConditionalStyleAttr(AttrCollectionType.CellPr, key);
            if (value != null)
                return value;

            // Not taken from table borders like in VBA.
            return CellPr.FetchDefaultAttr(key);
        }

        void IBorderAttrSource.SetBorderAttr(int key, object value)
        {
            CellPr.SetAttr(key, value);
        }

        SortedList<BorderType, int> IBorderAttrSource.PossibleBorderKeys
        {
            get { return CellPr.PossibleBorderKeys; }
        }

        #endregion

        #region IShadingAttrSource

        object IShadingAttrSource.FetchInheritedShadingAttr(int key)
        {
            return FetchInheritedCellAttr(key);
        }

        #endregion

        #region IParaAttrSource

        void IParaAttrSource.SetParaAttr(int key, object value)
        {
            ParaPr.SetAttr(key, value);
        }

        void IParaAttrSource.RemoveParaAttr(int key)
        {
            if (mParaPr != null)
                mParaPr.Remove(key);
        }

        void IParaAttrSource.ClearParaAttrs()
        {
            if (mParaPr != null)
                mParaPr.Clear();
        }

        object IParaAttrSource.FetchInheritedParaAttr(int key)
        {
            object value = GetInheritedConditionalStyleAttr(AttrCollectionType.ParaPr, key);
            if (value != null)
                return value;

            value = mParentStyle.GetParaAttr(key, RevisionsView.Original);
            if (value != null)
                return value;

            return mParentStyle.Styles.DefaultParaPr.FetchAttr(key);
        }

        object IParaAttrSource.GetDirectParaAttr(int key)
        {
            return (mParaPr != null) ? mParaPr.GetDirectAttr(key) : null;
        }

        object IParaAttrSource.GetDirectParaAttr(int key, RevisionsView revisionsView)
        {
            return (mParaPr != null) ? mParaPr.GetDirectAttr(key, revisionsView) : null;
        }

        object IParaAttrSource.FetchParaAttr(int key)
        {
            object value = (mParaPr != null) ? mParaPr[key] : null;
            return (value != null) ? value : ((IParaAttrSource)this).FetchInheritedParaAttr(key);
        }

        #endregion

        #region IRunAttrSource

        object IRunAttrSource.GetDirectRunAttr(int key)
        {
            return ((IRunAttrSource)this).GetDirectRunAttr(key, RevisionsView.Original);
        }

        object IRunAttrSource.GetDirectRunAttr(int key, RevisionsView revisionsView)
        {
            return (mRunPr != null) ? mRunPr.GetDirectAttr(key, revisionsView) : null;
        }

        object IRunAttrSource.FetchInheritedRunAttr(int key)
        {
            object value = GetInheritedConditionalStyleAttr(AttrCollectionType.RunPr, key);
            return (value != null) ? value : mParentStyle.GetFontAttr(key, true);
        }

        void IRunAttrSource.SetRunAttr(int key, object value)
        {
            RunPr.SetAttr(key, value);
        }

        void IRunAttrSource.RemoveRunAttr(int key)
        {
            if (mRunPr != null)
                mRunPr.Remove(key);
        }

        void IRunAttrSource.ClearRunAttrs()
        {
            if (mRunPr != null)
                mRunPr.Clear();
        }

        #endregion

        /// <summary>
        /// Fetches inherited attribute of the conditional style from the specified attribute collection.
        /// Only base conditional styles are searched for.
        /// </summary>
        private object GetInheritedConditionalStyleAttr(AttrCollectionType type, int key)
        {
            TableStyle style = mParentStyle;

            while (true)
            {
                style = style.GetBaseStyle() as TableStyle;
                if (style == null)
                    break;

                if (!style.ConditionalStyles.ContainsTableStyleOverride(mOverrideType))
                    continue;

                AttrCollection collection = style.ConditionalStyles[mOverrideType].GetCollection(type);
                if (collection == null)
                    continue;

                object value = collection[key];
                if (value != null)
                    return value;
            }

            return null;
        }

        /// <summary>
        /// Fetches inherited cell attribute from the specified attribute collection.
        /// </summary>
        /// <remarks>
        /// At first the attribute is searched in base conditional styles, then in table styles (in parent and base
        /// styles). If not found, a default value is returned.
        /// </remarks>
        private object FetchInheritedCellAttr(int key)
        {
            object value = GetInheritedConditionalStyleAttr(AttrCollectionType.CellPr, key);
            return (value != null) ? value : ((ICellAttrSource)mParentStyle).FetchCellAttr(key);
        }

        /// <summary>
        /// Gets a direct cell attribute, or inherited if not defined.
        /// </summary>
        private object GetCellAttr(int key)
        {
            if (mCellPr != null)
            {
                object value = mCellPr[key];
                if (value != null)
                    return value;
            }

            return FetchInheritedCellAttr(key);
        }

        /// <summary>
        /// Gets the paragraph formatting of the conditional style.
        /// </summary>
        public ParagraphFormat ParagraphFormat
        {
            get
            {
                if (mParagraphFormatCache == null)
                    mParagraphFormatCache = new ParagraphFormat(this, mParentStyle.Styles);
                return mParagraphFormatCache;
            }
        }

        /// <summary>
        /// Gets the character formatting of the conditional style.
        /// </summary>
        public Font Font
        {
            get
            {
                if (mFontCache == null)
                    mFontCache = new Font(this, mParentStyle.Document);
                return mFontCache;
            }
        }

        /// <summary>
        /// Gets a <see cref="Words.Shading"/> object that refers to the shading formatting for this conditional style.
        /// </summary>
        public Shading Shading
        {
            get
            {
                Shading shading = (Shading)CellPr[CellAttr.Shading];
                if (shading == null)
                {
                    ConditionalStyle parent = this;
                    shading = new Shading(parent, CellAttr.Shading);
                    shading = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(shading, parent);
                    CellPr.SetAttr(CellAttr.Shading, shading);
                }
                return shading;
            }
        }

        /// <summary>
        /// Gets the collection of default cell borders for the conditional style.
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
        /// Gets or sets the amount of space (in points) to add to the left of the contents of table cells.
        /// </summary>
        public double LeftPadding
        {
            get { return ConvertUtilCore.TwipToPoint((int)GetCellAttr(CellAttr.LeftPadding)); }
            set { CellPr.SetAttr(CellAttr.LeftPadding, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Gets or sets the amount of space (in points) to add to the right of the contents of table cells.
        /// </summary>
        public double RightPadding
        {
            get { return ConvertUtilCore.TwipToPoint((int)GetCellAttr(CellAttr.RightPadding)); }
            set { CellPr.SetAttr(CellAttr.RightPadding, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Gets or sets the amount of space (in points) to add above the contents of table cells.
        /// </summary>
        public double TopPadding
        {
            get { return ConvertUtilCore.TwipToPoint((int)GetCellAttr(CellAttr.TopPadding)); }
            set { CellPr.SetAttr(CellAttr.TopPadding, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Gets or sets the amount of space (in points) to add below the contents of table cells.
        /// </summary>
        public double BottomPadding
        {
            get { return ConvertUtilCore.TwipToPoint((int)GetCellAttr(CellAttr.BottomPadding)); }
            set { CellPr.SetAttr(CellAttr.BottomPadding, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Gets table area to which this conditional style relates.
        /// </summary>
        public ConditionalStyleType Type
        {
            get
            {
                switch (mOverrideType)
                {
                    case TableStyleOverrideType.FirstRow:
                        return ConditionalStyleType.FirstRow;
                    case TableStyleOverrideType.FirstColumn:
                        return ConditionalStyleType.FirstColumn;
                    case TableStyleOverrideType.LastRow:
                        return ConditionalStyleType.LastRow;
                    case TableStyleOverrideType.LastColumn:
                        return ConditionalStyleType.LastColumn;
                    case TableStyleOverrideType.OddRowBanding:
                        return ConditionalStyleType.OddRowBanding;
                    case TableStyleOverrideType.OddColumnBanding:
                        return ConditionalStyleType.OddColumnBanding;
                    case TableStyleOverrideType.EvenRowBanding:
                        return ConditionalStyleType.EvenRowBanding;
                    case TableStyleOverrideType.EvenColumnBanding:
                        return ConditionalStyleType.EvenColumnBanding;
                    case TableStyleOverrideType.TopLeftCell:
                        return ConditionalStyleType.TopLeftCell;
                    case TableStyleOverrideType.TopRightCell:
                        return ConditionalStyleType.TopRightCell;
                    case TableStyleOverrideType.BottomLeftCell:
                        return ConditionalStyleType.BottomLeftCell;
                    case TableStyleOverrideType.BottomRightCell:
                        return ConditionalStyleType.BottomRightCell;
                    case TableStyleOverrideType.WholeTable:
                    case TableStyleOverrideType.None:
                    default:
                    {
                        // WholeTable and None conditional styles are for internal usage and are not be visible for
                        // customers.
                        // 'Word does not apply and discards on save any properties within the tblStylePr element when
                        // the "type" attribute has a value of "wholeTable"' [MS-OI29500].
                        Debug.Assert(false);
                        return ConditionalStyleType.FirstRow;
                    }
                }
            }
        }

        /// <summary>
        /// Gets table area to which this conditional style relates.
        /// </summary>
        internal TableStyleOverrideType OverrideType
        {
            get { return mOverrideType; }
        }

        /// <summary>
        /// Paragraph properties of the conditional style.
        /// </summary>
        internal ParaPr ParaPr
        {
            get
            {
                if (mParaPr == null)
                    mParaPr = new ParaPr();
                return mParaPr;
            }
            set { mParaPr = value; }
        }

        /// <summary>
        /// Returns <c>true</c> if this conditional style has any paragraph formatting defined.
        /// </summary>
        internal bool HasParagraphFormatting
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                return (mParaPr != null) && (mParaPr.Count > 0);
            }
        }

        /// <summary>
        /// Run properties of the conditional style.
        /// </summary>
        internal RunPr RunPr
        {
            get
            {
                if (mRunPr == null)
                    mRunPr = new RunPr();
                return mRunPr;
            }
            set { mRunPr = value; }
        }

        /// <summary>
        /// Returns <c>true</c> if this conditional style has any run formatting defined.
        /// </summary>
        internal bool HasRunFormatting
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                return (mRunPr != null) && (mRunPr.Count > 0);
            }
        }

        /// <summary>
        /// Table properties of the conditional style.
        /// </summary>
        internal TablePr TablePr
        {
            get
            {
                if (mTablePr == null)
                    mTablePr = new TablePr();
                return mTablePr;
            }
            set { mTablePr = value; }
        }

        /// <summary>
        /// Returns <c>true</c> if this conditional style has any table formatting defined.
        /// </summary>
        internal bool HasTableFormatting
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                return (mTablePr != null) && (mTablePr.Count > 0);
            }
        }

        /// <summary>
        /// Row properties of the conditional style.
        /// </summary>
        internal TablePr RowPr
        {
            get
            {
                if (mRowPr == null)
                    mRowPr = new TablePr();
                return mRowPr;
            }
            set { mRowPr = value; }
        }

        /// <summary>
        /// Returns <c>true</c> if this conditional style has any row formatting defined.
        /// </summary>
        internal bool HasRowFormatting
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                return (mRowPr != null) && (mRowPr.Count > 0);
            }
        }

        /// <summary>
        /// Cell properties of the conditional style.
        /// </summary>
        internal CellPr CellPr
        {
            get
            {
                if (mCellPr == null)
                    mCellPr = new CellPr();
                return mCellPr;
            }
            set { mCellPr = value; }
        }

        /// <summary>
        /// Returns <c>true</c> if this conditional style has any cell formatting defined.
        /// </summary>
        internal bool HasCellFormatting
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                return (mCellPr != null) && (mCellPr.Count > 0);
            }
        }

        /// <summary>
        /// Returns <c>true</c> if there is at least one formatting attribute specified in this conditional style.
        /// </summary>
        internal bool HasFormatting
        {
            get
            {
                return
                    HasTableFormatting ||
                    HasRowFormatting ||
                    HasCellFormatting ||
                    HasParagraphFormatting ||
                    HasRunFormatting;
            }
        }

        /// <summary>
        /// Returns property collection of given type.
        /// </summary>
        internal WordAttrCollection GetCollection(AttrCollectionType type)
        {
            switch (type)
            {
                case AttrCollectionType.RunPr:
                    return mRunPr;
                case AttrCollectionType.ParaPr:
                    return mParaPr;
                case AttrCollectionType.CellPr:
                    return mCellPr;

                default:
                    throw new InvalidOperationException("Unexpected AttrCollectionType value.");
            }
        }

        private readonly TableStyleOverrideType mOverrideType;
        private ParaPr mParaPr;
        private RunPr mRunPr;
        private TablePr mTablePr;
        private TablePr mRowPr;
        private CellPr mCellPr;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private TableStyle mParentStyle;

        private ParagraphFormat mParagraphFormatCache;
        private Font mFontCache;
        private BorderCollection mBordersCache;
    }
}
