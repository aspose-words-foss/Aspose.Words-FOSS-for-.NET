// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/04/2014 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Core.Dml.Scene3D
{
    /// <summary>
    /// 20.1.5.5 camera (Camera)
    /// This element defines the placement and properties of the camera in the 3D scene. The camera position and
    /// properties modify the view of the scene.
    /// </summary>
    internal class DmlCamera
    {
        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlCamera value = (DmlCamera)obj;

            return (value.PresetCameraType == PresetCameraType) &&
                   MathUtil.AreEqual(value.Zoom, Zoom) &&
                   object.Equals(value.FovAngle, FovAngle) &&
                   object.Equals(value.Rotation, Rotation);
        }

        public override int GetHashCode()
        {
            int hash = FovAngle.GetHashCode();
            hash ^= PresetCameraType.GetHashCode();
            hash ^= Zoom.GetHashCode();
            if (Rotation != null)
                hash ^= Rotation.GetHashCode();
            return hash;
        }

        internal DmlCamera Clone()
        {
            DmlCamera lhs = (DmlCamera)MemberwiseClone();
            lhs.mFovAngle = mFovAngle.Clone();
            lhs.mPresetCameraType = mPresetCameraType;
            lhs.mZoom = mZoom;

            if (mRotation != null)
                lhs.mRotation = mRotation.Clone();

            return lhs;
        }

        /// <summary>
        /// Provides an override for the default field of view for the camera.
        /// Range from [0, 180] degrees.
        /// Default is 180 degrees (not sure if this is correct).
        /// </summary>
        internal DmlAngle FovAngle
        {
            get { return mFovAngle; }
            set { mFovAngle = value; }
        }

        /// <summary>
        /// Defines the preset camera that is being used by the camera element. 
        /// The preset camera defines a starting point for common preset rotations in space.
        /// </summary>
        internal DmlPresetCameraType PresetCameraType
        {
            get { return mPresetCameraType; }
            set { mPresetCameraType = value; }
        }

        /// <summary>
        /// Defines the zoom factor of a given camera element. 
        /// The zoom modifies the scene as a whole and zooms in or out accordingly.
        /// Default is 100%.
        /// Value is in fraction representation, so 100% equals 1.0.
        /// </summary>
        internal double Zoom
        {
            get { return mZoom; }
            set { mZoom = value; }
        }

        /// <summary>
        /// Defines a rotation in 3D space.
        /// </summary>
        internal DmlRotation3D Rotation
        {
            get { return mRotation; }
            set { mRotation = value; }
        }

        private DmlAngle mFovAngle = DmlAngle.FromDegrees(180.0d);
        private DmlPresetCameraType mPresetCameraType;
        private double mZoom = 1.0;
        private DmlRotation3D mRotation;
    }
}
