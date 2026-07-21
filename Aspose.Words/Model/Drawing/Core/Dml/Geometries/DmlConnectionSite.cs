// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/09/2016 by Dmitry Sokolov

using Aspose.Words.Drawing.Core.Dml.Path;

namespace Aspose.Words.Drawing.Core.Dml.Geometries
{
    /// <summary>
    /// Shape connection site.
    /// </summary>
    /// <remarks>
    /// 20.1.9.9 cxn (Shape Connection Site).
    /// This element specifies the existence of a connection site on a custom shape. A connection site allows a cxnSp
    /// to be attached to this shape. This connection is maintained when the shape is repositioned within the document.
    /// It should be noted that this connection is placed within the shape bounding box using the transform coordinate
    /// system which is also called the shape coordinate system, as it encompasses the entire shape. The width and
    /// height for this coordinate system are specified within the ext transform element. 
    /// </remarks>
    internal class DmlConnectionSite
    {
        /// <summary>
        /// Ctr.
        /// </summary>
        /// <param name="angle">Angle for incoming connector.</param>
        internal DmlConnectionSite(string angle)
        {
            mAngle = new DmlAdjustableAngle(angle);
        }

        /// <summary>
        /// Clones this instance of <see cref="DmlConnectionSite"/>.
        /// </summary>
        public DmlConnectionSite Clone()
        {
            DmlConnectionSite lhs = (DmlConnectionSite)MemberwiseClone();

            if (mCoordinates != null)
                lhs.mCoordinates = mCoordinates.Clone();

            // Cloning of the mAngle field is skipped since it is immutable.

            return lhs;
        }

        /// <summary>
        /// Specifies the incoming connector angle. 
        /// </summary>
        internal DmlAdjustableAngle Angle
        {
            get { return mAngle; }
            set { mAngle = value; }
        }

        /// <summary>
        /// Specifies a position coordinate within the shape bounding box.
        /// </summary>
        internal DmlAdjustablePoint Coordinates
        {
            get { return mCoordinates; }
            set { mCoordinates = value; }
        }

        private DmlAdjustableAngle mAngle;
        private DmlAdjustablePoint mCoordinates;
    }
}
