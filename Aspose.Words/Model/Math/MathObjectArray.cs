// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the Array object: an object consisting of one or more equations, expressions, or other
    /// mathematical text runs that can be vertically justified as a unit with respect to surrounding text on the line.
    /// </summary>
    /// <remarks>This element is sometimes referred to as "Equation Array", despite its ability to hold
    /// mathematical text other than equations.</remarks>
    internal class MathObjectArray : MathObject
    {
        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.Array; }
        }

        /// <summary>
        /// Specifies the vertical justification of the array relative to the text outside of the array.
        /// Default: <see cref="MathBaseJustification.Default"/>
        /// </summary>
        internal MathBaseJustification BaseJustification
        {
            get { return (MathBaseJustification)FetchAttr(MathAttr.BaseJustification); }
            set { SetAttr(MathAttr.BaseJustification, value, value != MathBaseJustification.Default); }
        }
        
        /// <summary>
        /// specifies Array Maximum Distribution. 
        /// When true, the array is spaced to the maximum width of the containing element (page, column, cell, etc.). 
        /// Default: false
        /// </summary>
        /// <remarks>
        /// This property is commonly used with the <see cref="IsObjectDistribution"/> property.
        /// </remarks>
        internal bool IsMaximumDistribution
        {
            get { return (bool)FetchAttr(MathAttr.MaxDist); }
            set { SetAttr(MathAttr.MaxDist, value, value); }
        }
        
        /// <summary>
        /// Specifies Array Object Distribution. When true, the contents of the array are spaced to the
        /// maximum width of the array object.
        /// Default: false
        /// </summary>
        /// <remarks>This property is ignored by MS Word if <see cref="IsMaximumDistribution"/> is false.
        /// </remarks>
        internal bool IsObjectDistribution
        {
            get { return (bool)FetchAttr(MathAttr.ObjDist); }
            set { SetAttr(MathAttr.ObjDist, value, value); }
        }
        
        /// <summary>
        /// Specifies the type of vertical spacing between columns in a array. 
        /// Default is <see cref="MathSpacingRule.Default"/>
        /// </summary>
        internal MathSpacingRule RowSpacingRule
        {
            get { return (MathSpacingRule)FetchAttr(MathAttr.RowSpacingRule); }
            set { SetAttr(MathAttr.RowSpacingRule, value, value != MathSpacingRule.Default); }
        }
        
        /// <summary>
        /// This element specifies spacing between rows of an array eqArr; it is used only when 
        /// <see cref="RowSpacingRule"/> is set to <see cref="MathSpacingRule.Exactly"/> or
        /// <see cref="MathSpacingRule.Multiple"/>.
        /// Default value is 0.
        /// </summary>
        internal int RowSpacing
        {
            get { return (int)FetchAttr(MathAttr.RowSpacing); }
            set { SetAttr(MathAttr.RowSpacing, value, value != 0); }
        }
    }
}
