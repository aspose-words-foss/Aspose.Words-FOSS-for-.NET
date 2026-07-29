// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/03/2011 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// Specifies the type of fraction sign that should be drawn between numerator and denominator.
    /// </summary>
    internal enum MathFractionType
    {
        /// <summary>
        /// Bar Fraction
        /// </summary>
        Bar,
        
        /// <summary>
        /// Linear fraction
        /// </summary>
        Linear,
        
        /// <summary>
        /// Stack object, no bar drawn
        /// </summary>
        NoBar,
        
        /// <summary>
        /// Skewed fraction
        /// </summary>
        Skewed,

        /// <summary>
        /// Default is <see cref="Bar"/>
        /// </summary>
        Default = Bar,
    }
}
