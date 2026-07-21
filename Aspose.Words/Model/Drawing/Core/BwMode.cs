// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/02/2006 by Roman Korchagin

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Specifies how a shape will render on a black and white device.
    /// </summary>
    /// <seealso cref="ShapeAttr.BWMode"/>
    /// <seealso cref="ShapeAttr.BWNormal"/>
    /// <seealso cref="ShapeAttr.BWPure"/>
    internal enum BWMode
    {
        /// <summary>
        /// Only used for predefined shades.
        /// </summary>
        Color = 0,          
        /// <summary>
        /// Depends on object type.
        /// </summary>
        Automatic = 1,      
        /// <summary>
        /// Shades of gray only.
        /// </summary>
        GrayScale = 2,      
        /// <summary>
        /// Shades of light gray only.
        /// </summary>
        LightGrayScale = 3, 
        /// <summary>
        /// Dark gray mapped to light gray, etc.
        /// </summary>
        InverseGray = 4,    
        /// <summary>
        /// Pure gray and white.
        /// </summary>
        GrayOutline = 5,    
        /// <summary>
        /// Black text and lines, all else grayscale.
        /// </summary>
        BlackTextLine = 6,  
        /// <summary>
        /// Pure black and white mode (no grays).
        /// </summary>
        HighContrast = 7,   
        /// <summary>
        /// Solid black.
        /// </summary>
        Black = 8,          
        /// <summary>
        /// Solid white.
        /// </summary>
        White = 9,          
        /// <summary>
        /// Object not drawn.
        /// </summary>
        DontShow = 10,
        /// <summary>
        /// Default value is <see cref="Automatic"/>.
        /// </summary>
        Default = Automatic
    };
}
