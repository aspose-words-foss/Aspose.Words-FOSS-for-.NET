// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Colors;

namespace Aspose.Words.Drawing.Core.Dml.Scene3D
{
    /// <summary>
    /// 20.1.5.12 sp3d (Apply 3D shape properties)
    /// This element defines the 3D properties associated with a particular shape 
    /// in DrawingML. The 3D properties which can be applied to a shape are top and 
    /// bottom bevels, a contour and an extrusion.
    /// </summary>
    internal class DmlShape3DProperties : DmlExtensionListSource
    {
        internal DmlShape3DProperties() : this(false)
        {
        }

        internal DmlShape3DProperties(bool isTheme)
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

            DmlShape3DProperties value = (DmlShape3DProperties)obj;

            return (value.PresetMaterial == PresetMaterial) &&
                   MathUtil.AreEqual(value.Z, Z) &&
                   MathUtil.AreEqual(value.ContourWidth, ContourWidth) &&
                   MathUtil.AreEqual(value.ExtrusionHeight, ExtrusionHeight) &&
                   object.Equals(value.BevelTop, BevelTop) &&
                   object.Equals(value.BevelBottom, BevelBottom) &&
                   object.Equals(value.ContourColor, ContourColor) &&
                   object.Equals(value.ExtrusionColor, ExtrusionColor);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (BevelTop != null)
                hash ^= BevelTop.GetHashCode();
            if (BevelBottom != null)
                hash ^= BevelBottom.GetHashCode();
            if (ContourColor != null)
                hash ^= ContourColor.GetHashCode();
            if (ExtrusionColor != null)
                hash ^= ExtrusionColor.GetHashCode();
            hash ^= Z.GetHashCode();
            hash ^= ContourWidth.GetHashCode();
            hash ^= ExtrusionHeight.GetHashCode();
            hash ^= PresetMaterial.GetHashCode();

            return hash;
        }

        internal DmlShape3DProperties Clone()
        {
            DmlShape3DProperties lhs = (DmlShape3DProperties)MemberwiseClone();
            if (mBevelTop != null)
                lhs.mBevelTop = mBevelTop.Clone();

            if (mBevelBottom != null)
                lhs.mBevelBottom = mBevelBottom.Clone();

            if (mContourColor != null)
                lhs.mContourColor = mContourColor.Clone();

            if (mExtrusionColor != null)
                lhs.mExtrusionColor = mExtrusionColor.Clone();

            lhs.mZ = mZ;
            lhs.ContourWidth = ContourWidth;
            lhs.ExtrusionHeight = ExtrusionHeight;
            lhs.PresetMaterial = PresetMaterial;

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        /// <summary>
        /// This element holds the properties associated with defining a bevel on the bottom or back face of a shape.
        /// </summary>
        internal DmlBevel BevelTop
        {
            get { return mBevelTop; }
            set { mBevelTop = value; }
        }

        /// <summary>
        /// This element holds the properties associated with defining a bevel on the top or front face of a shape.
        /// </summary>
        internal DmlBevel BevelBottom
        {
            get { return mBevelBottom; }
            set { mBevelBottom = value; }
        }

        /// <summary>
        /// This element defines the color for the contour on a shape.
        /// </summary>
        internal DmlColor ContourColor
        {
            get { return mContourColor; }
            set { mContourColor = value; }
        }

        /// <summary>
        /// This element defines the color of the extrusion applied to a shape.
        /// </summary>
        internal DmlColor ExtrusionColor
        {
            get { return mExtrusionColor; }
            set { mExtrusionColor = value; }
        }

        /// <summary>
        /// Defines the z coordinate for the 3D shape.
        /// </summary>
        internal double Z
        {
            get { return mZ; }
            set { mZ = value; }
        }

        /// <summary>
        /// Defines the width of the contour on the shape.
        /// </summary>
        internal double ContourWidth
        {
            get { return mContourWidth; }
            set { mContourWidth = value; }
        }

        /// <summary>
        /// Defines the height of the extrusion applied to the shape.
        /// </summary>
        internal double ExtrusionHeight
        {
            get { return mExtrusionHeight; }
            set { mExtrusionHeight = value; }
        }

        /// <summary>
        /// Defines the preset material which is combined with the lighting properties 
        /// to give the final look and feel of a shape.
        /// </summary>
        internal DmlPresetMaterialType PresetMaterial
        {
            get { return mPresetMaterial; }
            set { mPresetMaterial = value; }
        }

        /// <summary>
        /// Flag indicates that 3D properties represented by this class are applied through document theme.
        /// </summary>
        internal bool IsTheme
        {
            get { return mIsTheme; }
        }

        private DmlBevel mBevelTop;
        private DmlBevel mBevelBottom;
        private DmlColor mContourColor;
        private DmlColor mExtrusionColor;
        private double mZ;
        private double mContourWidth;
        private double mExtrusionHeight;
        private DmlPresetMaterialType mPresetMaterial = DmlPresetMaterialType.WarmMatte;
        private readonly bool mIsTheme;
    }
}
