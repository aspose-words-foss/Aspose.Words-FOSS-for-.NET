// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/04/2014 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Core.Dml.Scene3D
{
    /// <summary>
    /// 20.1.5.11 rot (Rotation)
    /// This element defines a rotation in 3D space. A rotation in DrawingML is defined through the use of a latitude
    /// coordinate, a longitude coordinate, and a revolution about the axis as the latitude and longitude coordinates.
    /// </summary>
    internal class DmlRotation3D
    {
        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlRotation3D value = (DmlRotation3D)obj;

            return object.Equals(value.Latitude, Latitude) &&
                   object.Equals(value.Longitude, Longitude) &&
                   object.Equals(value.Revolution, Revolution);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (Latitude != null)
                hash^=Latitude.GetHashCode();
            if (Longitude != null)
                hash ^= Longitude.GetHashCode();
            if (Revolution != null)
                hash ^= Revolution.GetHashCode();
            return hash;
        }

        internal DmlRotation3D Clone()
        {
            DmlRotation3D lhs = (DmlRotation3D)MemberwiseClone();
            if (mLatitude != null)
                lhs.mLatitude = mLatitude.Clone();
            if (mLongitude != null)
                lhs.mLongitude = mLongitude.Clone();
            if (mRevolution != null)
                lhs.mRevolution = mRevolution.Clone();
            return lhs;
        }

        /// <summary>
        /// Defines the latitude value of the rotation.
        /// </summary>
        internal DmlAngle Latitude
        {
            get { return mLatitude; }
            set { mLatitude = value; }
        }

        /// <summary>
        /// Defines the longitude value of the rotation.
        /// </summary>
        internal DmlAngle Longitude
        {
            get { return mLongitude; }
            set { mLongitude = value; }
        }

        /// <summary>
        /// This attributes defines the revolution around the central axis in the rotation.
        /// </summary>
        internal DmlAngle Revolution
        {
            get { return mRevolution; }
            set { mRevolution = value; }
        }

        private DmlAngle mLatitude;
        private DmlAngle mLongitude;
        private DmlAngle mRevolution;
    }
}
