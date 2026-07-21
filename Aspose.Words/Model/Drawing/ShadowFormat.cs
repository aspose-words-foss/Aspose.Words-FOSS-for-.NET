// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/05/2022 by Vadim Saltykov

using System;
using System.Drawing;
using Aspose.Words.Drawing.Core;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Represents shadow formatting for an object.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-graphic-elements/">Working with Graphic Elements</a> documentation article.</para>
    /// </summary>
    public class ShadowFormat
    {
        /// <summary>
        /// Creates ShadowFormat object with a specified parent.
        /// </summary>
        internal ShadowFormat(IShadow parent)
        {
            mParent = parent;
        }

        /// <summary>
        /// Clears shadow format.
        /// </summary>
        public void Clear()
        {
            mParent.RemoveShadow();
        }

        /// <summary>
        /// Gets or sets the specified <see cref="ShadowType"/> for ShadowFormat.
        /// </summary>
        /// <remarks>
        /// Setting a new shadow type will reset Color and Transparency values to their default ones.
        /// Therefore, it makes sense to first set the desired shadow type and only then Color and Transparency values.
        /// </remarks>
        public ShadowType Type
        {
            get { return mParent.ShadowType; }
            set
            {
                if (mParent.ShadowType != value )
                    mParent.ShadowType = value;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the formatting applied to this instance is visible.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="Clear()"/>, assigning <c>false</c> to Visible does not clear the formatting,
        /// it only hides the shape effect.
        /// </remarks>
        public bool Visible
        {
            get { return mParent.Visible; }
            internal set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets a <see cref="System.Drawing.Color"/> object that represents the color for the shadow.
        /// The default value is <see cref="System.Drawing.Color.Black"/>.
        /// </summary>
        public Color Color
        {
            get { return mParent.ShadowColors; }
            set { mParent.ShadowColors = value; }
        }

        /// <summary>
        /// Gets or sets the degree of transparency for the shadow effect as a value between 0.0 (opaque) and 1.0 (clear).
        /// The default value is 0.0.
        /// </summary>
        public double Transparency
        {
            get { return mParent.ShadowTransparency; }
            set { mParent.ShadowTransparency = value; }
        }

        /// <summary>
        /// The parent object.
        /// </summary>
        private readonly IShadow mParent;
    }
}
