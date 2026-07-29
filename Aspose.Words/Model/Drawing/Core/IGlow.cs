// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/03/2024 by Ilya Navrotskiy

using System.Drawing;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Implements interface for object that can have <see cref="GlowFormat"/>.
    /// </summary>
    internal interface IGlow
    {
        /// <summary>
        /// Removes glow from the parent object;
        /// </summary>
        void RemoveGlow();

        /// <summary>
        /// Gets or sets a Color object that represents the color for a glow effect.
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// Gets or sets a double value between 0.0 (opaque) and 1.0 (clear) representing the degree
        /// of transparency for the glow effect.
        /// </summary>
        double Transparency { get; set; }

        /// <summary>
        /// Gets or sets the length of the radius for a glow effect.
        /// </summary>
        double Radius { get; set; }
    }
}
