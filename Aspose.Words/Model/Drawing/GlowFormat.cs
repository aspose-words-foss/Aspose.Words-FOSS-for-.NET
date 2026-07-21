// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/03/2024 by Ilya Navrotskiy

using System.Drawing;
using Aspose.Words.Drawing.Core;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Represents the glow formatting for an object.
    /// </summary>
    /// <remarks>
    /// <p>Use the <see cref="ShapeBase.Glow"/> property to access glow properties of an object.
    /// You do not create instances of the <see cref="GlowFormat"/> class directly.</p>
    /// </remarks>
    /// <dev>
    /// This class is the public facade for <see cref="IGlow"/>.
    /// https://learn.microsoft.com/en-us/office/vba/api/word.glowformat
    /// </dev>
    public class GlowFormat
    {
        /// <summary>
        /// Creates an instance of the <see cref="GlowFormat"/> class.
        /// </summary>
        internal GlowFormat(IGlow parent)
        {
            mParent = parent;
        }

        /// <summary>
        /// Removes <see cref="GlowFormat"/> from the parent object.
        /// </summary>
        public void Remove()
        {
            mParent.RemoveGlow();
        }

        /// <summary>
        /// Gets or sets a <see cref="System.Drawing.Color"/> object that represents the color for a glow effect.
        /// The default value is <see cref="System.Drawing.Color.Black"/>.
        /// </summary>
        public Color Color
        {
            get { return mParent.Color; }
            set { mParent.Color = value; }
        }

        /// <summary>
        /// Gets or sets the degree of transparency for the glow effect as a value between 0.0 (opaque) and 1.0 (clear).
        /// The default value is 0.0.
        /// </summary>
        public double Transparency
        {
            get { return mParent.Transparency; }
            set { mParent.Transparency = value; }
        }

        /// <summary>
        /// Gets or sets a double value that represents the length of the radius for a glow effect in points (pt).
        /// The default value is 0.0.
        /// </summary>
        public double Radius
        {
            get { return mParent.Radius; }
            set { mParent.Radius = value; }
        }

        /// <summary>
        /// The parent object.
        /// </summary>
        private readonly IGlow mParent;
    }
}
