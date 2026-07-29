// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/05/2024 by Ilya Navrotskiy

using System.Drawing;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Implements interface for object that can have <see cref="ShadowFormat"/>.
    /// </summary>
    internal interface IShadow
    {
        /// <summary>
        /// Removes shadow from the parent object;
        /// </summary>
        void RemoveShadow();

        /// <summary>
        /// Gets or sets a <see cref="ShadowType"/> value for the shadow effect.
        /// </summary>
        ShadowType ShadowType { get; set; }

        /// <summary>
        /// Gets a boolean value indicating whether shadow effect is visible.
        /// </summary>
        bool Visible { get; }

        /// <summary>
        /// Gets or sets <see cref="System.Drawing.Color"/> object that represents the color for a shadow effect.
        /// </summary>
        Color ShadowColors { get; set; }

        /// <summary>
        /// Gets or sets the degree of transparency for a shadow effect.
        /// </summary>
        double ShadowTransparency { get; set; }
    }
}
