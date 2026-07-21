// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/02/2011 by Denis Darkin
namespace Aspose.Words.Math
{
    /// <summary>
    /// Specifies the type of vertical spacing between rows in a <see cref="MathObjectMatrix"/> and
    /// rows in a <see cref="MathObjectArray"/>.
    /// </summary>
    internal enum MathSpacingRule
    {
        /// <summary>
        /// Single spacing gap (1 em)
        /// </summary>
        Single = 0,
    
        /// <summary>
        /// 1.5 spacing gap (1.5 ems)
        /// </summary>
        OneAndHalf = 1,
        
        /// <summary>
        /// 2 spacing gap (2 ems)
        /// </summary>
        Double = 2,
        
        /// <summary>
        /// Exactly 
        /// (for <see cref="MathObjectMatrix"/>, rely on value of <see cref="MathObjectMatrix.ColumnGap"/>, measured in twips)
        /// (for <see cref="MathObjectArray"/>, rely on value of <see cref="MathObjectArray.RowSpacing"/>, measured in twips)
        /// </summary>
        Exactly = 3,
        
        /// <summary>
        /// Multiple.
        /// (for  <see cref="MathObjectMatrix"/>,  rely on value of <see cref="MathObjectMatrix.ColumnGap"/>, 
        /// measured in 0.5 em increments) 
        /// (for <see cref="MathObjectArray"/>, rely on value of <see cref="MathObjectArray.RowSpacing"/>, 
        /// measured in 0.5 line increments)
        /// </summary>
        Multiple = 4,

        /// <summary>
        /// Default = <see cref="Single"/>
        /// </summary>
        Default = Single
    }
}