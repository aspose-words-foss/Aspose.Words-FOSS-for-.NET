// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/04/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.Scene3D
{
    /// <summary>
    /// 20.1.5.9 lightRig (Light Rig)
    /// This element defines the light rig associated with the table. The light rig comes into play when there is a 3D
    /// bevel applied to a cell. When 3D is used, the light rig defines the lighting properties associated with the scene.
    /// </summary>
    internal class DmlLightRig
    {
        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlLightRig value = (DmlLightRig)obj;

            return (value.LightRigType == LightRigType) &&
                   (value.Direction == Direction) &&
                   object.Equals(value.Rotation, Rotation);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (Rotation != null)
                hash ^= Rotation.GetHashCode();

            hash ^= Direction.GetHashCode();
            hash ^= LightRigType.GetHashCode();

            return hash;
        }

        internal DmlLightRig Clone()
        {
            DmlLightRig lhs = (DmlLightRig)MemberwiseClone();
            if (mRotation != null)
                lhs.mRotation = mRotation.Clone();

            lhs.mDirection = mDirection;
            lhs.mLightRigType = mLightRigType;
            return lhs;
        }

        /// <summary>
        /// Defines a rotation in 3D space.
        /// </summary>
        internal DmlRotation3D Rotation
        {
            get { return mRotation; }
            set { mRotation = value; }
        }

        /// <summary>
        /// Defines the direction from which the light rig is oriented in relation to the scene.
        /// </summary>
        internal DmlLightRigDirection Direction
        {
            get { return mDirection; }
            set { mDirection = value; }
        }

        /// <summary>
        /// Defines the preset type of light rig which is to be applied to the scene.
        /// </summary>
        internal DmlLightRigType LightRigType
        {
            get { return mLightRigType; }
            set { mLightRigType = value; }
        }

        private DmlRotation3D mRotation;
        private DmlLightRigDirection mDirection;
        private DmlLightRigType mLightRigType;
    }
}
