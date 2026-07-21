// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/04/2024 by Ilya Navrotskiy

using Aspose.Words.Drawing.Core;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Represents the soft edge formatting for an object.
    /// </summary>
    /// <remarks>
    /// <p>Use the <see cref="ShapeBase.SoftEdge"/> property to access soft edge properties of an object.
    /// You do not create instances of the <see cref="SoftEdgeFormat"/> class directly.</p>
    /// </remarks>
    /// <dev>
    /// This class is the public facade for <see cref="ISoftEdge"/>.
    /// https://learn.microsoft.com/en-us/office/vba/api/word.softedgeformat
    /// </dev>
    public class SoftEdgeFormat
    {
        /// <summary>
        /// Creates an instance of the <see cref="SoftEdgeFormat"/> class.
        /// </summary>
        internal SoftEdgeFormat(ISoftEdge parent)
        {
            mParent = parent;
        }

        /// <summary>
        /// Removes <see cref="SoftEdgeFormat"/> from the parent object.
        /// </summary>
        public void Remove()
        {
            mParent.RemoveSoftEdge();
        }

        /// <summary>
        /// Gets or sets a double value that represents the length of the radius for a soft edge effect in points (pt).
        /// The default value is 0.0.
        /// </summary>
        public double Radius
        {
            get { return mParent.EdgeRadius; }
            set { mParent.EdgeRadius = value; }
        }

        /// <summary>
        /// The parent object.
        /// </summary>
        private readonly ISoftEdge mParent;
    }
}
