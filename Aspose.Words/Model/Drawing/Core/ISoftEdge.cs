// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/04/2024 by Ilya Navrotskiy

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Implements interface for object that can have <see cref="SoftEdgeFormat"/>.
    /// </summary>
    internal interface ISoftEdge
    {
        /// <summary>
        /// Removes soft edge effect from the parent object;
        /// </summary>
        void RemoveSoftEdge();

        /// <summary>
        /// Gets or sets the length of the radius for a soft edge effect.
        /// </summary>
        double EdgeRadius { get; set; }
    }
}
