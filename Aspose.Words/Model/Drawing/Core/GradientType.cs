// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/07/2006 by Roman Korchagin

using System;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Defines the method used to generate a gradient fill.
    /// </summary>
    [Flags]
    internal enum GradientType
    {
        /// <summary>
        /// Interpolate without correction between RGBs.
        /// </summary>
        None  = 0,        
        /// <summary>
        /// Apply gamma correction to colors.
        /// </summary>
        Gamma = 1,        
        /// <summary>
        /// Apply a sigma transfer function to position.
        /// </summary>
        Sigma = 2,        
        /// <summary>
        /// Add a flat band at the start of the shade.
        /// </summary>
        Band  = 4,        
        /// <summary>
        /// This is a one color shade.
        /// </summary>
        OneColor = 8,     

        // TODO shape
//        /* A parameter for the band or sigma function can be stored in the top
//           16 bits of the value - this is a proportion of *each* band of the
//           shade to make flat (or the approximate equal value for a sigma
//           function).  NOTE: the parameter is not used for the sigma function,
//           instead a built in value is used.  This value should not be changed
//           from the default! */
//        ParameterShift = 16,
//        ParameterMask  = 0xffff0000,
//
//        Default = (Gamma|Sigma|
//            (16384<<ParameterShift))
    }
}
