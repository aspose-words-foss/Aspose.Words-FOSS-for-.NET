// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/07/2006 by Roman Korchagin


namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Specifies the type of a shape shadow.
    /// </summary>
    internal enum ShadowTypeCore
    {
        /// <summary>
        /// N pixel offset shadow.
        /// </summary>
        Offset = 0,   
        /// <summary>
        /// Use second offset too.
        /// </summary>
        Double = 1,    
        /// <summary>
        /// Rich perspective shadow (cast relative to shape).
        /// </summary>
        Rich = 2,      
        /// <summary>
        /// Rich perspective shadow (cast in shape space).
        /// </summary>
        Shape = 3,     
        /// <summary>
        /// Perspective shadow cast in drawing space.
        /// </summary>
        Drawing = 4,   
        EmbossOrEngrave = 5,
    }
}
