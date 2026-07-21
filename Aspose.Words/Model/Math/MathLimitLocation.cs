// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/03/2011 by Denis Darkin
namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the location of limits in n-ary operators. Limits can be either centered above and below
    /// the n-ary operator, or positioned just to the right of the operator.
    /// </summary>
    internal enum MathLimitLocation
    {
        /// <summary>
        /// intLim (for integral n-ary operator objects) and naryLim (for all other n-ary operator objects) 
        /// options are looked to for the limit placement.
        /// </summary>
        Undefined = -1,
        
        /// <summary>
        /// Limits placed to the side of the base.
        /// </summary>
        SubscriptSuperscript = 0,
        
        /// <summary>
        /// Limits placed to the directly above and below the base.
        /// </summary>
        UnderOver = 1
    }
}