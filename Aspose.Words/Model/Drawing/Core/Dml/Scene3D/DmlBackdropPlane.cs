// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/04/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.Scene3D
{
    /// <summary>
    /// 20.1.5.2 backdrop (Backdrop Plane)
    /// This element defines a plane in which effects, such as glow and shadow, are applied in relation to the shape they
    /// are being applied to. The points and vectors contained within the backdrop define a plane in 3D space.
    /// </summary>
    internal class DmlBackdropPlane : DmlExtensionListSource
    {
        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlBackdropPlane value = (DmlBackdropPlane)obj;

            return Anchor.Equals(value.Anchor) &&
                   NormalVector.Equals(value.NormalVector) &&
                   UpVector.Equals(value.UpVector);
        }

        public override int GetHashCode()
        {
            int hash = Anchor.GetHashCode();
            hash ^= NormalVector.GetHashCode();
            hash ^= UpVector.GetHashCode();
            return hash;
        }

        internal DmlBackdropPlane Clone()
        {
            DmlBackdropPlane lhs = (DmlBackdropPlane)MemberwiseClone();
            lhs.Extensions = CloneExtensions();
            return lhs;
        }

        /// <summary>
        /// This point is the point in space that anchors the backdrop plane.
        /// </summary>
        internal DmlPoint3D Anchor
        {
            get { return mAnchor; }
            set { mAnchor = value; }
        }

        /// <summary>
        /// This element defines a normal vector.
        /// X, Y and Z coordinates of the point should be considered as a distance along the appropriate axis.
        /// </summary>
        internal DmlPoint3D NormalVector
        {
            get { return mNormalVector; }
            set { mNormalVector = value; }
        }

        /// <summary>
        /// This element defines a vector representing up.
        /// X, Y and Z coordinates of the point should be considered as a distance along the appropriate axis.
        /// </summary>
        internal DmlPoint3D UpVector
        {
            get { return mUpVector; }
            set { mUpVector = value; }
        }

        private DmlPoint3D mAnchor;
        private DmlPoint3D mNormalVector;
        private DmlPoint3D mUpVector;
    }
}
