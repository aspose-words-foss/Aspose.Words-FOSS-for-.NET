// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/01/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    /// <summary>
    /// 20.1.10.37 ST_PathFillMode (Path Fill Mode)
    /// This simple type specifies the manner in which a path should be filled. 
    /// The lightening and darkening of a path allow for certain parts of the 
    /// shape to be colored lighter or darker depending on user preference.
    /// </summary>
    internal enum DmlPathFillMode
    {
        /// <summary>
        /// This specifies that the corresponding path 
        /// should have a darker shaded color applied to it's fill.
        /// </summary>
        Darken,
        /// <summary>
        /// This specifies that the corresponding path 
        /// should have a slightly darker shaded color applied to it's fill.
        /// </summary>
        DarkenLess,
        /// <summary>
        /// This specifies that the corresponding path 
        /// should have a lightly shaded color applied to it's fill.
        /// </summary>
        Lighten,
        /// <summary>
        /// This specifies that the corresponding path 
        /// should have a slightly lighter shaded color applied to it's fill.
        /// </summary>
        LightenLess,
        /// <summary>
        /// This specifies that the corresponding path 
        /// should have no fill.
        /// </summary>
        None,
        /// <summary>
        /// This specifies that the corresponding path 
        /// should have a normally shaded color applied to it's fill.
        /// </summary>
        Norm
    }
}