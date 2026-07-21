// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/03/2011 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// Defines shape of delimiters.
    /// </summary>
    internal enum MathDelimiterShape
    {
        /// <summary>
        /// Delimiters centered around the math axis of the mathematical text and made to fit the entire height 
        /// of their contents.
        /// </summary>
        Centered,
        
        /// <summary>
        /// Delimiters height can be altered to exactly match their contents.
        /// </summary>
        Match,
      
        /// <summary>
        /// Default value is <see cref="Centered"/>.
        /// </summary>
        Default = Centered     
    }
}