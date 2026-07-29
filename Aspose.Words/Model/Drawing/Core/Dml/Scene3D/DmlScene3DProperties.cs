// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Scene3D
{
    /// <summary>
    /// 20.1.4.1.26 scene3d (3D Scene Properties)
    /// This element defines optional scene-level 3D properties to apply to an object.
    /// </summary>
    internal class DmlScene3DProperties : DmlExtensionListSource
    {
        internal DmlScene3DProperties() : this(false)
        {
        }

        internal DmlScene3DProperties(bool isTheme)
        {
            mIsTheme = isTheme;
        }

        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlScene3DProperties value = (DmlScene3DProperties)obj;

            return (value.IsTheme == IsTheme) &&
                   object.Equals(value.Camera, Camera) &&
                   object.Equals(value.LightRig, LightRig) &&
                   object.Equals(value.BackdropPlane, BackdropPlane);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash ^= Camera.GetHashCode();
            hash ^= LightRig.GetHashCode();
            if (BackdropPlane != null)
                hash ^= BackdropPlane.GetHashCode();
            hash ^= IsTheme.GetHashCode();

            return hash;
        }

        internal DmlScene3DProperties Clone()
        {
            DmlScene3DProperties lhs = (DmlScene3DProperties)MemberwiseClone();
            if (mCamera != null)
                lhs.mCamera = mCamera.Clone();

            if (mLightRig != null)
                lhs.mLightRig = mLightRig.Clone();

            if (mBackdropPlane != null)
                lhs.mBackdropPlane = mBackdropPlane.Clone();

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        /// <summary>
        /// Defines placement and properties of the camera in the 3D scene.
        /// </summary>
        internal DmlCamera Camera
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mCamera == null)
                    mCamera = new DmlCamera();

                return mCamera;
            }
            set
            {
                mCamera = value;
            }
        }

        /// <summary>
        /// Defines the lighting properties associated with the scene.
        /// </summary>
        internal DmlLightRig LightRig
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mLightRig == null)
                    mLightRig = new DmlLightRig();

                return mLightRig;
            }
            set
            {
                mLightRig = value;
            }
        }

        /// <summary>
        /// Defines a plane in which effects, such as glow and shadow, are applied.
        /// Can be null.
        /// </summary>
        internal DmlBackdropPlane BackdropPlane
        {
            get { return mBackdropPlane; }
            set { mBackdropPlane = value; }
        }

        /// <summary>
        /// Flag indicates that 3D properties represented by this class are applied thorough document theme.
        /// </summary>
        internal bool IsTheme
        {
            get { return mIsTheme; }
        }

        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private DmlCamera mCamera;
        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private DmlLightRig mLightRig;
        private DmlBackdropPlane mBackdropPlane;
        private readonly bool mIsTheme;
    }
}
