// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the Matrix object, consisting of one or more elements laid out in one or more rows and
    /// one or more columns. 
    /// </summary>
    /// <remarks>
    /// Matrices do not have built in delimiters. Like other math elements, matrices can be contained in a delimiter object 
    /// <see cref="MathObjectDelimiter"/> when delimiters are desired. 
    /// </remarks>
    internal class MathObjectMatrix : MathObject
    {
        /// <summary>
        /// Can only host element m:mr (math matrix row).
        /// </summary>
        internal override bool CanInsert(Node node)
        {
            return ((node.NodeType == NodeType.OfficeMath) && 
                    ((OfficeMath)node).MathObject.MathObjectType == MathObjectType.MatrixRow);
        }

        internal override AttrCollection CloneCore()
        {
            MathObjectMatrix lhs = (MathObjectMatrix)base.CloneCore();
            lhs.mColumnPrCollection = mColumnPrCollection.Clone();
            return lhs;
        }

        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.Matrix; }
        }

        /// <summary>
        /// Specifies the vertical justification of the array relative to the text outside of the array.
        /// Default: <see cref="MathBaseJustification.Default"/>
        /// </summary>
        /// <remarks>
        /// The standard does not specify in detail how to align the matrix with the surrounding content.
        /// MS Word uses baseline alignment to align the matrix with the surrounding content. 
        /// </remarks>
        internal MathBaseJustification BaseJustification
        {
            get { return (MathBaseJustification)FetchAttr(MathAttr.BaseJustification); }
            set { SetAttr(MathAttr.BaseJustification, value, value != MathBaseJustification.Default); }
        }

        /// <summary>
        /// This element specifies spacing between columns of matrix; it is used only when 
        /// <see cref="ColumnSpacingRule"/> is set to <see cref="MathSpacingRule.Exactly"/> or
        /// <see cref="MathSpacingRule.Multiple"/>.
        /// Default: 0.
        /// </summary>
        internal int ColumnGap
        {
            get { return (int)FetchAttr(MathAttr.ColumnGap); }
            set { SetAttr(MathAttr.ColumnGap, value, value != 0); }
        }
        
        /// <summary>
        /// Allows to define gaps between matrix columns.
        /// Default: <see cref="MathSpacingRule.Default"/>
        /// </summary>
        internal MathSpacingRule ColumnSpacingRule
        {
            get { return (MathSpacingRule)FetchAttr(MathAttr.ColumnGapRule); }
            set { SetAttr(MathAttr.ColumnGapRule, value, value != MathSpacingRule.Default); }
        }
        
        /// <summary>
        /// Specifies the minimum column width of a matrix. The actual column width of a matrix will be the
        /// greater of either the width of the column’s widest argument or this property value.
        /// Interpreted as twips.
        /// Default: 0
        /// </summary>
        internal int MinimumColumnWidth
        {
            get { return (int)FetchAttr(MathAttr.MinColumnWidth); }
            set
            {
                if (value > CeilingForMinimumColumnWidth)
                    value = CeilingForMinimumColumnWidth;
                    
                SetAttr(MathAttr.MinColumnWidth, value, value != 0);
            }
        }

        /// <summary>
        /// specifies the Hide Placeholders property on a matrix m. 
        /// When this property is true, placeholders do not appear in the matrix.
        /// Default: false.
        /// </summary>
        internal bool IsHidePlaceholders
        {
            get { return (bool)FetchAttr(MathAttr.IsHidePlaceholders); }
            set { SetAttr(MathAttr.IsHidePlaceholders, value, value); }
        }
        
        /// <summary>
        /// Specifies the type of vertical spacing between columns in a matrix. 
        /// Default: <see cref="MathSpacingRule.Default"/>
        /// </summary>
        internal MathSpacingRule RowSpacingRule
        {
            get { return (MathSpacingRule)FetchAttr(MathAttr.RowSpacingRule); }
            set { SetAttr(MathAttr.RowSpacingRule, value, value != MathSpacingRule.Default); }
        }
        
        /// <summary>
        /// This element specifies spacing between rows of this matrix; it is used only when 
        /// <see cref="RowSpacingRule"/> is set to <see cref="MathSpacingRule.Exactly"/> or
        /// <see cref="MathSpacingRule.Multiple"/>.
        /// Default: 0.
        /// </summary>
        internal int RowSpacing
        {
            get { return (int)FetchAttr(MathAttr.RowSpacing); }
            set { SetAttr(MathAttr.RowSpacing, value, value != 0); }
        }
        
        /// <summary>
        /// Contains a number of <see cref="MathMatrixColumnPr"/> object equal to number of columns in the matrix.
        /// Provide access to individual formatting properties of each matrix column.
        /// </summary>
        internal MathMatrixColumnPrCollection ColumnPrCollection
        {
            get { return mColumnPrCollection; }
        }

        private MathMatrixColumnPrCollection mColumnPrCollection = new MathMatrixColumnPrCollection();


        /// <summary>
        /// Office restricts the value of Minimum Matrix Column Width attribute to be at most 31680 (22").
        /// </summary>
        private const int CeilingForMinimumColumnWidth = 31680;

        /// <summary>
        /// Indicates whether the matrix is converted from EQ-array.
        /// </summary>
        internal bool IsEqArray { get; set; }
    }
}
