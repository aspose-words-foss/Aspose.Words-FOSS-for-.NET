// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the Border Box object, consisting of a border drawn around an instance of mathematical 
    /// text (such as a formula or equation).
    /// </summary>
    /// <remarks>
    /// If borderBoxPr is omitted then the default behavior of borderBox is a rectangular border.</remarks>
    internal class MathObjectBorderBox : MathObject
    {
        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.BorderBox; }
        }

         /// <summary>
        /// Specifies the hidden or shown state of the bottom edge of this borderBox.
        /// </summary>
        internal bool HideBottomEdge
        {
            get { return (bool)FetchAttr(MathAttr.HideBottom); }
            set { SetAttr(MathAttr.HideBottom, value, value); }
        }
        
        /// <summary>
        /// Specifies the hidden or shown state of the top edge of this borderBox.
        /// Default: false
        /// </summary>
        internal bool HideTopEdge
        {
            get { return (bool)FetchAttr(MathAttr.HideTop); }
            set { SetAttr(MathAttr.HideTop, value, value); }
        }
        
        /// <summary>
        /// Specifies the hidden or shown state of the left edge of this borderBox.
        /// Default: false
        /// </summary>
        internal bool HideLeftEdge
        {
            get { return (bool)FetchAttr(MathAttr.HideLeft); }
            set { SetAttr(MathAttr.HideLeft, value, value); }
        }
        
        /// <summary>
        /// Specifies the hidden or shown state of the left edge of this borderBox.
        /// Default: false
        /// </summary>
        internal bool HideRightEdge
        {
            get { return (bool)FetchAttr(MathAttr.HideRight); }
            set { SetAttr(MathAttr.HideRight, value, value); }
        }
        
        /// <summary>
        /// Specifies the hidden or shown state of a strikethrough diagonal line from the bottom-left corner to
        /// the top-right corner of this borderBox.
        /// When set to false, the strikethrough is not drawn.
        /// Default: false
        /// </summary>
        internal bool StrikeBLTR
        {
            get { return (bool)FetchAttr(MathAttr.StrikeBLTR); }
            set { SetAttr(MathAttr.StrikeBLTR, value, value); }
        }
        
        /// <summary>
        /// Specifies the hidden or shown state of a strikethrough horizontal line in this borderBox.
        /// When set to false, the strikethrough is not drawn.
        /// Default: false
        /// </summary>
        internal bool StrikeH
        {
            get { return (bool)FetchAttr(MathAttr.StrikeH); }
            set { SetAttr(MathAttr.StrikeH, value, value); }
        }
        
        /// <summary>
        /// Specifies the hidden or shown state of a strikethrough vertical line in this borderBox.
        /// When set to false, the strikethrough is not drawn.
        /// Default: false
        /// </summary>
        internal bool StrikeV
        {
            get { return (bool)FetchAttr(MathAttr.StrikeV); }
            set { SetAttr(MathAttr.StrikeV, value, value); }
        }

        /// <summary>
        /// Specifies the hidden or shown state of a strikethrough diagonal line from the top-left corner to 
        /// the bottom-right corner of this borderBox.
        /// When set to false, the strikethrough is not drawn.
        /// Default: false
        /// </summary>
        internal bool StrikeTLBR
        {
            get { return (bool)FetchAttr(MathAttr.StrikeTLBR); }
            set { SetAttr(MathAttr.StrikeTLBR, value, value); }
        }
    }
}
