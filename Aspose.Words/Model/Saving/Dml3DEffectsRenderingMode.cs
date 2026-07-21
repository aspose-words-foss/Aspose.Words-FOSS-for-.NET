// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/04/2019 by Dmitry Khudobin

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies how 3D shape effects are rendered.
    /// </summary>
    public enum Dml3DEffectsRenderingMode
    {
        /// <summary>
        /// A lightweight and stable rendering, based on the internal engine, 
        /// but advanced effects such as lighting, materials and other additional effects 
        /// are not displayed when using this mode.
        /// Please see documentation for details.
        /// </summary>
        Basic = 0,

        /// <summary>
        /// Rendering of an extended list of special effects including advanced 3D effects 
        /// such as bevels, lighting and materials.
        /// </summary>
        /// <remarks>
        /// The current implementation uses OpenGL.
        /// Please make sure that OpenGL library version 1.1 or higher is installed on your system before use.
        /// This mode is still under development, and some things may not be supported, so it's recommended to use 
        /// the <see cref="Basic"/> mode if the rendering result is not acceptable.
        /// Please see documentation for details.
        /// </remarks>
        Advanced = 1
    }
}
